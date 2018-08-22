using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using GestioneSarin2.Other_class_and_Helper;
using System;
using System.Linq;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityDocChoice", Theme = "@style/AppThemeNo")]
    public class ActivityDocChoice : AppCompatActivity
    {
        private Button rappoButton;
        private Button vendButton;
        private Button prevButton;
        private Button bollButton;
        private Button genButton;
        private Button fattButton;
        private Button incaButton;
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
            incaButton = FindViewById<Button>(Resource.Id.imageViewInca);

            rappoButton.Click += RappoButtonClick;
            vendButton.Click += VendButtonClick;
            prevButton.Click += PrevButton_Click;
            bollButton.Click += BollButton_Click;
            fattButton.Click += FattButton_Click;
            genButton.Click += GenButton_Click;
            AssetManager am = Assets;
            Typeface tvDoc = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");
            rappoButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            vendButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            prevButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            bollButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            genButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            fattButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            incaButton.SetTypeface(tvDoc, TypefaceStyle.Normal);


        }

        private void GenButton_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = Helper.GetAge(this).First(docR => docR[2] == "G");
                if (doc.Count != 4) return;
                var i = new Intent(this, typeof(ActivityCustomers));
                var type = (int)DocType.Generico;
                i.PutExtra("Type", type);
                StartActivity(i);
            }
            catch
            {
                Toast.MakeText(this, "Non sei autorizzato a questo documento", ToastLength.Short).Show();
            }
        }

        private void FattButton_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = Helper.GetAge(this).First(docR => docR[2] == "F");
                if (doc.Count != 4) return;
                var i = new Intent(this, typeof(ActivityCustomers));
                var type = (int)DocType.Fattura;
                i.PutExtra("Type", type);
                StartActivity(i);
            }
            catch
            {
                Toast.MakeText(this, "Non sei autorizzato a questo documento", ToastLength.Short).Show();
            }
        }

        private void BollButton_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = Helper.GetAge(this).First(docR => docR[2] == "O");
                if (doc.Count != 4) return;
                var i = new Intent(this, typeof(ActivityCustomers));
                var type = (int)DocType.Bolla;
                i.PutExtra("Type", type);
                StartActivity(i);
            }
            catch
            {
                Toast.MakeText(this, "Non sei autorizzato a questo documento", ToastLength.Short).Show();
            }
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = Helper.GetAge(this).First(docR => docR[2] == "P");
                if (doc.Count != 4) return;
                var i = new Intent(this, typeof(ActivityCustomers));
                var type = (int)DocType.Preventivo;
                i.PutExtra("Type", type);
                StartActivity(i);
            }
            catch
            {
                Toast.MakeText(this, "Non sei autorizzato a questo documento", ToastLength.Short).Show();
            }

        }

        private void VendButtonClick(object sender, EventArgs e)
        {
            try
            {
                var doc = Helper.GetAge(this).First(docR => docR[2] == "B");
                if (doc.Count != 4) return;
                var i = new Intent(this, typeof(ActivityCustomers));
                var type = (int)DocType.Vendita;
                i.PutExtra("Type", type);
                StartActivity(i);
            }
            catch
            {
                Toast.MakeText(this, "Non sei autorizzato a questo documento", ToastLength.Short).Show();
            }

        }

        private void RappoButtonClick(object sender, EventArgs e)
        {
            try
            {
                var doc = Helper.GetAge(this).First(docR => docR[2] == "R");
                if (doc.Count != 4) return;
                var i = new Intent(this, typeof(ActivityCustomers));
                var type = (int)DocType.Rapportino;
                i.PutExtra("Type", type);
                StartActivity(i);
            }
            catch
            {
                Toast.MakeText(this, "Non sei autorizzato a questo documento", ToastLength.Short).Show();
            }

        }
    }
}