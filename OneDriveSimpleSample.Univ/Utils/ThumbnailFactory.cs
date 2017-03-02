using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneDriveSimpleSample.Utils
{
    public class ThumbnailFactory
    {
        private static ThumbnailFactory instance;
        private static readonly String BaseLocalURI = "ms-appx://OneDriveSimpleSample/Assets/";
        private ThumbnailFactory() { }

        public static ThumbnailFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ThumbnailFactory();
                }
                return instance;
            }
        }

        public Uri GetThumbnailPath(String path )
        {
            switch (Path.GetExtension(path))
            {
                case ".pdf":
                    return new Uri(BaseLocalURI+"pdf.jpg");
                break;

                case ".mp3":
                case ".flac":
                case ".wav":
                    return new Uri(BaseLocalURI + "musicdrop.jpg");
                    break;
                case ".mp4":
                case ".avi":
                case ".wmv":
                    return new Uri(BaseLocalURI + "video.png");
                    break;
                case ".doc":
                case ".docx":
                    return new Uri(BaseLocalURI + "word.jpg");
                    break;
                case ".pptx":
                case ".ppt":
                    return new Uri(BaseLocalURI + "powerpoint.jpg");
                    break;
                case ".xlsx":
                case ".xls":
                    return new Uri(BaseLocalURI + "excel.jpg");
                    break;
                case ".txt":
                    return new Uri(BaseLocalURI + "doc_blc.png");
                    break;
                case ".png":
                case "jpg":
                case ".gif":
                case ".bmp":
                    return new Uri(BaseLocalURI + "doc_blc.png") ;
                    break;
                default: return new Uri(BaseLocalURI + "doc_blc.png") ;
            }

            
        }
    }
}
