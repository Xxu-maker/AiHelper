using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class windowTransparent: MonoBehaviour
{
// 导入 user32.dll 库以使用 Windows API 函数

  [DllImport("user32.dll")]

  public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);



  // 定义一个结构来存储窗口边框的边距大小

  private struct MARGINS

  {

    public int cxLeftWidth;

    public int cxRightWidth;

    public int cyTopHeight;

    public int cyBottomHeight;

  }



  public struct Rect

  {

    public int Left;

    public int Top;

    public int Right;

    public int Bottom;



    public int Width => Right - Left;

    public int Height => Bottom - Top;

  }



  // 导入 user32.dll 以获取活动窗口句柄 (HWND)

  [DllImport("user32.dll")]

  private static extern IntPtr GetActiveWindow();



  // 导入 Dwmapi.dll 以将窗口边框扩展到客户区域

  [DllImport("Dwmapi.dll")]

  private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);



  // 导入 user32.dll 以修改窗口属性

  [DllImport("user32.dll")]

  private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

  [DllImport("user32.dll")]

  private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);



  // 导入 user32.dll 以设置窗口位置

  [DllImport("user32.dll", SetLastError = true)]

  static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);



  // 导入 user32.dll 以设置分层窗口属性 (透明度)

  [DllImport("user32.dll")]

  static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);



  [DllImport("user32.dll", SetLastError = true)]

  private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

  [DllImport("user32.dll")]

  private static extern bool GetWindowRect(System.IntPtr hWnd, out Rect lpRect);

  [DllImport("user32.dll")]

  private static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);



  // 代码中使用的常量和变量

  const int GWL_EXSTYLE = -20; // 修改窗口样式的索引

  const uint WS_EX_LAYERED = 0x00080000; // 分层窗口的扩展样式

  const uint WS_EX_TRANSPARENT = 0x00000020; // 透明窗口的扩展样式

  static readonly IntPtr HWND_TOPMOST = new IntPtr(-1); // 窗口插入位置（始终置顶）

  const uint LWA_COLORKEY = 0x00000001; // 设置颜色键的标志（用于透明度）

  private IntPtr hWnd; // 活动窗口的句柄



  private const int GWL_STYLE = -16;

  private const int WS_BORDER = 0x00800000;

  private const int WS_CAPTION = 0x00C00000;

  private const uint SWP_NOSIZE = 0x0001;

  private const uint SWP_NOMOVE = 0x0002;

  private const uint SWP_SHOWWINDOW = 0x0040;



  private void Start()

  {



     hWnd = GetActiveWindow();

    int style = (int)GetWindowLong(hWnd, GWL_STYLE);

    style &= ~WS_BORDER;

    style &= ~WS_CAPTION;

    SetWindowLong(hWnd, GWL_STYLE, style);

    SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

    // 显示一个消息框（仅用于演示目的）

    // MessageBox(new IntPtr(0), "Hello world", "Hello Dialog", 0);



    MARGINS margins = new MARGINS { cxLeftWidth = -1 };

    DwmExtendFrameIntoClientArea(hWnd, ref margins);

    //SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);

    //SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

    SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

    SetTransparentWindow(false);

    // 允许应用在后台运行

    Application.runInBackground = true;

  }



  public void UpdateWindowPosition(Vector2 offset)

  {

    if (GetWindowRect(hWnd, out Rect windowRect))

    {

      int newX = windowRect.Left + Mathf.RoundToInt(offset.x);

      int newY = windowRect.Top - Mathf.RoundToInt(offset.y);



      MoveWindow(hWnd, newX, newY, windowRect.Width, windowRect.Height, true);

    }

  }

  public void SetTransparentWindow(bool enable)

  {

    uint styles = GetWindowLong(hWnd, GWL_EXSTYLE);



    if (enable)

    {

      // 开启点击穿透

      SetWindowLong(hWnd, GWL_EXSTYLE, styles | WS_EX_LAYERED | WS_EX_TRANSPARENT);

    }

    else

    {

      // 关闭点击穿透

      SetWindowLong(hWnd, GWL_EXSTYLE, styles & ~(WS_EX_TRANSPARENT));

    }

    MARGINS margins = new MARGINS { cxLeftWidth = -1 };

    DwmExtendFrameIntoClientArea(hWnd, ref margins);

  }

}

