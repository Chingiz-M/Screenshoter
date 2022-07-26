using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NewScreenShoter.Windows
{
    public partial class DedicatedScreen : Form
    {
        public BitmapSource bitmapsource { get; set; }
        public string UseWindow { get; set; }
        public DedicatedScreen(string usewindow)
        {
            InitializeComponent();
            UseWindow = usewindow;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            WindowState = FormWindowState.Maximized;
            BackgroundImage = Shoot();
        }
        private Bitmap Shoot()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (var gr = Graphics.FromImage(bmp))
                gr.CopyFromScreen(0, 0, 0, 0, new Size(bmp.Width, bmp.Height));
            return bmp;
        }

        private Rectangle SelectedRectangle;
        public Rectangle GetRectangle { get; set; }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SelectedRectangle.Location = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (SelectedRectangle.Width > 0 && SelectedRectangle.Height > 0)
            {
                OpenWindowScreenShotORStartVideoScreen();
                //SelectedRectangle.Size = Size.Empty;
                //Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var newSize = new Size(e.X - SelectedRectangle.Left, e.Y - SelectedRectangle.Top);

            if (MouseButtons == MouseButtons.Left)
                if (newSize.Width > 5 && newSize.Height > 5)
                {
                    SelectedRectangle.Size = newSize;
                    Invalidate();
                }
        }

        private void OpenWindowScreenShotORStartVideoScreen()
        {
            using (var bmp = new Bitmap(SelectedRectangle.Width, SelectedRectangle.Height))
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(BackgroundImage, -SelectedRectangle.Left, -SelectedRectangle.Top);
                bitmapsource = Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                if(UseWindow == "DedicatedScreenShot")
                {
                    ScreenShot screenshot = new ScreenShot(bitmapsource);
                    screenshot.Show();
                }
                if (UseWindow == "DedicatedVIdeoScreenShot")
                {
                    GetRectangle = SelectedRectangle;
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var r = new Region(ClientRectangle);
            r.Exclude(SelectedRectangle);
            using (var brush = new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
                e.Graphics.FillRegion(brush, r);
        }
    }
}
