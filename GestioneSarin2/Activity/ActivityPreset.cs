using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech.Tts;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Environment = System.Environment;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "Preset",Theme = "@style/AppTheme")]
    public class ActivityPreset : AppCompatActivity
    {
        private ListView listPres;
        private List<string> ordini;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutPreset);
            listPres = FindViewById<ListView>(Resource.Id.listViewPres);
            listPres.ItemClick += ListPres_ItemClick;
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            string file;
            try
            {
                using (var sw=new StreamReader(path+"/presets.csv"))
                {
                    file = sw.ReadToEnd();
                }
              
                var clienti = Helper.GetClienti(path);
                ordini = file.Split('#').ToList();
                ordini.RemoveAt(ordini.Count - 1);
                var listoOrdines = new List<Ordine>();
                foreach (var ordine in ordini)
                {
                    var ordDet = ordine.Split(
                        new[] { Environment.NewLine },
                        StringSplitOptions.None
                    ).ToList();
                    ordDet.RemoveAt(ordDet.Count - 1);
                    var testa = ordDet.Last().Split(';');
                    string nameTemp;
                    try
                    {
                        nameTemp = clienti.First(list => list[0] == testa[7])[1];
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    listoOrdines.Add(new Ordine
                    {
                        Date = testa[8],
                        Name = nameTemp,
                        Tot = testa[3],
                        CodCli = testa[5]
                    });

                }
                listPres.Adapter = new OrdineAdapter(listoOrdines);
            }
            catch (Exception e)
            {
                
            }
           
            // Create your application here
        }

        private void ListPres_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Vuoi rifare quest'ordine?");
            builder.SetCancelable(true);
            builder.SetNegativeButton("No", delegate { });
            builder.SetPositiveButton("Si", delegate
            {
                Order(ordini[e.Position]);
            });
            builder.Show();
        }

        private void Order(string order)
        {
            var ordsplit = order.Split(';');
            
             order = order.Replace(ordsplit.Last(), DateTime.Now.ToShortDateString());
            
            
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var curDir = new Java.IO.File(path);
            var csvlist = curDir.List();
            var last = 0;
            if (csvlist != null)
            {
                var tempcsvList = csvlist.ToList();
                tempcsvList.Remove("presets.csv");
                Array.Copy(tempcsvList.ToArray(),csvlist,tempcsvList.Count);
                Array.Resize(ref csvlist,tempcsvList.Count);
                foreach (var ord in csvlist.Where(f=>f.Contains("Ordine")))
                {
                    var narr = new string(ord.Where(char.IsDigit).ToArray());
                    var n = narr.Aggregate("", (current, digit) => current + digit);

                    if (Convert.ToInt32(n) > last)
                    {
                        last = Convert.ToInt32(n);
                    }
                }
            }
            using (var streamWriter = new StreamWriter(path + $"/OrdineN{last + 1}.csv"))
            {
                streamWriter.Write(order);
            }
            Toast.MakeText(this,"Ordine effetuato",ToastLength.Short).Show();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }
    }
}