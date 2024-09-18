using Mastering.NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Application.Services;
using Core.Entities;

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
            List<Topic> topics = await _topicRepository.GetAll();
            List<Lecture> lectures = await _lectureRepository.GetAll();
            List<Contact> contacts = await _contactRepository.GetAll();
            List<Project> projects = await _projectRepository.GetAll();
            if (projects==null)
            {
                Console.WriteLine("null");
            }

            List<TopicLectureViewModel> topicLectureViewModels = new List<TopicLectureViewModel>();

            foreach (var topic in topics)
            {
                var correspondingLectures = lectures.Where(l => l.TopId == topic.Id).ToList();

                TopicLectureViewModel viewModel = new TopicLectureViewModel
                {
                    Topic = topic,
                    Lectures = correspondingLectures,
                    Contacts = contacts,
                    Projects= projects                    
                };

                topicLectureViewModels.Add(viewModel);
            }
            return View(topicLectureViewModels);
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
                Message = message
            };

            await _contactRepository.Add(contact);

            return Ok("Your message has been successfully sent.");
        }
        public async Task<IActionResult> ViewNotes(string filePath, string fileName)
        {

            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
            {
                return NotFound("File path or file name is not provided.");
            }

            string extension = Path.GetExtension(filePath)?.ToLower();

            var model = new { FilePath = filePath, FileName = fileName, Extension = extension };
            return View(model);
        }

        public async Task<IActionResult> viewProjectDetails()
        {
            string? id = Request.Query["id"];

            return View(await _projectRepository.GetById(int.Parse(id)));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
