using System;
using System.Collections.Generic;
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

            // Create your application here
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                .DirectoryDownloads).AbsolutePath;
            var clienti = Helper.GetClienti(path);
            var descliforlist = new List<string>();
            var partitaivalist = new List<string>();
            var codcliforlist = new List<string>();
            foreach (var cliente in clienti)
            {
                codcliforlist.Add(cliente[7]);
                var parttemp = cliente[15];
                if (parttemp.Length != 0)
                {
                    parttemp = parttemp.Substring(1);
                }
                partitaivalist.Add(parttemp);


                descliforlist.Add(cliente[12]);
            }
            descliforlist.Sort();
            partitaivalist.Sort();
            codcliforlist.Sort();
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
            string codclifor;
            switch (custRadioGroup.CheckedRadioButtonId)
            {
                case 1:
                    codclifor = clienti.First(list => list[12] == custEditText.Text)[7]; break;
                case 2:
                    codclifor = clienti.First(list => list[15] == "0" + custEditText.Text)[7]; break;
                case 3:
                    codclifor = custEditText.Text; break;

            }

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
                    ada.Filter.InvokeFilter(custEditText.Text);

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

    }
}