using Dropbox.Api.Files;
using Dropbox.Api.Stone;
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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace OneDriveSimpleSample.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class DropBoxPage : Page, INotifyPropertyChanged
    {

        private static string _folderPath;
        private Node currentFolder;
        private List<Node> LstParent;
        public DelegateCommand<Node> NavigateCommand => new DelegateCommand<Node>(Navigate);
        public DelegateCommand<Node> DownloadCommand => new DelegateCommand<Node>(Download);
        public static readonly DropBoxService.DropBoxService _service = new DropBoxService.DropBoxService();

        private async void Navigate()
        {
            ShowBusy(true);
            IsNotRootFolder = false;
           var res = await _service.Client.Files.ListFolderAsync(new Dropbox.Api.Files.ListFolderArg(""));
           
            LstNode.Clear();
            foreach (Metadata meta in res.Entries)
            {

                if (meta.IsFile == true)
                {
                    try
                    {
                        var ext = Path.GetExtension(meta.Name);
                        if (ext==".jpg"||ext==".jpeg" || ext == ".png" || ext == ".tiff" || ext == ".tif" || ext == ".gif" || ext == ".bmp") LstNode.Add(new Node(meta, await GetThumbnail(meta.PathLower)));
                        else LstNode.Add(new Node(meta));
                    }
                    catch(Dropbox.Api.ApiException<ThumbnailError> ex)
                    {
                        Debug.WriteLine(ex.Message);
                        LstNode.Add(new Node(meta));
                    }
                }
                else
                {
                    LstNode.Add(new Node(meta));
                }
                
            }

            ShowBusy(false);
           
        }
        private async void Navigate(Node obj)
        {
            if (obj.Type == NodeType.Directory)
            {
                ShowBusy(true);
                currentFolder = (Node)obj;
                if (isNotRootFolder == false) IsNotRootFolder = true;
                currentFolder = (Node)obj;
                LstParent = new List<Node>();
                GetParent((Node)obj);
                var currentPath = ConstructPathFromNodeList(LstParent);

                var res = await _service.Client.Files.ListFolderAsync(new Dropbox.Api.Files.ListFolderArg("/"+currentPath + obj.Name));
                LstNode.Clear();

                foreach (Metadata meta in res.Entries)
                {

                    if (meta.IsFile == true)
                    {
                        try
                        {
                            var ext = Path.GetExtension(meta.Name);
                            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".tiff" || ext == ".tif" || ext == ".gif" || ext == ".bmp") LstNode.Add(new Node(meta, await GetThumbnail(meta.PathLower)) { _parent = (Node)obj });
                            else LstNode.Add(new Node(meta) { _parent = (Node)obj });
                        }
                        catch (Dropbox.Api.ApiException<ThumbnailError> ex)
                        {
                            Debug.WriteLine(ex.Message);
                            LstNode.Add(new Node(meta) { _parent = (Node)obj });
                        }
                    }
                    else
                    {
                        LstNode.Add(new Node(meta) { _parent = (Node)obj });
                    }

                }
                ShowBusy(false);
            }
        }

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

        private async Task<BitmapImage> GetThumbnail(string path)
        {

            
            var response = await _service.Client.Files.GetThumbnailAsync(path, null, ThumbnailSize.W128h128.Instance );


            BitmapImage bitmap = new BitmapImage();

            var stream = await response.GetContentAsStreamAsync();

            using (stream)
            {
                using (var memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    memStream.Position = 0;

                    bitmap.SetSource(memStream.AsRandomAccessStream());
                }
            }

            return bitmap;
        }

        

        private async void Download(Node obj)
        {
           
            FileSavePicker savePicker = new FileSavePicker();
            var fs = await obj.GetDropBoxStorageItem();

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
            if (targetFile != null)
            {
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
            }

            ShowBusy(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

       
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool isNotRootFolder;
        public bool IsNotRootFolder
        {
            get { return isNotRootFolder; }
            set
            {
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

        public DropBoxPage()
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

                    //ShowBusy(false);
                    Debug.WriteLine("Auth Success");
                    Frame.GoBack();


                },
                () =>
                {

                    //ShowBusy(false);
                    Debug.WriteLine("Auth Failed");
                    Frame.GoBack();
                }))
                {
                    Frame.Navigate(typeof(AuthDropBoxPage), null);
                }
            }

            else
            {

                Navigate();
               
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
                    

                    using (var stream = await file.OpenStreamForReadAsync())
                    {
                        var meta = await _service.SaveFile(currentFolder.dropRef.PathLower, file.Name, stream);

                        if (meta.IsFile == true)
                        {
                            try
                            {
                                var ext = Path.GetExtension(meta.Name);
                                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".tiff" || ext == ".tif" || ext == ".gif" || ext == ".bmp") LstNode.Add(new Node(meta, await GetThumbnail(meta.PathLower)) { _parent = currentFolder });
                                else LstNode.Add(new Node(meta) { _parent = currentFolder });
                            }
                            catch (Dropbox.Api.ApiException<ThumbnailError> ex)
                            {
                                Debug.WriteLine(ex.Message);
                                LstNode.Add(new Node(meta) { _parent = currentFolder });
                            }
                        }
                        else
                        {
                            LstNode.Add(new Node(meta) { _parent = currentFolder });
                        }

                        var successDialog =
                            new MessageDialog(
                                $"Uploaded file has ID {meta.Name}. You can now use the Get Link button to retrieve a direct link to the file",
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
    }
}
