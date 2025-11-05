# Copilot File Explorer

Microsoft Copilotにファイルを簡単に渡すためのWPFデスクトップアプリケーション。

## 概要

このアプリケーションは、ローカルファイルをFancyTreeで階層表示し、選択したファイルをMicrosoft Copilotに直接送信することを目的としています。ファイルエクスプローラーとCopilotを並べて表示でき、効率的な作業が可能です。

## 主な機能

### 📁 ファイルエクスプローラー
- **FancyTree**: 高機能なツリービューでファイル・フォルダを階層表示
- **複数選択**: チェックボックスで複数ファイルを選択可能
- **ファイルアイコン**: 拡張子に応じたアイコンを自動表示
- **ファイル情報**: サイズと更新日時を表示
- **検索・フィルター**: ファイル名でリアルタイム検索

### 🔄 パネル操作
- **入れ替え機能**: Copilotとファイルエクスプローラーの位置を入れ替え可能
- **リサイズ**: GridSplitterでパネル幅を調整
- **独立表示**: 各パネルを独立して操作

### 💾 設定保存
- **パス記憶**: 最後に選択したフォルダを自動保存
- **次回起動時復元**: 前回のフォルダを自動的に読み込み

### 🚀 Copilot連携
- **直接送信**: 選択ファイルをCopilotにドラッグ＆ドロップなしで送信
- **MIMEタイプ対応**: 各種ファイル形式に対応
- **エラーハンドリング**: 送信失敗時の詳細なエラー表示

## 対応ファイル形式

### テキストファイル
- `.txt`, `.md`, `.json`, `.xml`, `.csv`, `.tsv`, `.log`, `.ini`, `.yaml`, `.yml`
- `.html`, `.htm`, `.css`, `.js`, `.ts`, `.py`, `.java`, `.cpp`, `.c`, `.cs`, `.sql`

### Officeドキュメント
- `.pdf`, `.doc`, `.docx`, `.xls`, `.xlsx`, `.ppt`, `.pptx`, `.rtf`

### 画像ファイル
- `.png`, `.jpg`, `.jpeg`, `.gif`, `.bmp`, `.svg`, `.webp`

## インストール

### 前提条件
- Windows 10/11 (x64)
- WebView2ランタイム（通常はWindowsに標準搭載）

### リリース版のダウンロード
[Releasesページ](../../releases)から最新版のZIPファイルをダウンロードし、解凍して実行してください。

### ビルド方法
```bash
git clone <repository-url>
cd CopilotExtensionApp
dotnet build
dotnet run
```

### シングルバイナリでビルド
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### GitHub Actionsでの自動リリース
1. タグをプッシュ: `git tag v0.4.0 && git push origin v0.4.0`
2. GitHub Actionsが自動でビルド＆リリースを作成
3. ReleasesページからZIPをダウンロード可能

## 使用方法

1. **アプリ起動**: `dotnet run`でアプリを起動
2. **フォルダ選択**: 「フォルダ選択」ボタンで対象フォルダを選択
3. **ファイル選択**: FancyTreeでファイルのチェックボックスを選択
4. **Copilot送信**: 「Copilotに送信」ボタンで選択ファイルを送信
5. **パネル入れ替え**: 「⇄」ボタンでパネル位置を入れ替え

## 設定ファイル

設定は以下の場所に保存されます：
```
%APPDATA%\CopilotExtensionApp\settings.json
```

内容：
```json
{
  "LastPath": "C:\\Users\\username\\Documents\\Projects"
}
```

## 技術仕様

### フレームワーク
- **.NET 10.0 Windows**
- **WPF** (UIフレームワーク)
- **WebView2** (Webコンテンツ表示)
- **R3** (Reactive Extensions)

### 主要ライブラリ
- `Microsoft.Web.WebView2`: WebView2機能
- `R3`: Reactiveプログラミング
- `FancyTree`: JavaScriptツリービュー
- `jQuery`: JavaScriptライブラリ

### アーキテクチャ
- **MVVMパターン**: ViewModelでの状態管理
- **非同期処理**: async/awaitによるレスポンシブUI
- **WebView2連携**: C#とJavaScriptの双方向通信

## 開発情報

### プロジェクト構成
```
CopilotExtensionApp/
├── MainWindow.xaml          # メインウィンドウ
├── MainWindow.xaml.cs       # メインロジック
├── ViewModels/
│   └── MainWindowViewModel.cs
├── Models/
│   ├── FileData.cs
│   └── AppSettings.cs
├── wwwroot/
│   └── file-explorer.html   # FancyTree UI
└── FileHelper.cs            # ファイル操作ヘルパー
```

### 機能拡張
- 新しいファイル形式の追加: `GetContentType()`メソッドを修正
- UIカスタマイズ: `MainWindow.xaml`と`file-explorer.html`を編集
- 新機能追加: ViewModelに新しいReactivePropertyを追加

## ライセンス

MIT License

## コントリビューション

バグ報告や機能要望はIssueにてお願いします。プルリクエストも歓迎します。

## 更新履歴

### v0.4.0
- 初版リリース
- FancyTreeによるファイル表示
- Copilot連携機能
- パネル入れ替え機能
- 設定保存機能
