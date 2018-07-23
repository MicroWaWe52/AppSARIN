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
using GestioneSarin2.Other_class_and_Helper;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityDocChoice", Theme = "@style/AppTheme")]
    public class ActivityDocChoice : AppCompatActivity
    {
        private ImageView rappoButton;
        private ImageView vendButton;
        private ImageView prevButton;
        private ImageView bollButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutDocChoice);

            rappoButton = FindViewById<ImageView>(Resource.Id.imageViewRapp);
            vendButton = FindViewById<ImageView>(Resource.Id.imageViewVend);
            prevButton = FindViewById<ImageView>(Resource.Id.imageViewPrev);
            bollButton = FindViewById<ImageView>(Resource.Id.imageViewBoll);
            rappoButton.Click += RappoButtonClick;
            vendButton.Click += VendButtonClick;

        }

        private void VendButtonClick(object sender, EventArgs e)
        {
            Intent i = new Intent(this,typeof(ActivityCustomers));
            var type = (int)DocType.Vendita;
            i.PutExtra("Type", type);
            StartActivity(i);
        }

        private void RappoButtonClick(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ActivityCustomers));
            var type = (int)DocType.Rapportino;
            i.PutExtra("Type", type);
            StartActivity(i);
        }
    }
}