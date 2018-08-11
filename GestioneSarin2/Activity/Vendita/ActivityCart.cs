using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using GestioneSarin2.Activity;
using GestioneSarin2.Other_class_and_Helper;
using Java.IO;
using Java.Nio.Channels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Console = System.Console;
using Environment = System.Environment;
using File = System.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace GestioneSarin2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppThemeNo", MainLauncher = false, ParentActivity = typeof(ActivityHome))]
    public class ActivityCart : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private List<string> listprod;
        private List<List<string>> query = Helper.table;
        private ListView listView;
        private List<string> listURI;
        private int docType;
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
                    //  var syncConnPref = sharedPref.GetBoolean(ActivitySettings.KeyAutoDelete,false);

                    var radiobuttonCateg = new RadioButton(this)
                    {
                        Text = "Categorie"
                    };
                    var radiobuttonCatal = new RadioButton(this)
                    {
                        Text = "Catalogo"
                    };
                    radiogroup.Orientation = Orientation.Horizontal;
                    radiogroup.AddView(radiobuttonAll);
                    radiogroup.AddView(radiobuttonCateg);
                    radiogroup.AddView(radiobuttonCatal);

                    var lw = new ListView(this);
                    var prodListAll = new List<string>();
                    var textSearch = new EditText(this);
                    void OnRadiogroupOnCheckedChange(object sender, RadioGroup.CheckedChangeEventArgs args)
                    {
                        lw.ItemClick -= Lw_ItemClickAll;
                        lw.ItemClick -= Lw_ItemClickCat;
                        var id = args.CheckedId;
                        if (id == radiobuttonCateg.Id)
                        {
                            var group = Helper.GetGroup(this);
                            var grouptemp = group[2].Where(g => g.Split(';')[0].Length == 3).ToList();
                            var groupfianal=new List<string>();
                            foreach (var groupr in grouptemp)
                            {
                                groupfianal.Add(groupr.Split(';')[1]);
                            }

                            lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, groupfianal);
                            lw.ItemClick -= Lw_ItemClickAll;
                            lw.ItemClick -= Lw_ItemClickCatalogo;

                            lw.ItemClick += Lw_ItemClickCat;
                            textSearch.Enabled = false;

                        }
                        else if (id == radiobuttonAll.Id)
                        {
                            var prodList = new List<string>();
                            for (var i = 1; i < Helper.table.Count; i++)
                            {
                                var prod = Helper.table[i];
                                prodList.Add(prod[1]);
                            }

                            prodListAll = prodList;
                            lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, prodList);
                            lw.ItemClick -= Lw_ItemClickCat;
                            lw.ItemClick -= Lw_ItemClickCatalogo;

                            lw.ItemClick += Lw_ItemClickAll;
                            textSearch.Enabled = true;

                        }
                        else if (id == radiobuttonCatal.Id)
                        {
                            lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, new List<string> { "Apri catalogo" });
                            lw.ItemClick -= Lw_ItemClickCat;
                            lw.ItemClick -= Lw_ItemClickAll;
                            lw.ItemClick += Lw_ItemClickCatalogo;
                            textSearch.Enabled = false;
                        }

                    }
                    lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Helper.GetGroup(this)[1]);

                    textSearch.TextChanged += (object sender, TextChangedEventArgs args) =>
                    {
                        var newList = prodListAll.Where(p => p.Contains(textSearch.Text.ToUpper())).ToList();
                        lw.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, newList);
                    };
                    radiogroup.CheckedChange += OnRadiogroupOnCheckedChange;
                    radiogroup.Check(radiobuttonCateg.Id);
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
                    break;
                case Resource.Id.navigation_notifications:
                    if (codclifor == null)
                    {
                        Toast.MakeText(this, "Selezionare un cliente prima!", ToastLength.Short).Show();
                        break;
                    }

                    var builder1 = new AlertDialog.Builder(this);
                    var editSconto = new EditText(this) { Hint = "Sconto" };
                    var editNote = new EditText(this) { Hint = "Note" };
                    var editAcc = new EditText(this) { Hint = "Acconto" };
                    var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };
                    layout.AddView(editSconto);
                    layout.AddView(editNote);
                    layout.AddView(editAcc);
                    builder1.SetTitle("Conferma ordine");
                    builder1.SetView(layout);
                    var totNoIva = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                    var tot = Convert.ToDecimal(totNoIva) + Convert.ToDecimal(totNoIva) / 100 * 22;
                    tot = Math.Round(tot, 2);
                    builder1.SetMessage("Totale Ordine:" + tot);
                    builder1.SetCancelable(true);
                    builder1.SetNegativeButton("Annulla", delegate { });
                    builder1.SetPositiveButton("Conferma", delegate
                    {


                        var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var last = 0;
                        try
                        {
                            using (StreamReader sr = new StreamReader(path + "/docTes.csv"))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var line = sr.ReadLine();
                                    last = Convert.ToInt32(line?.Split(';')[0]) + 1;
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            last = 0;
                        }
                        using (StreamWriter streamWriter = new StreamWriter(path + "/docRig.txt", true))
                        {
                            for (var index = 0; index < listprod.Count; index++)
                            {
                                var prod = listprod[index];
                                var prodSplit = prod.Split(';');
                                var prodFirst = Helper.table.First(p => p[0] == prodSplit[0].ToUpper()).ToList();
                                var codPRd = prodFirst[0];

                                var prodFin = codPRd;
                                for (var i = 1; i < prodSplit.Length - 1; i++)
                                {
                                    prodFin += ";" + prodSplit[i];
                                }
                                //  var rig = index + ";" + last + ";" + prodFin;
                                var pUni = prodSplit[4];
                                var rig = $"{last};{index + 1};{docType};{DateTime.Now.ToShortDateString()};{index + 1};{prodSplit[0]};{prodSplit[2]};{prodSplit[1]};{prodSplit[4]};{pUni}";
                                streamWriter.WriteLine(rig);
                            }
                        }



                        using (StreamWriter streamWriter = new StreamWriter(path + "/docTes.txt", true))
                        {
                            var totNoIva2 = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                            var totIva = Convert.ToDecimal(totNoIva2) + Convert.ToDecimal(totNoIva2) / 100 * 22;
                            //aggiungere tipi
                            var zero = nProgPhoto;
                            var ns = "";
                            while (zero > 0)
                            {
                                ns += zero + "-";
                                zero--;
                            }
                            var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                            var codAge = sharedPref.GetString(ActivitySettings.KeyCodAge, "");
                            switch (docType)
                            {
                                case (int)DocType.Vendita:
                                    streamWriter.Write(
                                        $"{last};ORDCL;{last};{DateTime.Now.ToShortDateString()};{codclifor + codDest};{codAge};{editSconto.Text};{editNote.Text};{editAcc.Text}"); //todo sconti testa note testa acconto
                                    break;
                                case (int)DocType.Rapportino:
                                    streamWriter.Write(
                                        $"{last};RAPLA;{last};{DateTime.Now.ToShortDateString()};{codclifor + codDest};{codAge};{editSconto.Text};{editNote.Text};{editAcc.Text}"); //todo sconti testa note testa acconto
                                    break;
                            }
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

        private void Lw_ItemClickCatalogo(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent i5 = new Intent(this, typeof(ActivityGallery));
            i5.PutExtra("prod", listprod.ToArray());
            i5.PutExtra("uri", listURI.ToArray());
            i5.PutExtra("Type", docType);
            i5.PutExtra("nprog", nProgPhoto);
            StartActivity(i5);
        }

        private AlertDialog alertall;
        private void Lw_ItemClickAll(object sender, AdapterView.ItemClickEventArgs e)
        {
            var itemName = ((ListView)sender).GetItemAtPosition(e.Position).ToString();
            var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };
            var textQta = new EditText(this) { Hint = "Quantità" };
            var textPPart = new EditText(this) { Hint = "Prezzo particolare" };
            var textScon = new EditText(this) { Hint = "Sconto" };
            var textNote = new EditText(this) { Hint = "Note aggiuntive" };
            layout.AddView(textQta);
            layout.AddView(textPPart);
            layout.AddView(textScon);
            layout.AddView(textNote);

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Seleziona la quantita");
            builder.SetCancelable(true);
            builder.SetView(layout);
            builder.SetNegativeButton("Annulla", delegate { });
            builder.SetPositiveButton("Conferma",
                delegate
                {
                    var psel = Helper.table.First(p => p[1] == itemName);
                    listprod.Add($"{psel[0]};{textQta.Text.Replace(',', '.')};{psel[4]};{textPPart.Text};{textScon.Text};{textNote.Text}");
                    var urisplit = psel[15].Split('\\');
                    listURI.Add("\\");



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
                        var ptemp = new Prodotto { ImageUrl = prod.uri };
                        var split = prod.prodotto.Split(';');//todo crash in differentmode of adding (seems fixed now keep eyes on it)

                        var pqueryed = query.First(p => p[0] == split[0].ToUpper());
                        var namet = pqueryed[1];
                        namet = textInfo.ToLower(namet);
                        ptemp.Name = textInfo.ToTitleCase(namet);
                        ptemp.CodArt = pqueryed[4];
                        try
                        {
                            if (Convert.ToDecimal(split[3]) != 0)
                            {
                                split[2] = split[3];
                                split[3] = "";
                            }
                        }
                        catch
                        {
                           
                        }
                        ptemp.QuantityPrice = split[1] + '/' + split[2];
                        ptemp.Note = split[5];
                        if (split[4] == "")
                        {
                            split[4] = "0";
                        }
                        ptemp.Sconto = split[4];
                        ptemp.IVA = pqueryed[13];
                        templist.Add(ptemp);

                    }
                    listView.Adapter = new ProdottoAdapter(templist);

                    alertall.Dismiss();
                    var totNoIva = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                    var tot = Convert.ToDecimal(totNoIva) + Convert.ToDecimal(totNoIva) / 100 * 22;
                    tot = Math.Round(tot, 2);
                    toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = $"Tot:{totNoIva}+IVA={tot}";
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
            i.PutExtra("Type", docType);
            i.PutExtra("ndoc", Intent.GetIntExtra("ndoc", 0));
            i.PutExtra("mod", i.GetBooleanExtra("mod", false));
            i.PutExtra("nprog", nProgPhoto);
            StartActivity(i);
        }

        private Toolbar toolbar;
        private string codDest;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layoutCart);
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin";
            if (!File.Exists(path + "/catalogo.csv"))
            {
                Helper.GetArticoli(this);
            }
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            listView = FindViewById<ListView>(Resource.Id.listViewMainProd);
            listView.ItemClick += ListView_ItemClick;
            listView.ItemLongClick += ListView_ItemLongClick;
            toolbar = FindViewById<Toolbar>(Resource.Id.my_toolbarMain);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            toolbar.FindViewById<TextView>(Resource.Id.toolbar_title)
                .SetTextColor(Android.Graphics.Color.ParseColor("#f2efe8"));
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
                    var line = stream.ReadLine()?.Split('/');
                    codclifor = line?[0];
                    codDest = line?[1];
                }
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
                    var pqueryed = query.First(p => p[0] == split[0].ToUpper());
                    var namet = pqueryed[1];
                    ptemp.CodArt = pqueryed[0];
                    namet = textInfo.ToLower(namet);
                    ptemp.Name = textInfo.ToTitleCase(namet);
                    try
                    {
                        if (Convert.ToDecimal(split[3]) != 0)
                        {
                            split[2] = split[3];
                            split[3] = "";
                        }
                    }
                    catch 
                    {
                        
                    }
                    ptemp.QuantityPrice = split[1] + '/' + split[2];
                    ptemp.Note = split[5];
                    ptemp.Sconto = split[4];

                    templist.Add(ptemp);

                }
                listView.Adapter = new ProdottoAdapter(templist);
                var totNoIva = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                var tot = Convert.ToDecimal(totNoIva) + Convert.ToDecimal(totNoIva) / 100 * 22;
                tot = Math.Round(tot, 2);
                toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = $"Tot:{totNoIva}+IVA={tot}";

            }
            catch (Exception e)
            {
                listURI = new List<string>();
                listprod = new List<string>();
            }
            IsPlayServicesAvailable();
            FirebaseMessaging.Instance.SubscribeToTopic("all");
            docType = Intent.GetIntExtra("Type", 0);
            nProgPhoto = Intent.GetIntExtra("nprog", 0);

        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var prod = listprod[e.Position];
            var prodSplit = prod.Split(';');
            var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };
            var editTexQ = new EditText(this) { Text = prodSplit[1], Hint = "Quantità" };
            var editTexS = new EditText(this) { Text = prodSplit[4], Hint = "Sconto" };
            var editTextP = new EditText(this) { Text = prodSplit[3], Hint = "Prezzo particolare" };
            var editTexN = new EditText(this) { Text = prodSplit[5], Hint = "Note" };
            layout.AddView(editTexQ);
            layout.AddView(editTexS);
            layout.AddView(editTextP);
            layout.AddView(editTexN);
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Modifica informazioni riga");
            builder.SetView(layout);
            builder.SetCancelable(true);
            builder.SetNegativeButton("Annulla", delegate { });
            builder.SetPositiveButton("Conferma", delegate
            {
                prodSplit[1] = editTexQ.Text;
                prodSplit[4] = editTexS.Text;
                prodSplit[5] = editTexN.Text;
                prodSplit[3] = editTextP.Text;
                var pTemp = prodSplit.Aggregate((current, next) => current + ";" + next);
                listprod[e.Position] = pTemp;
                var templist = new List<Prodotto>();
                var finalList = listprod.Zip(listURI, (p, u) => new
                {
                    prodotto = p,
                    uri = u
                }).ToList();
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                foreach (var prodAda in finalList)
                {
                    var ptemp = new Prodotto { ImageUrl = prodAda.uri };
                    var split = prodAda.prodotto.Split(';');
                    var pqueryed = query.First(p => p[5] == split[0]);
                    var namet = pqueryed[5];
                    namet = textInfo.ToLower(namet);
                    ptemp.Name = textInfo.ToTitleCase(namet);
                    ptemp.QuantityPrice = split[1] + '/' + split[2];
                    ptemp.CodArt = pqueryed[4];
                    templist.Add(ptemp);

                }
                listView.Adapter = new ProdottoAdapter(templist);
                var totNoIva2 = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                var tot2 = Convert.ToDecimal(totNoIva2) + Convert.ToDecimal(totNoIva2) / 100 * 22;
                tot2 = Math.Round(tot2, 2);
                toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = $"Tot:{totNoIva2}+IVA={tot2}";

            });
            builder.Show();
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(ActivityHome));
            base.OnBackPressed();
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
                //todo delete
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
                    var pqueryed = query.First(p => p[0] == split[0]);
                    var namet = pqueryed[1];
                    namet = textInfo.ToLower(namet);
                    ptemp.Name = textInfo.ToTitleCase(namet);
                    ptemp.CodArt = pqueryed[0];
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
                    Helper.GetArticoli(this, true);
                    break;
                case Resource.Id.Cliente:
                    SetCodCliFor();
                    break;
                case Resource.Id.savePres:

                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Vuoi salvare quest'ordine?");
                    builder.SetCancelable(true);
                    builder.SetNegativeButton("No", delegate { });
                    builder.SetPositiveButton("Si", delegate
                    {


                        var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                            .DirectoryDownloads).AbsolutePath + "/Sarin";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }


                        using (StreamWriter streamWriter = new StreamWriter(path + "/presets.csv", true))
                        {
                            foreach (var prod in listprod)
                            {
                                streamWriter.WriteLine(prod);
                            }
                            var totNoIva = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                            var tot = Convert.ToDecimal(totNoIva) + Convert.ToDecimal(totNoIva) / 100 * 22;
                            tot = Math.Round(tot, 2);
                            //aggiungere tipi
                            var sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                            var codAge = sharedPref.GetString(ActivitySettings.KeyCodAge, "");
                            switch (docType)
                            {
                                case (int)DocType.Vendita:
                                    streamWriter.WriteLine($";;;{Helper.GetTot(listprod)};22;{tot};{codAge};{codclifor};{DateTime.Now.ToShortDateString()};{codDest};ORDCL");
                                    streamWriter.Write('#');
                                    break;
                                case (int)DocType.Rapportino:
                                    streamWriter.WriteLine($";;;{Helper.GetTot(listprod)};22;{tot};{codAge};{codclifor};{DateTime.Now.ToShortDateString()};{codDest};RAPLA");
                                    streamWriter.Write('#');
                                    break;
                                default:
                                    streamWriter.WriteLine($";;;{Helper.GetTot(listprod)};22;{tot};{codAge};{codclifor};{DateTime.Now.ToShortDateString()};{codDest};ORDCL");
                                    streamWriter.Write('#');
                                    break;
                            }

                        }
                        Toast.MakeText(this, "Ordine salvato.", ToastLength.Short).Show();
                        listprod = new List<string>();
                        listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);
                    });
                    builder.Show();
                    break;
                case Resource.Id.addPhoto:
                    Intent getIntent = new Intent(Intent.ActionGetContent);
                    getIntent.SetType("image/*");

                    Intent pickIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                    pickIntent.SetType("image/*");

                    Intent chooserIntent = Intent.CreateChooser(getIntent, "Select Image");
                    chooserIntent.PutExtra(Intent.ExtraInitialIntents, new IParcelable[] { pickIntent });

                    StartActivityForResult(chooserIntent, PICK_IMAGE);

                    break;


            }
            return base.OnOptionsItemSelected(item);
        }
        public static readonly int PICK_IMAGE = 1;
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode != PICK_IMAGE || resultCode != Result.Ok) return;
            var d = data.Data;
            var picturePath = GetPath(this, d);
        }

        private int nProgPhoto;
        public string GetPath(Context context, Android.Net.Uri uri)
        {
            var result = "";
            var proj = new[] { MediaStore.Images.Media.InterfaceConsts.Data };
            var cursor = context.ContentResolver.Query(uri, proj, null, null, null);
            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    var columnIndex = cursor.GetColumnIndexOrThrow(proj[0]);
                    result = cursor.GetString(columnIndex);
                }
                cursor.Close();
            }
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin/photoa";
            nProgPhoto++;

            var img = new Java.IO.File(result);
            var newImg = new Java.IO.File(path + $"/{DateTime.Now:yyyy-mm-dd}-codag-{codclifor}-{nProgPhoto}.jpg");
            FileChannel inputChannel = null;
            FileChannel outputChannel = null;
            try
            {
                inputChannel = new FileInputStream(img).Channel;
                outputChannel = new FileOutputStream(newImg).Channel;
                outputChannel.TransferFrom(inputChannel, 0, inputChannel.Size());
            }
            finally
            {
                inputChannel?.Close();
                outputChannel?.Close();
                inputChannel?.Dispose();
                outputChannel?.Dispose();
            }
            return result ?? "Not found";
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

