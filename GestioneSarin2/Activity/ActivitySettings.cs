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
    public class ActivitySettings : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {

        public static readonly string KeyIp = "pref_key_ip";
        public static readonly string KeyUsern = "pref_key_usern";
        public static readonly string KeyPassw = "pref_key_passw";

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
                if (valurpref!="")
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