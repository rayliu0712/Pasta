using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
namespace Pasta;

public partial class MainWindow : Window
{
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
    Process.Start("notepad.exe", _contentPath);
  }

  private void EditImagesButton_Click(object sender, RoutedEventArgs e)
  {
    Process.Start("explorer.exe", _imagesPath);
  }

  private void HideMessage()
  {
    StatusText.Visibility = Visibility.Hidden;
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

  private async void CopyContentButton_Click(object sender, RoutedEventArgs e)
  {
    HideMessage();

    if (_cachedContent.Length == 0)
    {
      ShowErrorMessage("沒有內容");
      return;
    }

    Mouse.OverrideCursor = Cursors.Wait;

    if (await TryCopyContent())
      ShowOkMessage("複製內容成功");
    else
      ShowErrorMessage("複製內容失敗");

    Mouse.OverrideCursor = null;
  }

  private async void CopyImagesButton_Click(object sender, RoutedEventArgs e)
  {
    HideMessage();

    if (_cachedImages.Length == 0)
    {
      ShowErrorMessage("沒有圖片");
      return;
    }

    Mouse.OverrideCursor = Cursors.Wait;

    if (await TryCopyImages())
      ShowOkMessage("複製圖片成功");
    else
      ShowErrorMessage("複製圖片失敗");

    Mouse.OverrideCursor = null;
  }
}
