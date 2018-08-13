using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GestioneSarin2
{
    class OrdineAdapter : BaseAdapter<Ordine>
    {

        private List<Ordine> ordineList;
        public override long GetItemId(int position) => position;
        public OrdineAdapter(List<Ordine> ordini)
        {
            this.ordineList = ordini;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RowModelHist, parent, false);
                var name = view.FindViewById<TextView>(Resource.Id.nameTextViewHist);
                var Data = view.FindViewById<TextView>(Resource.Id.DataTextViewHist);
                var Price = view.FindViewById<TextView>(Resource.Id.PriceTextViewHist);
                var codCli = view.FindViewById<TextView>(Resource.Id.CodCliTextViewHist);
                var type = view.FindViewById<TextView>(Resource.Id.TypeTextViewHist);
                view.Tag = new ViewHolderOrdine() { Name = name, Data = Data, Price = Price,CodCli = codCli,Type = type};

            }

            var holder = (ViewHolderOrdine)view.Tag;
          

            AssetManager am = parent.Context.Assets;
            Typeface tvName = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            holder.Name.SetTypeface(tvName, TypefaceStyle.Normal);
            holder.Name.Text = textInfo.ToTitleCase(ordineList[position].Name);
            holder.Data.SetTypeface(tvName,TypefaceStyle.Normal);
            holder.Data.Text = textInfo.ToTitleCase(ordineList[position].Date);
            holder.Price.SetTypeface(tvName, TypefaceStyle.Normal);
            holder.Price.Text = ordineList[position].Tot+"€";
            holder.CodCli.SetTypeface(tvName, TypefaceStyle.Normal);
            holder.CodCli.Text = ordineList[position].CodCli;
            holder.Type.SetTypeface(tvName,TypefaceStyle.Normal);
            holder.Type.Text = ordineList[position].Type;

            return view;
        }

        public override int Count => ordineList.Count;

        public override Ordine this[int position] => ordineList[position];
    }


}