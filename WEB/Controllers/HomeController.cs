using Mastering.NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Application.Services;
using Core.Entities;
using System.Numerics;

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

        public async Task<IActionResult> Index(int topicPage = 1, int projectPage = 1, int pageSize = 6)
        {
            List<Topic> topics = await _topicRepository.GetAll();
            List<Contact> contacts = await _contactRepository.GetAll();
            List<Project> projects = await _projectRepository.GetAll();

            int totalTopics = topics.Count();
            int totalTopicPages = (int)Math.Ceiling(totalTopics / (double)pageSize);
            List<Topic> paginatedTopics = topics.Skip((topicPage - 1) * pageSize).Take(pageSize).ToList();

            int totalProjects = projects.Count();
            int totalProjectPages = (int)Math.Ceiling(totalProjects / (double)pageSize);
            List<Project> paginatedProjects = projects.Skip((projectPage - 1) * pageSize).Take(pageSize).ToList();

            TCPViewModel tcp = new TCPViewModel
            {
                Topic = topics,  
                PaginatedTopics = paginatedTopics,
                Contacts = contacts, 
                PaginatedProjects = paginatedProjects 
            };

            ViewBag.CurrentPage = topicPage;
            ViewBag.TotalPages = totalTopicPages;

            ViewBag.CurrentProjectPage = projectPage;
            ViewBag.TotalProjectPages = totalProjectPages;

            return View(tcp);
        }
        public IActionResult CareerPathways()
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
        public async Task<IActionResult> ProjectsPartialViewUpdate(int page = 1, int pageSize = 6)
        {
            List<Project> projects = await _projectRepository.GetAll();

            int totalprojects = projects.Count();
            int totalPages = (int)Math.Ceiling(totalprojects / (double)pageSize);
            List<Project> paginatedProjects = projects.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentProjectPage = page;
            ViewBag.TotalProjectPages = totalPages;

            return PartialView("_allProjectsPartial", paginatedProjects);
        }


        [HttpGet]
        public async Task<IActionResult> topicNavBarUpdate()
        {
            List<Topic> topics = await _topicRepository.GetAll();

            return PartialView("_topicsNavBarPartial", topics);
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

            await _contactRepository.Add(contact);

            return Ok("Your message has been successfully sent.");
        }

        public async Task<IActionResult> viewProjectDetails()
        {
            string? id = Request.Query["id"];

            return View(await _projectRepository.GetById(int.Parse(id)));
        }

        public async Task<IActionResult> Topic(int id, string topicName)
        {
            List<Lecture> lectures = await _lectureRepository.GetAll();
            var filteredLectures = lectures.Where(lecture => lecture.TopId == id).ToList();

            ViewBag.TopicName = topicName;
            ViewBag.TopicId = id;

            return View(filteredLectures);
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
        public async Task<IActionResult> SearchProjects(string projectName)
        {
            if (projectName == null)
            {
                return RedirectToAction("ProjectsPartialViewUpdate", "Home");
            }
            else
            {
                List<Project> list = await _projectRepository.GetAll();
                var filteredList = list.Where(a => a.Title.Contains(projectName, StringComparison.OrdinalIgnoreCase)).ToList();
                return PartialView("_allProjectsPartial", filteredList);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
