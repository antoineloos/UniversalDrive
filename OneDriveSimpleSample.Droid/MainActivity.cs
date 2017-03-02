using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.OS;
using OneDriveSimpleSample.Helpers;
using OneDriveSimpleSample.Response;
using Environment = System.Environment;

namespace OneDriveSimpleSample.Droid
{
    [Activity(Label = "OneDrive Simple Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public partial class MainActivity
    {
        private static string _downloadFilePath;
        private static string _folderPath;
        private readonly OneDriveService _service = App.ServiceInstance;
        private string _savedId;
        private bool _wasAuthenticating;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            AuthenticateButton.Click += AuthenticateClick;
            GetAppFolderButton.Click += GetAppRootClick;
            BrowseSubfolderButton.Click += BrowseSubfolderClick;
            DownloadFileButton.Click += DownloadFileClick;
        }

        private async void GetAppRootClick(object sender, EventArgs e)
        {
            Exception error = null;
            ItemInfoResponse folder = null;
            IList<ItemInfoResponse> children = null;

            ShowBusy(true);

            try
            {
                folder = await _service.GetAppRoot();
                children = await _service.PopulateChildren(folder);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                ShowDialog(error.Message, "Error!");
                ShowBusy(false);
                return;
            }

            DisplayHelper.ShowContent(
                "SHOW APP FOLDER CONTENT ++++++++++++++++++++++",
                folder,
                children,
                message =>
                {
                    ShowDialog(message, "");
                });
        }

    private void AuthenticateClick(object sender, EventArgs e)
        {
            ShowBusy(true);

            if (!_service.CheckAuthenticate(
                () =>
                {
                    App.CurrentActivity.OnBackPressed();
                    ShowBusy(false);
                },
                () =>
                {
                    App.CurrentActivity.OnBackPressed();
                    ShowBusy(false);
                }))
            {
                var intent = new Intent(this, typeof(AuthenticationActivity));
                StartActivity(intent);
                _wasAuthenticating = true;
            }
        }

        private async void BrowseSubfolderClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FolderPathText.Text))
            {
                ShowDialog("Please enter a path to a folder, for example Apps/OneDriveSample", "Error!");
                return;
            }

            Exception error = null;
            ItemInfoResponse subfolder = null;

            ShowBusy(true);

            try
            {
                subfolder = await _service.GetItem(FolderPathText.Text);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                ShowDialog(error.Message, "Error!");
                ShowBusy(false);
                return;
            }

            if (subfolder == null)
            {
                ShowDialog($"Not found: {FolderPathText.Text}", string.Empty);
            }
            else
            {
                var children = await _service.PopulateChildren(subfolder);
                DisplayHelper.ShowContent(
                    "SHOW SUBFOLDER CONTENT ------------------------",
                    subfolder,
                    children,
                    message =>
                    {
                        ShowDialog(message, string.Empty);
                    });
            }

            ShowBusy(false);
        }

        private async void DownloadFileClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DownloadFilePathText.Text))
            {
                ShowDialog("Please enter a path to an existing file, for example Apps/OneDriveSample/Test.jpg", "Error!");
                return;
            }

            Exception error = null;
            ItemInfoResponse foundFile = null;
            Stream contentStream = null;

            ShowBusy(true);

            try
            {
                foundFile = await _service.GetItem(DownloadFilePathText.Text);

                if (foundFile == null)
                {
                    ShowDialog($"Not found: {DownloadFilePathText.Text}");
                    ShowBusy(false);
                    return;
                }

                // Get the file's content
                contentStream = await _service.RefreshAndDownloadContent(foundFile, false);

                if (contentStream == null)
                {
                    ShowDialog($"Content not found: {DownloadFilePathText.Text}");
                    ShowBusy(false);
                    return;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                ShowDialog(error.Message, "Error!");
                ShowBusy(false);
                return;
            }

            // Save the retrieved stream to the Documents folder

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.Combine(documents, foundFile.Name);

            using (var targetStream = File.OpenWrite(filePath))
            {
                using (var writer = new BinaryWriter(targetStream))
                {
                    contentStream.Position = 0;

                    using (var reader = new BinaryReader(contentStream))
                    {
                        byte[] bytes;

                        do
                        {
                            bytes = reader.ReadBytes(1024);
                            writer.Write(bytes);
                        }
                        while (bytes.Length == 1024);
                    }
                }
            }

            ShowDialog("Done saving the file!", "Success");
            ShowBusy(false);
        }

        private void ShowDialog(string message, string title = "")
        {
            var builder = new AlertDialog.Builder(App.CurrentActivity);
            builder.SetMessage(message);
            builder.SetTitle(title);

            builder.Show();
        }

        protected override void OnResume()
        {
            // Restore the entered values
            FolderPathText.Text = _folderPath ?? string.Empty;
            DownloadFilePathText.Text = _downloadFilePath ?? string.Empty;
            App.CurrentActivity = this;

            if (_wasAuthenticating)
            {
                _wasAuthenticating = false;

                if (_service.IsAuthenticated)
                {
                    ShowDialog("You are authenticated!", "Success!");
                }
                else
                {
                    ShowDialog("Problem when authenticating!", "Sorry!");
                }
            }

            base.OnResume();
        }

        protected override void OnPause()
        {
            // Save the entered values for later
            _folderPath = FolderPathText.Text;
            _downloadFilePath = DownloadFilePathText.Text;
            base.OnPause();
        }

        private void ShowBusy(bool isBusy)
        {
            //Progress.IsActive = isBusy;
            //PleaseWaitCache.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

