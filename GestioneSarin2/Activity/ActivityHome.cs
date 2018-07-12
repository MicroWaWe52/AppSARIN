﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using GestioneSarin2.Activity;
using Java.Lang;
using ActionBar = Android.Support.V7.App.ActionBar;

namespace GestioneSarin2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class ActivityHome : AppCompatActivity
    {
        private ImageView ordButton;
        private ImageView histButton;
        private ImageView presetButton;
        private ImageView settButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutHome);

            ordButton = FindViewById<ImageView>(Resource.Id.imageViewOrd);
            histButton = FindViewById<ImageView>(Resource.Id.imageViewHist);
            presetButton = FindViewById<ImageView>(Resource.Id.imageViewPres);
            settButton = FindViewById<ImageView>(Resource.Id.imageViewSett);
            ordButton.Click += OrdButton_Click;
            histButton.Click += HistButton_Click;
            presetButton.Click += PresetButton_Click;
            settButton.Click += SettButton_Click;
            var st = new SpannableString("Nome ditta");
            st.SetSpan(new TypefaceSpan("FiraSans-Regular.otf"), 0, st.Length(), SpanTypes.ExclusiveExclusive);
            var sequence = st.SubSequenceFormatted(0, st.Length());
            SupportActionBar.TitleFormatted = sequence;

            const string permission = Manifest.Permission.ReadExternalStorage;
            if (CheckSelfPermission(permission) != (int)Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, 5);
            }
            
            

        }

        protected override void OnResume()
        {
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            
            if (!Directory.Exists(path))
            {
                Toast.MakeText(this, "Aggiornamento in corso...\r\n un secondo", ToastLength.Short).Show();
               Directory.CreateDirectory(path);
                Task.Factory.StartNew(() =>
                {
                    Helper.GetClienti(this, true);
                    Helper.GetArticoli(this, true);
                    Helper.GetDest(this, true);
                    RunOnUiThread( () =>
                    {
                        Toast.MakeText(this,"Aggiornamento completato",ToastLength.Short).Show();
                    });
                });

            }
            base.OnResume();
        }

        private void SettButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivitySettings));
        }

        private void PresetButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityPreset));
        }

        private void HistButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityGallery));
        }

        private void OrdButton_Click(object sender, EventArgs e)
        {
            var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
             var ip = sharedPref.GetString(ActivitySettings.KeyIp,"");
            if (ip!="")
            {
                StartActivity(typeof(ActivityCustomers));

            }
            else
            {
                Toast.MakeText(this,"Accedi ad un server prima",ToastLength.Short).Show();
            }

        }
    }
}