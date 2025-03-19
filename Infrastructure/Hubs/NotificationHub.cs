using Microsoft.AspNetCore.SignalR;

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
