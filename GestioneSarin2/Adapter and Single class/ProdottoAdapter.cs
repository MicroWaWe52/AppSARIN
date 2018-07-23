using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Content.Res;
using Environment = Android.OS.Environment;

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

                view.Tag = new ViewHolderProdotto { Photo = photo, Name = name, QuantPrice = quantPrice };

            }

            var holder = (ViewHolderProdotto)view.Tag;
            if (position > 0)
            {
                if (prodottolList[position].ImageUrl != prodottolList[position - 1].ImageUrl)
                {
                    try
                    {
                        var photoname = prodottolList[position].ImageUrl.Split('\\');

                        var path = Environment.GetExternalStoragePublicDirectory(Environment
                                       .DirectoryDownloads).AbsolutePath + "/Sarin";
                        path += "/" + photoname.Last();
                        if (!File.Exists(path))
                        {
                            //   Helper.GetMIssPhoto(path,parent.Context);
                        }

                        using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            holder.Photo.SetImageBitmap(BitmapFactory.DecodeStream(stream));
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(parent.Context, "Alcune immagini non sono state trovate.Aggiorna l'archivio", ToastLength.Short);
                    }
                }
            }
            else
            {
                try
                {
                    var photoname = prodottolList[position].ImageUrl.Split('\\');

                    var path = Environment.GetExternalStoragePublicDirectory(Environment
                                   .DirectoryDownloads).AbsolutePath + "/Sarin";
                    path += "/" + photoname.Last();
                    if (!File.Exists(path))
                    {
                        //    Helper.GetMIssPhoto(path,parent.Context);
                    }

                    using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        holder.Photo.SetImageBitmap(BitmapFactory.DecodeStream(stream));
                    }
                }
                catch (Exception e)
                {
                    Toast.MakeText(parent.Context, "Alcune immagini non sono state trovate.Aggiorna l'archivio", ToastLength.Short);
                }
            }

            AssetManager am = parent.Context.Assets;
            Typeface tvName = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            holder.Name.SetTypeface(tvName, TypefaceStyle.Normal);
            holder.Name.Text = textInfo.ToTitleCase(prodottolList[position].Name);
            holder.QuantPrice.SetTypeface(tvName, TypefaceStyle.Normal);
            var qpSplit = prodottolList[position].QuantityPrice.Split('/');
            if (qpSplit.Length < 2) return view;
            var qta = new string(qpSplit[0].Where(char.IsDigit).ToArray());
            var puni = qpSplit.Last();
            puni = puni.Where(ch => char.IsNumber(ch) || char.IsPunctuation(ch)).Aggregate("", (current, ch) => current + ch);

            var ttemp = Convert.ToDecimal(Convert.ToInt32(qta) * float.Parse(puni.Replace(',', '.')));
            var ivatem = Convert.ToDecimal(Helper.table.First(prodl => prodl[4] == prodottolList[position].CodArt)[6]);

            var totIva = ttemp+(ttemp / 100) * ivatem;
            totIva = Math.Round(totIva, 2);

            var qpString = $"Qta:{qpSplit[0]}    P.Uni {qpSplit.Last()}     Tot+IVA:{totIva}";
            holder.QuantPrice.Text = qpString;



            return view;
        }

        public override int Count => prodottolList.Count;

        public override Prodotto this[int position] => prodottolList[position];
    }
}