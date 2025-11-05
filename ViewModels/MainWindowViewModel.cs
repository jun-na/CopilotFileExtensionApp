using R3;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CopilotExtensionApp.Models;

namespace CopilotExtensionApp.ViewModels
{
    public class MainWindowViewModel
    {
        public BindableReactiveProperty<string> CurrentPath { get; } = new(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        
        public ObservableCollection<FileSystemItem> RootItems { get; } = new();
        
        public ObservableCollection<FileSystemItem> SelectedItems { get; } = new();
        
        public BindableReactiveProperty<string> StatusMessage { get; } = new("準備完了");
        
        public ReactiveCommand<Unit> RefreshCommand { get; }

        public MainWindowViewModel()
        {
            RefreshCommand = Observable.Return(true).ToReactiveCommand<Unit>();
            RefreshCommand.Subscribe(_ => LoadFileSystem());
            
            LoadFileSystem();
        }

        public void LoadFileSystem()
        {
            try
            {
                RootItems.Clear();
                
                if (Directory.Exists(CurrentPath.Value))
                {
                    var rootItem = new FileSystemItem(CurrentPath.Value);
                    rootItem.IsExpanded = true;
                    RootItems.Add(rootItem);
                }
                
                StatusMessage.Value = $"ファイルシステム読み込み完了";
            }
            catch (Exception ex)
            {
                StatusMessage.Value = $"エラー: {ex.Message}";
            }
        }
    }
}
