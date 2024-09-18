using Core.Entities;

namespace Mastering.NET.Models
{
    public class TopicLectureViewModel
    {
        public Topic Topic { get; set; }
        public List<Lecture> Lectures { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Project> Projects { get; set; }
    }
}
