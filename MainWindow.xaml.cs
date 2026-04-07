using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
namespace Pasta;

public partial class MainWindow : Window
{
  private void HideMessage()
  {
    StatusText.Visibility = Visibility.Hidden;
  }

  private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    DragMove();
  }

  private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
  {
    ProcessStartInfo info = new()
    {
      FileName = e.Uri.AbsoluteUri,
      UseShellExecute = true
    };
    Process.Start(info);
  }

  private void EditContentButton_Click(object sender, RoutedEventArgs e)
  {
    HideMessage();
    Process.Start("notepad.exe", _contentPath);
  }

  private void EditImagesButton_Click(object sender, RoutedEventArgs e)
  {
    HideMessage();
    Process.Start("explorer.exe", _imagesPath);
  }

  private void ShowOkMessage(string message)
  {
    StatusText.Text = message;
    StatusText.Foreground = new SolidColorBrush(Colors.Green);
    StatusText.Visibility = Visibility.Visible;
  }

  private void ShowErrorMessage(string message)
  {
    StatusText.Text = message;
    StatusText.Foreground = new SolidColorBrush(Colors.Red);
    StatusText.Visibility = Visibility.Visible;
  }

  private void CopyContentButton_Click(object sender, RoutedEventArgs e)
  {
    HideMessage();

    if (_cachedContent.Length == 0)
    {
      ShowErrorMessage("沒有內容");
      return;
    }

    if (CopyContent())
      ShowOkMessage("複製內容成功");
    else
      ShowErrorMessage("複製內容失敗");
  }

  private void CopyImagesButton_Click(object sender, RoutedEventArgs e)
  {
    HideMessage();

    if (_cachedImages.Length == 0)
    {
      ShowErrorMessage("沒有圖片");
      return;
    }

    if (CopyImages())
      ShowOkMessage("複製圖片成功");
    else
      ShowErrorMessage("複製圖片失敗");
  }
}
