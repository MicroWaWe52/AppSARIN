using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Net;
using Console = System.Console;
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

            var holder = (ViewHolder)view.Tag;
            try
            {
                var photoname = prodottolList[position].ImageUrl.Split('\\');

                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                    .DirectoryDownloads).AbsolutePath;
                path += "/" + photoname.Last();
                if (!System.IO.File.Exists(path))
                {
                    Helper.GetMIssPhoto(path);
                }
                using (Stream stream = new FileStream(path, FileMode.Open,FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    holder.Photo.SetImageBitmap(BitmapFactory.DecodeStream(stream));
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(parent.Context, "Alcune immagini non sono state trovate.Aggiorna l'archivio",ToastLength.Short);
            }

            holder.Name.Text = prodottolList[position].Name;
            holder.QuantPrice.Text = prodottolList[position].QuantityPrice;

            return view;
        }

        public override int Count => prodottolList.Count;

        public override Prodotto this[int position] => prodottolList[position];
    }
}