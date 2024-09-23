using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        // This method can be called to broadcast a message to all connected clients
        public async Task SendNotification(int id, string topicName, string lectureTitle)
        {
            // Notify all connected clients
            await Clients.All.SendAsync("ReceiveNotification", id, topicName, lectureTitle);
        }
    }
}
