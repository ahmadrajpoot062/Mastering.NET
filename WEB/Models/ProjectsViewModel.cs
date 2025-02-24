using Core.Entities;

namespace WEB.Models
{
    public class ProjectsViewModel
    {
        public List<Project> DotNetProjects { get; set; }
        public List<Project> BlazorProjects { get; set; }
    }
}
