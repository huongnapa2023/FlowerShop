using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Hubs;
using FlowerShop.Models;
using FlowerShop.Services;
using Microsoft.AspNet.SignalR;

namespace FlowerShop.Controllers
{
    public class ChatController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        [HttpGet]
        public ActionResult InitSession()
        {
            string sessionKey = Session["ChatSessionKey"] as string;
            int? userId = null;
            string customerName = "Khách truy cập";

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = AppHelpers.GetCurrentUserId(User);
                customerName = !string.IsNullOrWhiteSpace(User.Identity.Name) ? User.Identity.Name : "Khách hàng";
                if (userId.HasValue)
                {
                    sessionKey = string.Format("user_{0}", userId.Value);
                    Session["ChatSessionKey"] = sessionKey;
                }
            }

            if (string.IsNullOrEmpty(sessionKey))
            {
                sessionKey = "guest_" + Guid.NewGuid().ToString("N").Substring(0, 10);
                Session["ChatSessionKey"] = sessionKey;
            }

            var conv = _context.ChatConversations
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.SessionKey == sessionKey);

            object messagesObj = new object[0];
            if (conv != null && conv.Messages != null)
            {
                messagesObj = conv.Messages
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new
                    {
                        id = m.Id,
                        senderRole = m.SenderRole,
                        senderName = m.SenderName,
                        content = m.Content,
                        messageType = m.MessageType,
                        attachmentUrl = m.AttachmentUrl,
                        createdAt = m.CreatedAt.ToString("HH:mm dd/MM")
                    })
                    .ToList();
            }

            return Json(new
            {
                sessionKey = sessionKey,
                customerName = customerName,
                userId = userId,
                conversationId = conv != null ? conv.Id : 0,
                messages = messagesObj
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendCustomerMessage(string sessionKey, string customerName, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Nội dung tin nhắn không được để trống." });
            }

            int? userId = null;
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = AppHelpers.GetCurrentUserId(User);
                if (!string.IsNullOrWhiteSpace(User.Identity.Name))
                    customerName = User.Identity.Name;
                if (userId.HasValue)
                    sessionKey = string.Format("user_{0}", userId.Value);
            }

            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                sessionKey = (Session["ChatSessionKey"] as string) ?? ("guest_" + Guid.NewGuid().ToString("N").Substring(0, 10));
            }
            Session["ChatSessionKey"] = sessionKey;

            var conv = _context.ChatConversations.FirstOrDefault(c => c.SessionKey == sessionKey);
            if (conv == null)
            {
                conv = new ChatConversation
                {
                    SessionKey = sessionKey,
                    UserId = userId,
                    CustomerName = string.IsNullOrWhiteSpace(customerName) ? "Khách truy cập" : customerName.Trim(),
                    Status = "Đang xử lý",
                    IsCustomerOnline = true,
                    UnreadAdminCount = 1,
                    CreatedAt = DateTime.Now,
                    LastMessageAt = DateTime.Now
                };
                _context.ChatConversations.Add(conv);
                _context.SaveChanges();
            }
            else
            {
                conv.IsCustomerOnline = true;
                conv.LastMessageAt = DateTime.Now;
                conv.UnreadAdminCount += 1;
                if (conv.Status == "Đã giải quyết")
                    conv.Status = "Đang xử lý";
            }

            var msg = new ChatMessage
            {
                ConversationId = conv.Id,
                SenderRole = "Customer",
                SenderName = conv.CustomerName,
                Content = content.Trim(),
                MessageType = "Text",
                CreatedAt = DateTime.Now
            };

            _context.ChatMessages.Add(msg);
            _context.SaveChanges();

            var payload = new
            {
                id = msg.Id,
                conversationId = conv.Id,
                sessionKey = conv.SessionKey,
                customerName = conv.CustomerName,
                senderRole = msg.SenderRole,
                senderName = msg.SenderName,
                content = msg.Content,
                messageType = msg.MessageType,
                attachmentUrl = msg.AttachmentUrl,
                status = conv.Status,
                unreadAdminCount = conv.UnreadAdminCount,
                createdAt = msg.CreatedAt.ToString("HH:mm dd/MM")
            };

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.Group("conv_" + conv.SessionKey).receiveMessage(payload);
            hubContext.Clients.Group("Admins").receiveMessage(payload);

            return Json(new { success = true, message = payload });
        }

        // ==================== TRUNG TÂM CHĂM SÓC KHÁCH HÀNG ADMIN ====================

        [AdminPermissionAuthorize(AdminPermissions.Chat)]
        public ActionResult Index(int? id, string statusFilter)
        {
            var query = _context.ChatConversations
                .Include(c => c.Messages)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(c => c.Status == statusFilter);
            }

            var conversations = query
                .OrderByDescending(c => c.LastMessageAt)
                .ToList();

            ChatConversation activeConv = null;
            if (conversations.Any())
            {
                activeConv = id.HasValue
                    ? (conversations.FirstOrDefault(c => c.Id == id.Value) ?? conversations.First())
                    : conversations.First();

                if (activeConv.UnreadAdminCount > 0)
                {
                    activeConv.UnreadAdminCount = 0;
                    _context.SaveChanges();
                }
            }

            ViewBag.Conversations = conversations;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.QuickProducts = _context.Products.Include(p => p.Images).Where(p => p.IsActive).Take(8).ToList();
            ViewBag.QuickCoupons = _context.Coupons.Where(c => c.IsActive).ToList();

            return View(activeConv);
        }

        [AdminPermissionAuthorize(AdminPermissions.Chat)]
        [HttpPost]
        public ActionResult AdminReply(int conversationId, string content, string messageType = "Text", string attachmentUrl = null)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung phản hồi." });
            }

            var conv = _context.ChatConversations.Find(conversationId);
            if (conv == null)
                return Json(new { success = false, message = "Không tìm thấy hội thoại." });

            conv.LastMessageAt = DateTime.Now;
            conv.UnreadAdminCount = 0;
            conv.UnreadCustomerCount += 1;

            var msg = new ChatMessage
            {
                ConversationId = conv.Id,
                SenderRole = "Admin",
                SenderName = "CHAM Flowers CSKH",
                Content = content.Trim(),
                MessageType = messageType,
                AttachmentUrl = attachmentUrl,
                CreatedAt = DateTime.Now
            };

            _context.ChatMessages.Add(msg);
            _context.SaveChanges();

            var payload = new
            {
                id = msg.Id,
                conversationId = conv.Id,
                sessionKey = conv.SessionKey,
                customerName = conv.CustomerName,
                senderRole = msg.SenderRole,
                senderName = msg.SenderName,
                content = msg.Content,
                messageType = msg.MessageType,
                attachmentUrl = msg.AttachmentUrl,
                status = conv.Status,
                unreadAdminCount = 0,
                createdAt = msg.CreatedAt.ToString("HH:mm dd/MM")
            };

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.Group("conv_" + conv.SessionKey).receiveMessage(payload);
            hubContext.Clients.Group("Admins").receiveMessage(payload);

            return Json(new { success = true, message = payload });
        }

        [AdminPermissionAuthorize(AdminPermissions.Chat)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int conversationId, string status)
        {
            var conv = _context.ChatConversations.Find(conversationId);
            if (conv != null)
            {
                conv.Status = status;
                _context.SaveChanges();
                TempData["Success"] = string.Format("Đã chuyển trạng thái hội thoại sang \"{0}\".", status);
            }
            return RedirectToAction("Index", new { id = conversationId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
