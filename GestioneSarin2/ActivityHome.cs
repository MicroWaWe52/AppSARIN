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
        private ImageButton ordButton;
        private ImageButton histButton;
        private ImageButton presetButton;
        private ImageButton settButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutHome);

            ordButton = FindViewById<ImageButton>(Resource.Id.imageButtonHomeOrd);
            histButton = FindViewById<ImageButton>(Resource.Id.imageButtonHist);
            presetButton = FindViewById<ImageButton>(Resource.Id.imageButtonPres);
            settButton = FindViewById<ImageButton>(Resource.Id.imageButtonSett);
            ordButton.Click += OrdButton_Click;
            histButton.Click += HistButton_Click;
            presetButton.Click += PresetButton_Click;
            settButton.Click += SettButton_Click;
            // Create your application here
        }

        private void SettButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();

        }

        private void PresetButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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