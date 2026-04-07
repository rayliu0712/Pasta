using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace Pasta;

public partial class MainWindow : Window
{
  private static readonly string _dataPath = Path.Combine(AppContext.BaseDirectory, "data");
  private static readonly string _imagesPath = Path.Combine(_dataPath, "圖片");
  private const string _contentFilename = "內容.txt";
  private static readonly string _contentPath = Path.Combine(_dataPath, _contentFilename);

  private readonly FileSystemWatcher _contentWatcher;
  private readonly FileSystemWatcher _imagesWatcher;

  private string _cachedContent = "";
  private string[] _cachedImages = [];

  public MainWindow()
  {
    InitializeComponent();

    if (Directory.Exists(_imagesPath))
      ReloadImages();
    else
      Directory.CreateDirectory(_imagesPath);

    if (File.Exists(_contentPath))
      ReloadContent();
    else
      File.Create(_contentPath).Dispose();

    _contentWatcher = new()
    {
      Path = _dataPath,
      Filter = _contentFilename,
      NotifyFilter = NotifyFilters.LastWrite,
      EnableRaisingEvents = true
    };
    _contentWatcher.Changed += OnContentChanged;

    _imagesWatcher = new()
    {
      Path = _imagesPath,
      Filter = "*.*",
      NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
      EnableRaisingEvents = true
    };
    _imagesWatcher.Created += OnImagesChanged;
    _imagesWatcher.Deleted += OnImagesChanged;
    _imagesWatcher.Renamed += OnImagesChanged;
    _imagesWatcher.Changed += OnImagesChanged;
  }

  private void OnContentChanged(object sender, FileSystemEventArgs e)
  {
    // FileSystemWatcher 事件在執行緒池執行，需透過 Dispatcher 切回 UI 執行緒才能安全存取 WPF 元件
    Dispatcher.InvokeAsync(ReloadContent);
  }

  private void OnImagesChanged(object sender, FileSystemEventArgs e)
  {
    Dispatcher.InvokeAsync(ReloadImages);
  }

  protected override void OnClosed(EventArgs e)
  {
    _contentWatcher.Dispose();
    _imagesWatcher.Dispose();
    base.OnClosed(e);
  }

  private async void ReloadContent()
  {
    using FileStream fs = new(
      _contentPath,
      FileMode.Open,
      FileAccess.Read,
      FileShare.ReadWrite  // 允許其他程式在你開著這個 FileStream 的同時，也能對同一個檔案進行讀寫
    );
    using StreamReader sr = new(fs);
    _cachedContent = (await sr.ReadToEndAsync()).Trim();
  }

  private void ReloadImages()
  {
    _cachedImages = Directory.GetFiles(_imagesPath);
  }

  private static Task<bool> RunOnStaThread(Action action)
  {
    // Clipboard API 需要 STA 執行緒，但 Task.Run 預設使用 MTA 執行緒池，因此需要手動建立 STA 執行緒

    TaskCompletionSource<bool> tsc = new();

    Thread thread = new(() =>
    {
      try
      {
        action();
        tsc.SetResult(true);
      }
      catch (COMException)
      {
        tsc.SetResult(false);
      }
      catch (Exception e)
      {
        tsc.SetException(e);
      }
    })
    {
      IsBackground = true
    };

    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();

    return tsc.Task;
  }

  private Task<bool> TryCopyContent()
  {
    return RunOnStaThread(() =>
    {
      Clipboard.Clear();
      Clipboard.SetText(_cachedContent);
    });
  }

  private Task<bool> TryCopyImages()
  {
    return RunOnStaThread(() =>
    {
      Clipboard.Clear();
      Clipboard.SetFileDropList([.. _cachedImages]);
    });
  }
}
