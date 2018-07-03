using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Environment = System.Environment;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityCustomers", Theme = "@style/AppTheme", MainLauncher = true)]
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
            // Create your application here
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                .DirectoryDownloads).AbsolutePath+"/Sarin";
            clienti = Helper.GetClienti(path);
           
            foreach (var cliente in clienti)
            {
                codcliforlist.Add(cliente[0]);
                var parttemp = cliente[8];
                partitaivalist.Add(parttemp);


                descliforlist.Add(cliente[1]);
            }
          
            descliforlist.RemoveAt(0);
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
                    ada.Filter.InvokeFilter('0'+custEditText.Text);

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

        private void CustListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var codclifor = "";
            List<string> items = new List<string>();
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
                codclifor = clienti.First(list => list[8] == items[e.Position])[0];
            }
            else if (custRadioGroup.CheckedRadioButtonId == codRadioButton.Id)
            {
                codclifor =items[e.Position];
            }

            using (StreamWriter stream = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
            {
                 stream.WriteLine(codclifor);
            }
            Intent inte =new Intent(this,typeof(MainActivity));
            inte.PutExtra("first", false);
            StartActivity(inte);

        }
    }
}