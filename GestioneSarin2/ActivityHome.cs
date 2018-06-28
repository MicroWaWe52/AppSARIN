using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GestioneSarin2
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class ActivityHome : Activity
    {
        private ImageView ordButton;
        private ImageView histButton;
        private ImageView presetButton;
        private ImageView settButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutMain);

            ordButton = FindViewById<ImageView>(Resource.Id.imageViewOrd);
            histButton = FindViewById<ImageView>(Resource.Id.imageViewHist);
            presetButton = FindViewById<ImageView>(Resource.Id.imageViewPres);
            settButton = FindViewById<ImageView>(Resource.Id.imageViewSett);
            ordButton.Click += OrdButton_Click;
            histButton.Click += HistButton_Click;
            presetButton.Click += PresetButton_Click;
            settButton.Click += SettButton_Click;
            // Create your application here
        }

        private void SettButton_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "SETTING", ToastLength.Short).Show();

        }

        private void PresetButton_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "PRESEPIO", ToastLength.Short).Show();
        }

        private void HistButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityHist));
        }

        private void OrdButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
        }
    }
}