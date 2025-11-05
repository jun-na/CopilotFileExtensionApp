using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using R3;
using CopilotExtensionApp.Models;

namespace CopilotExtensionApp.ViewModels
{
    public class MainWindowViewModel
    {
        public BindableReactiveProperty<string> CurrentPath { get; set; } = new();
        public BindableReactiveProperty<string> StatusMessage { get; set; } = new("準備完了");
        public ObservableCollection<FileItem> AllFiles { get; set; } = new();
        
        public ReactiveCommand<Unit> RefreshCommand { get; set; }

        public MainWindowViewModel()
        {
            // 初期パスをデスクトップに設定
            CurrentPath.Value = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            RefreshCommand = new ReactiveCommand<Unit>();
            RefreshCommand.Subscribe(_ => LoadFiles());
            
            LoadFiles();
        }

        public void LoadFiles()
        {
            AllFiles.Clear();
            
            if (!Directory.Exists(CurrentPath.Value))
            {
                StatusMessage.Value = "フォルダが存在しません";
                return;
            }

            try
            {
                var files = Directory.GetFiles(CurrentPath.Value, "*.*", SearchOption.AllDirectories)
                    .Where(f => !IsSystemFile(f))
                    .OrderBy(f => f)
                    .Select(f => new FileItem(f));

                foreach (var file in files)
                {
                    AllFiles.Add(file);
                }

                StatusMessage.Value = $"{AllFiles.Count}件のファイルを読み込みました";
            }
            catch (Exception ex)
            {
                StatusMessage.Value = $"エラー: {ex.Message}";
            }
        }

        private bool IsSystemFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath).ToLower();
            var systemFiles = new[] { "thumbs.db", "desktop.ini", ".ds_store" };
            return systemFiles.Contains(fileName);
        }
    }
}
