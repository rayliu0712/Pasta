using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Pasta;

public partial class MainWindow
{
  [LibraryImport("dwmapi.dll")]
  private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

  private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

  private void EnableDarkMode()
  {
    var hwnd = new WindowInteropHelper(this).EnsureHandle();
    int darkMode = 1;
    DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
  }
}
