using System.Runtime.InteropServices;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int x, int y, uint dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int VK_SPACE = 0x20;
        const int KEYEVENTF_KEYDOWN = 0x0;
        const int KEYEVENTF_KEYUP = 0x2;

        const uint LEFTDOWN = 0x02;
        const uint LEFTUP = 0x04;
        Keys hotkey = Keys.XButton2;
        Keys Testing = Keys.V;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread th = new Thread(BackgroundLogic) { IsBackground = true };
            th.Start();


        }

        void BackgroundLogic()
        {
            int boxWidth = 9; // Horizontal size
            int boxHeight = 24; // Vertical size
            int x_center = Screen.PrimaryScreen.Bounds.Width / 2;
            int y_center = Screen.PrimaryScreen.Bounds.Height / 2;
            int x1 = x_center - (boxWidth / 2);
            int y1 = y_center - (boxHeight / 2);
            int x2 = x_center + (boxWidth / 2);
            int y2 = y_center + (boxHeight / 2);
            while (true)
            {
                if (KeyPressed(hotkey))
                {
                    if (ColorSearch(ConvertIntToColor(0xff88ff), x1, y1, x2, y2))
                    {
                        PerformClick(0, 0);
                    }
                }
                Thread.Sleep(10);
            }
        }

        Color ConvertIntToColor(int intColor)
        {
            int red = (intColor >> 16) & 0xFF;
            int green = (intColor >> 8) & 0xFF;
            int blue = intColor & 0xFF;
            return Color.FromArgb(red, green, blue);
        }


        void PerformClick(int x, int y)
        {
            mouse_event(LEFTDOWN, x, y, 0, IntPtr.Zero);
            Thread.Sleep(70);
            mouse_event(LEFTUP, x, y, 0, IntPtr.Zero);
        }

        bool AreColorsClose(Color color1, Color color2, int maxColorDifference)
        {
            int redDiff = Math.Abs(color1.R - color2.R);
            int greenDiff = Math.Abs(color1.G - color2.G);
            int blueDiff = Math.Abs(color1.B - color2.B);
            return redDiff <= maxColorDifference && greenDiff <= maxColorDifference && blueDiff <= maxColorDifference;

        }

        bool ColorSearch(Color wantedColor, int x1, int y1, int x2, int y2)
        {

            using (Bitmap screenshot = new Bitmap(x2 - x1, y2 - y1))
            {
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(new Point(x1, y1), Point.Empty, screenshot.Size);

                    for (int x = 0; x < screenshot.Width; x++)
                    {
                        for (int y = 0; y < screenshot.Height; y++)
                        {
                            Color pixelColor = screenshot.GetPixel(x, y);

                            if (AreColorsClose(wantedColor, pixelColor, 70))
                            {
                                return true;
                            }
                        }
                    }
                    return false;

                }
            }
        }

        bool KeyPressed(Keys vKey)
        {
            return GetAsyncKeyState(vKey) < 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}