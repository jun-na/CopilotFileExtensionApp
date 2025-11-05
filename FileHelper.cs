using System.IO;
using System.Runtime.InteropServices;

namespace CopilotExtensionApp
{
    [ComVisible(true)]
    public class FileHelper
    {
        public string GetFileBase64(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var bytes = File.ReadAllBytes(filePath);
                    return Convert.ToBase64String(bytes);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public string GetFileName(string filePath)
        {
            try
            {
                return Path.GetFileName(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public string GetContentType(string filePath)
        {
            try
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                return extension switch
                {
                    ".txt" => "text/plain",
                    ".md" => "text/markdown",
                    ".json" => "application/json",
                    ".xml" => "text/xml",
                    ".csv" => "text/csv",
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    _ => "application/octet-stream"
                };
            }
            catch (Exception)
            {
                return "application/octet-stream";
            }
        }
    }
}
