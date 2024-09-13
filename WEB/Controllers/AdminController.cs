using Application.Services;
using Core.Entities;
using Mastering.NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mastering.NET.Controllers
{
    [Authorize(Policy = "AdminPolicy")]    
    public class AdminController : Controller
    {
        private readonly GenericService<Topic> _topicRepository;
        private readonly GenericService<Lecture> _lectureRepository;
        private readonly GenericService<Contact> _contactRepository;
        private readonly IWebHostEnvironment _env;
        public AdminController(GenericService<Topic> topicRepository, GenericService<Lecture> lectureRepository, IWebHostEnvironment env, GenericService<Contact> contactRepository)
        {
            _topicRepository = topicRepository;
            _lectureRepository = lectureRepository;
            _env = env;
            _contactRepository = contactRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddTutorial(string tutorialName)
        {
            Topic t = new Topic { TopicName = tutorialName };
            await _topicRepository.Add(t);

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

    }
}
