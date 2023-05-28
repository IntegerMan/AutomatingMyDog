using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

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
            webCam.Visibility = Visibility.Visible;
            webCam.Start();

            // Hide the button
            FrameworkElement control = (FrameworkElement)sender;
            control.Visibility = Visibility.Collapsed;

            // Show our snapshot button
            btnSnapshot.Visibility = Visibility.Visible;
        }

        private void RadWebCam_SnapshotTaken(object sender, SnapshotTakenEventArgs e)
        {
            BitmapSource snapshot = e.Snapshot;

            // Save the file to disk
            string imageFilePath = Path.Combine(Environment.CurrentDirectory, "Snapshot.png");
            using (Stream fileStream = File.OpenWrite(imageFilePath))
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
