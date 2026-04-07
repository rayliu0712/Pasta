using System;
using System.Collections.Specialized;
using System.IO;
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

    if (File.Exists(_contentPath))
      ReloadContent();
    else
      File.Create(_contentPath).Dispose();

    if (Directory.Exists(_imagesPath))
      ReloadImages();
    else
      Directory.CreateDirectory(_imagesPath);

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
      NotifyFilter = NotifyFilters.LastWrite,
      EnableRaisingEvents = true
    };
    _imagesWatcher.Created += OnImagesChanged;
    _imagesWatcher.Deleted += OnImagesChanged;
    _imagesWatcher.Renamed += OnImagesChanged;
    _imagesWatcher.Changed += OnImagesChanged;
  }

  private void OnContentChanged(object sender, FileSystemEventArgs e)
  {
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
    using FileStream fs = new(_contentPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    using StreamReader sr = new(fs);
    _cachedContent = (await sr.ReadToEndAsync()).Trim();
  }

  private void ReloadImages()
  {
    _cachedImages = Directory.GetFiles(_imagesPath);
  }

  private bool CopyContent()
  {
    try
    {
      Clipboard.SetText(_cachedContent);
      return true;
    }
    catch
    {
      return false;
    }
  }

  private bool CopyImages()
  {
    try
    {
      StringCollection dropList = [];
      dropList.AddRange(_cachedImages);
      Clipboard.SetFileDropList(dropList);
      return true;
    }
    catch
    {
      return false;
    }
  }
}
