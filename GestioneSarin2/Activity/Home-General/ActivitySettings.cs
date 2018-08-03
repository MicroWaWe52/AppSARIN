using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    [Activity(Label = "ActivitySettings", MainLauncher = false, Theme = "@style/AppTheme")]
    public class ActivitySettings : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener, Preference.IOnPreferenceClickListener
    {

        public static readonly string KeyIp = "pref_key_ip";
        public static readonly string KeyUsern = "pref_key_usern";
        public static readonly string KeyPassw = "pref_key_passw";
        public static readonly string KeyUpdate = "pref_key_update";
        public static readonly string KeyDelete = "pref_key_delete";
        public static readonly string KeyCodAge = "pref_key_codAge";


        protected override void OnCreate(Bundle savedInstanceState)
        {

            var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);

            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.PreferenceScxml);

            var keyList = new List<string> { KeyIp, KeyUsern, KeyPassw };
            Preference pref;
            foreach (var key in keyList)
            {
                pref = FindPreference(key);
                var valurpref = sharedPref.GetString(key, "");
                if (valurpref != "")
                {
                    pref.Summary = valurpref;
                }
            }


            // Create your application here
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(KeyIp))
            {
                Preference connectionPref = FindPreference(key);
                connectionPref.SetDefaultValue(sharedPreferences.GetString(key, ""));
                connectionPref.Summary = sharedPreferences.GetString(key, "");
            }
            else if (key.Equals(KeyUsern))
            {
                Preference connectionPref = FindPreference(key);
                connectionPref.SetDefaultValue(sharedPreferences.GetString(key, ""));
                connectionPref.Summary = sharedPreferences.GetString(key, "");

            }
            else if (key.Equals(KeyPassw))
            {
                Preference connectionPref = FindPreference(key);
                connectionPref.SetDefaultValue(sharedPreferences.GetString(key, ""));
                connectionPref.Summary = sharedPreferences.GetString(key, "");

            }else if (key.Equals(KeyCodAge))
            {
                Preference connectionPref = FindPreference(key);
                connectionPref.SetDefaultValue(sharedPreferences.GetString(key, ""));
                connectionPref.Summary = sharedPreferences.GetString(key, "");
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            PreferenceScreen.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
            FindPreference(KeyUpdate).PreferenceClick += ActivitySettings_PreferenceClick_Update;
            FindPreference(KeyDelete).PreferenceClick += ActivitySettings_PreferenceClick_Delete;
        }

        private void ActivitySettings_PreferenceClick_Delete(object sender, Preference.PreferenceClickEventArgs e)
        {
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            Directory.Delete(path, true);
        }

        private void ActivitySettings_PreferenceClick_Update(object sender, Preference.PreferenceClickEventArgs e)
        {
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            Task.Factory.StartNew(() =>
           {
               if (!Directory.Exists(path))
               {
                   Directory.CreateDirectory(path);
               }
               Helper.GetClienti(this, true);
               Helper.GetArticoli(this, true);
              // Helper.GetDest(this, true);
               Helper.GetAge(this,true);
               RunOnUiThread(() =>
               {
                   Toast.MakeText(this, "Aggiornamento completato", ToastLength.Short).Show();
               });

           });
            Toast.MakeText(this, "Aggiornamento in corso...", ToastLength.Short).Show();
        }

        protected override void OnPause()
        {
            base.OnPause();
            PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public bool OnPreferenceClick(Preference preference)
        {
            return false;
        }
    }


}