using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivitySettings", MainLauncher = false)]
    public class ActivitySettings : PreferenceActivity,ISharedPreferencesOnSharedPreferenceChangeListener
    {

        public static readonly string KeyAutoDelete = "pref_key_auto_delete";
        public static readonly string KeySmsDeleteLimit = "pref_key_sms_delete_limit";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.PreferenceScxml);

            // Create your application here
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(KeyAutoDelete))
            {
                Preference connectionPref = FindPreference(key);
                // Set summary to be the user-description for the selected value
                connectionPref.SetDefaultValue(sharedPreferences.GetBoolean(key, true));
                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                               .DirectoryDownloads).AbsolutePath + "/Sarin";
                Directory.Delete(path,true);
            }
            else if (key.Equals(KeySmsDeleteLimit))
            {
                Preference connectionPref = FindPreference(key);
                // Set summary to be the user-description for the selected value
                connectionPref.SetDefaultValue(sharedPreferences.GetString(key, ""));
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            PreferenceScreen.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }
    }

    
}