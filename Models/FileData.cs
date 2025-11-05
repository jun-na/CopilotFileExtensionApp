namespace CopilotExtensionApp.Models
{
    public class FileData
    {
        public string fullPath { get; set; } = string.Empty;
        public string relativePath { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public long size { get; set; }
        public DateTime lastModified { get; set; }
        public string extension { get; set; } = string.Empty;
    }
}
