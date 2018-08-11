using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Environment = System.Environment;

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

            }
            else if (key.Equals(KeyCodAge))
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
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            Directory.Delete(path, true);
        }

        private void ActivitySettings_PreferenceClick_Update(object sender, Preference.PreferenceClickEventArgs e)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Helper.GetClienti(this, true);
                    Helper.GetArticoli(this, true);
                    Helper.GetAge(this, true);
                    Helper.GetGroup(this, true);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Aggiornamento completato", ToastLength.Short).Show();
                        File.Create(path + "/first.dow");
                    });
                }
                catch
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Aggiornamento non riuscito", ToastLength.Short).Show();
                    });
                }
               

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

        public override void OnBackPressed()
        {
           
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (!File.Exists(path+"/first.dow"))
            {
               Toast.MakeText(this,"Scarica i file almeno una volta prima",ToastLength.Short).Show(); 
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }


}