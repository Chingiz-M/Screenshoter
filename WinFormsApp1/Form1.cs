using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            //new Button { Text = "Close me", Parent = this }.Click += (o, e) => Application.Exit();
            //this.FormBorderStyle = FormBorderStyle.None;
            //TopMost = true;
            //ShowInTaskbar = false;
            //WindowState = FormWindowState.Maximized;
            // BackgroundImage = Shoot();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            foreach (var device in devices)
                MessageBox.Show(device.FriendlyName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private Bitmap Shoot()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (var gr = Graphics.FromImage(bmp))
                gr.CopyFromScreen(0, 0, 0, 0, new Size(bmp.Width, bmp.Height));
            return bmp;
        }

        private Rectangle SelectedRectangle;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SelectedRectangle.Location = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (SelectedRectangle.Width > 0 && SelectedRectangle.Height > 0)
            {
                SaveSelectedRectangle();
                SelectedRectangle.Size = Size.Empty;
                Invalidate();
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

        private void SaveSelectedRectangle()
        {
            var sfd = new SaveFileDialog() { Filter = "Images|*.png;*.bmp;*.jpg", DefaultExt = ".png" };

            if (sfd.ShowDialog() == DialogResult.OK)
                using (var bmp = new Bitmap(SelectedRectangle.Width, SelectedRectangle.Height))
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.DrawImage(BackgroundImage, -SelectedRectangle.Left, -SelectedRectangle.Top);
                    bmp.Save(sfd.FileName);
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
