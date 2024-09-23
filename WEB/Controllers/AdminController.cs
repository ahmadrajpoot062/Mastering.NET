using Application.Services;
using Core.Entities;
using Infrastructure.Hubs;
using Mastering.NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static System.Net.Mime.MediaTypeNames;

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
        public async Task<IActionResult> AddTutorial(string tutorialName)
        {
            Topic t = new Topic { TopicName = tutorialName };
            await _topicRepository.Add(t);

            List<Topic> topics = await _topicRepository.GetAll();

            return PartialView("_OffCanvasPartial", topics);
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


        //public string getFilePath(IFormFile myFile)
        //{
        //    if (myFile != null && myFile.Length > 0)
        //    {
        //        string folderPath = Path.Combine("UploadedFiles", "TutorialNotes"); // Relative path from wwwroot
        //        string fileName = Path.Combine(folderPath, Guid.NewGuid().ToString() + myFile.FileName);
        //        string wwwRootPath = _env.WebRootPath;
        //        if (!Directory.Exists(Path.Combine(wwwRootPath, folderPath))) // Check if path exists relative to wwwroot
        //        {
        //            Directory.CreateDirectory(Path.Combine(wwwRootPath, folderPath)); // Create directory if needed
        //        }

        //        string filePath = Path.Combine(wwwRootPath, fileName); // Full path for saving
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            myFile.CopyTo(fileStream);
        //        }
        //        return $"\\{fileName}"; // Return file path relative to wwwroot
        //    }
        //    return string.Empty;
        //}


        [HttpPost]
        public async Task<IActionResult> DeleteLecture(int id, int topicid)
        {
            await _lectureRepository.Delete(id);

            List<Lecture> lectures = await _lectureRepository.GetAll();
            var filteredLectures = lectures.Where(lecture => lecture.TopId == topicid).ToList();

            return PartialView("_leftNavPartial", filteredLectures);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTutorial(int id)
        {
            await _topicRepository.Delete(id);

            List<Topic> topics = await _topicRepository.GetAll();

            return PartialView("_OffCanvasPartial", topics);
        }


        [HttpPost]
        public async Task AddProject(string projectTitle, string projectDescription, string gitHubLink, List<IFormFile> projectImages, IFormFile userManual)
        {
            if (string.IsNullOrEmpty(projectTitle) || string.IsNullOrEmpty(projectDescription) || string.IsNullOrEmpty(gitHubLink))
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
                    imageUrls.Add( $"\\{fileName}"); // Return file path relative to wwwroot
                }                
            }

            string userManualUrl = null;
            string userManualName = null;
            var newProject = new Project();

            if (userManual != null && userManual.Length > 0) 
            {
                string folderPath2 = Path.Combine("UploadedFiles", "ProjectUserManuals"); // Relative path from wwwroot
                string fileName2 = Path.Combine(folderPath2, Guid.NewGuid().ToString() + userManual.FileName.Replace(" ", ""));
                string wwwRootPath2 = _env.WebRootPath;
                if (!Directory.Exists(Path.Combine(wwwRootPath2, folderPath2))) // Check if path exists relative to wwwroot
                {
                    Directory.CreateDirectory(Path.Combine(wwwRootPath2, folderPath2)); // Create directory if needed
                }

                string filePath2 = Path.Combine(wwwRootPath2, fileName2); // Full path for saving
                using (var fileStream2 = new FileStream(filePath2, FileMode.Create))
                {
                    userManual.CopyTo(fileStream2);
                }
                userManualUrl = $"\\{fileName2}";
                userManualName = userManual.FileName;


                newProject.Title = projectTitle;
                newProject.Description = projectDescription;
                newProject.GitHubLink = gitHubLink;
                newProject.ImageUrls = string.Join(" ", imageUrls); // Space-separated image URLs
                newProject.UserManual = userManualUrl + " " + userManualName;
                
            }
            else
            {
                newProject.Title = projectTitle;
                newProject.Description = projectDescription;
                newProject.GitHubLink = gitHubLink;
                newProject.ImageUrls = string.Join(" ", imageUrls); // Space-separated image URLs
                newProject.UserManual = null;
            }
            
            await _projectRepository.Add(newProject);
        }


        [HttpPost]
        public async Task DeleteProject(int id)
        {
            await _projectRepository.Delete(id);  
        }

    }
}
