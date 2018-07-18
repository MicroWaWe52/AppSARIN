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

            i.SetScaleType(ImageView.ScaleType.CenterInside);

            return i;
        }

       
    }
}


