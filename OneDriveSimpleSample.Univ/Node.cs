using OneDriveSimpleSample.Response;
using OneDriveSimpleSample.Utils;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OneDriveSimpleSample
{
    public enum NodeType
    {
        File,
        Directory
    }

    public interface INode
    {
        NodeType Type { get; }

        string Name { get; }

        Task<Stream> GetFileStream();

        /// <summary>
        /// Return a local copy of the item
        /// </summary>
        /// <returns></returns>
        Task<IStorageItem> GetStorageItem();

        /// <summary>
        /// Thumbnail of the item
        /// </summary>
        ImageSource Thumbnail { get; }
    }

    public class Node : BindableBase, INode
    {
        internal readonly IStorageItem _item;
        

        public Node(IStorageItem item)
        {
            _item = item;
        }

        public Node(ItemInfoResponse item)
        {
            this.Name = item.Name;
            this.ApiResponse = item;
            if(item.Thumbnails.Any()) this.ThumbnailUrl = item.Thumbnails.FirstOrDefault().Large.Url;
            if (item.Kind == 0)
            {
                this.Type = NodeType.Directory;
                this._thumbnail = new BitmapImage(new Uri("ms-appx://OneDriveSimpleSample/Assets/folder_drop.jpg"));
            }
            else this.Type = NodeType.File;

        }


        public string Name { get; set; }

        public string ThumbnailUrl { get; set; }

        public NodeType Type { get; set; }

        public Node _parent { get; set; }

        public ItemInfoResponse ApiResponse { get; set; }

        public async Task<Stream> GetFileStream()
        {
            var item = _item as StorageFile;
            if (item == null)
                throw new ArgumentException();

            var stream = await item.OpenStreamForReadAsync();

            return stream;
        }

        public async Task<IStorageItem> GetStorageItem()
        {

            return await SaveStreamToFile(await MainPage._service.RefreshAndDownloadContent(this.ApiResponse, false), Name);
                
        }


        public async Task<StorageFile> SaveStreamToFile(Stream streamToSave, string fileName)
        {
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            using (Stream fileStram = await file.OpenStreamForWriteAsync())
            {
                const int BUFFER_SIZE = 1024;
                byte[] buf = new byte[BUFFER_SIZE];

                int bytesread = 0;
                while ((bytesread = await streamToSave.ReadAsync(buf, 0, BUFFER_SIZE)) > 0)
                {
                    await fileStram.WriteAsync(buf, 0, bytesread);
                    
                }
            }
            return file;
        }

        public ImageSource _thumbnail;

       

        public ImageSource Thumbnail
        {
            get
            {
                if (_thumbnail == null && ThumbnailUrl!="" && ThumbnailUrl!=null )
                {
                    getImageFromURL(ThumbnailUrl);
                }
                else if (_thumbnail == null)
                {
                    _thumbnail = new BitmapImage(ThumbnailFactory.Instance.GetThumbnailPath(Name));
                }
                return _thumbnail;
            }

            set
            {
                SetProperty(ref _thumbnail , value);
            }
        }

       

        public async Task getImageFromURL(String sURL)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(new Uri(sURL));

                    BitmapImage bitmap = new BitmapImage();

                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var memStream = new MemoryStream())
                            {
                                await stream.CopyToAsync(memStream);
                                memStream.Position = 0;

                                bitmap.SetSource(memStream.AsRandomAccessStream());
                                Thumbnail = bitmap;
                            }
                        }
                        
                    }
                    
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

       
    }
}
