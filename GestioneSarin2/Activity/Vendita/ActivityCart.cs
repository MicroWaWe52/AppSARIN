using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Common;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using GestioneSarin2.Activity;
using GestioneSarin2.Adapter_and_Single_class;
using GestioneSarin2.Other_class_and_Helper;
using Java.IO;
using Java.Nio.Channels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Android.Graphics.Drawables;
using Android.Util;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Environment = System.Environment;
using File = System.IO.File;
using FileNotFoundException = Java.IO.FileNotFoundException;
using Orientation = Android.Widget.Orientation;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace GestioneSarin2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppThemeNo", MainLauncher = false, ParentActivity = typeof(ActivityHome))]
    public class ActivityCart : AppCompatActivity
    {
        private List<string> listprod;
        private List<List<string>> query = Helper.GetArticoli(Application.Context);
        private ExpandableListView listView;
        private List<string> listURI;
        private int docType;
        private TextView totNoIvaTextView;
        private TextView totIvaTextView;
        readonly List<string> group = new List<string>();
        readonly Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layoutCart);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin";

            if (!File.Exists(path + "/catalogo.csv"))
            {
                Helper.GetArticoli(this);
            }

            deleteButton = FindViewById<RadioButton>(Resource.Id.ButtonElimina);
            addButton = FindViewById<RadioButton>(Resource.Id.ButtonAggiungi);
            orderButton = FindViewById<RadioButton>(Resource.Id.ButtonOrdina);
            totIvaTextView = FindViewById<TextView>(Resource.Id.totIvaText);
            totNoIvaTextView = FindViewById<TextView>(Resource.Id.totNoIvaText);
            AssetManager am = Assets;
            Typeface tvDoc = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");
            totIvaTextView.SetTypeface(tvDoc, TypefaceStyle.Normal);
            totNoIvaTextView.SetTypeface(tvDoc, TypefaceStyle.Normal);
            deleteButton.Click += DeleteButton_Click;
            addButton.Click += AddButton_Click;
            orderButton.Click += OrderButton_Click;
            deleteButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            addButton.SetTypeface(tvDoc, TypefaceStyle.Normal);
            orderButton.SetTypeface(tvDoc, TypefaceStyle.Normal);

            listView = FindViewById<ExpandableListView>(Resource.Id.listViewMainProd);
            // listView.ItemClick += ListView_ItemClick;
            // listView.ItemLongClick += ListView_ItemLongClick;
            toolbar = FindViewById<Toolbar>(Resource.Id.my_toolbarMain);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            toolbar.FindViewById<TextView>(Resource.Id.toolbar_title)
                .SetTextColor(Android.Graphics.Color.ParseColor("#f2efe8"));
            var prodArray = Intent.GetStringArrayExtra("prod");
            var uriArray = Intent.GetStringArrayExtra("uri");

            using (StreamReader stream = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/codclifor.txt"))
            {
                var line = stream.ReadLine()?.Split('/');
                codclifor = line?[0];
                codDest = line?[1];
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
                Setdata();

                var totNoIva = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                var tot = Convert.ToDecimal(totNoIva) + Convert.ToDecimal(totNoIva) / 100 * 22;
                tot = Math.Round(tot, 2);
                totNoIvaTextView.Text = totNoIva + '€';
                totIvaTextView.Text = tot.ToString(CultureInfo.CurrentCulture) + '€';

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
                    var qta = "0.00";
                    if (textQta.Text != "")
                    {
                        qta = textQta.Text.Replace(',', '.');
                    }
                    var psel = Helper.Table.First(p => p[1] == itemName);
                    listprod.Add($"{psel[0]};{qta};{psel[4]};{textPPart.Text};{textScon.Text};{textNote.Text}");
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
                    query = Helper.Table;
                    foreach (var prod in finalList)
                    {
                        var gap = 1;
                        try
                        {
                            gap = Convert.ToInt32(Helper.GetClienti(this).First(cli => cli[0] == codclifor + codDest)[12]);

                        }
                        catch
                        {
                            // ignored
                        }

                        var ptemp = new Prodotto { ImageUrl = prod.uri };
                        var split = prod.prodotto.Split(';');

                        var pqueryed = query.First(p => p[0] == split[0].ToUpper());
                        var namet = pqueryed[1];
                        namet = textInfo.ToLower(namet);
                        ptemp.Name = textInfo.ToTitleCase(namet);
                        ptemp.CodArt = pqueryed[0];
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
                        listprod.RemoveAt(listprod.Count - 1);
                        listprod.Add($"{psel[0]};{qta};{split[2]};{textPPart.Text};{split[4]};{textNote.Text}");
                        var urisplit2 = psel[15].Split('\\');
                        listURI.Add("\\");
                        Intent.PutExtra("prod", listprod.ToArray());
                        Intent.PutExtra("uri", listURI.ToArray());
                    }
                    Setdata();
                    alertall.Dismiss();
                    var totNoIva = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                    var tot = Convert.ToDecimal(totNoIva) + Convert.ToDecimal(totNoIva) / 100 * 22;
                    tot = Math.Round(tot, 2);
                    totNoIvaTextView.Text = totNoIva + '€';
                    totIvaTextView.Text = tot.ToString(CultureInfo.CurrentCulture) + '€';
                });
            builder.Show();


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
        private RadioButton deleteButton;
        private RadioButton addButton;
        private RadioButton orderButton;



        private void OrderButton_Click(object sender, EventArgs e)
        {
            if (codclifor == null)
            {
                Toast.MakeText(this, "Selezionare un cliente prima!", ToastLength.Short).Show();
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
                int last;
                try
                {
                    using (StreamReader sr = new StreamReader(path + "/docTes.txt"))
                    {
                        var teslist = new List<string>();
                        while (!sr.EndOfStream)
                        {
                            teslist.Add(sr.ReadLine());
                        }

                        last = teslist.Count + 1;
                    }

                }
                catch
                {
                    last = 0;
                }
                using (StreamWriter streamWriter = new StreamWriter(path + "/docRig.txt", true))
                {
                    for (var index = 0; index < listprod.Count; index++)
                    {
                        var prod = listprod[index];
                        var prodSplit = prod.Split(';');
                        var prodFirst = Helper.Table.First(p => p[0] == prodSplit[0].ToUpper()).ToList();
                        var codPRd = prodFirst[0];

                        var prodFin = codPRd;
                        for (var i = 1; i < prodSplit.Length - 1; i++)
                        {
                            prodFin += ";" + prodSplit[i];
                        }
                        //  var rig = index + ";" + last + ";" + prodFin;
                        var pUni = prodSplit[2].Split('/').Last();
                        pUni = new string(pUni.Take(pUni.Length - 1).ToArray());

                        var rig = $"{last};{index + 1};{docType};{DateTime.Now.ToShortDateString()};{index + 1};{prodSplit[0]};{pUni};{prodSplit[1]};{prodSplit[4]};{prodSplit[5]}{Environment.NewLine}";
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
                    var doc = "O";
                    switch (docType)
                    {
                        case (int)DocType.Vendita:
                            doc = Helper.GetAge(this).First(docR => docR[2] == "B")[0];
                            break;
                        case (int)DocType.Rapportino:
                            doc = Helper.GetAge(this).First(docR => docR[2] == "R")[0];
                            break;
                        case (int)DocType.Fattura:
                            doc = Helper.GetAge(this).First(docR => docR[2] == "F")[0];
                            break;
                        case (int)DocType.Bolla:
                            doc = Helper.GetAge(this).First(docR => docR[2] == "O")[0];
                            break;
                        case (int)DocType.Preventivo:
                            doc = Helper.GetAge(this).First(docR => docR[2] == "P")[0];
                            break;
                        case (int)DocType.Generico:
                            doc = Helper.GetAge(this).First(docR => docR[2] == "G")[0];
                            break;

                    }
                    streamWriter.Write($"{last};{doc};{last};{DateTime.Now.ToShortDateString()};{codclifor + codDest};{codAge};{editSconto.Text};{editNote.Text};{editAcc.Text}{Environment.NewLine}");

                }



                Toast.MakeText(this, "Ordine effetuato.", ToastLength.Short).Show();
                listprod = new List<string>();
                listView.SetAdapter(null);

            });
            builder1.Show();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
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
            void OnRadiogroupOnCheckedChange(object sender3, RadioGroup.CheckedChangeEventArgs args)
            {
                lw.ItemClick -= Lw_ItemClickAll;
                lw.ItemClick -= Lw_ItemClickCat;
                var id = args.CheckedId;
                if (id == radiobuttonCateg.Id)
                {
                    var group = Helper.GetGroup(this);
                    var grouptemp = group[2].Where(g => g.Split(';')[0].Length == 3).ToList();
                    var groupfianal = new List<string>();
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
                    for (var i = 1; i < Helper.Table.Count; i++)
                    {
                        var prod = Helper.Table[i];
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

            textSearch.TextChanged += (object sender2, TextChangedEventArgs args) =>
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
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Sicuro di volere cancellare tutto?");
            builder.SetCancelable(true);
            builder.SetPositiveButton("Si", delegate
            {
                listprod = new List<string>();
                listView.Adapter = null;
            });
            builder.SetNegativeButton("No", delegate { });
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
                Setdata();
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


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            AlertDialog alertMod = null;
            AlertDialog alertDel = null;
            switch (id)
            {

                case Resource.Id.addPhoto:
                    {
                        Intent getIntent = new Intent(Intent.ActionGetContent);
                        getIntent.SetType("image/*");

                        Intent pickIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                        pickIntent.SetType("image/*");

                        Intent chooserIntent = Intent.CreateChooser(getIntent, "Select Image");
                        chooserIntent.PutExtra(Intent.ExtraInitialIntents, new IParcelable[] { pickIntent });

                        StartActivityForResult(chooserIntent, PICK_IMAGE);

                        break;
                    }
                case Resource.Id.Modifica:
                    {
                        var prodTit = new List<string>();
                        foreach (var prod in listprod)
                        {
                            prodTit.Add(Helper.Table.First(prodo => prodo[0] == prod.Split(';')[0])[1]);
                        }

                        var listp = new ListView(this)
                        {
                            Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, prodTit)
                        };
                        var alertSeleMod = new AlertDialog.Builder(this);
                        alertSeleMod.SetCancelable(true);
                        alertSeleMod.SetView(listp);
                        alertSeleMod.SetTitle("Seleziona il prodotto da modificare");
                        listp.ItemClick += (ee, se) =>
                        {
                            var prod = listprod[se.Position];
                            var prodSplit = prod.Split(';');
                            var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };
                            var editTexQ = new EditText(this) { Text = prodSplit[1], Hint = "Quantità" };
                            var editTexS = new EditText(this) { Text = prodSplit[4], Hint = "Sconto" };
                            var editTextP = new EditText(this) { Text = prodSplit[2], Hint = "Prezzo particolare" };
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
                                prodSplit[2] = editTextP.Text;
                                var pTemp = prodSplit.Aggregate((current, next) => current + ";" + next);
                                listprod[se.Position] = pTemp;
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
                                    var pqueryed = query.First(p => p[0] == split[0].ToUpper());
                                    var namet = pqueryed[1];
                                    namet = textInfo.ToLower(namet);
                                    ptemp.Name = textInfo.ToTitleCase(namet);
                                    ptemp.QuantityPrice = split[1] + '/' + split[2];
                                    ptemp.CodArt = pqueryed[0];
                                    templist.Add(ptemp);

                                }
                                Setdata();
                                var totNoIva2 = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                                var tot2 = Convert.ToDecimal(totNoIva2) + Convert.ToDecimal(totNoIva2) / 100 * 22;
                                tot2 = Math.Round(tot2, 2);
                                totNoIvaTextView.Text = totNoIva2 + '€';
                                totIvaTextView.Text = tot2.ToString(CultureInfo.CurrentCulture) + '€';
                                alertMod.Dismiss();
                            });
                            builder.Show();
                        };
                        alertMod = alertSeleMod.Create();
                        alertMod.Show();



                        break;
                    }
                case Resource.Id.Elimina:
                    {
                        var prodTit = new List<string>();
                        foreach (var prod in listprod)
                        {
                            prodTit.Add(Helper.Table.First(prodo => prodo[0] == prod.Split(';')[0])[1]);
                        }

                        var listp = new ListView(this)
                        {
                            Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, prodTit)
                        };
                        var alertSeleDel = new AlertDialog.Builder(this);
                        alertSeleDel.SetCancelable(true);
                        alertSeleDel.SetView(listp);
                        alertSeleDel.SetTitle("Seleziona il prodotto da modificare");
                        listp.ItemClick += (ee, se) =>
                        {
                            listprod.RemoveAt(se.Position);
                            listURI.RemoveAt(se.Position);
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
                            foreach (var prodAda in finalList)
                            {
                                var ptemp = new Prodotto { ImageUrl = prodAda.uri };
                                var split = prodAda.prodotto.Split(';');
                                var pqueryed = query.First(p => p[0] == split[0].ToUpper());
                                var namet = pqueryed[1];
                                namet = textInfo.ToLower(namet);
                                ptemp.Name = textInfo.ToTitleCase(namet);
                                ptemp.QuantityPrice = split[1] + '/' + split[2];
                                ptemp.CodArt = pqueryed[0];
                                templist.Add(ptemp);

                            }
                            Setdata();
                            var totNoIva2 = Helper.GetTot(listprod).ToString(CultureInfo.CurrentCulture);
                            var tot2 = Convert.ToDecimal(totNoIva2) + Convert.ToDecimal(totNoIva2) / 100 * 22;
                            tot2 = Math.Round(tot2, 2);
                            totNoIvaTextView.Text = totNoIva2 + '€';
                            totIvaTextView.Text = tot2.ToString(CultureInfo.CurrentCulture) + '€';
                            alertDel.Dismiss();
                        };
                        alertDel = alertSeleDel.Create();
                        alertDel.Show();

                        break;
                    }
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            base.OnPrepareOptionsMenu(menu);
            defaultmenu = menu;

            setCount(this, nProgPhoto.ToString());
            return true;

        }

        private IMenu defaultmenu;
        public void setCount(Context context, String count)
        {
            var menuItem = defaultmenu.FindItem(Resource.Id.addPhoto);
            LayerDrawable icon = (LayerDrawable)menuItem.Icon;

            CountDrawable badge;

            // Reuse drawable if possible
            Drawable reuse = icon.FindDrawableByLayerId(Resource.Id.ic_group_count);
            if (reuse is CountDrawable drawable)
            {
                badge = drawable;
            }
            else
            {
                badge = new CountDrawable(context);
            }

            badge.setCount(count);
            icon.Mutate();
            icon.SetDrawableByLayerId(Resource.Id.ic_group_count, badge);
        }
        public static readonly int PICK_IMAGE = 1;
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode != PICK_IMAGE || resultCode != Result.Ok) return;
            var d = data.Data;
            MoveFile(ImageFilePath.getPath(this, d));
        }

        private int nProgPhoto;

        private void MoveFile(string inputPath)
        {
            var outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin/photoa" + $"/{DateTime.Now:yyyy-mm-dd}-codag-{codclifor}-{nProgPhoto}.jpg";
            InputStream input = null;
            OutputStream output = null;
            try
            {

                //create output directory if it doesn't exist
                var dir = new Java.IO.File(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Sarin/photoa");
                if (!dir.Exists())
                {
                    dir.Mkdirs();
                }


                input = new FileInputStream(inputPath);
                output = new FileOutputStream(outputPath);

                byte[] buffer = new byte[1024];
                int read;
                while ((read = input.Read(buffer)) != -1)
                {
                    output.Write(buffer, 0, read);
                }
                input.Close();
                input = null;

                // write the output file
                output.Flush();
                output.Close();
                output = null;



            }

            catch (FileNotFoundException fnfe1)
            {
                Log.Error("tag", fnfe1.Message);
            }
            catch (Exception e)
            {
                Log.Error("tag", e.Message);
            }
            nProgPhoto++;
            setCount(this, nProgPhoto.ToString());
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
        private void Setdata()
        {
            group.Clear();
            dictionary.Clear();
            var i = 0;
            foreach (var prod in listprod)
            {
                try
                {
                    var listInfo = new List<string>();
                    listInfo.AddRange(Enumerable.Repeat(prod, 7));
                    var p = Helper.Table.Find(ppp => ppp[0] == prod.Split(';')[0]);
                    group.Add(p[1]);
                    dictionary.Add(group[i], listInfo);
                    i++;
                }
                catch
                {
                    //todo handle two equals items
                    var listInfo = new List<string>();
                    listInfo.AddRange(Enumerable.Repeat(prod, 7));
                    var p = Helper.Table.Find(ppp => ppp[0] == prod.Split(';')[0]);
                    group.Add(p[1]);
                    dictionary.Add(group[i] + i, listInfo);
                    i++;
                    Toast.MakeText(this, "Non puoi aggiungere due oggetti uguali", ToastLength.Short).Show();
                }
            }

            var ada = new CartAdapter(this, group, dictionary);
            listView.SetAdapter(ada);
            Intent.PutExtra("prod", listprod.ToArray());
            Intent.PutExtra("uri", listURI.ToArray());


        }

    }
}

