using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Android.Preferences;
using Android.Support.Design.Internal;
using Android.Text;
using GestioneSarin2.Activity;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Environment = System.Environment;

namespace GestioneSarin2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppThemeNo", MainLauncher = false, ParentActivity = typeof(ActivityHome))]
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
                    var main = new LinearLayout(this)
                    {
                        Orientation = Orientation.Vertical
                    };
                    var radiogroup = new RadioGroup(this);
                    var radiobuttonAll = new RadioButton(this)
                    {
                        Text = "Tutti"
                    };
                    //todo setting code var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    //  var syncConnPref = sharedPref.GetBoolean(ActivitySettings.KeyAutoDelete,false);

                    var radiobuttonCat = new RadioButton(this)
                    {
                        Text = "Categorie"
                    };
                    radiogroup.Orientation = Orientation.Horizontal;
                    radiogroup.AddView(radiobuttonAll);
                    radiogroup.AddView(radiobuttonCat);
                    var lw = new ListView(this);
                    var prodListAll = new List<string>();
                    var textSearch = new EditText(this);
                    void OnRadiogroupOnCheckedChange(object sender, RadioGroup.CheckedChangeEventArgs args)
                    {
                        lw.ItemClick -= Lw_ItemClickAll;
                        lw.ItemClick -= Lw_ItemClickCat;
                        var id = args.CheckedId;
                        if (id == radiobuttonCat.Id)
                        {
                            lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Helper.GetGroup());
                            lw.ItemClick -= Lw_ItemClickAll;
                            lw.ItemClick += Lw_ItemClickCat;
                            textSearch.Enabled = false;

                        }
                        else if (id == radiobuttonAll.Id)
                        {
                            var prodList = new List<string>();
                            for (var i = 1; i < Helper.table.Count; i++)
                            {
                                var prod = Helper.table[i];
                                prodList.Add(prod[5]);
                            }

                            prodListAll = prodList;
                            lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, prodList);
                            lw.ItemClick -= Lw_ItemClickCat;
                            lw.ItemClick += Lw_ItemClickAll;
                            textSearch.Enabled = true;

                        }
                    }
                    lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Helper.GetGroup());

                    textSearch.TextChanged += (sender, args) =>
                    {
                        var newList = prodListAll.Where(p => p.Contains(textSearch.Text.ToUpper())).ToList();
                        lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, newList);
                    };
                    radiogroup.CheckedChange += OnRadiogroupOnCheckedChange;
                    radiogroup.Check(radiobuttonCat.Id);
                    main.AddView(radiogroup);
                    main.AddView(textSearch);
                    main.AddView(lw);
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Seleziona gruppo");
                    builder.SetCancelable(true);
                    builder.SetView(main);
                    builder.SetNegativeButton("Annulla", delegate { });
                    alertall = builder.Create();
                    alertall.Show();
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
                            .DirectoryDownloads).AbsolutePath + "/Sarin";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var curDir = new Java.IO.File(path);
                        var csvlist = curDir.List();
                        var last = 0;
                        if (csvlist != null)
                        {
                            var tempcsvList = csvlist.ToList();
                            tempcsvList.Remove("presets.csv");
                            Array.Copy(tempcsvList.ToArray(), csvlist, tempcsvList.Count);
                            Array.Resize(ref csvlist, tempcsvList.Count);
                            foreach (var ord in csvlist)
                            {
                                var narr = new string(ord.Where(char.IsDigit).ToArray());
                                var n = narr.Aggregate("", (current, digit) => current + digit);

                                if (Convert.ToInt32(n) > last)
                                {
                                    last = Convert.ToInt32(n);
                                }
                            }
                        }

                        using (StreamWriter streamWriter = new StreamWriter(path + $"/OrdineN{last + 1}.csv"))
                        {
                            foreach (var prod in listprod)
                            {
                                streamWriter.WriteLine(prod);
                            }

                            streamWriter.WriteLine($";;;{Helper.GetTot(listprod)};{edittextAgente.Text};{codclifor};{DateTime.Now.ToShortDateString()}");
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

        private AlertDialog alertall;
        private void Lw_ItemClickAll(object sender, AdapterView.ItemClickEventArgs e)
        {
            var itemName = ((ListView)sender).GetItemAtPosition(e.Position).ToString();

            //listProd.Add(subqueryList[e.Position].CodArt + ';' + text.Text.Replace(',','.') + ';' + subqueryList[e.Position].UnitPrice);
            var text = new EditText(this);

            text.SetRawInputType(InputTypes.ClassNumber);
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Seleziona la quantita");
            builder.SetCancelable(true);
            builder.SetView(text);
            builder.SetNegativeButton("Annulla", delegate { });
            builder.SetPositiveButton("Conferma",
                delegate
                {
                    var psel = Helper.table.First(p => p[5] == itemName);
                    listprod.Add(psel[4] + ";" + text.Text.Replace(',', '.') + ";" + psel[12]);
                    var urisplit = psel[15].Split('\\');
                    listURI.Add(urisplit.Last());



                    var templist = new List<Prodotto>();
                    var finalList = listprod.Zip(listURI, (p, u) => new
                    {
                        prodotto = p,
                        uri = u
                    }).ToList();
                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;
                    query = Helper.table;
                    foreach (var prod in finalList)
                    {
                        var ptemp = new Prodotto();
                        ptemp.ImageUrl = prod.uri;
                        var split = prod.prodotto.Split(';');
                        var namet = query.First(p => p[4] == split[0])[5];
                        namet = textInfo.ToLower(namet);
                        ptemp.Name = textInfo.ToTitleCase(namet);
                        ptemp.QuantityPrice = split[1] + '/' + split[2];
                        templist.Add(ptemp);

                    }
                    listView.Adapter = new ProdottoAdapter(templist);

                    alertall.Dismiss();
                });
            builder.Show();


        }



        protected override void OnDestroy()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt");
            }
            base.OnDestroy();
        }

        private void Lw_ItemClickCat(object sender, AdapterView.ItemClickEventArgs e)
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
            listView.ItemLongClick += ListView_ItemLongClick;
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.my_toolbarMain);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            const string permission = Manifest.Permission.ReadExternalStorage;
            if (CheckSelfPermission(permission) != (int)Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, 5);
            }

            var prodArray = Intent.GetStringArrayExtra("prod");
            var uriArray = Intent.GetStringArrayExtra("uri");

            if (Intent.GetBooleanExtra("first", true))
            {
                SetCodCliFor();
            }
            else
            {
                using (StreamReader stream = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
                {
                    codclifor = stream.ReadLine();
                }
                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                    .DirectoryDownloads).AbsolutePath + "";
                var clienti = Helper.GetClienti(path);
                SupportActionBar.Subtitle = "Cliente: " + clienti.First(list => list[7] == codclifor)[12];

            }
            try
            {
                listURI = uriArray.ToList();
                listprod = prodArray.ToList();
                var templist = new List<Prodotto>();
                var finalList = listprod.Zip(listURI, (p, u) => new
                {
                    prodotto = p,
                    uri = u
                }).ToList();
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                foreach (var prod in finalList)
                {
                    var ptemp = new Prodotto();
                    ptemp.ImageUrl = prod.uri;
                    var split = prod.prodotto.Split(';');
                    var namet = query.First(p => p[4] == split[0])[5];
                    namet = textInfo.ToLower(namet);
                    ptemp.Name = textInfo.ToTitleCase(namet);
                    ptemp.QuantityPrice = split[1] + '/' + split[2];
                    templist.Add(ptemp);

                }
                listView.Adapter = new ProdottoAdapter(templist);

            }
            catch (Exception)
            {
                listURI = new List<string>();
                listprod = new List<string>();
            }
            IsPlayServicesAvailable();
            FirebaseMessaging.Instance.SubscribeToTopic("all");


        }

        private void ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Sicuro di voler eliminare il prodotto?");
            builder.SetCancelable(true);
            builder.SetNegativeButton("Annulla", delegate { });
            builder.SetPositiveButton("Conferma", delegate
            {
                listprod.RemoveAt(e.Position);
                listURI.RemoveAt(e.Position);
                if (listprod.Count <= 0)
                {
                    listView.Adapter = null;
                    return;
                }

                var templist = new List<Prodotto>();
                var finalList = listprod.Zip(listURI, (p, u) => new
                {
                    prodotto = p,
                    uri = u
                }).ToList();
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                foreach (var prod in finalList)
                {
                    var ptemp = new Prodotto();
                    ptemp.ImageUrl = prod.uri;
                    var split = prod.prodotto.Split(';');
                    var namet = query.First(p => p[4] == split[0])[5];
                    namet = textInfo.ToLower(namet);
                    ptemp.Name = textInfo.ToTitleCase(namet);
                    ptemp.QuantityPrice = split[1] + '/' + split[2];
                    templist.Add(ptemp);

                }
                listView.Adapter = new ProdottoAdapter(templist);
            });
            builder.Show();
        }

        private string codclifor;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuMain, menu);
            MenuInflater.Inflate(Resource.Menu.actionbarMain, menu);
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
            layoutRadio.Check(radioDesc.Id);
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

                SupportActionBar.Subtitle = "Cliente: " + clienti.First(list => list[7] == codclifor)[12];
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
                case Resource.Id.savePres:
                    var edittextAgente = new EditText(this);
                    edittextAgente.Hint = "Codice agente";
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Vuoi saalvare quest'ordine?");
                    builder.SetCancelable(true);
                    builder.SetView(edittextAgente);
                    builder.SetNegativeButton("No", delegate { });
                    builder.SetPositiveButton("Si", delegate
                    {

                        if (!edittextAgente.Text.StartsWith("C") && edittextAgente.Text.Length != 6)
                        {
                            Toast.MakeText(this, "Codice agente non valido", ToastLength.Short).Show();
                            return;
                        }
                        var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                            .DirectoryDownloads).AbsolutePath + "/Sarin";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                     

                        using (StreamWriter streamWriter = new StreamWriter(path + "/presets.csv",true))
                        {
                            foreach (var prod in listprod)
                            {
                                streamWriter.WriteLine(prod);
                            }

                            streamWriter.WriteLine($";;;{Helper.GetTot(listprod)};{edittextAgente.Text};{codclifor};{DateTime.Now.ToShortDateString()}");
                            streamWriter.Write('#');
                        }
                        Toast.MakeText(this, "Ordine salvato.", ToastLength.Short).Show();
                        listprod = new List<string>();
                        listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);
                    });
                    builder.Show();
                    break;


            }
            return base.OnOptionsItemSelected(item);
        }
        public bool IsPlayServicesAvailable()
        {
            string x;
            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    x = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    x = "This device is not supported";
                    Finish();
                }
                return false;
            }
            x = "Google Play Services is available.";
            return true;
        }

    }
}

