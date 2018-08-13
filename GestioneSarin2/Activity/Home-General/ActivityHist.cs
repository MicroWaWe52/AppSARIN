using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using ExtensionMethods;
using GestioneSarin2;
using GestioneSarin2.Activity;
using GestioneSarin2.Other_class_and_Helper;
using AlertDialog = Android.App.AlertDialog;
using Environment = System.Environment;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace GestioneSarin2
{
    [Activity(Label = "", Theme = "@style/AppThemeNo", ParentActivity = typeof(ActivityHome))]
    public class ActivityHist : AppCompatActivity
    {
        private ListView listViewHist;

        readonly string pathpp = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                            .DirectoryDownloads).AbsolutePath + "/Sarin";
        private List<string> csvlist = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutHistory);
            // Create your application here
            listViewHist = FindViewById<ListView>(Resource.Id.listViewHist);
            listViewHist.ItemLongClick += ListViewHist_ItemLongClick;
            listViewHist.ItemClick += ListViewHist_ItemClick;

            //ActionBar.SetDisplayHomeAsUpEnabled(true);
            var toolbar = FindViewById<Toolbar>(Resource.Id.my_toolbarSett);
            toolbar.SetTitleTextColor(Resource.Color.colorPrimary);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Refresh();
        }

        protected void Refresh()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";
            var clienti = Helper.GetClienti(this);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            var listoOrdines = new List<Ordine>();
            {


                var pathord = path + "/docTes.txt";
                var ordDet = new List<string>();
                using (var streamWriter = new StreamReader(pathord))
                {
                    var ordStr = streamWriter.ReadToEnd();

                    ordDet = ordStr.Split(
                        new[] { Environment.NewLine },
                        StringSplitOptions.None
                    ).ToList();

                }
                foreach (var ord in ordDet)
                {
                    var testa = ord.Split(';');
                    var nameTemp = clienti.First(list => ("C" + list[0]).Contains(testa[5]))[1];
                    listoOrdines.Add(new Ordine
                    {
                        Date = testa[3],
                        Name = nameTemp,
                        CodCli = testa[4],
                        Type = testa[1],
                        Tot = GetTot(Convert.ToInt32(testa[2])).ToString(CultureInfo.CurrentCulture)
                    });
                }


            }
            listViewHist.Adapter = new OrdineAdapter(listoOrdines);
        }

        public decimal GetTot(int idTesta)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            var pathord = path + "/docRig.txt";
            var ordPrz = new List<string>();
            using (var streamWriter = new StreamReader(pathord))
            {
                while (!streamWriter.EndOfStream)
                {
                    var ordStr = streamWriter.ReadLine();
                    if (ordStr?.Split(';')[0] == idTesta.ToString())
                    {
                        ordPrz.Add(ordStr);
                    }
                }

            }

            foreach (var rig in ordPrz)
            {
                var rigDet = rig.Split(';');
                var puni = rigDet[6].ToDecimal();
                var qta = rigDet[7].ToDecimal();
                var sc = rigDet[8].ToDecimal();
                var tot = qta * puni;
                var tots = sc * tot / 100;
                return tot - tots;
            }

            return 0;
        }
        private void ListViewHist_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Sei sicuro di voler modificare l'ordine");
            builder.SetMessage("Proseguendo con la modifica l'ordine verrà cancellato per la modifica");
            builder.SetCancelable(true);
            builder.SetNegativeButton("No", delegate { });
            builder.SetPositiveButton("Si", delegate
            {
                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                               .DirectoryDownloads).AbsolutePath + "/Sarin";
                var tesList = new List<string>();
                using (var sw = new StreamReader(path + "/doctes.csv"))
                {
                    while (!sw.EndOfStream)
                    {
                        tesList.Add(sw.ReadLine());

                    }
                }

                var tesSel = tesList[e.Position];
                var tesSplit = tesSel.Split(';');
                var type = tesSplit[8];
                var cli = tesSplit[5];
                var codDest = tesSplit[7];
                //aggiungere altri tipi di docuimento quando richiesto
                switch (type)
                {
                    case "RAPLA":
                        type = ((int)DocType.Rapportino).ToString();
                        break;
                    case "ORDCL":
                        type = ((int)DocType.Vendita).ToString();
                        break;
                    default:
                        type = 0.ToString();
                        break;
                }
                using (StreamWriter stream = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
                {
                    stream.WriteLine(cli + '/' + codDest);
                }
                var nDoc = tesSplit[0];
                var rigList = new List<string>();
                using (var sr = new StreamReader(path + "/docrig.csv"))
                {
                    while (!sr.EndOfStream)
                    {
                        var rig = sr.ReadLine();
                        var rigsplit = rig.Split(';');
                        if (rigsplit[1] == nDoc)
                        {
                            var codDesc = Helper.GetArticoli(this).First(p => p[4] == rigsplit[2])[5];
                            var prodFin = codDesc;
                            for (var i = 2; i < rigsplit.Length - 1; i++)
                            {
                                prodFin += ';' + rigsplit[i];
                            }
                            rigList.Add(prodFin);
                        }
                    }

                }

                var listprod = new List<string>();
                var listUri = new List<string>();
                foreach (var prod in rigList)
                {
                    var prodSplit = prod.Split(';');
                    var descPRod = Helper.Table.First(p => p[5] == prodSplit[0])[5];
                    var prodFin = descPRod;
                    for (var i = 2; i < prodSplit.Length; i++)
                    {
                        prodFin += ';' + prodSplit[i];
                    }
                    listprod.Add(prodFin);
                    listUri.Add(Helper.Table.First(p => p[5] == descPRod)[16].Split('\\').Last());
                }
                var inte = new Intent(this, typeof(ActivityCart));
                inte.PutExtra("Type", Convert.ToInt32(type));
                inte.PutExtra("prod", listprod.ToArray());
                inte.PutExtra("uri", listUri.ToArray());
                inte.PutExtra("ndoc", nDoc);
                inte.PutExtra("mod", true);
                inte.PutExtra("first", false);

                var tesListr = new List<string>();
                using (var sw = new StreamReader(path + "/doctes.csv"))
                {
                    while (!sw.EndOfStream)
                    {
                        tesListr.Add(sw.ReadLine());
                    }
                }

                if (tesListr.Count == 1)
                {
                    System.IO.File.Delete(path + "/doctes.csv");
                    System.IO.File.Delete(path + "/docrig.csv");
                }
                else
                {
                    using (var sw = new StreamWriter(path + "/doctes.csv"))
                    {
                        for (var i = 0; i < tesListr.Count; i++)
                        {
                            if (i != Convert.ToInt32(nDoc))
                            {
                                sw.WriteLine(tesListr[i]);
                            }
                        }
                    }
                    var rigListr = new List<string>();
                    var rigListTemp = new List<string>();
                    using (var sr = new StreamReader(path + "/docrig.csv"))
                    {
                        while (!sr.EndOfStream)
                        {
                            rigListr.Add(sr.ReadLine());
                        }
                    }
                    rigListTemp.AddRange(rigListr.Where(rig => !rig.Split(';')[1].Contains(nDoc.ToString())));
                    using (var sw = new StreamWriter(path + "/docRig.csv"))
                    {
                        foreach (var rig in rigListTemp)
                        {
                            sw.WriteLine(rig);
                        }
                    }
                }
                StartActivity(inte);



            });
            builder.Show();
        }

        private void ListViewHist_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Sei sicuro di voler eliminare l'ordine");
            builder.SetCancelable(true);
            builder.SetNegativeButton("No", delegate { });
            builder.SetPositiveButton("Si", delegate
            {

                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                               .DirectoryDownloads).AbsolutePath + "/Sarin";
                var tesList = new List<string>();
                using (var sw = new StreamReader(path + "/doctes.csv"))
                {
                    while (!sw.EndOfStream)
                    {
                        tesList.Add(sw.ReadLine());
                    }
                }

                if (tesList.Count == 1)
                {
                    System.IO.File.Delete(path + "/doctes.csv");
                    System.IO.File.Delete(path + "/docrig.csv");
                }
                else
                {
                    using (var sw = new StreamWriter(path + "/doctes.csv"))
                    {
                        for (var i = 0; i < tesList.Count; i++)
                        {
                            if (i != e.Position)
                            {
                                sw.WriteLine(tesList[i]);
                            }
                        }
                    }
                    var rigList = new List<string>();
                    var rigListTemp = new List<string>();
                    using (var sr = new StreamReader(path + "/docrig.csv"))
                    {
                        while (!sr.EndOfStream)
                        {
                            rigList.Add(sr.ReadLine());
                        }
                    }
                    rigListTemp.AddRange(rigList.Where(rig => !rig.Split(';')[1].Contains(e.Position.ToString())));
                    using (var sw = new StreamWriter(path + "/docRig.csv"))
                    {
                        foreach (var rig in rigListTemp)
                        {
                            sw.WriteLine(rig);
                        }
                    }
                }
                Refresh();
            });
            builder.Show();

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ActionbarHist, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            switch (id)
            {
                case Resource.Id.sendHistory:
                    var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                                   .DirectoryDownloads).AbsolutePath + "/Sarin";
                    var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    csvlist.Add(path + "/doctes.csv");
                    csvlist.Add(path + "/docrig.csv");
                    var directory = new File(pathpp + "/photoa");
                    var files = directory.ListFiles();
                    if (files != null)
                        foreach (var t in files)
                        {
                            csvlist.Add(t.Name);
                        }

                    foreach (var csv in csvlist)
                    {
                        var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                        var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                        var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                        using (var reader = new StreamReader(csv))
                        {
                            var name = csv.Split('/').Last();
                            var url = $"ftp://{ip}/{name}";
                            var request = (FtpWebRequest)WebRequest.Create(url);
                            request.Method = WebRequestMethods.Ftp.UploadFile;
                            request.Credentials = new NetworkCredential(usern, passw);


                            var fileContents = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                            reader.Close();
                            request.ContentLength = fileContents.Length;

                            var requestStream = request.GetRequestStream();
                            requestStream.Write(fileContents, 0, fileContents.Length);
                            requestStream.Close();
                        }


                    }
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }
    }
}

namespace ExtensionMethods
{
    public static class Extension
    {/// <summary>
     /// Simple converter String->Decimal
     /// </summary>
     /// <param name="str">Value</param>
     /// <returns></returns>
        public static decimal ToDecimal(this string str)
        {
            decimal result = 0;
            try
            {
                result = Convert.ToDecimal(str);
            }
            catch
            {
                result = 0;
            }

            return result;
        }

        public static string GetName(this string codArt)
        {
            var artTab = Helper.Table;
            codArt = codArt.Split(';')[0];
            return artTab.First(prod => prod[0] == codArt)[1];
        }
    }

}