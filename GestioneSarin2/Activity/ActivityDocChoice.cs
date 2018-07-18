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

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityDocChoice",Theme = "@style/AppTheme")]
    public class ActivityDocChoice : AppCompatActivity
    {
        private ImageView vendButton;
        private ImageView rappButton;
        private ImageView presetButton;
        private ImageView settButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutHome);

            vendButton = FindViewById<ImageView>(Resource.Id.imageViewOrd);
            rappButton = FindViewById<ImageView>(Resource.Id.imageViewHist);
            presetButton = FindViewById<ImageView>(Resource.Id.imageViewPres);
            settButton = FindViewById<ImageView>(Resource.Id.imageViewSett);
            vendButton.Click += VendButtonClick;
            rappButton.Click += RappButtonClick;
            
        }

        private void RappButtonClick(object sender, EventArgs e)
        {
        }

        private void VendButtonClick(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityCustomers));
        }
    }
}