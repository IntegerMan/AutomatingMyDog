using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.Pages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace MattEland.AutomatingMyDog.Desktop.ViewModels
{
    public class VisionViewModel : ViewModelBase
    {
        private AppViewModel _vm;
        private VisionHelper? _vision;

        public VisionViewModel(AppViewModel appViewModel)
        {
            _vm = appViewModel;

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            Update(appViewModel);
        }

        private void Update(AppViewModel appViewModel)
        {
            if (appViewModel.IsConfigured)
            {
                _vision = new VisionHelper(appViewModel.Key, appViewModel.Endpoint);
            }
        }

        internal void UpdateAzureSettings(AppViewModel appViewModel)
        {
            Update(appViewModel);
        }

        private readonly BackgroundWorker worker = new();

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = (string)e.Argument;
            e.Result = _vision?.AnalyzeImageAsync(filePath).Result;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<AppMessage> results = ((IEnumerable<AppMessage>)e.Result).ToList();
            foreach (AppMessage message in results)
            {
                _vm.RegisterMessage(message);
            }
        }

        public async Task RespondToAsync(string imagePath)
        {
            // Abort if the app is not configured
            if (_vision == null)
            {
                AppMessage notConfigMessage = new("The application settings have not been configured. Please configure those first and try again.", MessageSource.DogOS);
                await _vm.RegisterMessageAsync(notConfigMessage);
            }
            else
            {
                worker.RunWorkerAsync(imagePath);
            }

        }
    }
}