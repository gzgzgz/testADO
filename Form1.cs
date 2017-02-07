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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder connectString = new SqlConnectionStringBuilder();
            connectString["Data Source"] = "localhost\\sqlexpress";
            connectString["Initial Catalog"] = "master";
            connectString["User ID"] = "sa";
            connectString["Password"] = "00110901";
            Console.Out.WriteLine(connectString.ConnectionString);
            using (SqlConnection curConnection = new SqlConnection(connectString.ConnectionString))
            {
                try
                {
                    curConnection.Open();

                    // Test database connection code here;
                    List<Int32> connSpeed = new List<Int32>();
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

        private void TestConnectionSpeed(SqlConnection curConnection, ref List<int> connSpeed)
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
    }
}
