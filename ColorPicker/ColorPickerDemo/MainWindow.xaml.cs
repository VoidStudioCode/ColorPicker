using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColorPickerDemo
{
    public partial class MainWindow : Window
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewDragOver(Object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
                Mouse.SetCursor(Cursors.Arrow);
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_PreviewDrop(Object sender, DragEventArgs e)
        {
            Mouse.SetCursor(Cursors.AppStarting);
            String path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            OpenFile(path);
        }

        private void Button_Click(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = ".jpg",
                Filter = "Image File|*.jpg|*.png|*.bmp"
            };
            if (ofd.ShowDialog() == true)
            {
                OpenFile(ofd.FileName);
            }
        }

        private void OpenFile(String path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.DecodePixelWidth = 300;
            bitmap.DecodePixelHeight = 300;
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();
            Img.Source = bitmap;

            stopwatch.Restart();
            ColorPicker.ColorPicker colorPicker = new ColorPicker.ColorPicker();
            Color color = colorPicker.GetMediaColor(path);
            stopwatch.Stop();

            Rec.Fill = new SolidColorBrush(color);
            Tb_Time.Text = $"{stopwatch.ElapsedMilliseconds} ms";
        }
    }
}