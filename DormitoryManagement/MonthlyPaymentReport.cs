using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DormitoryManagement
{
    public partial class MonthlyPaymentReport : Form
    {
        private string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

        public MonthlyPaymentReport()
        {
            InitializeComponent();
        }

        // Method to load payments based on date range
        public void LoadMonthlyPayment(DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    // Query with date range filter
                    string query = @"
                        SELECT * 
                        FROM [ReportOfPayment] 
                        WHERE Date >= @FromDate AND Date <= @ToDate";

                    // Initialize SqlCommand
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        // Execute the query and load data into a DataTable
                        SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sqlDA.Fill(dt);

                        // Bind data to DataGridView
                        dataGridView1.DataSource = dt;

                        // Auto-resize columns for better display
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading payment data: " + ex.Message);
                }
            }
        }

        // Event handler for loading the form
        private void MonthlyPaymentReport_Load(object sender, EventArgs e)
        {
            // Optionally, initialize your DataGridView here if necessary
        }

        // Event handler for the filter button
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse the dates from input controls
                DateTime FromDate = fromDate.Value.Date;
                DateTime ToDate = toDate.Value.Date;

                // Validate date range
                if (FromDate > ToDate)
                {
                    MessageBox.Show("From Date cannot be later than To Date.");
                    return;
                }

                // Load the data with the specified date range
                LoadMonthlyPayment(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing the date filter: " + ex.Message);
            }
        }

        // Event handler for close button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optional: Handle cell click events if needed
        }
    }
}
