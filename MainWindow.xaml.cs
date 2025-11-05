using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CopilotExtensionApp.ViewModels;
using Microsoft.Web.WebView2.Core;
using CopilotExtensionApp.Models;
using Microsoft.Win32;

namespace CopilotExtensionApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // WebView2の初期化を待つ
        await CopilotWebView.EnsureCoreWebView2Async(null);
        
        // C#オブジェクトをJavaScriptに公開
        CopilotWebView.CoreWebView2.AddHostObjectToScript("fileHelper", new FileHelper());
    }

    private void FileSystemTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        // 選択されたアイテムをViewModelに反映
        ViewModel.SelectedItems.Clear();
        
        if (e.NewValue is FileSystemItem selectedItem)
        {
            // ファイルのみを選択対象にする
            if (!selectedItem.IsDirectory)
            {
                ViewModel.SelectedItems.Add(selectedItem);
            }
        }
    }

    private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
    {
        using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
        {
            dialog.Description = "フォルダを選択してください";
            dialog.SelectedPath = ViewModel.CurrentPath.Value;
            dialog.ShowNewFolderButton = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel.CurrentPath.Value = dialog.SelectedPath;
                ViewModel.LoadFileSystem();
            }
        }
    }

    private void SendToCopilotButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedFiles = ViewModel.SelectedItems.Where(x => !x.IsDirectory).ToList();
        
        if (selectedFiles.Count == 0)
        {
            ViewModel.StatusMessage.Value = "ファイルが選択されていません";
            return;
        }

        var filePaths = selectedFiles.Select(x => x.FullPath).ToList();
        
        // 直接送信処理を呼び出す
        _ = SendFilesToCopilotAsync(filePaths);
    }

    private async Task SendFilesToCopilotAsync(List<string> filePaths)
    {
        try
        {
            ViewModel.StatusMessage.Value = $"ファイルをCopilotに送信中... ({filePaths.Count}件)";

            foreach (var filePath in filePaths)
            {
                await SendSingleFileToCopilotAsync(filePath);
            }

            ViewModel.StatusMessage.Value = $"{filePaths.Count}件のファイルをCopilotに送信しました";
        }
        catch (Exception ex)
        {
            ViewModel.StatusMessage.Value = $"Copilotへの送信エラー: {ex.Message}";
        }
    }

    private async Task SendSingleFileToCopilotAsync(string filePath)
    {
        try
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            var fileInfo = new FileInfo(filePath);
            
            // 10MB制限
            if (fileInfo.Length > 10 * 1024 * 1024)
            {
                ViewModel.StatusMessage.Value = $"ファイルが大きすぎます ({fileName}): {fileInfo.Length / 1024 / 1024}MB (制限: 10MB)";
                return;
            }

            ViewModel.StatusMessage.Value = $"送信中: {fileName} ({fileInfo.Length / 1024}KB)";

            // HostObject経由でファイルデータを渡すJavaScriptを実行
            if (CopilotWebView.CoreWebView2 != null)
            {
                var script = $@"
(async function() {{
    try {{
        console.log('Uploading file via HostObject: {fileName}');
        
        // C#のFileHelperからBase64データを取得
        const fileHelper = window.chrome.webview.hostObjects.fileHelper;
        const base64Data = await fileHelper.GetFileBase64('{filePath.Replace("\\", "\\\\")}');
        const contentType = await fileHelper.GetContentType('{filePath.Replace("\\", "\\\\")}');
        
        if (base64Data) {{
            console.log('Base64 data received, length:', base64Data.length);
            
            // Base64からBlobを作成
            const byteCharacters = atob(base64Data);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {{
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }}
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], {{ type: contentType }});
            
            // Fileオブジェクトを作成
            const file = new File([blob], '{fileName}', {{ type: contentType }});
            console.log('File object created:', file.name, file.size, file.type);
            
            // ファイル入力に設定
            const fileInput = document.querySelector('input[type=""file""]') ||
                             document.querySelector('[data-testid=""file-input""]') ||
                             document.querySelector('input[accept*=""*""]');
            
            if (fileInput) {{
                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(file);
                fileInput.files = dataTransfer.files;
                
                fileInput.dispatchEvent(new Event('input', {{ bubbles: true }}));
                fileInput.dispatchEvent(new Event('change', {{ bubbles: true }}));
                
                console.log('File uploaded via HostObject:', file.name);
            }} else {{
                console.log('No file input found for HostObject method');
            }}
        }} else {{
            console.error('Failed to get Base64 data from C#');
        }}
        
    }} catch (error) {{
        console.error('HostObject method failed:', error);
    }}
}})();";

                await CopilotWebView.CoreWebView2.ExecuteScriptAsync(script);
                
                // 少し待機して処理を完了させる
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            ViewModel.StatusMessage.Value = $"ファイル送信エラー ({System.IO.Path.GetFileName(filePath)}): {ex.Message}";
        }
    }

    private string GetContentType(string fileName)
    {
        var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".txt" => "text/plain",
            ".md" => "text/markdown",
            ".json" => "application/json",
            ".xml" => "text/xml",
            ".csv" => "text/csv",
            ".tsv" => "text/tab-separated-values",
            ".log" => "text/plain",
            ".ini" => "text/plain",
            ".yaml" => "text/yaml",
            ".yml" => "text/yaml",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".html" => "text/html",
            ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "text/javascript",
            ".ts" => "text/typescript",
            ".py" => "text/x-python",
            ".java" => "text/x-java-source",
            ".cpp" => "text/x-c++src",
            ".c" => "text/x-csrc",
            ".cs" => "text/x-csharp",
            ".sql" => "text/x-sql",
            ".rtf" => "application/rtf",
            _ => "application/octet-stream"
        };
    }
}