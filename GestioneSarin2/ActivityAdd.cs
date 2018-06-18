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
using Environment = System.Environment;

namespace GestioneSarin2
{
    [Activity(Label = "ActivityAdd")]
    public class ActivityAdd : Activity
    {
        private ListView listPRoduct;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutAdd);
            listPRoduct = FindViewById<ListView>(Resource.Id.listViewProdottiact);
            //WAIT richiesta al database ,costruzione dell prodotto per ogni prodotto con immagine scaricata dal server.
            var listatemp=new List<Prodotto>();
            var ptemp=new Prodotto();

            ptemp.ImageUrl = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/littlegeorge.png";
            ptemp.Name = "test";
            ptemp.QuantityPrice = "2/2€";


            listatemp.Add(ptemp);
            listatemp.Add(ptemp);
            listatemp.Add(ptemp);
        
            listPRoduct.Adapter=new ProdottoAdapter(listatemp);
            // Create your application here
        }
    }
}