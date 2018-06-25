using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Net;
using Console = System.Console;
using Environment = System.Environment;
using File = System.IO.File;
using Uri = System.Uri;


namespace GestioneSarin2
{
    static class Helper
    {
        public static List<List<string>> table;

        private static string con = $@"Data Source={
                Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/_catandr.xls"
            };Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
        //  $@"Excel File={Environment.GetFolderPath(Environment.SpecialFolder.Personal)+ "/_catandr.xls"};";
        public static List<DirectoryItem> GetDirectoryInformation(string address, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(address);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(username, password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            List<DirectoryItem> returnValue = new List<DirectoryItem>();
            string[] list = null;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                list = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (string line in list)
            {
                if (line.Contains(".xls"))
                {
                    continue;
                }
                // Windows FTP Server Response Format
                // DateCreated    IsDirectory    Name
                string data = line;

                // Parse date
                string date = data.Substring(36, 12);
                DateTime dateTime = DateTime.Parse(date);
                data = data.Remove(0, 24);

                // Parse <DIR>
                string dir = data.Substring(0, 5);
                bool isDirectory = dir.Equals("<dir>", StringComparison.InvariantCultureIgnoreCase);
                data = data.Remove(0, 5);
                data = data.Remove(0, 10);

                // Parse name
                string name = data;

                // Create directory info
                DirectoryItem item = new DirectoryItem();
                item.BaseUri = new Uri(address);
                item.DateCreated = dateTime;
                item.IsDirectory = isDirectory;
                var split = name.Split(' ');
                string n = "";
                for (int i = 3; i < split.Length - 1; i++)
                {
                    n += split[i] + " ";
                }

                n += split[split.Length - 1];
                item.Name = n;

                item.Items = item.IsDirectory ? GetDirectoryInformation(item.AbsolutePath, username, password) : null;

                returnValue.Add(item);
            }

            return returnValue;
        }

        public static string GetName(string path)
        {
            return "2";
        }

        public static List<string> GetGroup()
        {
            var data = table ?? GetData();

            var descgruppolist=new List<string>();
            foreach (var row in data)
            {
                descgruppolist.Add(row[row.Count-2]);
            }
            var output= descgruppolist
                .GroupBy(word => word)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .ToList();
            output.Remove("desgruppo");
            output.Reverse();
            return output;
        }

        public static decimal GetTot(List<string> prod)
        {
            decimal TOTALE=0;
            foreach (var singprod in prod)
            {
               var prodsplit= singprod.Split(';');
                prodsplit[2] = prodsplit[2].Replace(',', '.');
                TOTALE += Convert.ToDecimal(prodsplit[1]) * Convert.ToDecimal(prodsplit[2]);
            }
            return TOTALE;
        }

        public static List<List<string>> GetData(bool force=false)
        {
            /* //  FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://217.133.0.34/" + "_catandr.xls");

             using (WebClient request = new WebClient())
             {
                 request.Credentials = new NetworkCredential("spigam", "123456");
                 byte[] fileData = request.DownloadData("ftp://217.133.0.34/" + "catalogo2.xls");
                 using (FileStream file = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/_catandr.xml", FileMode.Create))
                 {
                     file.Write(fileData, 0, fileData.Length);
                     file.Close();
                 }

             }*/


            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal)+"/catalogo.csv")||force)
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("http://www.teatrotse.com/DasGappArchives/_catandr.csv"), Environment.GetFolderPath(Environment.SpecialFolder.Personal)+"/catalogo.csv");
                }
            }
            var tableTemp = new List<List<string>>();
            using (var fs = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/catalogo.csv"))
            {
                while (!fs.EndOfStream)
                {
                    var row = fs.ReadLine();
                    if (row == null) continue;
                    var columns = row.Split(';');
                    tableTemp.Add(columns.ToList());
                }
            }

            table = tableTemp;
            return tableTemp;
        }
       
        public static List<List<string>> GetData(string path)
        {

            try
            {
                var tableTemp = new List<List<string>>();
                using (var fs = new StreamReader(path))
                {
                    while (!fs.EndOfStream)
                    {
                        var row = fs.ReadLine();
                        if (row == null) continue;
                        var columns = row.Split(';');
                        tableTemp.Add(columns.ToList());
                    }
                }

                table = tableTemp;
                return tableTemp;
            }
            catch (Exception e)
            {
                throw new NotSupportedException(e.Message);
            }
        }

        public static bool GetMIssPhoto(string path)
        {
            var pathdow = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                .DirectoryDownloads).AbsolutePath;
            var pathsplit = path.Split('/');
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential("spigam", "123456");
                byte[] fileData = request.DownloadData($"ftp://217.133.0.34/{pathsplit[pathsplit.Length - 1]}");
                using (FileStream file = new FileStream(pathdow + $"/{pathsplit[pathsplit.Length-1]}", FileMode.Create))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
            }
            return true;
        }

        public static List<List<string>> GetClienti(string path)
        {
            try
            {
                var tableTemp = new List<List<string>>();
                using (var fs = new StreamReader(path+"/clienti.csv"))
                {
                    while (!fs.EndOfStream)
                    {
                        var row = fs.ReadLine();
                        if (row == null) continue;
                        var columns = row.Split(';');
                        tableTemp.Add(columns.ToList());
                    }
                }
                return tableTemp;
            }
            catch (Exception e)
            {
                throw new NotSupportedException(e.Message);
            }
        }
    }
}

    struct DirectoryItem
    {
        public Uri BaseUri;

        public string AbsolutePath => $"{BaseUri}/{Name}";

        public DateTime DateCreated;
        public bool IsDirectory;
        public string Name;
        public List<DirectoryItem> Items;
    }





