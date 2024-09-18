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

            // Get the updated list of topics and lectures
            List<Topic> topics = await _topicRepository.GetAll();
            List<Lecture> lectures = await _lectureRepository.GetAll();

            // Create the list of TopicLectureViewModel to pass to the view
            List<TopicLectureViewModel> topicLectureViewModels = new List<TopicLectureViewModel>();

            foreach (var topic in topics)
            {
                var correspondingLectures = lectures.Where(l => l.TopId == topic.Id).ToList();

                TopicLectureViewModel viewModel = new TopicLectureViewModel
                {
                    Topic = topic,
                    Lectures = correspondingLectures
                };

                topicLectureViewModels.Add(viewModel);
            }


            // Return the updated partial view
            return PartialView("_OffCanvasPartial", topicLectureViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> UploadTutorial(string fileName, IFormFile fileUpload, int topicId)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                string filePath = getFilePath(fileUpload); 

                Lecture lecture = new Lecture
                {
                    LectureTitle = fileName,
                    FilePath = filePath,
                    TopId = topicId
                };

                await _lectureRepository.Add(lecture);

                List<Topic> topics = await _topicRepository.GetAll();
                List<Lecture> lectures = await _lectureRepository.GetAll();

                List<TopicLectureViewModel> topicLectureViewModels = new List<TopicLectureViewModel>();

                var topicName = "";

                foreach (var topic in topics)
                {
                    var correspondingLectures = lectures.Where(l => l.TopId == topic.Id).ToList();

                    TopicLectureViewModel viewModel = new TopicLectureViewModel
                    {
                        Topic = topic,
                        Lectures = correspondingLectures
                    };

                    topicLectureViewModels.Add(viewModel);

                    if (topic.Id==topicId)
                    {
                        topicName = topic.TopicName;
                    }
                }

                return PartialView("_OffCanvasPartial", topicLectureViewModels);
            }

            return BadRequest("Invalid file upload.");
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
        public async Task<IActionResult> DeleteLecture(int id)
        {
            await _lectureRepository.Delete(id); 

            List<Topic> topics = await _topicRepository.GetAll();
            List<Lecture> lectures = await _lectureRepository.GetAll();

            List<TopicLectureViewModel> topicLectureViewModels = new List<TopicLectureViewModel>();

            foreach (var topic in topics)
            {
                var correspondingLectures = lectures.Where(l => l.TopId == topic.Id).ToList();

                TopicLectureViewModel viewModel = new TopicLectureViewModel
                {
                    Topic = topic,
                    Lectures = correspondingLectures
                };

                topicLectureViewModels.Add(viewModel);
            }

            return PartialView("_OffCanvasPartial", topicLectureViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTutorial(int id)
        {
            await _topicRepository.Delete(id);

            List<Topic> topics = await _topicRepository.GetAll();
            List<Lecture> lectures = await _lectureRepository.GetAll();

            List<TopicLectureViewModel> topicLectureViewModels = new List<TopicLectureViewModel>();

            foreach (var topic in topics)
            {
                var correspondingLectures = lectures.Where(l => l.TopId == topic.Id).ToList();

                TopicLectureViewModel viewModel = new TopicLectureViewModel
                {
                    Topic = topic,
                    Lectures = correspondingLectures
                };

                topicLectureViewModels.Add(viewModel);
            }

            return PartialView("_OffCanvasPartial", topicLectureViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(string projectTitle, string projectDescription, string gitHubLink, List<IFormFile> projectImages)
        {
            if (string.IsNullOrEmpty(projectTitle) || string.IsNullOrEmpty(projectDescription) || string.IsNullOrEmpty(gitHubLink))
            {
                return BadRequest("Invalid project data.");
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

            var allProjects = await _projectRepository.GetAll(); 
            return PartialView("_allProjectsPartial", allProjects);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectRepository.Delete(id);           
            var allProjects = await _projectRepository.GetAll();
            return PartialView("_allProjectsPartial", allProjects);
        }


    }
}
