using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GestioneSarin2.Adapter_and_Single_class;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "DED", Theme = "@style/cartTheme")]
    public class Activitytextexpan : AppCompatActivity
    {
        private CartAdapter adapt;
        private ExpandableListView listViewE;
        List<string>group=new List<string>();
        Dictionary<string,List<string>> dictionary=new Dictionary<string, List<string>>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.textExpan);
            listViewE = FindViewById<ExpandableListView>(Resource.Id.expandableListView1);
            setdata(out adapt);
            listViewE.SetAdapter(adapt);
            // Create your application here
        }

        private void setdata(out CartAdapter adapter)
        {
            var groupa = new List<string> {"a1", "a2", "a3"};
            var groupb=new List<string>{"b1","b2","b3","b4"};
            group.Add("GA");
            group.Add("GB");
            dictionary.Add(group[0],groupa);
            dictionary.Add(group[1],groupb);
            adapter=new CartAdapter(this, group,dictionary);
        }
    }
}