using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DormitoryManagement
{
    public partial class MonthlyBalanceReport : Form
    {

        private string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

        public void setCashtoCollect()
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    // Query to sum up all outstanding balances
                    string query = "SELECT SUM(TotalOutstandingBalance) AS TotalBalance FROM [ReportOfBalance];";

                    SqlCommand cmd = new SqlCommand(query, cnn);

                    object result = cmd.ExecuteScalar();

                    // Check if result is not null and update the label
                    if (result != DBNull.Value)
                    {
                        decimal totalBalance = Convert.ToDecimal(result);
                        lblCashtoCollect.Text = totalBalance.ToString("C"); // Format as currency
                    }
                    else
                    {
                        lblCashtoCollect.Text = "0.00"; // Default value if no rows exist
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching total outstanding balance: " + ex.Message);
                }
            }
        }



        public void loadBalanceReport() 
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                cnn.Open();

                // Query to fetch data from DormList
                string query = "SELECT * FROM [ReportOfBalance];";

                // Initialize SqlDataAdapter
                SqlDataAdapter sqlDA = new SqlDataAdapter(query, cnn);

                // Fill the DataTable
                DataTable dt = new DataTable();
                sqlDA.Fill(dt);

                // Bind the data to the DataGridView
                dataGridView1.DataSource = dt;

                // Optional: Auto-resize columns for better display
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                // Handle or log the error
                MessageBox.Show("Please input numbers only on Dorm Unit.");
            }
            finally
            {
                // Ensure the connection is closed
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }

        }

        public MonthlyBalanceReport()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void MonthlyBalanceReport_Load(object sender, EventArgs e)
        {
            loadBalanceReport();
            setCashtoCollect();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
