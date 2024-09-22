using Application.Services;
using Core.Entities;
using Mastering.NET.Models;
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
        public AdminController(GenericService<Topic> topicRepository, GenericService<Lecture> lectureRepository, IWebHostEnvironment env, GenericService<Contact> contactRepository, GenericService<Project> projectRepository)
        {
            _topicRepository = topicRepository;
            _lectureRepository = lectureRepository;
            _env = env;
            _contactRepository = contactRepository;
            _projectRepository = projectRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddTutorial(string tutorialName)
        {
            // Create a new topic and add it to the database
            Topic t = new Topic { TopicName = tutorialName };
            await _topicRepository.Add(t);

            // Get the updated list of topics
            List<Topic> topics = await _topicRepository.GetAll();

            // Return the updated partial view
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

            await _lectureRepository.Add(lecture);

            List<Lecture> lectures = await _lectureRepository.GetAll();
            var filteredLectures = lectures.Where(lecture => lecture.TopId == topicId).ToList();

            return PartialView("_leftNavPartial", filteredLectures);
            
        }

        public string getFilePath(IFormFile myFile)
        {
            if (myFile != null && myFile.Length > 0)
            {
                string folderPath = Path.Combine("UploadedFiles", "TutorialNotes"); // Relative path from wwwroot
                string fileName = Path.Combine(folderPath, Guid.NewGuid().ToString() + myFile.FileName);
                string wwwRootPath = _env.WebRootPath;
                if (!Directory.Exists(Path.Combine(wwwRootPath, folderPath))) // Check if path exists relative to wwwroot
                {
                    Directory.CreateDirectory(Path.Combine(wwwRootPath, folderPath)); // Create directory if needed
                }

                string filePath = Path.Combine(wwwRootPath, fileName); // Full path for saving
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    myFile.CopyTo(fileStream);
                }
                return $"\\{fileName}"; // Return file path relative to wwwroot
            }
            return string.Empty;
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
        public async Task<IActionResult> DeleteTutorial(int id)
        {
            await _topicRepository.Delete(id);

            List<Topic> topics = await _topicRepository.GetAll();

            return PartialView("_OffCanvasPartial", topics);
        }

        [HttpPost]
        public async Task AddProject(string projectTitle, string projectDescription, string gitHubLink, List<IFormFile> projectImages)
        {
            if (string.IsNullOrEmpty(projectTitle) || string.IsNullOrEmpty(projectDescription) || string.IsNullOrEmpty(gitHubLink))
            {
                BadRequest("Invalid project data.");
            }

            // Process the images - store them and generate URLs
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

            var newProject = new Project
            {
                Title = projectTitle,
                Description = projectDescription,
                GitHubLink = gitHubLink,
                ImageUrls = string.Join(" ", imageUrls) // Space-separated image URLs
            };

            await _projectRepository.Add(newProject);
        }

        [HttpPost]
        public async Task DeleteProject(int id)
        {
            await _projectRepository.Delete(id);  
        }


    }
}
