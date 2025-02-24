using Mastering.NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Application.Services;
using Core.Entities;
using System.Numerics;
using Microsoft.VisualBasic;
using WEB.Models;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GenericService<Topic> _topicRepository;
        private readonly GenericService<Lecture> _lectureRepository;
        private readonly GenericService<Contact> _contactRepository;
        private readonly GenericService<Project> _projectRepository;

        public HomeController(ILogger<HomeController> logger, GenericService<Topic> topicRepository, GenericService<Lecture> lectureRepository, GenericService<Contact> contactRepository, GenericService<Project> projectRepository)
        {
            _topicRepository = topicRepository;
            _lectureRepository = lectureRepository;
            _logger = logger;
            _contactRepository = contactRepository;
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> Index()
        {
            List<Contact> contacts = await _contactRepository.GetAll();
            return View(contacts);
        }
        public IActionResult CareerPathways()
        {
            return View();
        }
        public IActionResult privacy_policy()
        {
            return View();
        }
        public IActionResult terms_of_use()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> topicsPartialViewUpdate(int page = 1, int pageSize = 6)
        {
            List<Topic> topics = await _topicRepository.GetAll();

            int totalTopics = topics.Count();
            int totalPages = (int)Math.Ceiling(totalTopics / (double)pageSize);
            List<Topic> paginatedTopics = topics.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return PartialView("_allTopicsPartial", paginatedTopics);
        }

        [HttpGet]
        public async Task<IActionResult> DotNetProjectsPartialViewUpdate(int page = 1, int pageSize = 6)
        {
            List<Project> projects = await _projectRepository.GetAll();
            var dotNetProjects = projects.Where(p => p.ProjectType == ".NET").ToList();

            int totalprojects = dotNetProjects.Count();
            int totalPages = (int)Math.Ceiling(totalprojects / (double)pageSize);
            List<Project> paginatedProjects = dotNetProjects.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentDotNetPage = page;
            ViewBag.TotalDotNetPages = totalPages;

            return PartialView("_dotNetProjectsPartial", paginatedProjects);
        }

        [HttpGet]
        public async Task<IActionResult> BlazorProjectsPartialViewUpdate(int page = 1, int pageSize = 6)
        {
            List<Project> projects = await _projectRepository.GetAll();
            var blazorProjects = projects.Where(p => p.ProjectType == "Blazor").ToList();

            int totalprojects = blazorProjects.Count();
            int totalPages = (int)Math.Ceiling(totalprojects / (double)pageSize);
            List<Project> paginatedProjects = blazorProjects.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentBlazorPage = page;
            ViewBag.TotalBlazorPages = totalPages;

            return PartialView("_blazorProjectsPartial", paginatedProjects);
        }

        [HttpGet]
        public async Task<IActionResult> topicNavBarUpdate()
        {
            List<Topic> topics = await _topicRepository.GetAll();

            return PartialView("_topicsNavBarPartial", topics);
        }

        private void sanatize(Contact contact)
        {

            if (contact == null)
            {
                return;
            }

            contact.Email = System.Web.HttpUtility.HtmlEncode(contact.Email);
            contact.Subject = System.Web.HttpUtility.HtmlEncode((string)contact.Subject);
            contact.Message = System.Web.HttpUtility.HtmlEncode(contact.Message);
            contact.Name = System.Web.HttpUtility.HtmlEncode(contact.Name);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> sendMessage(string name, string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                return BadRequest("All fields are required.");
            }

            Contact contact = new Contact
            {
                Name = name,
                Email = email,
                Subject = subject,
                Message = message,
                IsRead = false
            };

            sanatize(contact);

            await _contactRepository.Add(contact);

            return Ok("Your message has been successfully sent.");
        }

        public async Task<IActionResult> viewProjectDetails()
        {
            string? id = Request.Query["id"];

            return View(await _projectRepository.GetById(int.Parse(id)));
        }

        public async Task<IActionResult> Topic(int id)
        {
            List<Lecture> lectures = await _lectureRepository.GetAll();
            var filteredLectures = lectures.Where(lecture => lecture.TopId == id).ToList();

            var topic = await _topicRepository.GetById(id);

            ViewBag.TopicName = topic?.TopicName;
            ViewBag.TopicId = id;

            return View(filteredLectures);
        }

        public async Task<IActionResult> Topics(int topicPage = 1,int pageSize = 6)
        {
            List<Topic> topics = await _topicRepository.GetAll();

            int totalTopics = topics.Count();
            int totalTopicPages = (int)Math.Ceiling(totalTopics / (double)pageSize);
            List<Topic> paginatedTopics = topics.Skip((topicPage - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = topicPage;
            ViewBag.TotalPages = totalTopicPages;

            return View(paginatedTopics);
        }
        public async Task<IActionResult> Projects(int dotNetPage = 1, int blazorPage = 1, int pageSize = 6)
        {
            List<Project> allProjects = await _projectRepository.GetAll();

            // Filter projects by type
            List<Project> dotNetProjects = allProjects.Where(p => p.ProjectType == ".NET").ToList();
            List<Project> blazorProjects = allProjects.Where(p => p.ProjectType == "Blazor").ToList();

            // Pagination for .NET projects
            int totalDotNetProjects = dotNetProjects.Count();
            int totalDotNetPages = (int)Math.Ceiling(totalDotNetProjects / (double)pageSize);
            List<Project> paginatedDotNetProjects = dotNetProjects.Skip((dotNetPage - 1) * pageSize).Take(pageSize).ToList();

            // Pagination for Blazor projects
            int totalBlazorProjects = blazorProjects.Count();
            int totalBlazorPages = (int)Math.Ceiling(totalBlazorProjects / (double)pageSize);
            List<Project> paginatedBlazorProjects = blazorProjects.Skip((blazorPage - 1) * pageSize).Take(pageSize).ToList();

            // Pass data to the view
            ViewBag.CurrentDotNetPage = dotNetPage;
            ViewBag.TotalDotNetPages = totalDotNetPages;
            ViewBag.CurrentBlazorPage = blazorPage;
            ViewBag.TotalBlazorPages = totalBlazorPages;

            var model = new ProjectsViewModel
            {
                DotNetProjects = paginatedDotNetProjects,
                BlazorProjects = paginatedBlazorProjects
            };

            return View(model);
        }

        public async Task<string> loadLecture(int id)
        {
            Lecture lecture = await _lectureRepository.GetById(id);
            return lecture.htmlcontent;
        }
        public async Task<IActionResult> Search(string topicName)
        {
            if (topicName == null)
            {
                return RedirectToAction("topicsPartialViewUpdate","Home");
            }
            else
            {
                List<Topic> list = await _topicRepository.GetAll();
                var filteredList = list.Where(a => a.TopicName.Contains(topicName, StringComparison.OrdinalIgnoreCase)).ToList();
                return PartialView("_allTopicsPartial", filteredList);
            }
        }

        public async Task<IActionResult> SearchDotNetProjects(string projectName)
        {
            if (projectName == null)
            {
                return RedirectToAction("DotNetProjectsPartialViewUpdate", "Home");
            }
            else
            {
                List<Project> list = await _projectRepository.GetAll();
                var filteredList = list.Where(a => a.ProjectType == ".NET" && a.Title.Contains(projectName, StringComparison.OrdinalIgnoreCase)).ToList();
                return PartialView("_dotNetProjectsPartial", filteredList);
            }
        }

        public async Task<IActionResult> SearchBlazorProjects(string projectName)
        {
            if (projectName == null)
            {
                return RedirectToAction("BlazorProjectsPartialViewUpdate", "Home");
            }
            else
            {
                List<Project> list = await _projectRepository.GetAll();
                var filteredList = list.Where(a => a.ProjectType == "Blazor" && a.Title.Contains(projectName, StringComparison.OrdinalIgnoreCase)).ToList();
                return PartialView("_blazorProjectsPartial", filteredList);
            }
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
