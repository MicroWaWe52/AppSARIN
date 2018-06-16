using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GestioneSarin
{
    [Activity(Label = "ActivityUpdate")]
    public class ActivityUpdate : Activity
    {
        private EditText searchEditText;
        private ListView updateListView;
        private string connectionString =
            @"Data Source=tcp:SarinCagliari.ddns.net\SQLEXPRESS01,1500; Initial Catalog=VISIONESEMPIO; User ID=sa;Password=Sarin2018";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutRemove);
            searchEditText = FindViewById<EditText>(Resource.Id.editTextRemovec);
            searchEditText.TextChanged += SearchEditText_TextChanged; 
            updateListView = FindViewById<ListView>(Resource.Id.listViewRemove);
            updateListView.ItemClick += UpdateListViewItemClick;
            // Create your application here
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand listCommand = new SqlCommand("SELECT Desclifor FROM Clifor WHERE Codclifor LIKE 'C%'", connection);
            SqlDataReader listreader = listCommand.ExecuteReader();
            var listDesc = new List<string>();
            while (listreader.Read())
            {
                listDesc.Add(listreader[0].ToString());
            }
            ArrayAdapter listAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, listDesc);
            updateListView.Adapter = listAdapter;
            connection.Close();
            connection.Dispose();
            // Create your application here
        }

        private void UpdateListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var desc = updateListView.GetItemAtPosition(e.Position).ToString();
            AlertDialog.Builder builder=new AlertDialog.Builder(this);
            EditText updaEditText=new EditText(this);
            updaEditText.Text = desc;
            builder.SetTitle("Modifica cliente");
            builder.SetPositiveButton("OK", delegate
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand updatecommand = new SqlCommand($"UPDATE Clifor SET Desclifor='{updaEditText.Text}' WHERE Desclifor='{desc}'", connection);
                updatecommand.ExecuteNonQuery();
                connection.Close();
                connection.Dispose();
            });
            builder.SetNegativeButton("Annulla", delegate { });
            builder.SetView(updaEditText);
            builder.Show();


        }

        private void SearchEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand listCommand = new SqlCommand("SELECT Desclifor FROM Clifor WHERE Codclifor LIKE 'C%'", connection);
            SqlDataReader listreader = listCommand.ExecuteReader();
            var listDesc = new List<string>();
            while (listreader.Read())
            {
                if (listreader[0].ToString().Contains(searchEditText.Text))
                {
                    listDesc.Add(listreader[0].ToString());
                }
            }
            ArrayAdapter listAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, listDesc);
            updateListView.Adapter = listAdapter;
            connection.Close();
            connection.Dispose();
        }
    }
}