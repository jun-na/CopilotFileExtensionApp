using System.Collections.ObjectModel;
using System.IO;

namespace CopilotExtensionApp.Models
{
    public class FileSystemItem
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public bool IsExpanded { get; set; }
        public ObservableCollection<FileSystemItem> Children { get; set; } = new();
        
        public FileSystemItem? Parent { get; set; }

        public FileSystemItem(string path, FileSystemItem? parent = null)
        {
            FullPath = path;
            Parent = parent;
            Name = Path.GetFileName(path);
            IsDirectory = Directory.Exists(path);
            
            if (IsDirectory)
            {
                LoadChildren();
            }
        }

        private void LoadChildren()
        {
            try
            {
                if (!IsDirectory) return;

                Children.Clear();
                
                // サブフォルダを追加
                foreach (var dir in Directory.GetDirectories(FullPath))
                {
                    Children.Add(new FileSystemItem(dir, this));
                }
                
                // ファイルを追加
                foreach (var file in Directory.GetFiles(FullPath))
                {
                    Children.Add(new FileSystemItem(file, this));
                }
            }
            catch (Exception)
            {
                // アクセス権限などで読み込めない場合は無視
            }
        }

        public void Refresh()
        {
            if (IsDirectory)
            {
                LoadChildren();
            }
        }
    }
}
