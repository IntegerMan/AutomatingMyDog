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

        public Vision()
        {
            InitializeComponent();
        }

        private void TakeSnapshot_Click(object sender, RoutedEventArgs e)
        {
            webCam.TakeSnapshot();
        }

        private void StartCamera_Click(object sender, RoutedEventArgs e)
        {
            // TODO: These visibility tweaks feel like hacks. This would be better with bindings and converters

            // Start the camera
            ReadOnlyCollection<MediaFoundationDeviceInfo> videoDevices = RadWebCam.GetVideoCaptureDevices();
            MediaFoundationDeviceInfo? cam = videoDevices.FirstOrDefault();
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

                // Hide the button
                FrameworkElement control = (FrameworkElement)sender;
                control.Visibility = Visibility.Collapsed;

                // Show our snapshot button
                btnSnapshot.Visibility = Visibility.Visible;
            }
            else
            {
                AppViewModel vm = (AppViewModel)DataContext;
                vm.HandleError("No cameras found", "Could not start camera");
            }
        }

        private void RadWebCam_SnapshotTaken(object sender, SnapshotTakenEventArgs e)
        {
            BitmapSource snapshot = e.Snapshot;

            // Save the file to disk
            string imageFilePath = Path.Combine(Environment.CurrentDirectory, "Snapshot.png");
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
