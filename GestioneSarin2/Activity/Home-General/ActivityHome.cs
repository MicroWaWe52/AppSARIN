using System;
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
using Environment = System.Environment;
using Path = System.IO.Path;

namespace GestioneSarin2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppThemeNo", MainLauncher = true)]
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
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin");
            ordButton = FindViewById<ImageView>(Resource.Id.imageViewOrd);
            histButton = FindViewById<ImageView>(Resource.Id.imageViewHist);
            presetButton = FindViewById<ImageView>(Resource.Id.imageViewPres);
            settButton = FindViewById<ImageView>(Resource.Id.imageViewSett);
            ordButton.Click += OrdButton_Click;
            histButton.Click += HistButton_Click;
            presetButton.Click += PresetButton_Click;
            settButton.Click += SettButton_Click;
            const string permissiones = Manifest.Permission.ReadExternalStorage;
            if (CheckSelfPermission(permissiones) != (int)Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, 5);
            }
            const string permissionca = Manifest.Permission.Camera;
            if (CheckSelfPermission(permissionca) != (int)Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.Camera }, 6);
            }
            const string permissionwe = Manifest.Permission.WriteExternalStorage;
            if (CheckSelfPermission(permissionwe) != (int)Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.WriteExternalStorage }, 7);
            }
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (File.Exists(path + "/first.first")) return;
            StartActivity(typeof(ActivitySettings));
            File.Create(path + "/first.first").Close();
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
            StartActivity(typeof(ActivityHist));
        }

        private void OrdButton_Click(object sender, EventArgs e)
        {
            var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            var ip = sharedPref.GetString(ActivitySettings.KeyIp, "");
            if (ip != "")
            {
                StartActivity(typeof(ActivityDocChoice));

            }
            else
            {
                Toast.MakeText(this, "Accedi ad un server prima", ToastLength.Short).Show();
            }
        }
    }
   
}