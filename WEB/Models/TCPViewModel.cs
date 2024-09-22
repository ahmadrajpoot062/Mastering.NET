using Core.Entities;

namespace Mastering.NET.Models
{
    public class TCPViewModel
    {
        public List<Topic> Topic {  get; set; } 
        public List<Topic> PaginatedTopics { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Project> PaginatedProjects { get; set; }
    }
}
