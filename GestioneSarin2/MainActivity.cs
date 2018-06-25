using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware;
using Android.Net;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Telecom;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Environment = System.Environment;

namespace GestioneSarin2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private List<string> listprod;
        private List<List<string>> query = Helper.table;
        private ListView listView;
        private List<string> listURI;
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    listprod = new List<string>();
                    listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);
                    return true;
                case Resource.Id.navigation_dashboard:

                    var lw = new ListView(this);
                    lw.ItemClick += Lw_ItemClick;
                    lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Helper.GetGroup());
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Seleziona gruppo");
                    builder.SetCancelable(true);
                    builder.SetView(lw);
                    builder.SetNegativeButton("Annulla", delegate { });
                    builder.Show();
                    return true;
                case Resource.Id.navigation_notifications:
                    if (codclifor == null)
                    {
                        Toast.MakeText(this, "Selezionare un cliente prima!", ToastLength.Short).Show();
                        break;
                    }
                    var layout = new LinearLayout(this);
                    layout.Orientation = Orientation.Vertical;
                    var edittextAgente = new EditText(this);
                    edittextAgente.Hint = "Codice agente";
                    layout.AddView(edittextAgente, 0);
                    var builder1 = new AlertDialog.Builder(this);
                    builder1.SetTitle("Conferma ordine");
                    builder1.SetMessage("Totale Ordine:" + Helper.GetTot(listprod));
                    builder1.SetCancelable(true);
                    builder1.SetView(layout);
                    builder1.SetNegativeButton("Annulla", delegate { });
                    builder1.SetPositiveButton("Conferma", delegate
                    {

                        if (!edittextAgente.Text.StartsWith("C") && edittextAgente.Text.Length != 6)
                        {
                            Toast.MakeText(this, "Codice agente non valido", ToastLength.Short).Show();
                            return;

                        }
                        var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                            .DirectoryDownloads).AbsolutePath;
                        using (StreamWriter streamWriter = new StreamWriter(path + "/Ordine.csv"))
                        {
                            foreach (var prod in listprod)
                            {
                                streamWriter.WriteLine(prod);
                            }

                            streamWriter.WriteLine($";;;{Helper.GetTot(listprod)};{edittextAgente.Text};{codclifor}");
                        }
                        Toast.MakeText(this, "Ordine effetuato e salvato nella cartella /Downloads.", ToastLength.Short).Show();
                        listprod = new List<string>();
                        listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);
                    });
                    builder1.Show();
                    return true;

            }

            return false;
        }

        protected override void OnDestroy()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt");
            }
            base.OnDestroy();
        }

        private void Lw_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent i = new Intent(this, typeof(ActivityAdd));
            i.PutExtra("gruppo", ((ListView)sender).GetItemAtPosition(e.Position).ToString());
            i.PutExtra("prod", listprod.ToArray());
            i.PutExtra("uri", listURI.ToArray());
            StartActivity(i);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/catalogo.csv"))
            {
                Helper.GetData();
            }
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            listView = FindViewById<ListView>(Resource.Id.listViewMainProd);


            const string permission = Manifest.Permission.ReadExternalStorage;
            if (CheckSelfPermission(permission) != (int)Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, 5);
            }

            var prodArray = Intent.GetStringArrayExtra("prod");
            var uriArray = Intent.GetStringArrayExtra("uri");

            try
            {
                listURI = uriArray.ToList();
                listprod = prodArray.ToList();
                var templist = new List<Prodotto>();
                var finalList = listprod.Zip(listURI, (p, u) =>new 
                {
                    prodotto=p,
                    uri=u
                }).ToList();
                foreach (var prod in finalList)
                {
                    var ptemp=new Prodotto();
                    ptemp.ImageUrl= prod.uri;
                    var split = prod.prodotto.Split(';');
                    ptemp.Name = query.First(p => p[4] == split[0])[5];
                    ptemp.QuantityPrice = split[1] + '/' + split[2];
                    templist.Add(ptemp);

                }
                listView.Adapter = new ProdottoAdapter(templist);

            }
            catch (Exception)
            {
                listURI=new List<string>();
                listprod = new List<string>();
            }
            

        }


        private string codclifor;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuMain, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void SetCodCliFor()
        {
            var layoutprinc = new LinearLayout(this);
            layoutprinc.Orientation = Orientation.Vertical;
            var separator = new View(this);
            separator.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1);
            separator.SetBackgroundResource(Android.Resource.Color.DarkerGray);
            var layoutRadio = new RadioGroup(this);
            layoutRadio.Orientation = Orientation.Horizontal;
            var radioDesc = new RadioButton(this)
            {
                Text = "Descrizione",

            };
            var radioIva = new RadioButton(this)
            {
                Text = "Partita iva"
            };
            var radioCod = new RadioButton(this)
            {
                Text = "Codice"
            };
            layoutRadio.AddView(radioDesc);
            layoutRadio.AddView(radioIva);
            layoutRadio.AddView(radioCod);

            layoutprinc.AddView(layoutRadio);
            layoutprinc.AddView(separator);
            var searchText = new AutoCompleteTextView(this);
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                .DirectoryDownloads).AbsolutePath;
            var clienti = Helper.GetClienti(path);
            var descliforlist = new List<string>();
            var partitaivalist = new List<string>();
            var codcliforlist = new List<string>();
            foreach (var cliente in clienti)
            {
                codcliforlist.Add(cliente[7]);
                var parttemp = cliente[15];
                if (parttemp.Length != 0)
                {
                    parttemp = parttemp.Substring(1);
                }
                partitaivalist.Add(parttemp);


                descliforlist.Add(cliente[12]);
            }
            descliforlist.RemoveAt(0);
            searchText.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, descliforlist);

            layoutRadio.CheckedChange += (s, e) =>
            {
                switch (e.CheckedId)
                {
                    case 1:
                        searchText.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                            descliforlist);
                        break;
                    case 2:
                        searchText.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                            partitaivalist);
                        break;
                    case 3:
                        searchText.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                            codcliforlist);
                        break;
                }
            };
            layoutprinc.AddView(searchText);
            var builder1 = new AlertDialog.Builder(this);

            builder1.SetTitle("Seleziona cliente");
            builder1.SetCancelable(false);
            builder1.SetView(layoutprinc);
            builder1.SetPositiveButton("Conferma", delegate
            {
                switch (layoutRadio.CheckedRadioButtonId)
                {
                    case 1:
                        codclifor = clienti.First(list => list[12] == searchText.Text)[7]; break;
                    case 2:
                        codclifor = clienti.First(list => list[15] == "0" + searchText.Text)[7]; break;
                    case 3:
                        codclifor = searchText.Text; break;

                }

                using (StreamWriter stream = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
                {
                    stream.Write(codclifor);
                }

                SupportActionBar.Subtitle = "Cliente:" + clienti.First(list => list[7] == codclifor)[12];
            });
            layoutRadio.Check(1);
            builder1.Show();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            switch (id)
            {
                case Resource.Id.Aggiorna_Il_Database:
                    Helper.GetData(true);
                    break;
                case Resource.Id.Cliente:
                    SetCodCliFor();
                    break;


            }
            return base.OnOptionsItemSelected(item);
        }

    }
}

