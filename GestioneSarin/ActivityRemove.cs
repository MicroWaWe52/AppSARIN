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
    [Activity(Label = "ActivityRemove")]
    public class ActivityRemove : Activity
    {
        private EditText searchEditText;
        private ListView removeListView;
        private string connectionString =
            @"Data Source=tcp:SarinCagliari.ddns.net\SQLEXPRESS01,1500; Initial Catalog=VISIONESEMPIO; User ID=sa;Password=Sarin2018";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutRemove);

            searchEditText = FindViewById<EditText>(Resource.Id.editTextRemovec);
            searchEditText.TextChanged += SearchEditText_TextChanged;
            removeListView = FindViewById<ListView>(Resource.Id.listViewRemove);
            removeListView.ItemClick += RemoveListView_ItemClick;
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
            removeListView.Adapter = listAdapter;
            connection.Close();
            connection.Dispose();
        }

        private void RemoveListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string descRemove = removeListView.GetItemAtPosition(Convert.ToInt32(e.Position)).ToString();
            SqlCommand removeCommand =
                new SqlCommand(
                    $"DELETE FROM Clifor WHERE Desclifor= '{descRemove}' ", connection);
            removeCommand.ExecuteNonQuery();
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
            removeListView.Adapter = listAdapter;
            connection.Close();
            connection.Dispose();
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
            removeListView.Adapter = listAdapter;
            connection.Close();
            connection.Dispose();
        }
    }
}