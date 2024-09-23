using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Lecture
    {
        public int Id { get; set; }
        public string LectureTitle { get; set; }
        public string htmlcontent { get; set; }
        public int TopId { get; set; }

    }
}
