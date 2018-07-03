using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Preferences;
using GestioneSarin2.Activity;
using Environment = System.Environment;
using File = System.IO.File;
using Uri = System.Uri;


namespace GestioneSarin2
{
    static class Helper
    {
        public static List<List<string>> table;

        public static List<string> GetGroup(Context context)
        {
            var data = table ?? GetData(context);

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

            TOTALE = Math.Round(TOTALE,2);
            return TOTALE;
        }

        public static List<List<string>> GetData(Context context, bool force = false)
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

            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            if (!File.Exists(path+"/catalogo.csv")||force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);
                var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri($"http://www.{ip}.com/DasGappArchives/_catandr.csv"), path + "/catalogo.csv");
                }
            }
            var tableTemp = new List<List<string>>();
            using (var fs = new StreamReader(path + "/catalogo.csv"))
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

        public static bool GetMIssPhoto(string path,Context context)
        {   var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);
            var pathdow = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                .DirectoryDownloads).AbsolutePath;
            var pathsplit = path.Split('/');
            using (WebClient request = new WebClient())
            {
                var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                request.Credentials = new NetworkCredential(usern, passw);
             
                var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                byte[] fileData = request.DownloadData($"ftp:/{ip}/{pathsplit[pathsplit.Length - 1]}");
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





