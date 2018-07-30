using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Preferences;
using GestioneSarin2.Activity;
using Java.Lang;
using Java.Net;
using Environment = System.Environment;
using Exception = System.Exception;
using File = System.IO.File;
using Math = System.Math;
using Process = Android.OS.Process;
using SocketAddress = System.Net.SocketAddress;


namespace GestioneSarin2
{
    static class Helper
    {
        public static List<List<string>> table;

        public static List<string> GetGroup(Context context)
        {
            var data = table ?? GetArticoli(context);

            var descgruppolist = new List<string>();
            foreach (var row in data)
            {
                descgruppolist.Add(row[21]);
            }

            var output = descgruppolist
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
            decimal TOTALE = 0;
            foreach (var singprod in prod)
            {
                var prodsplit = singprod.Split(';');
                prodsplit[2] = prodsplit[2].Replace(',', '.');
                var quant = prodsplit[1];
                if (!float.TryParse(prodsplit[2], out var qta))
                {
                    var qtaSplit = prodsplit[2].Split('/')[1].ToCharArray();
                    qta = float.Parse(qtaSplit.Where(ch => char.IsNumber(ch) || char.IsPunctuation(ch)).Aggregate("", (current, ch) => current + ch));
                   
                }
                var quants = prodsplit[1].Split();
                quant = quants[0].Replace(',', '.');
                TOTALE += Convert.ToDecimal(quant) * Convert.ToDecimal(qta);
            }

            TOTALE = Math.Round(TOTALE, 2);
            return TOTALE;
        }

        public static decimal GetTotIva(List<string> prod)
        {
            decimal Tot = 0;
            foreach (var singprod in prod)
            {
                var prodsplit = singprod.Split(';');
                prodsplit[2] = prodsplit[2].Replace(',', '.');
                if (!float.TryParse(prodsplit[2], out var qta))
                {
                    var qtaSplit = prodsplit[2].Split('/')[1].ToCharArray();
                    qta = float.Parse(qtaSplit.Where(ch => char.IsNumber(ch) || char.IsPunctuation(ch))
                        .Aggregate("", (current, ch) => current + ch));
                    var ttemp = Convert.ToDecimal(prodsplit[1]) * Convert.ToDecimal(qta);
                    var ivatem = Convert.ToDecimal(table.First(prodl => prodl[5] == prodsplit[0].ToUpper())[6]);
                    Tot += (ttemp / 100) * ivatem;
                }
                else
                {
                    var ttemp = Convert.ToDecimal(prodsplit[1]) * Convert.ToDecimal(qta);
                    var ivatem = Convert.ToDecimal(table.First(prodl => prodl[5] == prodsplit[0].ToUpper())[6]);
                    Tot += (ttemp / 100) * ivatem;
                }




            }

            Tot = Math.Round(Tot, 2);
            return Tot;
        }

        public static List<List<string>> GetDest(Context contex, bool force = false)
        {
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            if (!File.Exists(path + "/destdiv.csv") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(contex);
                using (WebClient request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    byte[] fileData = request.DownloadData($"ftp://{ip}/_destdiv.csv");
                    using (FileStream file = new FileStream(path + "/destdiv.csv", FileMode.Create))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }

            var tableTemp = new List<List<string>>();
            using (var fs = new StreamReader(path + "/destdiv.csv"))
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

        public static List<List<string>> GetArticoli(Context context, bool force = false)
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
            if (!File.Exists(path + "/catalogo.csv") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);
                using (WebClient request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    byte[] fileData = request.DownloadData($"ftp://{ip}/_Articoli.csv");
                    using (FileStream file = new FileStream(path + "/catalogo.csv", FileMode.Create))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
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

        public static List<List<string>> GetArticoli(string path)
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

        public static bool GetMIssPhoto(string path, Context context)
        {
            var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);
            var pathdow = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                              .DirectoryDownloads).AbsolutePath + "/Sarin";
            var pathsplit = path.Split('/');
            using (WebClient request = new WebClient())
            {
                var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                request.Credentials = new NetworkCredential(usern, passw);

                var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                var imPath = $"ftp://{ip}/foto/{pathsplit[pathsplit.Length - 1]}";
                byte[] fileData = request.DownloadData(imPath);

                using (FileStream file =
                    new FileStream(pathdow + $"/{pathsplit[pathsplit.Length - 1]}", FileMode.Create))
                {
                    file.Write(fileData);
                    file.Flush(true);
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
                using (var fs = new StreamReader(path + "/clienti.csv"))
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

        public static List<List<string>> GetClienti(Context context, bool force = false)
        {
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            if (!File.Exists(path + "/clienti.csv") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);

                using (WebClient request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    byte[] fileData = request.DownloadData($"ftp://{ip}/_clienti.csv");
                    using (FileStream file = new FileStream(path + "/clienti.csv", FileMode.Create))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }


            try
            {
                var tableTemp = new List<List<string>>();
                using (var fs = new StreamReader(path + "/clienti.csv"))
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

        public static async Task<bool> IsOnline()
        {
            try
            {
                await Task.Delay(1);
                int timeoutMs = 1500;
                Socket sock = new Socket();
                var sockaddr = new InetSocketAddress("8.8.8.8", 53);

                sock.Connect(sockaddr, timeoutMs);
                sock.Close();

                return true;
            }
            catch (IOException e)
            {
                return false;
            }
        }

        public static List<string> GetImgList()
        {
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin/imacat";
            var listFiles = Directory.GetFiles(path, "*.jpg").ToList();
            return listFiles;

        }
    }
}