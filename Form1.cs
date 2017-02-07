using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Configuration;

namespace TestMyDatabaseTech
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> connStrPool;
        private Dictionary<string, SqlConnection> connPool;
        List<Int32> connSpeed;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // detect configuration file here, do configuration setup
            connStrPool = new Dictionary<string, string>();
            connPool = new Dictionary<string, SqlConnection>();
            List<Int32> connSpeed = new List<Int32>();
            ReadSettingsFromConfig();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            foreach (var connectPair in connStrPool)
            {
                // Add code to check if this connection is already available
                string connectString = connectPair.Value;
                SqlConnection curConnection;

                if (connPool.ContainsKey(connectPair.Key))
                {
                    if (connPool[connectPair.Key].State == ConnectionState.Open)
                        continue;
                    else
                        curConnection = connPool[connectPair.Key];
                }
                else curConnection = new SqlConnection(connectString);

                try
                {
                    curConnection.Open();

                    // Test database connection code here; best be asynchronous code
                    TestConnectionSpeed(curConnection, ref connSpeed);
                    //

                    SqlCommand myCommand = new SqlCommand("select * from myPerson", curConnection);
                    SqlDataAdapter myDA = new SqlDataAdapter(myCommand);
                    DataTable myTable = new DataTable();
                    myDA.Fill(myTable);
                    dataGridView1.DataSource = myTable;
                }
                catch (SqlException myDatabaseException)
                {
                    Console.Out.WriteLine(myDatabaseException.ToString());
                    string errmsg = "Database connection error! " + myDatabaseException.ToString();
                    MessageBox.Show(errmsg);
                    this.Dispose();
                }
                
                Console.Out.WriteLine("Success!");
            }
        }




        private void TestConnectionSpeed(SqlConnection curConnection, ref List<Int32> connSpeed)
        {
            try
            {
                // use some asynchronous method
                Stopwatch myTimer = Stopwatch.StartNew();
                SqlCommand testCommand = new SqlCommand("select 1", curConnection);
                object retScalar = testCommand.ExecuteScalar();
                myTimer.Stop();
                if (retScalar != null)
                    connSpeed.Add(myTimer.Elapsed.Milliseconds);
                else
                    connSpeed.Add(-1);

            }
            catch (SqlException curException)
            {
                MessageBox.Show(curException.Message);
            }
        }



        private void ReadSettingsFromConfig()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
             //   MessageBox.Show(appSettings.GetType().ToString());
                if (appSettings.Count == 0)
                    Console.Out.WriteLine("App.config file probably empty");
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        Console.Out.WriteLine("key: {0}, value: {1}", key, appSettings[key]);
                        connStrPool[key] = appSettings[key];
                    }
                }
            } 
            catch (ConfigurationException myConfigExcept)
            {
                MessageBox.Show(myConfigExcept.Message);
                this.Dispose();
            }

        }
    }
}
