using System;
using System.Collections.Generic;
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
using Android.Views;
using Android.Widget;
using GestioneSarin2.Activity;
using GestioneSarin2.Other_class_and_Helper;
using AlertDialog = Android.App.AlertDialog;
using Environment = System.Environment;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace GestioneSarin2
{
    [Activity(Label = "         Storico Ordini", Theme = "@style/AppThemeNo", ParentActivity = typeof(ActivityHome))]
    public class ActivityHist : AppCompatActivity
    {
        private ListView listViewHist;
        private List<string> csvlist;

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
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            var clienti = Helper.GetClienti(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            var listoOrdines = new List<Ordine>();
            {


                var pathord = path + "/docTes.csv";
                var ordDet = new List<string>();
                using (var streamWriter = new StreamReader(pathord))
                {
                    var ordStr = streamWriter.ReadToEnd();

                    ordDet = ordStr.Split(
                        new[] { Environment.NewLine },
                        StringSplitOptions.None
                    ).ToList();

                }
                ordDet.RemoveAt(ordDet.Count - 1);
                foreach (var ord in ordDet)
                {
                    var testa = ord.Split(';');
                    var nameTemp = clienti.First(list => ("C" + list[0]).Contains(testa[5]))[1];
                    listoOrdines.Add(new Ordine
                    {
                        Date = testa[6],
                        Name = nameTemp,
                        Tot = testa[3],
                        CodCli = testa[5],
                        Type = testa[8]
                    });
                }


            }
            listViewHist.Adapter = new OrdineAdapter(listoOrdines);
        }

        private void ListViewHist_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Sei sicuro di voler modificare l'ordine");
            builder.SetCancelable(true);
            builder.SetNegativeButton("No", delegate { });
            builder.SetPositiveButton("Si", delegate
            {
                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                               .DirectoryDownloads).AbsolutePath + "/Sarin";
                var tesList = new List<string>();
                using (var sw = new StreamReader(path + "/doctes.csv"))
                {
                    tesList.Add(sw.ReadLine());
                }

                var tesSel = tesList[e.Position];
                var tesSplit = tesSel.Split(';');
                var type =tesSplit[8];
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

                var nDoc = tesSplit[1];
                var rigList=new List<string>();
                using (var sr=new StreamReader(path+"/docrig.csv"))
                {
                    var rig = sr.ReadLine();
                    var rigsplit = rig.Split(';');
                    if (rigsplit[1]==nDoc)
                    {
                        var codDesc = Helper.table.First(p => p[4] == rigsplit[2])[5];
                        var prodFin = codDesc;
                        for (var i = 2; i < rigsplit.Length-1; i++)
                        {
                            prodFin += ';' + rigsplit[i];
                        }
                        rigList.Add(prodFin);
                    }
                }

                var listprod=new List<string>();
                var listUri=new List<string>();
                foreach (var prod in rigList)
                {
                    var prodSplit = prod.Split(';');
                    var descPRod=Helper.table.First(p=>p[4])
                    listprod.Add();
                }


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
                                   .DirectoryDownloads).AbsolutePath + "/Sarin/";
                    var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    foreach (var csv in csvlist)
                    {
                        var usern = sharedPref.GetString(ActivitySettings.KeyUsern, "");
                        var passw = sharedPref.GetString(ActivitySettings.KeyPassw, "");
                        var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
                        using (var reader = new StreamReader(path + csv))
                        using (var client = new WebClient())
                        {
                            client.Credentials = new NetworkCredential(usern, passw);

                            var file = Encoding.UTF8.GetBytes(reader.ReadToEnd());

                            client.UploadData(new Uri($"ftp://{ip}/{csv}"), file);
                        }
                    }
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }
    }
}