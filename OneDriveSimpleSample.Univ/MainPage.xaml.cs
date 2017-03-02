using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using OneDriveSimpleSample.Request;
using OneDriveSimpleSample.Response;
using OneDriveSimpleSample.Helpers;
using Windows.UI.Xaml.Media;
using System.Net;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using OneDriveSimpleSample.Utils;
using Prism.Commands;
using Windows.Storage;

namespace OneDriveSimpleSample
{
    public sealed partial class MainPage :  Page, INotifyPropertyChanged
    {
        private static string _downloadFilePath;
        private static string _folderPath;
        public static readonly OneDriveService _service = new OneDriveService("000000004C169646");
        private string _savedId;
        private Node currentFolder;
        public event PropertyChangedEventHandler PropertyChanged;
        public DelegateCommand<Node> DownloadCommand => new DelegateCommand<Node>(Download);
        public DelegateCommand<Node> NavigateCommand => new DelegateCommand<Node>(Navigate);

        private List<Node> LstParent;

        private void GetParent(Node child)
        {
            if (child._parent != null)
            {

                LstParent.Add(child._parent);
                GetParent(child._parent);
            }
            
        }



        private string ConstructPathFromNodeList(List<Node> lstParent)
        {
            String currentPath = "";
            lstParent.Reverse();
            foreach (Node n in lstParent) currentPath += n.Name + "/";
            return currentPath;
        }

        private async void Navigate(INode obj)
        {
            ShowBusy(true);
            currentFolder = (Node)obj;
            LstParent = new List<Node>();
            GetParent((Node)obj);
            var currentPath = ConstructPathFromNodeList(LstParent);



            var subfolder = await _service.GetItem(currentPath+obj.Name);
           

            
           
                LstNode.Clear();
                var children = await _service.PopulateChildren(subfolder);

                foreach (ItemInfoResponse item in children)
                {
                    Debug.WriteLine(item.Name);
                    Debug.WriteLine(item.Thumbnails.Length);
                    LstNode.Add(new Node(item) { _parent = (Node)obj});

                }

            

            ShowBusy(false);
        }

        private async void Download(Node obj)
        {
            FileSavePicker savePicker = new FileSavePicker();
            var fs = await obj.GetStorageItem();

            ShowBusy(true);

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = obj.Name
            };

            var extension = Path.GetExtension(obj.Name);

            picker.FileTypeChoices.Add(
                $"{extension} files",
                new List<string>
                {
                    extension
                });
            var contentStream = await ((StorageFile)fs).OpenStreamForReadAsync();
            var targetFile = await picker.PickSaveFileAsync();

            using (var targetStream = await targetFile.OpenStreamForWriteAsync())
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

            var successDialog = new MessageDialog("Done saving the file!", "Success");
            await successDialog.ShowAsync();
            ShowBusy(false);
        }

        

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private ObservableCollection<Node> lstNode;
        public ObservableCollection<Node> LstNode {
            get
            {
                return lstNode;
            }
            set
            {
                lstNode = value;
                NotifyPropertyChanged(nameof(LstNode));
            }
        }

        public MainPage()
        {
            InitializeComponent();

            var navManager = SystemNavigationManager.GetForCurrentView();
            navManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            LstNode = new ObservableCollection<Node>();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
          
            DownloadFilePathText.Text = _downloadFilePath ?? string.Empty;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
         
            base.OnNavigatingFrom(e);
        }

        private void AuthenticateClick(object sender, RoutedEventArgs e)
        {
            ShowBusy(true);

            if (!_service.CheckAuthenticate(
                async () =>
                {
                    var dialog = new MessageDialog("You are authenticated!", "Success!");
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    Frame.GoBack();
                },
                async () =>
                {
                    var dialog = new MessageDialog("Problem when authenticating!", "Sorry!");
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    Frame.GoBack();
                }))
            {
                Frame.Navigate(typeof(AuthenticationPage));
            }
        }

        private async void BrowseSubfolderClick(object sender, RoutedEventArgs e)
        {
            
        }

        private async void DownloadFileClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DownloadFilePathText.Text))
            {
                var dialog = new MessageDialog("Please enter a path to an existing file, for example Apps/OneDriveSample/Test.jpg", "Error!");
                await dialog.ShowAsync();
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
                    var dialog = new MessageDialog($"Not found: {DownloadFilePathText.Text}");
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    return;
                }

                // Get the file's content
                contentStream = await _service.RefreshAndDownloadContent(foundFile, false);

                if (contentStream == null)
                {
                    var dialog = new MessageDialog($"Content not found: {DownloadFilePathText.Text}");
                    await dialog.ShowAsync();
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
                var dialog = new MessageDialog(error.Message, "Error!");
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }

            // Save the retrieved stream to the local drive

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = foundFile.Name
            };

            var extension = Path.GetExtension(foundFile.Name);

            picker.FileTypeChoices.Add(
                $"{extension} files",
                new List<string>
                {
                    extension
                });

            var targetFile = await picker.PickSaveFileAsync();

            using (var targetStream = await targetFile.OpenStreamForWriteAsync())
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

            var successDialog = new MessageDialog("Done saving the file!", "Success");
            await successDialog.ShowAsync();
            ShowBusy(false);
        }

        private async void GetAppRootClick(object sender, RoutedEventArgs e)
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
                var dialog = new MessageDialog(error.Message, "Error!");
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }


           
        }


        

        private async void GetLinkClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_savedId))
            {
                var dialog =
                    new MessageDialog(
                        "For the purpose of this demo, save a file first using the Upload File button",
                        "No file saved");
                await dialog.ShowAsync();
                return;
            }

            Exception error = null;
            LinkResponseInfo linkInfo = null;

            ShowBusy(true);

            try
            {
                linkInfo = await _service.GetLink(LinkKind.View, _savedId); // This could also be LinkKind.Edit
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                var dialog = new MessageDialog(error.Message, "Error!");
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }

            Debug.WriteLine("RETRIEVED LINK ---------------------");
            Debug.WriteLine(linkInfo.Link.WebUrl);
            var successDialog = new MessageDialog(
                $"The link was copied to the Debug window: {linkInfo.Link.WebUrl}",
                "No file saved");
            await successDialog.ShowAsync();
            ShowBusy(false);
        }

        private async void GetRootFolderClick(object sender, RoutedEventArgs e)
        {
            Navigate();
        }

        private async void Navigate()
        {
            Exception error = null;
            ItemInfoResponse folder = null;
            IList<ItemInfoResponse> children = null;

            ShowBusy(true);

            try
            {
                folder = await _service.GetRootFolder();
                children = await _service.PopulateChildren(folder);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                var dialog = new MessageDialog(error.Message, "Error!");
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }


            LstNode.Clear();

            foreach (ItemInfoResponse item in children)
            {
                Debug.WriteLine(item.Name);
                Debug.WriteLine(item.Thumbnails.Length);
                LstNode.Add(new Node(item));

            }
            Debug.WriteLine(LstNode.Count.ToString());
            ShowBusy(false);
        }

        private async void LogOffClick(object sender, RoutedEventArgs e)
        {
            Exception error = null;
            ShowBusy(true);

            try
            {
                await _service.Logout();
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                var dialog = new MessageDialog(error.Message, "Error!");
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }

            var successDialog = new MessageDialog("You are now logged off", "Success");
            await successDialog.ShowAsync();
            ShowBusy(false);
        }

        private void ShowBusy(bool isBusy)
        {
            Progress.IsActive = isBusy;
            PleaseWaitCache.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void UploadFileClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                ShowBusy(true);

                Exception error = null;

                try
                {
                    // For the demo, save this file in the App folder
                    var folder = await _service.GetAppRoot();

                    using (var stream = await file.OpenStreamForReadAsync())
                    {
                        var info = await _service.SaveFile(folder.Id, file.Name, stream);

                        // Save for the GetLink demo
                        _savedId = info.Id;

                        var successDialog =
                            new MessageDialog(
                                $"Uploaded file has ID {info.Id}. You can now use the Get Link button to retrieve a direct link to the file",
                                "Success");
                        await successDialog.ShowAsync();
                    }

                    ShowBusy(false);
                }
                catch (Exception ex)
                {
                    error = ex;
                }

                if (error != null)
                {
                    var dialog = new MessageDialog(error.Message, "Error!");
                    await dialog.ShowAsync();
                    ShowBusy(false);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentFolder._parent != null) Navigate(currentFolder._parent);
            else Navigate();
        }
    }
}
