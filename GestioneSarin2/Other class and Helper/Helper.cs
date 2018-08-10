using Android.Content;
using Android.Preferences;
using GestioneSarin2.Activity;
using Java.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Exception = System.Exception;
using File = System.IO.File;
using Math = System.Math;


namespace GestioneSarin2
{
    internal static class Helper
    {
        public static List<List<string>> table;

        public static List<List<string>> GetGroup(Context context, bool force = false)
        {
            /*var data = table ?? GetArticoli(context);

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
            return output;*/
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (!File.Exists(path + "/maggrp.txt") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);
                using (WebClient request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    var codA = sharedPref.GetString(ActivitySettings.KeyCodAge, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    var fileData = request.DownloadData($"ftp://{ip}/{codA}/import/maggrp.txt");
                    using (var file = new FileStream(path + "/maggrp.txt", FileMode.Create))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }

            var tableTemp = new List<List<string>> { new List<string>(), new List<string>() };
            using (var fs = new StreamReader(path + "/maggrp.txt"))
            {
                while (!fs.EndOfStream)
                {
                    var split = fs.ReadLine()?.Split(';');
                    if (split == null) continue;
                    tableTemp[0].Add(split[0]);
                    tableTemp[1].Add(split[1]);
                }
            }

            return tableTemp;
        }

        public static decimal GetTot(List<string> prod)
        {
            decimal totale = 0;
            foreach (var singprod in prod)
            {
                var prodsplit = singprod.Split(';');
                prodsplit[2] = prodsplit[2].Replace(',', '.');
                if (!float.TryParse(prodsplit[2], out var qta))
                {
                    var qtaSplit = prodsplit[2].Split('/')[1].ToCharArray();
                    qta = float.Parse(qtaSplit.Where(ch => char.IsNumber(ch) || char.IsPunctuation(ch)).Aggregate("", (current, ch) => current + ch));

                }
                var quants = prodsplit[1].Split();
                var quant = quants[0].Replace(',', '.');
                totale += Convert.ToDecimal(quant) * Convert.ToDecimal(qta);
            }

            totale = Math.Round(totale, 2);
            return totale;
        }

        public static decimal GetTotIva(List<string> prod)
        {
            decimal tot = 0;
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
                    tot += (ttemp / 100) * ivatem;
                }
                else
                {
                    var ttemp = Convert.ToDecimal(prodsplit[1]) * Convert.ToDecimal(qta);
                    var ivatem = Convert.ToDecimal(table.First(prodl => prodl[5] == prodsplit[0].ToUpper())[6]);
                    tot += (ttemp / 100) * ivatem;
                }




            }

            tot = Math.Round(tot, 2);
            return tot;
        }

        /*public static List<List<string>> GetDest(Context contex, bool force = false)
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
                    var codA = sharedPref.GetString(ActivitySettings.KeyCodAge, "");

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    byte[] fileData = request.DownloadData($"ftp://{ip}/{codA}/import/_destdiv.csv");
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
        }*/

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

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (!File.Exists(path + "/catalogo.txt") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);
                using (var request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    var codA = sharedPref.GetString(ActivitySettings.KeyCodAge, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    var fileData = request.DownloadData($"ftp://{ip}/{codA}/import/magart.txt");
                    using (var file = new FileStream(path + "/catalogo.txt", FileMode.Create))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }

            var tableTemp = new List<List<string>>();
            using (var fs = new StreamReader(path + "/catalogo.txt"))
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
            var pathdow = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            var pathsplit = path.Split('/');
            using (var request = new WebClient())
            {
                var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                request.Credentials = new NetworkCredential(usern, passw);

                var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                var imPath = $"ftp://{ip}/foto/{pathsplit[pathsplit.Length - 1]}";
                var fileData = request.DownloadData(imPath);

                using (var file = new FileStream(pathdow + $"/{pathsplit[pathsplit.Length - 1]}", FileMode.Create))
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
                using (var fs = new StreamReader(path + "/clienti.txt"))
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
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";
            if (!File.Exists(path + "/clienti.txt") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);

                using (var request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    var codA = sharedPref.GetString(ActivitySettings.KeyCodAge, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    var dpath = $"ftp://{ip}/{codA}/import/anagrafe.txt";
                    var fileData = request.DownloadData(dpath);
                    using (var file = new FileStream(path + "/clienti.txt", FileMode.OpenOrCreate,FileAccess.Write,FileShare.ReadWrite))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }


            try
            {
                var tableTemp = new List<List<string>>();
                using (var fs = new StreamReader(path + "/clienti.txt"))
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

        public static List<List<string>> GetAge(Context context, bool force = false)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (!File.Exists(path + "/docana.txt") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);

                using (WebClient request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    var codA = sharedPref.GetString(ActivitySettings.KeyCodAge, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    var fileData = request.DownloadData($"ftp://{ip}/{codA}/import/docana.txt");
                    using (FileStream file = new FileStream(path + "/docana.txt", FileMode.Create))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }


            try
            {
                var tableTemp = new List<List<string>>();
                using (var fs = new StreamReader(path + "/docana.txt"))
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
        public static void GetDoc(Context context, bool force = false)
        {
             var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (!File.Exists(path + "/docana.txt") || force)
            {
                var sharedPref = PreferenceManager.GetDefaultSharedPreferences(context);

                using (WebClient request = new WebClient())
                {
                    var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                    var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                    var codA = sharedPref.GetString(ActivitySettings.KeyCodAge, "");
                    request.Credentials = new NetworkCredential(usern, passw);

                    var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                    var fileData = request.DownloadData($"ftp://{ip}/{codA}/import/docana.txt");
                    using (FileStream file = new FileStream(path + "/docana.txt", FileMode.Create))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }


            try
            {
                var tableTemp = new List<List<string>>();
                using (var fs = new StreamReader(path + "/docana.txt"))
                {
                    while (!fs.EndOfStream)
                    {
                        var row = fs.ReadLine();
                        if (row == null) continue;
                        var columns = row.Split(';');
                        tableTemp.Add(columns.ToList());
                    }
                }

            }
            catch (Exception e)
            {
                throw new NotSupportedException(e.Message);
            }
        }
    }
}