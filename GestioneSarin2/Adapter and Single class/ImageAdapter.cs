using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace GestioneSarin2.Adapter_and_Single_class
{
    public class ImageAdapter : BaseAdapter
    {
        Context context;
        private List<string> photos;

        public ImageAdapter(Context c, List<string> listca)
        {
            context = c;
            photos = listca;
        }

        public override int Count => photos.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView i = new ImageView(context);
            i.Id = position + 1;
            using (Stream stream = new FileStream(photos[position], FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                i.SetImageBitmap(BitmapFactory.DecodeStream(stream));
            }

            i.SetScaleType(ImageView.ScaleType.FitXy);

            i.Click += I_Click;
            return i;
        }

        private void I_Click(object sender, EventArgs e)
        {
            var ima = (ImageView)sender;
            var id = ima.Id.ToString();
            var digitsToAdd = 3 - id.Length;
            var fileName = "";
            switch (digitsToAdd)
            {
                case 0:
                    fileName = $"ima-{ima.Id}";
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
            var textRecognizer = new TextRecognizer.Builder(context).Build();
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
                        sbuilder.Append(item.Value + '/');
                    }

                    var stringReco = sbuilder.ToString();
                    var recoSplit = stringReco.Split('/');
                    var recoFinalSplit = new List<string>();
                    foreach (var str in recoSplit)
                    {
                        var splitTemp = str.Split('\n');
                        recoFinalSplit.AddRange(splitTemp);
                    }

                    var query = new List<string>();
                    foreach (var VARIABLE in Helper.GetArticoli(context))
                    {
                        query.Add(VARIABLE[4]);
                    }

                    foreach (var maybeCod in recoFinalSplit)
                    {
                        int inde = query.IndexOf(maybeCod);

                        if (inde!=-1)
                        {
                            
                        }
                    }
                }

            }


        }
    }
}


