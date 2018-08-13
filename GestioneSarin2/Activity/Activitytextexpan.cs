using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GestioneSarin2.Adapter_and_Single_class;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "DED", Theme = "@style/cartTheme")]
    public class Activitytextexpan : AppCompatActivity
    {
        private ExpandableListView listViewE;
        private RadioButton addButton;
        private List<string> listprod = new List<string>();
        List<string> group = new List<string>();
        Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.textExpan);
            listViewE = FindViewById<ExpandableListView>(Resource.Id.expandableListView1);
            addButton = FindViewById<RadioButton>(Resource.Id.ButtonAggiungi);
            addButton.Click += AddButton_Click;
            ;
            // Create your application here
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            listprod.Add(
                "00.14               ;1.00;1.000NR/1.800€;;;Ciao sono una nota di dimensioni non troppo grandi ma media diciamo");
            listprod.Add(
                "10008               ;1.00;0.000NR/1.800€;;;Ciao sono una nota di dimensioni non troppo grandi ma media diciamo");

            Setdata();
        }

        private void Setdata()
        {
            group.Clear();
            dictionary.Clear();
            var i = 0;
            foreach (var prod in listprod)
            {
                var listInfo = new List<string>();
                listInfo.AddRange(Enumerable.Repeat(prod, 7));
                var p = Helper.Table.Find(ppp => ppp[0] == prod.Split(';')[0]);
                group.Add(p[0] + p[1]);
                dictionary.Add(group[i], listInfo);
                i++;
            }

            var ada = new CartAdapter(this, group, dictionary);
            listViewE.SetAdapter(ada);

        }
    }
}