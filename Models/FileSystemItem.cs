using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;

namespace CopilotExtensionApp.Models
{
    public class FileSystemItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isExpanded;
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public ObservableCollection<FileSystemItem> Children { get; set; } = new();
        public FileSystemItem? Parent { get; set; }

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

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                    
                    if (value && IsDirectory && Children.Count == 0)
                    {
                        LoadChildren();
                    }
                }
            }
        }

        public FileSystemItem(string path, FileSystemItem? parent = null)
        {
            FullPath = path;
            Parent = parent;
            Name = Path.GetFileName(path);
            IsDirectory = Directory.Exists(path);
            
            if (IsDirectory)
            {
                // ダミーの子アイテムを追加して展開可能に表示
                Children.Add(new FileSystemItem("Loading..."));
            }
        }

        public void LoadChildren()
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
