using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
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
        private Button rappoButton;
        private Button vendButton;
        private Button prevButton;
        private Button bollButton;
        private Button genButton;
        private Button fattButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutDocChoice);

            rappoButton = FindViewById<Button>(Resource.Id.imageViewRapp);
            vendButton = FindViewById<Button>(Resource.Id.imageViewOrd);
            prevButton = FindViewById<Button>(Resource.Id.imageViewPrev);
            bollButton = FindViewById<Button>(Resource.Id.imageViewBoll);
            fattButton = FindViewById<Button>(Resource.Id.imageViewFatt);
            genButton = FindViewById<Button>(Resource.Id.imageViewGen);

            rappoButton.Click += RappoButtonClick;
            vendButton.Click += VendButtonClick;
            AssetManager am = Assets;
            Typeface tvName = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");
            rappoButton.SetTypeface(tvName,TypefaceStyle.Normal);
            vendButton.SetTypeface(tvName, TypefaceStyle.Normal);
            prevButton.SetTypeface(tvName, TypefaceStyle.Normal);
            bollButton.SetTypeface(tvName, TypefaceStyle.Normal);
            genButton.SetTypeface(tvName, TypefaceStyle.Normal);
            fattButton.SetTypeface(tvName, TypefaceStyle.Normal);


        }

        private void VendButtonClick(object sender, EventArgs e)
        {
            var doc = Helper.GetAge(this).First(docR => docR[2] == "B");
            if (doc.Count != 4) return;
            var i = new Intent(this, typeof(ActivityCustomers));
            var type = (int)DocType.Vendita;
            i.PutExtra("Type", type);
            StartActivity(i);

        }

        private void RappoButtonClick(object sender, EventArgs e)
        {
            var doc = Helper.GetAge(this).First(docR => docR[2] == "R");
            if (doc.Count != 4) return;
            Intent i = new Intent(this, typeof(ActivityCustomers));
            var type = (int)DocType.Rapportino;
            i.PutExtra("Type", type);
            StartActivity(i);
        }
    }
}