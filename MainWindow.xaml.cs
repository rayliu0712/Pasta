using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using SendKeys = System.Windows.Forms.SendKeys;

namespace Pasta;

public partial class MainWindow : Window
{
  private static readonly string _dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pasta", "data");
  private static readonly string _textPath = Path.Combine(_dataPath, "text.txt");
  private static readonly string _imagesPath = Path.Combine(_dataPath, "images");

  static MainWindow()
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
    Process.Start("explorer.exe", _dataPath);
  }

  private async void PasteButton_Click(object sender, RoutedEventArgs e)
  {
    SendKeys.SendWait("%{tab}");

    string textToCopy = await File.ReadAllTextAsync(_textPath);
    textToCopy = textToCopy.Trim();
    Clipboard.SetText(textToCopy);
    SendKeys.SendWait("^v");

    string[] imagesToCopy = Directory.GetFiles(_imagesPath);
    if (imagesToCopy.Length == 0) return;

    var dropList = new StringCollection();
    dropList.AddRange(imagesToCopy);
    Clipboard.SetFileDropList(dropList);
    SendKeys.SendWait("^v");
  }
}
