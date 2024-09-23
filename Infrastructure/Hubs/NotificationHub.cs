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
        public async Task SendNotification(int id, string topicName, string lectureTitle)
        {
            await Clients.All.SendAsync("ReceiveNotification", id, topicName, lectureTitle);
        }
    }
}
