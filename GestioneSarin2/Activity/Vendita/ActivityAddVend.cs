﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace GestioneSarin2
{

    [Activity(Label = "ActivityAdd", Theme = "@style/AppThemeNo", NoHistory = true)]
    public class ActivityAdd : AppCompatActivity
    {
        public string GroupSel;
        private bool subGrouop;
        private ListView listPRoduct;
        private List<List<string>> query;
        private List<string> listProd;
        private List<string> listURI;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutAdd);
            listPRoduct = FindViewById<ListView>(Resource.Id.listViewProdottiact);
            listPRoduct.ItemClick += ListPRoduct_ItemClick;
            GroupSel = Intent.GetStringExtra("gruppo");
            try
            {
                listProd = Intent.GetStringArrayExtra("prod").ToList();
            }
            catch (Exception e)
            {
                listProd = new List<string>();
            }
            try
            {
                listURI = Intent.GetStringArrayExtra("uri").ToList();
            }
            catch (Exception e)
            {
                listURI = new List<string>();
            }

            var groups = Helper.GetGroup(this);
            var codGroup = groups[0][groups[1].IndexOf(GroupSel)];
            query = Helper.GetArticoli(this).Where(s => s[3] == codGroup).ToList();
            var subGroupList = new List<string>();
            foreach (var row in query)
            {
                subGroupList.Add(row[3]);
            }
            var output = subGroupList
                .GroupBy(word => word)
                .OrderByDescending(subgroup => subgroup.Count())
                .Select(group => group.Key)
                .ToList();
            var pListSub = new List<Prodotto>();
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;
            foreach (var subGroup in output)
            {

                var psub = new Prodotto
                {
                    ImageUrl = query.First(list => list[3].Equals(subGroup))[16],
                    Grouop = query.First(list => list[3].Equals(subGroup))[2],
                    SubGroup = query.First(list => list[3].Equals(subGroup))[3],

                    Name =/* textInfo.ToTitleCase(query.First(list => list[3].Equals(subGroup))[1]),*/GroupSel,
                    QuantityPrice = ""
                };
                pListSub.Add(psub);
            }


            listPRoduct.Adapter = new ProdottoAdapter(pListSub);
            // Create your application here
        }

        private void ListPRoduct_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!subGrouop)
            {
                var subSelected = ((Prodotto)listPRoduct.GetItemAtPosition(e.Position)).SubGroup;
                var subQuery = query.Where(s => s[3].Contains(subSelected)).ToList();
                GetInSubAsync(subQuery);
                subGrouop = !subGrouop;
            }
            else
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

                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Seleziona la quantita");
                builder.SetCancelable(true);

                builder.SetView(layout);
                builder.SetNegativeButton("Annulla", delegate { });
                builder.SetPositiveButton("Conferma",
                    delegate
                    {
                        var qta = textQta.Text.Replace(',', '.');
                        if (!qta.Contains('.'))
                        {
                            qta += ".00";
                        }
                        listProd.Add($"{subqueryList[e.Position].CodArt};{qta};{subqueryList[e.Position].QuantityPrice};{textPPart.Text};{textScon.Text};{textNote.Text}");
                        subqueryList[e.Position].Note = textNote.Text;
                        Intent i = new Intent(this, typeof(ActivityCartVend));
                        var urisplit = subqueryList[e.Position].ImageUrl.Split('\\');
                        listURI.Add(urisplit.Last());
                        var uriarr = listURI.ToArray();
                        var array = listProd.ToArray();
                        i.PutExtra("prod", array);
                        i.PutExtra("uri", uriarr);
                        i.PutExtra("first", false);
                        i.PutExtra("Type", Intent.GetIntExtra("Type", 0));
                        var nprog = Intent.GetIntExtra("nprog", 0);
                        i.PutExtra("nprog", nprog);


                        StartActivity(i);

                    });
                builder.Show();
            }
        }

        private List<Prodotto> subqueryList;
        public void GetInSub(List<List<string>> querys)
        {
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            TextInfo ti = ci.TextInfo;
            var listtemp = new List<Prodotto>();
            foreach (var sDirectoryItem in querys)
            {
                var name = ti.ToLower(sDirectoryItem[0]);
                var ptemp = new Prodotto
                {
                    ImageUrl = sDirectoryItem[15],
                    Name = name,
                    QuantityPrice = $"{sDirectoryItem[7]}pz/{sDirectoryItem[12]}€",
                    Grouop = sDirectoryItem[sDirectoryItem.Count - 2],
                    SubGroup = sDirectoryItem.Last(),
                    UnitPrice = sDirectoryItem[12],
                    CodArt = sDirectoryItem[4]

                };
                listtemp.Add(ptemp);
            }
            listPRoduct.Adapter = new ProdottoAdapter(listtemp);
            subqueryList = listtemp;


        }

        public async void GetInSubAsync(List<List<string>> querys)
        {
            listPRoduct.Enabled = false;

            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            TextInfo ti = ci.TextInfo;
            var listtemp = new List<Prodotto>();
            foreach (var sDirectoryItem in querys)
            {
                var name = ti.ToLower(sDirectoryItem[1]);
                var ptemp = new Prodotto
                {
                    ImageUrl = sDirectoryItem[16],
                    Name = name,
                    QuantityPrice = $"{sDirectoryItem[7]}{sDirectoryItem[2]}/{sDirectoryItem[4]}€",
                    Grouop = sDirectoryItem[sDirectoryItem.Count - 2],
                    SubGroup = sDirectoryItem.Last(),
                    UnitPrice = sDirectoryItem[4],
                    CodArt = sDirectoryItem[0],


                };
                listtemp.Add(ptemp);
                listPRoduct.Adapter = new ProdottoAdapter(listtemp);
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            subqueryList = listtemp;
            listPRoduct.Enabled = true;

        }
    }
}