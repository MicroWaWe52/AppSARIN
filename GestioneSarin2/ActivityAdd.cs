using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace GestioneSarin2
{
    [Activity(Label = "ActivityAdd", Theme = "@style/AppTheme")]
    public class ActivityAdd : Activity
    {
        public string GroupSel;
        private bool subGrouop;
        private ListView listPRoduct;
        private List<List<string>> query;
        private List<string> listProd;
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
            query = Helper.table.Where(s => s[s.Count - 2] == GroupSel).ToList();
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
            foreach (var subGroup in output)
            {

                var psub = new Prodotto
                {
                    ImageUrl = query.First(list => list[3].Equals(subGroup))[15],
                    Grouop = query.First(list => list[3].Equals(subGroup))[5],
                    SubGroup = query.First(list => list[3].Equals(subGroup)).Last(),
                    Name = query.First(list => list[3].Equals(subGroup)).Last(),
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
                var subQuery = query.Where(s => s[s.Count - 1].Contains(subSelected)).ToList();
                GetInSub(subQuery);
                subGrouop = !subGrouop;
            }
            else
            {
                var text = new EditText(this);
                text.InputType = InputTypes.NumberFlagDecimal;
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Seleziona la quantita");
                builder.SetCancelable(true);
                builder.SetView(text);
                builder.SetNegativeButton("Annulla", delegate { });
                builder.SetPositiveButton("Conferma",
                    delegate
                    {
                        listProd.Add(query[e.Position][4] + ';' + text.Text + ';' + query[e.Position][12]);
                        Intent i = new Intent(this, typeof(MainActivity));
                        var array = listProd.ToArray();
                        i.PutExtra("prod", array);
                        StartActivity(i);

                    });
                builder.Show();
            }
        }

        public void GetInSub(List<List<string>> querys)
        {
            var listtemp = new List<Prodotto>();
            foreach (var sDirectoryItem in querys)
            {
                var ptemp = new Prodotto
                {
                    ImageUrl = sDirectoryItem[15],
                    Name = sDirectoryItem[5],
                    QuantityPrice = $"{sDirectoryItem[6]}pz/{sDirectoryItem[12]}€",
                    Grouop = sDirectoryItem[sDirectoryItem.Count - 2],
                    SubGroup = sDirectoryItem.Last()

                };
                listtemp.Add(ptemp);
            }
            listPRoduct.Adapter = new ProdottoAdapter(listtemp);
        }
    }
}