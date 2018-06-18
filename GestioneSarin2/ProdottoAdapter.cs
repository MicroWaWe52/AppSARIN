using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Net;
using Environment = System.Environment;

namespace GestioneSarin2
{
    
    class ProdottoAdapter : BaseAdapter<Prodotto>
    {
        private List<Prodotto> prodottolList;
        public override long GetItemId(int position) => position;
        public ProdottoAdapter(List<Prodotto> prodottos)
        {
            this.prodottolList = prodottos;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RowModel, parent, false);
                var photo = view.FindViewById<ImageView>(Resource.Id.photoImageView);
                var name = view.FindViewById<TextView>(Resource.Id.nameTextView);
                var quantPrice = view.FindViewById<TextView>(Resource.Id.QuantPriceTextView);

                view.Tag = new ViewHolder() { Photo = photo, Name = name, QuantPrice = quantPrice };

            }

           
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
            StrictMode.SetThreadPolicy(policy);
            URL url = new URL("http://www.teatrotse.com/DasGappArchives/littlegeorge.png");
            HttpURLConnection connection = (HttpURLConnection)url.OpenConnection();
            Stream issInputStream = connection.InputStream;
            Bitmap img = BitmapFactory.DecodeStream(issInputStream);
          
            
            var holder = (ViewHolder)view.Tag;
            holder.Photo.SetImageBitmap(img);
            holder.Name.Text = prodottolList[position].Name;
            holder.QuantPrice.Text = prodottolList[position].QuantityPrice;

            return view;
        }

        public override int Count => prodottolList.Count;

        public override Prodotto this[int position] => prodottolList[position];
    }
}