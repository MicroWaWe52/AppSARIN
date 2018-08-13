using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using GestioneSarin2;
using GestioneSarin2.Adapter_and_Single_class;
using Java.IO;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Void = Java.Lang.Void;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityGallery", Theme = "@style/AppThemeNo")]
    public class ActivityGallery : Android.App.Activity
    {
        private RecyclerView recyclerView;
        private List<string> listProd;
        private List<string> listURI;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutGalleryG);
            var galleryList = Helper.GetImgList();
            var layout = FindViewById<Gallery>(Resource.Id.galleryCata);
            layout.Adapter = new ImageAdapter(this, galleryList);
            try
            {
                listProd = Intent.GetStringArrayExtra("prod").ToList();
            }
            catch (Exception)
            {
                listProd = new List<string>();
            }
            try
            {
                listURI = Intent.GetStringArrayExtra("uri").ToList();
            }
            catch (Exception)
            {
                listURI = new List<string>();
            }

            layout.ItemClick += Layout_ItemClick; ;

        }

        private void Layout_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var id = (e.Position + 1).ToString();
            var digitsToAdd = 3 - id.Length;
            var fileName = "";
            switch (digitsToAdd)
            {
                case 0:
                    fileName = $"ima-{e.Id}";
                    break;
                case 1:
                    var idTemp1 = id;
                    idTemp1 += "0";
                    char[] array1 = idTemp1.ToCharArray();
                    Array.Reverse(array1);
                    var x1 = new string(array1);
                    fileName = $"ima-{x1}";
                    break;
                case 2:
                    var idTemp2 = id;
                    idTemp2 += "00";
                    char[] array = idTemp2.ToCharArray();
                    Array.Reverse(array);
                    var x = new string(array);
                    fileName = $"ima-{x}";
                    break;
            }
            var textRecognizer = new TextRecognizer.Builder(this).Build();
            if (!textRecognizer.IsOperational)
            {
                Log.Error("recogn", "dependencies not avaiable");
            }
            else
            {
                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                               .DirectoryDownloads).AbsolutePath + "/Sarin/imacat/";
                using (Stream stream = new FileStream(path + fileName + ".jpg", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var f = new Frame.Builder().SetBitmap(BitmapFactory.DecodeStream(stream)).Build();
                    var array = textRecognizer.Detect(f);
                    var sbuilder = new StringBuilder();
                    for (int i = 0; i < array.Size(); i++)
                    {
                        TextBlock item = (TextBlock)array.ValueAt(i);
                        sbuilder.Append(item.Value + '#');
                    }

                    var stringReco = sbuilder.ToString();
                    var recoSplit = stringReco.Split('#');
                    var recoFinalSplit = new List<string>();
                    foreach (var str in recoSplit)
                    {
                        var splitTemp = str.Split('\n');
                        recoFinalSplit.AddRange(splitTemp);
                    }

                    var query = new List<string>();
                    foreach (var VARIABLE in Helper.GetArticoli(this))
                    {
                        query.Add(VARIABLE[4]);
                    }

                    foreach (var maybeCod in recoFinalSplit)
                    {
                        var cod = maybeCod;
                        if (maybeCod.Contains('/'))
                        {
                            cod = maybeCod.Split('/')[0];
                        }
                        var result = Enumerable.Range(0, query.Count)
                            .Where(i => query[i] == cod);


                        if (result.Count() != 0)
                        {
                            var resultl = result.ToList();

                            var list = new ListView(this);
                            var prodResult = new List<string>();
                            foreach (var prod in resultl)
                            {
                                prodResult.Add(Helper.Table[prod][5]);
                            }
                            list.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, prodResult);
                            list.ItemClick += List_ItemClick;

                            var builder = new Android.App.AlertDialog.Builder(this);
                            builder.SetTitle("Seleziona taglia");
                            builder.SetView(list);
                            adTagle = builder.Create();
                            adTagle.Show();

                        }
                    }
                }
            }
        }

        private Android.App.AlertDialog adTagle;
        private void List_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            adTagle.Dismiss();
            var taglia = ((ListView)sender).Adapter.GetItem(e.Position).ToString();
            var prod = Helper.Table.First(i => i[5] == taglia);
            Order(prod.ToList());

        }

        public void Order(List<string> query)
        {

            var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };
            var textQta = new EditText(this) { Hint = "Quantità" };
            var textPPart = new EditText(this) { Hint = "Prezzo particolare" };
            var textScon = new EditText(this) { Hint = "Sconto" };
            var textNote = new EditText(this) { Hint = "Note aggiuntive" };
            layout.AddView(textQta);
            layout.AddView(textPPart);
            layout.AddView(textScon);
            layout.AddView(textNote);

            var builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("Aggiungi informazioni");
            builder.SetCancelable(true);
            builder.SetView(layout);
            builder.SetNegativeButton("Annulla", delegate { });
            builder.SetPositiveButton("Conferma",
                delegate
                {
                    listProd.Add($"{query[5]};{textQta.Text.Replace(',', '.')};{query[12]};{textPPart.Text};{textScon.Text};{textNote.Text}");
                    Intent i = new Intent(this, typeof(ActivityCart));
                    var urisplit = query[16].Split('\\');
                    listURI.Add(urisplit.Last());
                    var uriarr = listURI.ToArray();
                    var array = listProd.ToArray();
                    i.PutExtra("prod", array);
                    i.PutExtra("uri", uriarr);
                    i.PutExtra("first", false);
                    i.PutExtra("Type", Intent.GetIntExtra("Type", 0));
                    i.PutExtra("nprog", Intent.GetIntExtra("nprog", 0));


                    StartActivity(i);

                });
            builder.Show();

        }
    }
}

public class CreateList
{
    public string ImageTitle { get; set; }

    public int ImageId { get; set; }

    public static List<CreateList> prepareData()
    {
        var imageList = Helper.GetImgList();

        List<CreateList> theimage = new List<CreateList>();

        foreach (var t in imageList)
        {
            CreateList createList = new CreateList();
            createList.ImageTitle = t;
            theimage.Add(createList);
        }

        return theimage;
    }
}

