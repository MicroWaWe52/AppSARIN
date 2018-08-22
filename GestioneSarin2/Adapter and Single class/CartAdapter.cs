using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Object = Java.Lang.Object;

namespace GestioneSarin2.Adapter_and_Single_class
{
    class CartAdapter : BaseExpandableListAdapter
    {
        public CartAdapter(Context context, List<string> listGroup, Dictionary<string, List<string>> listChild)
        {
            Context = context;
            ListGroup = listGroup;
            ListChild = listChild;
        }

        public override Object GetChild(int groupPosition, int childPosition)
        {
            var result = new List<string>();
            ListChild.TryGetValue(ListGroup[groupPosition], out result);
            return result[childPosition];


        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            ListChild.TryGetValue(ListGroup[groupPosition], out var result);
            return result.Count;

        }

        public override int GetChildrenCount(int groupPosition)
        {
            var result = new List<string>();
            ListChild.TryGetValue(ListGroup[groupPosition], out result);
            return result.Count;
        }
        //INFO
        private ProdInfo info;
        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inf = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                convertView = inf.Inflate(Resource.Layout.itemLayout, null);
            }

            var itemView = convertView.FindViewById<TextView>(Resource.Id.textItem);
            var valView = convertView.FindViewById<TextView>(Resource.Id.textItemSoldi);
            AssetManager am = Context.Assets;
            Typeface tvDoc = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");
            itemView.SetTypeface(tvDoc,TypefaceStyle.Normal);
            valView.SetTypeface(tvDoc,TypefaceStyle.Normal);
            var content = (string)GetChild(groupPosition, childPosition);

            switch (childPosition)
            {
                case 0:
                    info = GetProdInfo(content);
                    itemView.Text = "Quantita:";
                    valView.Text = info.Quantita;
                    break;
                case 1:
                    itemView.Text = "Prezzo unitario:";
                    valView.Text = info.Unitario.ToString(CultureInfo.CurrentCulture) + "€";
                    break;
                case 2:
                    itemView.Text = "Prezzo imponibile:";
                    valView.Text = info.Imponibile.ToString(CultureInfo.CurrentCulture) + "€";
                    break;
                case 3:
                    itemView.Text = "Sconto:";
                    valView.Text = info.Sconto + "%";
                    break;
                case 4:
                    itemView.Text = "Prezzo totale:";
                    valView.Text = info.Totale.ToString(CultureInfo.CurrentCulture) + "€";
                    break;
                case 5:
                    itemView.Text = "Iva:";
                    valView.Text = info.Iva.ToString(CultureInfo.CurrentCulture) + "%";
                    break;
                case 6:
                    itemView.Text = "Note:";
                    valView.Text = info.Note;
                    break;
                default:
                    itemView.Text = "Errore";
                    valView.Text = "Errore";
                    break;
            }

            return convertView;
        }

        public ProdInfo GetProdInfo(string content)
        {
            var prodInfoSplit = content.Split(';');
            var qtaspSplit = prodInfoSplit[1].Split('/');
            var codArt = prodInfoSplit[0];
            var qta = new string((from c in qtaspSplit[0]
                                  where char.IsDigit(c) || char.IsPunctuation(c)
                                  select c
                ).ToArray());

            var puni = prodInfoSplit[2].Split('/').Last().Replace(',', '.');

            puni = new string((from c in puni
                               where char.IsDigit(c) || char.IsPunctuation(c)
                               select c
                ).ToArray());
            var ttemp = Convert.ToDecimal(Convert.ToDecimal(qta) * decimal.Parse(puni.Replace(',', '.')));
            decimal ivatem = 22;
            try
            {
                ivatem = Convert.ToDecimal(Helper.Table.First(prodl => prodl[0] == codArt)[13]);
            }
            catch (Exception e)
            {

            }
            var totIva = ttemp + (ttemp / 100) * ivatem;
            totIva = Math.Round(totIva, 3);
            decimal tot = 0;
            if (prodInfoSplit[4] == "")
            {
                prodInfoSplit[4] = "0.00";
            }
            try
            {

                var valsconto = totIva / 100 * Convert.ToDecimal(prodInfoSplit[4]);
                tot = totIva - valsconto;
                tot = Math.Round(tot, 2);
            }
            catch
            {
                // ignored
            }
          
            // var qpString = $"Q.:{qtaspSplit[0]}        Pz.U:{Convert.ToDecimal(puni)}     Imp:{ttemp}     Sc:{prodInfoSplit[4]}        Tot:{tot}    IVA:{ivatem}";
            var q = qtaspSplit[0];
            return new ProdInfo(q, Convert.ToDecimal(puni), ttemp, prodInfoSplit[4], tot, ivatem, prodInfoSplit.Last(),codArt);
        }

        public struct ProdInfo
        {
            public string Quantita { get; }
            public decimal Unitario { get; }
            public decimal Imponibile { get; }
            public string Sconto { get; }
            public decimal Totale { get; }
            public decimal Iva { get; }
            public string Note { get; }
            public string CodArt { get; }
            public ProdInfo(string quantita, decimal unitario, decimal imponibile, string sconto, decimal totale, decimal iva, string note,string codArt)
            {
                Quantita = quantita;
                Unitario = unitario;
                Imponibile = imponibile;
                Sconto = sconto;
                Totale = totale;
                Iva = iva;
                Note = note;
                CodArt = codArt;
            }
        }
        public override Object GetGroup(int groupPosition)
        {
            return ListGroup[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }
        //PROD BASE
        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.groupLayout, null);

            }
            var linfo = Getinfo(ListGroup[groupPosition]);
            string textGroup;
            try
            {
                textGroup = Helper.Table.First(art => art[0] == ListGroup[groupPosition]).ToList()[1];
                if (string.IsNullOrEmpty(textGroup))
                {
                    textGroup = linfo.Note;
                }
            }
            catch (Exception e)
            {
                textGroup = ListGroup[groupPosition];
            }

            var textGroupV = convertView.FindViewById<TextView>(Resource.Id.textGroup);
            var texgroupSoldi = convertView.FindViewById<TextView>(Resource.Id.textGroupSoldi);
            AssetManager am = Context.Assets;
            Typeface tvDoc = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");
            textGroupV.SetTypeface(tvDoc,TypefaceStyle.Normal);
            texgroupSoldi.SetTypeface(tvDoc,TypefaceStyle.Normal);
            var textinfoq = convertView.FindViewById<TextView>(Resource.Id.textGroupAll);
         
            textinfoq.Text = $"{linfo.CodArt}{linfo.Quantita}\t{linfo.Unitario}€\t{linfo.Imponibile}€\t{linfo.Sconto}%\t{linfo.Iva}%";
            texgroupSoldi.Text = linfo.Totale.ToString(CultureInfo.CurrentCulture) + "€";
            textGroupV.Text =textGroup;
            return convertView;
        }

        public ProdInfo Getinfo(string artName)
        {
            var rigDet = ListChild[artName].First();
            return GetProdInfo(rigDet);

        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        public override int GroupCount => ListGroup.Count;
        public override bool HasStableIds => false;
        public Context Context { get; set; }

        public List<string> ListGroup { get; set; }

        public Dictionary<string, List<string>> ListChild { get; set; }
    }
}