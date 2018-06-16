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
    [Activity(Label = "ActivityAdd")]
    public class ActivityAdd : Activity
    {
        private string connectionString =
            @"Data Source=tcp:SarinCagliari.ddns.net\SQLEXPRESS01,1500; Initial Catalog=VISIONESEMPIO; User ID=sa;Password=Sarin2018";

        private Button addButton;
        private EditText addEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutAddc);
            // Create your application here
            addEditText = FindViewById<EditText>(Resource.Id.editTextaddc);
            addButton = FindViewById<Button>(Resource.Id.buttonAddCliente);
            addButton.Click += AddButton_Click;

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var lastinc = 0;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {

                connection.Open();
                SqlCommand lastCommand =
                    new SqlCommand("SELECT Codclifor FROM Clifor WHERE Codclifor LIKE 'C%'", connection);
                lastCommand.ExecuteNonQuery();
                SqlDataReader lastReader = lastCommand.ExecuteReader();
                string last = "";
                while (lastReader.Read())
                {
                    if (lastReader[0].ToString().Contains('0'))
                    {
                        last = lastReader[0].ToString();
                    }

                }

                var splitlast = last.Split('C');
                lastinc = Convert.ToInt32(splitlast[1]);
                lastinc++;
                last = string.Concat("C", lastinc.ToString());
                lastReader.Close();
                SqlCommand command =
                    new SqlCommand(
                        $"INSERT INTO Clifor (Codclifor,Desclifor,Codpagam) VALUES ('{last}','{addEditText.Text}','RDVF')",
                        connection);
                command.ExecuteNonQuery();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception exception)
            {
                connection.Close();
                connection.Dispose();
                Add(lastinc++);
            }
            /*  SqlCommand checkCommand = new SqlCommand($"SELECT Desclifor FROM Clifor WHERE Desclifor='{addEditText.Text}'", connection);
            SqlDataReader checkReader = checkCommand.ExecuteReader();
            while (checkReader.Read())
            {
                var x = checkReader[0].ToString();
            }*/


        }

        public void Add(int key=0)
        {
            var lastinc=key;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                var last = string.Concat("C", lastinc.ToString());
                SqlCommand command =
                    new SqlCommand(
                        $"INSERT INTO Clifor (Codclifor,Desclifor,Codpagam) VALUES ('{last}','{addEditText.Text}','RDVF')",
                        connection);
                command.ExecuteNonQuery();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception exception)
            {
                Add(++lastinc);
            }
        }
    }
}