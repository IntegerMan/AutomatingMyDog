using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Telerik.Windows.MediaFoundation;

namespace MattEland.AutomatingMyDog.Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Vision.xaml
    /// </summary>
    public partial class Vision : UserControl
    {
        private readonly ReadOnlyCollection<MediaFoundationDeviceInfo> _videoDevices;

        public Vision()
        {
            InitializeComponent();

            _videoDevices = RadWebCam.GetVideoCaptureDevices();
            comboCameras.ItemsSource = _videoDevices;
            comboCameras.SelectedIndex = 0;
        }

        private void TakeSnapshot_Click(object sender, RoutedEventArgs e)
        {
            webCam.TakeSnapshot();
        }

        private void StartCamera_Click(object sender, RoutedEventArgs e)
        {
            // TODO: These visibility tweaks feel like hacks. This would be better with bindings and converters

            // Start the camera
            MediaFoundationDeviceInfo? cam = comboCameras.SelectedItem as MediaFoundationDeviceInfo;
            if (cam != null)
            {
                ReadOnlyCollection<MediaFoundationVideoFormatInfo> videoFormats = RadWebCam.GetVideoFormats(cam);
                MediaFoundationVideoFormatInfo info = videoFormats[0];

                MediaFoundationVideoFormatInfo? preferredFormat = videoFormats.FirstOrDefault(f => f.FrameSizeWidth == 640 && f.FrameSizeHeight == 360);
                preferredFormat ??= videoFormats.FirstOrDefault(f => f.FrameSizeWidth == 960 && f.FrameSizeHeight == 540);
                preferredFormat ??= videoFormats.FirstOrDefault(f => f.FrameSizeWidth == 1280 && f.FrameSizeHeight == 720);
                preferredFormat ??= videoFormats.FirstOrDefault(f => f.FrameSizeWidth == 1920 && f.FrameSizeHeight == 1080);
                preferredFormat ??= videoFormats[0];

                webCam.Initialize(cam, preferredFormat);
                webCam.Visibility = Visibility.Visible;
                webCam.Start();

                // Hide the camera select button
                panelCameraSelect.Visibility = Visibility.Collapsed;

                // Show our snapshot button
                btnSnapshot.Visibility = Visibility.Visible;
            }
            else
            {
                AppViewModel vm = (AppViewModel)DataContext;
                vm.HandleError("No camera found", "Could not start camera");
            }
        }

        private void RadWebCam_SnapshotTaken(object sender, SnapshotTakenEventArgs e)
        {
            BitmapSource snapshot = e.Snapshot;

            // Save the file to disk
            string imageFilePath = Path.GetTempFileName();
            using (Stream fileStream = new FileStream(imageFilePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(e.Snapshot));
                encoder.Save(fileStream);
            }

            // Record the message
            AppViewModel vm = (AppViewModel)DataContext;
            vm.RegisterMessage(new ChatMessageViewModel("Image Uploaded", Chat.GetAuthor(MessageSource.User))
            {
                ImageSource = snapshot,
                ImagePath = imageFilePath
            });
        }
    }
}
