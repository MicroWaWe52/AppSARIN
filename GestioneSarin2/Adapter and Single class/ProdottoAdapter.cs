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
            holder.Name.Text = textInfo.ToTitleCase(prodottolList[position].Name) + '\n' + '\r' + prodottolList[position].Note;
            holder.QuantPrice.SetTypeface(tvName, TypefaceStyle.Normal);
            var qpSplit = prodottolList[position].QuantityPrice.Split('/');
            if (qpSplit.Length < 2) return view;
            var qta = new string((from c in qpSplit[0]
                                  where char.IsDigit(c) || char.IsPunctuation(c)
                                  select c
                 ).ToArray());

            var puni = qpSplit.Last().Replace(',', '.');

            puni = new string((from c in puni
                               where char.IsDigit(c) || char.IsPunctuation(c)
                               select c
                  ).ToArray());

            var ttemp = Convert.ToDecimal(Convert.ToSingle(qta) * float.Parse(puni.Replace(',', '.')));
            decimal ivatem = 22;
            try
            {
                ivatem = Convert.ToDecimal(Helper.Table.First(prodl => prodl[0] == prodottolList[position].CodArt)[13]);
            }
            catch (Exception e)
            {

            }

            var totIva = ttemp + (ttemp / 100) * ivatem;
            totIva = Math.Round(totIva, 2);
            decimal tot = 0;
            var um="";
            try
            {

                var valsconto = totIva / 100 * Convert.ToDecimal(prodottolList[position].Sconto);
                tot = totIva - valsconto;
                tot = Math.Round(tot, 2);
            }
            catch
            {
                // ignored
            }

            try
            {
                um = Helper.Table.First(prodl => prodl[0] == prodottolList[position].CodArt)[2];
            }
            catch 
            {
               
            }
            if (prodottolList[position].Sconto == "")
            {
                prodottolList[position].Sconto = "0.00";
            }
            var qpString = Convert.ToDecimal(qta) == 0 ? $"Pz.U:{puni}" : $"Q.:{qpSplit[0]}{um}    Pz.U:{Convert.ToDecimal(puni)}     Imp:{ttemp}     Sc:{prodottolList[position].Sconto}        Tot:{tot}    IVA:{ivatem}";
            holder.QuantPrice.Text = qpString;
            return view;
        }

        public override int Count => prodottolList.Count;

        public override Prodotto this[int position] => prodottolList[position];
    }


}