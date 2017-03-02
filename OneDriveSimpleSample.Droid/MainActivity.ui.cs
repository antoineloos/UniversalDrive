using Android.App;
using Android.Widget;

namespace OneDriveSimpleSample.Droid
{
    public partial class MainActivity : Activity
    {
        private EditText _downloadFilePathText;

        public EditText DownloadFilePathText => _downloadFilePathText
                                                ?? (_downloadFilePathText = FindViewById<EditText>(Resource.Id.DownloadFilePathText));

        private EditText _folderPathText;

        public EditText FolderPathText => _folderPathText
                                          ?? (_folderPathText = FindViewById<EditText>(Resource.Id.FolderPathText));

        private Button _authenticateButton;

        public Button AuthenticateButton => _authenticateButton
                                            ?? (_authenticateButton = FindViewById<Button>(Resource.Id.AuthenticateButton));

        private Button _getRootFolderButton;

        public Button GetRootFolderButton => _getRootFolderButton
                                             ?? (_getRootFolderButton = FindViewById<Button>(Resource.Id.GetRootFolderButton));

        private Button _getAppFolderButton;

        public Button GetAppFolderButton => _getAppFolderButton
                                            ?? (_getAppFolderButton = FindViewById<Button>(Resource.Id.GetAppFolderButton));

        private Button _browseSubfolderButton;

        public Button BrowseSubfolderButton => _browseSubfolderButton
                                               ?? (_browseSubfolderButton = FindViewById<Button>(Resource.Id.BrowseSubfolderButton));

        private Button _downloadFileButton;

        public Button DownloadFileButton => _downloadFileButton
                                            ?? (_downloadFileButton = FindViewById<Button>(Resource.Id.DownloadFileButton));

        private Button _uploadFileButton;

        public Button UploadFileButton => _uploadFileButton
                                          ?? (_uploadFileButton = FindViewById<Button>(Resource.Id.UploadFileButton));

        private Button _getLinkButton;

        public Button GetLinkButton => _getLinkButton
                                       ?? (_getLinkButton = FindViewById<Button>(Resource.Id.GetLinkButton));

        private Button _logOffButton;

        public Button LogOffButton => _logOffButton
                                      ?? (_logOffButton = FindViewById<Button>(Resource.Id.LogOffButton));
    }
}