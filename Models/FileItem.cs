using System.IO;
using System.ComponentModel;

namespace CopilotExtensionApp.Models
{
    public class FileItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string LastModified { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public FileItem(string filePath)
        {
            FullPath = filePath;
            Name = Path.GetFileName(filePath);
            
            var fileInfo = new FileInfo(filePath);
            FileType = fileInfo.Extension.ToUpper();
            FileSize = FormatFileSize(fileInfo.Length);
            LastModified = fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm");
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.#} {sizes[order]}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
