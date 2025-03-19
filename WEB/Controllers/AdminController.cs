using Application.Services;
using Core.Entities;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Mastering.NET.Controllers
{
    [Authorize(Policy = "AdminPolicy")]    
    public class AdminController : Controller
    {
        private readonly GenericService<Topic> _topicRepository;
        private readonly GenericService<Lecture> _lectureRepository;
        private readonly GenericService<Contact> _contactRepository;
        private readonly GenericService<Project> _projectRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public AdminController(GenericService<Topic> topicRepository, GenericService<Lecture> lectureRepository, IWebHostEnvironment env, GenericService<Contact> contactRepository, GenericService<Project> projectRepository, IHubContext<NotificationHub> notificationHubContext)
        {
            _topicRepository = topicRepository;
            _lectureRepository = lectureRepository;
            _env = env;
            _contactRepository = contactRepository;
            _projectRepository = projectRepository;
            _notificationHubContext = notificationHubContext;
        }


        [HttpPost]
        public async Task AddTutorial(string tutorialName)
        {
            Topic t = new Topic { TopicName = tutorialName };
            await _topicRepository.Add(t);
        }


        [HttpPost]
        public async Task<IActionResult> UploadLecture(string title, string content, int topicId)
        {
            Lecture lecture = new Lecture
            {
                LectureTitle = title,
                htmlcontent = content,
                TopId = topicId
            };

            Topic topic = await _topicRepository.GetById(topicId);

            await _lectureRepository.Add(lecture);

            List<Lecture> lectures = await _lectureRepository.GetAll();
            var filteredLectures = lectures.Where(lecture => lecture.TopId == topicId).ToList();

            await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification",topicId, topic.TopicName, title);

            return PartialView("_leftNavPartial", filteredLectures);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLecture(int id, int topicid)
        {
            await _lectureRepository.Delete(id);

            List<Lecture> lectures = await _lectureRepository.GetAll();
            var filteredLectures = lectures.Where(lecture => lecture.TopId == topicid).ToList();

            return PartialView("_leftNavPartial", filteredLectures);
        }


        [HttpPost]
        public async Task DeleteTutorial(int id)
        {
            await _topicRepository.Delete(id);
        }

        [HttpPost]
        public async Task AddDotNetProject(string projectTitle, string projectDescription, IFormFile profileImage, List<IFormFile> projectImages)
        {
            await AddProject(projectTitle, projectDescription, profileImage, projectImages, ".NET");
        }

        [HttpPost]
        public async Task AddBlazorProject(string projectTitle, string projectDescription, IFormFile profileImage, List<IFormFile> projectImages)
        {
            await AddProject(projectTitle, projectDescription, profileImage, projectImages, "Blazor");
        }

        private async Task AddProject(string projectTitle, string projectDescription, IFormFile profileImage, List<IFormFile> projectImages, string projectType)
        {
            if (string.IsNullOrEmpty(projectTitle) || string.IsNullOrEmpty(projectDescription))
            {
                BadRequest("Invalid project data.");
            }

            List<string> imageUrls = new List<string>();

            foreach (var image in projectImages)
            {
                if (image != null && image.Length > 0)
                {
                    string folderPath = Path.Combine("UploadedFiles", "ProjectImages"); // Relative path from wwwroot
                    string fileName = Path.Combine(folderPath, Guid.NewGuid().ToString() + image.FileName.Replace(" ", ""));
                    string wwwRootPath = _env.WebRootPath;
                    if (!Directory.Exists(Path.Combine(wwwRootPath, folderPath))) // Check if path exists relative to wwwroot
                    {
                        Directory.CreateDirectory(Path.Combine(wwwRootPath, folderPath)); // Create directory if needed
                    }

                    string filePath = Path.Combine(wwwRootPath, fileName); // Full path for saving
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }
                    imageUrls.Add($"\\{fileName}"); // Return file path relative to wwwroot
                }
            }

            string profileImg = null;
            if (profileImage != null && profileImage.Length > 0)
            {
                string folderPath3 = Path.Combine("UploadedFiles", "ProjectsProfileImages"); // Relative path from wwwroot
                string fileName3 = Path.Combine(folderPath3, Guid.NewGuid().ToString() + profileImage.FileName.Replace(" ", ""));
                string wwwRootPath3 = _env.WebRootPath;
                if (!Directory.Exists(Path.Combine(wwwRootPath3, folderPath3))) // Check if path exists relative to wwwroot
                {
                    Directory.CreateDirectory(Path.Combine(wwwRootPath3, folderPath3)); // Create directory if needed
                }

                string filePath3 = Path.Combine(wwwRootPath3, fileName3); // Full path for saving
                using (var fileStream = new FileStream(filePath3, FileMode.Create))
                {
                    profileImage.CopyTo(fileStream);
                }
                profileImg = $"\\{fileName3}"; // Return file path relative to wwwroot
            }

            Project newProject = new Project();

            newProject.Title = projectTitle;
            newProject.Description = projectDescription;
            newProject.ProfileImageURL = profileImg;
            newProject.ImageUrls = string.Join(" ", imageUrls); // Space-separated image URLs            
            newProject.ProjectType = projectType;

            await _projectRepository.Add(newProject);
        }

        [HttpPost]
        public async Task DeleteDotNetProject(int id)
        {
            await DeleteProject(id);
        }

        [HttpPost]
        public async Task DeleteBlazorProject(int id)
        {
            await DeleteProject(id);
        }

        private async Task DeleteProject(int id)
        {
            await _projectRepository.Delete(id);
        }

        public async Task<IActionResult> MessageDetails(int id)
        {
            // Fetch the message by ID using the service/repository
            var message = await _contactRepository.GetById(id);

            // If no message is found, return a 404 page
            if (message == null)
            {
                return NotFound();
            }

            message.IsRead = true;
            await _contactRepository.Update(message);

            // Pass the message details to the view
            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> MessageDelete(int id)
        {
            Console.WriteLine(id);
            // Your logic to delete the contact by ID
            await _contactRepository.Delete(id); 

            return PartialView("_sendMessagesPartial",await _contactRepository.GetAll());   

        }

    }
}
