using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string GitHubLink { get; set; }

        // Space-separated URLs
        public string ImageUrls { get; set; }
        public string? UserManual { get; set; }  
    }
}
