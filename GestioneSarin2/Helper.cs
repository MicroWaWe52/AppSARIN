using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Net;
using Console = System.Console;
using Environment = System.Environment;
using File = System.IO.File;

namespace GestioneSarin2
{
    static class Helper
    {
        public static void GetFtpImg(string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.teatrotse.com/teatrotse.com/DasGappArchives/www.png");
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.  
            request.Credentials = new NetworkCredential("3835532@aruba.it", "Teatro09127");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            int bufferSize = 1024000;  //Image file cannot be greater than 40 Kb
            int readCount = 0;
            byte[] buffer = new byte[bufferSize];
            MemoryStream memStream = new MemoryStream();
            readCount = responseStream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                memStream.Write(buffer, 0, readCount);
                readCount = responseStream.Read(buffer, 0, bufferSize);
            }
            response.Close();
            using (FileStream streamW = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/littlegeorge.png", FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite))
            {
                streamW.Write(buffer, 0, bufferSize);
                streamW.Close();
                streamW.Dispose();

            }


            Console.WriteLine("Download Complete, status {0}", response.StatusDescription);
            memStream.Close();
            memStream.Dispose();
            responseStream.Close();
            responseStream.Dispose();
        }

        public static Drawable GetDrawableFromp(Stream inputStream)
        {
            Bitmap b = BitmapFactory.DecodeStream(inputStream);
            b.Density = Bitmap.DensityNone;
            Drawable d = new BitmapDrawable(b);
            return d;

        }
    }
}