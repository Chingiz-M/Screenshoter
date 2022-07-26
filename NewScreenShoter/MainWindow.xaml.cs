using NewScreenShoter.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewScreenShoter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        public Process ProcessVideoScreen { get; set; }
        public Process ProcessDedicatedVideoScreen { get; set; }
        private void DedicatedScreenShot_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            Thread.Sleep(200);
            DedicatedScreen dedicatedscreen = new DedicatedScreen("DedicatedScreenShot");
            this.WindowState = WindowState.Normal;
            dedicatedscreen.Show();
        }

        private void AllScreenShot_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            Thread.Sleep(200);
            ScreenShot screenshot = new ScreenShot(AllScreenShotMethod());
            this.WindowState = WindowState.Normal;
            screenshot.Owner = this;
            screenshot.Show();
        }
        private BitmapSource AllScreenShotMethod()
        {
            var left = Screen.AllScreens.Min(screen => screen.Bounds.X);
            var top = Screen.AllScreens.Min(screen => screen.Bounds.Y);
            var right = Screen.AllScreens.Max(screen => screen.Bounds.X + screen.Bounds.Width);
            var bottom = Screen.AllScreens.Max(screen => screen.Bounds.Y + screen.Bounds.Height);
            var width = right - left;
            var height = bottom - top;

            using (var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(left, top, 0, 0, new System.Drawing.Size(width, height));
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
        private void VideoScreen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessVideoScreen = Process.Start("ffmpeg.exe", "-y -rtbufsize 100M -f gdigrab -framerate 30" +
                    $" -probesize 10M -draw_mouse 1 -i desktop -f dshow -i audio=\"{Properties.Settings.Default.AudioName}\" -c:v libx264 -r 30" +
                    " -preset ultrafast -tune zerolatency -crf 25 -pix_fmt yuv420p screen.avi");
                this.WindowState = WindowState.Minimized;
            }
            catch (SystemException ex)
            {
                System.Windows.Forms.MessageBox.Show("Неверное название микрофона!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.WindowState = WindowState.Normal;
            }

            
        }

        private void VideoScreenStop_Click(object sender, RoutedEventArgs e)
        {
            //ProcessExtensions.Suspend(ProcessVideoScreen);
            ProcessVideoScreen.Kill();
        }

        private void DedicatedVideoScreen_Click(object sender, RoutedEventArgs e)
        {
            DedicatedScreen dedicatedscreen = new DedicatedScreen("DedicatedVIdeoScreenShot");
            System.Drawing.Rectangle SelectedRectangle = new System.Drawing.Rectangle();
            if (dedicatedscreen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedRectangle = dedicatedscreen.GetRectangle;
            }
            ProcessDedicatedVideoScreen = Process.Start("ffmpeg.exe", $"-y -rtbufsize 100M -f gdigrab -framerate 30" +
                        $" -probesize 10M -draw_mouse 1 -i desktop -f dshow -i audio=\"Микрофон (Realtek(R) Audio)\"" +
                        $" -filter:v \"crop = 340:414:100:100\"" +
                        $" -c:v libx264 -r 30 -preset ultrafast -tune zerolatency -crf 25 -pix_fmt yuv420p screen.avi");
        }

        private void DedicatedVideoScreenStop_Click(object sender, RoutedEventArgs e)
        {
            ProcessDedicatedVideoScreen.Kill();
        }

        //
    }
    
}
