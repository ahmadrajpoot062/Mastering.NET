namespace Core.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProfileImageURL {  get; set; }

        // Space-separated URLs
        public string ImageUrls { get; set; }
        public string ProjectType { get; set; } // Values: ".NET", "Blazor"
    }
}
