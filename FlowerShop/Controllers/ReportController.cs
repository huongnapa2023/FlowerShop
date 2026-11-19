using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    [AdminPermissionAuthorize(AdminPermissions.Report)]
    public class ReportController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        public ActionResult Index(DateTime? fromDate, DateTime? toDate, string preset = null)
        {
            DateTime end = (toDate ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);
            DateTime start = (fromDate ?? DateTime.Today.AddDays(-29)).Date;
            string label = "Tùy chọn thời gian";

            if (preset == "7d")
            {
                start = DateTime.Today.AddDays(-6);
                end = DateTime.Today.AddDays(1).AddTicks(-1);
                label = "7 ngày gần nhất";
            }
            else if (preset == "30d" || (!fromDate.HasValue && !toDate.HasValue))
            {
                start = DateTime.Today.AddDays(-29);
                end = DateTime.Today.AddDays(1).AddTicks(-1);
                label = "30 ngày gần nhất";
            }
            else if (preset == "90d")
            {
                start = DateTime.Today.AddDays(-89);
                end = DateTime.Today.AddDays(1).AddTicks(-1);
                label = "Quý này (90 ngày)";
            }

            var ordersInPeriod = _context.Orders
                .Include(o => o.User)
                .Include("OrderItems.Product.Category")
                .Include("OrderItems.Product.Images")
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            var revenueOrders = ordersInPeriod
                .Where(o => o.Status == "Hoàn thành" || o.IsPaid)
                .ToList();

            decimal totalRevenue = revenueOrders.Sum(o => o.Total);
            decimal grossProfit = Math.Round(totalRevenue * 0.42m, 0);

            var series = new List<RevenuePoint>();
            int totalDays = Math.Max(1, (end.Date - start.Date).Days + 1);
            if (totalDays <= 14)
            {
                for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
                {
                    var dayOrders = revenueOrders.Where(o => o.CreatedAt.Date == d).ToList();
                    decimal rev = dayOrders.Sum(o => o.Total);
                    series.Add(new RevenuePoint
                    {
                        Label = d.ToString("dd/MM"),
                        Revenue = rev,
                        GrossProfit = Math.Round(rev * 0.42m, 0),
                        OrderCount = dayOrders.Count
                    });
                }
            }
            else
            {
                DateTime cursor = start.Date;
                int weekIdx = 1;
                while (cursor <= end.Date)
                {
                    DateTime weekEnd = cursor.AddDays(6) > end.Date ? end.Date : cursor.AddDays(6);
                    var wkOrders = revenueOrders.Where(o => o.CreatedAt.Date >= cursor && o.CreatedAt.Date <= weekEnd).ToList();
                    decimal rev = wkOrders.Sum(o => o.Total);
                    series.Add(new RevenuePoint
                    {
                        Label = string.Format("Tuần {0} ({1:dd/MM}-{2:dd/MM})", weekIdx++, cursor, weekEnd),
                        Revenue = rev,
                        GrossProfit = Math.Round(rev * 0.42m, 0),
                        OrderCount = wkOrders.Count
                    });
                    cursor = weekEnd.AddDays(1);
                }
            }

            var topProducts = ordersInPeriod
                .Where(o => o.Status != "Đã hủy")
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.Product != null)
                .GroupBy(oi => oi.ProductId)
                .Select(g =>
                {
                    var firstProd = g.First().Product;
                    return new TopProductStat
                    {
                        ProductId = firstProd.Id,
                        ProductName = firstProd.Name,
                        CategoryName = firstProd.Category != null ? firstProd.Category.Name : "Hoa tươi",
                        ImageUrl = firstProd.MainImageUrl,
                        SoldQuantity = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity),
                        StockQuantity = firstProd.StockQuantity
                    };
                })
                .OrderByDescending(x => x.SoldQuantity)
                .ThenByDescending(x => x.TotalRevenue)
                .Take(5)
                .ToList();

            var allCustomers = _context.Users.Where(u => u.Role == "Customer").ToList();
            var allProducts = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToList();

            var lowStock = allProducts.Where(p => p.IsActive && p.StockQuantity <= 5).OrderBy(p => p.StockQuantity).ToList();
            var expiring = allProducts
                .Where(p => p.IsActive && p.ExpiryDate.HasValue && p.ExpiryDate.Value <= DateTime.Now.AddDays(4))
                .OrderBy(p => p.ExpiryDate)
                .ToList();

            var vm = new ReportDashboardViewModel
            {
                FromDate = start,
                ToDate = end.Date,
                PeriodLabel = label,
                TotalRevenue = totalRevenue,
                GrossProfit = grossProfit,
                TotalOrders = ordersInPeriod.Count,
                PendingOrders = ordersInPeriod.Count(o => o.Status == "Chờ xác nhận"),
                ProcessingOrders = ordersInPeriod.Count(o => o.Status == "Đang xử lý"),
                ShippingOrders = ordersInPeriod.Count(o => o.Status == "Đang giao"),
                CompletedOrders = ordersInPeriod.Count(o => o.Status == "Hoàn thành"),
                CancelledOrders = ordersInPeriod.Count(o => o.Status == "Đã hủy"),
                TotalCustomers = allCustomers.Count,
                VipCustomers = allCustomers.Count(u => u.IsVip),
                NewCustomersInPeriod = allCustomers.Count(u => u.CreatedAt >= start && u.CreatedAt <= end),
                TotalProducts = allProducts.Count,
                LowStockCount = lowStock.Count,
                ExpiringSoonCount = expiring.Count,
                RevenueSeries = series,
                BestSellingProducts = topProducts,
                TopVipCustomers = allCustomers.OrderByDescending(u => u.TotalSpent).Take(5).ToList(),
                LowStockProducts = lowStock,
                ExpiringProducts = expiring,
                RecentOrders = ordersInPeriod.Take(8).ToList()
            };

            if (!ordersInPeriod.Any())
            {
                ViewBag.NoDataMessage = "Không có dữ liệu để thống kê trong khoảng thời gian đã chọn.";
            }

            return View(vm);
        }

        public ActionResult ExportCsv(DateTime? fromDate, DateTime? toDate)
        {
            DateTime start = (fromDate ?? DateTime.Today.AddDays(-29)).Date;
            DateTime end = (toDate ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);

            var orders = _context.Orders
                .Include("OrderItems.Product")
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("BÁO CÁO KINH DOANH CỬA HÀNG HOA TƯƠI CHAM FLOWERS");
            sb.AppendLine(string.Format("Từ ngày:,{0:dd/MM/yyyy},Đến ngày:,{1:dd/MM/yyyy}", start, end));
            sb.AppendLine();
            sb.AppendLine("Mã Đơn,Ngày Đặt,Khách Nhận,Số Điện Thoại,Phương Thức,Đã Thanh Toán,Trạng Thái,Tổng Tiền (VNĐ),Chi Tiết Sản Phẩm");

            foreach (var o in orders)
            {
                string itemsDesc = string.Join(" | ", o.OrderItems.Select(i => string.Format("{0} x{1}", i.Product != null ? i.Product.Name : "", i.Quantity)));
                sb.AppendLine(string.Format("#{0},{1:dd/MM/yyyy HH:mm},\"{2}\",\"{3}\",{4},{5},\"{6}\",{7},\"{8}\"",
                    o.Id,
                    o.CreatedAt,
                    o.RecipientName,
                    o.RecipientPhone,
                    o.PaymentMethod,
                    o.IsPaid ? "Đã TT" : "Chưa TT",
                    o.Status,
                    o.Total,
                    itemsDesc));
            }

            byte[] preamble = Encoding.UTF8.GetPreamble();
            byte[] body = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] finalBytes = new byte[preamble.Length + body.Length];
            Buffer.BlockCopy(preamble, 0, finalBytes, 0, preamble.Length);
            Buffer.BlockCopy(body, 0, finalBytes, preamble.Length, body.Length);

            string fileName = string.Format("BaoCao_CHAMFlowers_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", start, end);
            return File(finalBytes, "text/csv; charset=utf-8", fileName);
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
