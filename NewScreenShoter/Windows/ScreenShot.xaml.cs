using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewScreenShoter.Windows
{
    /// <summary>
    /// Логика взаимодействия для ScreenShot.xaml
    /// </summary>
    public partial class ScreenShot : Window
    {
        public BitmapSource bitmapSource { get; set; }
        public ScreenShot(BitmapSource bitmap)
        {
            InitializeComponent();
            bitmapSource = bitmap;
            ImageScreen.Source = bitmap;
            WindowState = WindowState.Maximized;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog() { Filter = "Images|*.png;*.bmp;*.jpg", DefaultExt = ".png" };

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var fileStream = new FileStream(sfd.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(fileStream);
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
