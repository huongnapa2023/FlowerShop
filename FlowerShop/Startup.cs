using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FlowerShop.Startup))]

namespace FlowerShop
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Khởi tạo định tuyến SignalR 2 cho tính năng Chat thời gian thực (/signalr)
            app.MapSignalR();
        }
    }
}
