using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Environment = System.Environment;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityCustomers", Theme = "@style/AppTheme", MainLauncher = false)]
    public class ActivityCustomers : AppCompatActivity
    {
        private RadioGroup custRadioGroup;
        private ListView custListView;
        private EditText custEditText;
        private RadioButton descRadioButton;
        private RadioButton pivaRadioButton;
        private RadioButton codRadioButton;
        private List<List<string>> clienti;
        List<string> descliforlist = new List<string>();
        List<string> partitaivalist = new List<string>();
        List<string> codcliforlist = new List<string>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutCustomers);
            custRadioGroup = FindViewById<RadioGroup>(Resource.Id.radioGroupSearchCust);
            custListView = FindViewById<ListView>(Resource.Id.listViewSearchCust);
            custEditText = FindViewById<EditText>(Resource.Id.custSearch);
            descRadioButton = FindViewById<RadioButton>(Resource.Id.radioButtonSearchDesc);
            pivaRadioButton = FindViewById<RadioButton>(Resource.Id.radioButtonSearchIva);
            codRadioButton = FindViewById<RadioButton>(Resource.Id.radioButtonSearchCod);
            custListView.ItemClick += CustListView_ItemClick;
            AssetManager am = Assets;
            Typeface tvName = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");
            codRadioButton.SetTypeface(tvName, TypefaceStyle.Normal);
            descRadioButton.SetTypeface(tvName, TypefaceStyle.Normal);
            codRadioButton.SetTypeface(tvName, TypefaceStyle.Normal);
            // Create your application here
            try
            {
                clienti = Helper.GetClienti(this);
            }
            catch (Exception e)
            {
                clienti = Helper.GetClienti(this, true);
            }

            foreach (var cliente in clienti)
            {
                codcliforlist.Add(cliente[0]);
                var parttemp = cliente[3];
                partitaivalist.Add(parttemp);


                descliforlist.Add(cliente[1]);
            }

            descliforlist = descliforlist
                .GroupBy(word => word)
                .Select(group => group.Key).ToList();
            partitaivalist = partitaivalist
                .GroupBy(word => word)
                .Select(group => group.Key).ToList();
            codcliforlist = codcliforlist
                .GroupBy(word => word)
                .Select(group => group.Key).ToList();

            custRadioGroup.CheckedChange += (s, e) =>
            {
                if (custRadioGroup.CheckedRadioButtonId == descRadioButton.Id)
                {


                    custListView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                        descliforlist);
                }
                else if (custRadioGroup.CheckedRadioButtonId == pivaRadioButton.Id)
                {

                    custListView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                        partitaivalist);
                }
                else if (custRadioGroup.CheckedRadioButtonId == codRadioButton.Id)
                {
                    custListView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                        codcliforlist);
                }
            };
            custListView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                descliforlist);

            custEditText.TextChanged += (send, arg) =>
            {
                if (custRadioGroup.CheckedRadioButtonId == descRadioButton.Id)
                {

                    var ada = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                        descliforlist);
                    ada.Filter.InvokeFilter(custEditText.Text);
                    custListView.Adapter = ada;
                }
                else if (custRadioGroup.CheckedRadioButtonId == pivaRadioButton.Id)
                {
                    var ada = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                        partitaivalist);
                    ada.Filter.InvokeFilter('0' + custEditText.Text);

                    custListView.Adapter = ada;
                }
                else if (custRadioGroup.CheckedRadioButtonId == codRadioButton.Id)
                {
                    var ada = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                        codcliforlist);
                    ada.Filter.InvokeFilter(custEditText.Text);

                    custListView.Adapter = ada;
                }
            };
        }

        private string codclifor;
        private List<List<string>> query;
        private Dictionary<string, List<string>> dictDest;
        private void CustListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            query = Helper.GetClienti(this);
            List<string> items = new List<string>();
            dictDest = new Dictionary<string, List<string>>();
            for (int i = 0; i < custListView.Adapter.Count; i++)
            {
                items.Add(custListView.Adapter.GetItem(i).ToString());
            }
            if (custRadioGroup.CheckedRadioButtonId == descRadioButton.Id)
            {

                codclifor = clienti.First(cod => cod[1] == items[e.Position])[0];
            }
            else if (custRadioGroup.CheckedRadioButtonId == pivaRadioButton.Id)
            {
                codclifor = clienti.First(list => list[3] == items[e.Position])[0];
            }
            else if (custRadioGroup.CheckedRadioButtonId == codRadioButton.Id)
            {
                codclifor = items[e.Position];
            }

            var builder = new AlertDialog.Builder(this);

            var output = query
                .GroupBy(word =>
                {
                    var codde = word[0].ToCharArray();
                    var ret= new string(codde.Take(6).ToArray());
                    return ret;
                })
                .Select(group => group.Key)
                .ToList();
            foreach (var group in output)
            {
                var l = query.Where(p =>
                {
                    var codde = p[0].ToCharArray();
                    var ret = new string(codde.Take(6).ToArray());
                    return ret== group;
                }).ToList();
                var addList = new List<string>();
                foreach (var add in l)
                {
                    addList.Add(add[1]);
                }
                dictDest.Add(group, addList);
            }

            var c = new string(codclifor.Take(6).ToArray());
            var listv = new ListView(this)
            {
                Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, dictDest[c])
            };
            builder.SetTitle("Seleziona destinazione");
            builder.SetCancelable(true);
            listv.ItemClick += Listv_ItemClick;
            builder.SetView(listv);
            builder.Show();



        }

        private void Listv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var codCli = new string(codclifor.Take(6).ToArray());

            codclifor = query.First(ci => ci[1] == dictDest[codCli][e.Position])[0];
            var codDest=new string(codclifor.Skip(6).ToArray());
            using (StreamWriter stream = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
            {
                stream.WriteLine(codCli + '/' + codDest);
            }
            Intent inte = new Intent(this, typeof(ActivityCart));
            inte.PutExtra("first", false);
            var type = Intent.GetIntExtra("Type", 0);
            inte.PutExtra("Type", type);
            inte.PutExtra("nprog", Intent.GetIntExtra("nprog", 0));

            StartActivity(inte);
        }
    }
}