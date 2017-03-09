using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using GoogleDriveService;
using OneDriveSimpleSample.Utils;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace OneDriveSimpleSample.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class GoogleDrivePage : Page,INotifyPropertyChanged
    {
        private static string _folderPath;
        private Node currentFolder;
        private List<Node> LstParent;
        public DelegateCommand<Node> NavigateCommand => new DelegateCommand<Node>(Navigate);
        public DelegateCommand<Node> DownloadCommand => new DelegateCommand<Node>(Download);

        private bool isNotRootFolder;
        public bool IsNotRootFolder
        {
            get { return isNotRootFolder; }
            set {
                isNotRootFolder = value;
                NotifyPropertyChanged(nameof(IsNotRootFolder));
            }
        }

        private ObservableCollection<Node> lstNode;
        public ObservableCollection<Node> LstNode
        {
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

        public void Navigate(Node obj)
        {
            if(obj.Type== NodeType.Directory)
            {
                ShowBusy(true);
                currentFolder = (Node)obj;
                if (isNotRootFolder == false) IsNotRootFolder = true;

                if (obj.Name.Contains("Shared with me"))
                {
                    LstNode.Clear();
                    var children = _service.GetSharedWithMeChildren();

                    foreach (Google.Apis.Drive.v3.Data.File item in children)
                    {

                        LstNode.Add(new Node(item) { _parent = (Node)obj });

                    }
                }

                else
                {
                    currentFolder = (Node)obj;
                    LstParent = new List<Node>();


                    LstNode.Clear();
                    var children = _service.GetSubFolderChildren(obj.googleRef.Id);

                    foreach (Google.Apis.Drive.v3.Data.File item in children)
                    {

                        LstNode.Add(new Node(item) { _parent = (Node)obj });

                    }
                }




                ShowBusy(false);
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly GoogleDriveService.GoogleDriveService _service = new GoogleDriveService.GoogleDriveService();

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        

        public GoogleDrivePage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            LstNode = new ObservableCollection<Node>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_service.IsAuthenticated)
            {
                if (!_service.CheckAuthenticate(
                () =>
                {

                    ShowBusy(false);
                    Debug.WriteLine("Auth Success");
                    Frame.GoBack();


                },
                () =>
                {

                    ShowBusy(false);
                    Debug.WriteLine("Auth Failed");
                    Frame.GoBack();
                }))
                {
                    Frame.Navigate(typeof(AuthGooglePage), null);
                }
            }

            else
            {
                Navigate();
               
            }
        }

        private void Navigate()
        {
           
            LstNode.Clear();
            foreach (Google.Apis.Drive.v3.Data.File elem in _service.GetRootFolderChildren())
            {
                LstNode.Add(new Node(elem));
                
            }
            LstNode.Add(new Node("Shared with me", NodeType.Directory) );
        }



        


        

        private async void Download(Node obj)
        {

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

            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                ShowBusy(true);

                var request = _service._service.Files.Get(obj.googleRef.Id);
                Stream stream = new MemoryStream();
                request.MediaDownloader.ProgressChanged +=
               async (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            {
                                Console.WriteLine(progress.BytesDownloaded);
                                break;
                            }
                        case DownloadStatus.Completed:
                            {
                                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                async () =>
                                {
                                    var successDialog = new MessageDialog("Done saving the file!", "Success");
                                    await successDialog.ShowAsync();
                                    stream.Dispose();
                                    
                                    ShowBusy(false);
                                });
                                
                                break;
                            }
                        case DownloadStatus.Failed:
                            {
                                Console.WriteLine("Download failed.");
                                break;
                            }
                    }
                };

                stream = await file.OpenStreamForWriteAsync();
                var downloadManager =  await request.DownloadAsync(stream);
                
                
            }
            


        }


        private void ShowBusy(bool isBusy)
        {
            Progress.IsActive = isBusy;
            PleaseWaitCache.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentFolder._parent != null) Navigate(currentFolder._parent);
            else
            {
                Navigate();
                IsNotRootFolder = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void Upload_Click(object sender, RoutedEventArgs e)
        {
            Stream stream;
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();
            ShowBusy(true);
            if (file != null)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = file.Name,
                    MimeType = MIMEAssistant.GetMIMEType(file.Name)
                };
            
                FilesResource.CreateMediaUpload request;
                using (stream = await file.OpenStreamForReadAsync())
                {
                    
                    request = _service._service.Files.Create(
                        fileMetadata, stream, MIMEAssistant.GetMIMEType(file.Name));
                    request.Fields = "id , name , mimeType";
                    //request.ChunkSize = FilesResource.CreateMediaUpload.MinimumChunkSize;
                    request.ProgressChanged += async(IUploadProgress progress) => 
                    {
                        if (progress?.Status == UploadStatus.Completed )
                        {
                            try
                            {


                                Debug.WriteLine("completed");
                                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                    async() =>
                                    {
                                        var successDialog = new MessageDialog("Done saving the file!", "Success");
                                        await successDialog.ShowAsync();
                                        ShowBusy(false);
                                  });
                                var resfile = request.ResponseBody;

                                if (resfile != null)
                                {
                                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                     () =>
                                     {
                                         LstNode.Add(new Node(resfile) { _parent = currentFolder });
                                     });
                                    Debug.WriteLine("File: " + resfile.Name + "  " + resfile.Id);
                                    stream?.Dispose();

                                }
                            }
                            catch(Exception ex) { Debug.WriteLine(ex.Message); }
                            
                        }
                        else if (progress?.Status == UploadStatus.Uploading ) Debug.WriteLine("uploading");
                    };





                    await request.UploadAsync();
                   
                  
                }

                
            }
            else ShowBusy(false);

        }
    }
}
