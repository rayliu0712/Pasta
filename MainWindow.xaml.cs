using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using SendKeys = System.Windows.Forms.SendKeys;

namespace Pasta;

public partial class MainWindow : Window
{
  private static readonly string _dataPath = Path.Combine(AppContext.BaseDirectory, "data");
  private static readonly string _textPath = Path.Combine(_dataPath, "text.txt");
  private static readonly string _imagesPath = Path.Combine(_dataPath, "images");

  static MainWindow()
  {
    CreatePath();
  }

  private static void CreatePath()
  {
    Directory.CreateDirectory(_dataPath);
    Directory.CreateDirectory(_imagesPath);

    if (!File.Exists(_textPath))
      File.Create(_textPath).Dispose();
  }

  public MainWindow()
  {
    InitializeComponent();
    EnableDarkMode();
  }

  private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    DragMove();
  }

  private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
  {
    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
  }

  private void OpenButton_Click(object sender, RoutedEventArgs e)
  {
    CreatePath();
    Process.Start("explorer.exe", _dataPath);
  }

  private async void PasteButton_Click(object sender, RoutedEventArgs e)
  {
    CreatePath();
    ErrorText.Visibility = Visibility.Collapsed;

    SendKeys.SendWait("%{tab}");

    using var fs = new FileStream(_textPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    using var sr = new StreamReader(fs);
    string textToCopy = await sr.ReadToEndAsync();
    textToCopy = textToCopy.Trim();
    if (textToCopy.Length > 0)
    {
      bool succeed = await Retry(() => Clipboard.SetText(textToCopy));
      if (succeed)
        SendKeys.SendWait("^v");
    }

    string[] imagesToCopy = Directory.GetFiles(_imagesPath);
    if (imagesToCopy.Length > 0)
    {
      var dropList = new StringCollection();
      dropList.AddRange(imagesToCopy);

      bool succeed = await Retry(() => Clipboard.SetFileDropList(dropList));
      if (succeed)
        SendKeys.SendWait("^v");
    }
  }

  private async Task<bool> Retry(Action action)
  {
    for (int i = 0; i < 10; i++)
    {
      try
      {
        action();
        return true;
      }
      catch (COMException ex) when ((uint)ex.ErrorCode == 0x800401D0)
      {
        await Task.Delay(50);
      }
    }

    ErrorText.Text = "Failed to access clipboard";
    ErrorText.Visibility = Visibility.Visible;
    return false;
  }
}
