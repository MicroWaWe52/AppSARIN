using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Environment = System.Environment;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace GestioneSarin2
{
    [Activity(Label = "Storico Ordini",Theme = "@style/AppThemeNo", ParentActivity = typeof(ActivityHome))]
    public class ActivityHist : AppCompatActivity
    {
        private ListView listViewHist;
        private string[] csvlist;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.layoutHistory);
                // Create your application here
                listViewHist = FindViewById<ListView>(Resource.Id.listViewHist);
          
                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                               .DirectoryDownloads).AbsolutePath + "/Sarin";
                var clienti = Helper.GetClienti(path);
       
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var curDir = new File(path);
                var csvlist = curDir.List();
                var listoOrdines = new List<Ordine>();
                if (csvlist != null)
                {
                    var tempcsvList = csvlist.ToList();
                    tempcsvList = tempcsvList.Where(csv => csv.Contains("Ordine")).ToList();
                    Array.Copy(tempcsvList.ToArray(), csvlist, tempcsvList.Count);
                    Array.Resize(ref csvlist, tempcsvList.Count);
                    foreach (var ord in csvlist)
                    {
                        var pathord = path + '/' + ord;
                        var ordDet = new List<string>();
                        using (var streamWriter = new StreamReader(pathord))
                        {
                            var ordStr = streamWriter.ReadToEnd();

                            ordDet = ordStr.Split(
                                new[] { Environment.NewLine },
                                StringSplitOptions.None
                            ).ToList();

                        }

                        if (string.IsNullOrEmpty(ordDet[ordDet.Count - 1]))
                        {
                            ordDet.RemoveAt(ordDet.Count - 1);
                        }
                        var testa = ordDet.Last().Split(';');
                        string nameTemp;
                        try
                        {
                            nameTemp= clienti.First(list => list[0] == testa[7])[1];

                        }
                        catch (Exception e)
                        {
                            continue;
                        }

                        listoOrdines.Add(new Ordine
                        {
                            Date = testa[8],
                            Name = nameTemp,
                            Tot = testa[5],
                            CodCli= testa[7]
                        });

                    }
                }
                listViewHist.Adapter = new OrdineAdapter(listoOrdines);
                //ActionBar.SetDisplayHomeAsUpEnabled(true);
                var toolbar = FindViewById<Toolbar>(Resource.Id.my_toolbarSett);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            catch (Exception e)
            {
              
            }


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
                    foreach (var csv in csvlist)
                    {
                        using (var reader = new StreamReader(path + csv))
                        using (var client = new WebClient())
                        {
                            client.Credentials = new NetworkCredential("spigam", "123456");

                            var file = Encoding.UTF8.GetBytes(reader.ReadToEnd());

                            client.UploadData(new Uri($"ftp://217.133.0.34/{csv}.csv"), file);
                        }
                    }
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }
    }
}