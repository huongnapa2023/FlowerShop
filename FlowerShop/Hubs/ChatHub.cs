using System.Linq;
using System.Threading.Tasks;
using FlowerShop.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace FlowerShop.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        public async Task JoinCustomerRoom(string sessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
                return;

            await Groups.Add(Context.ConnectionId, "conv_" + sessionKey);

            using (var db = new FlowerShopDbContext())
            {
                var conv = db.ChatConversations.FirstOrDefault(c => c.SessionKey == sessionKey);
                if (conv != null)
                {
                    conv.IsCustomerOnline = true;
                    db.SaveChanges();
                    Clients.Group("Admins").customerStatusChanged(conv.Id, true);
                }
            }
        }

        public Task JoinAdminChannel()
        {
            return Groups.Add(Context.ConnectionId, "Admins");
        }
    }
}
