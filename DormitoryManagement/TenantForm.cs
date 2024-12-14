using Bunifu.UI.WinForms;
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

namespace DormitoryManagement
{
    public partial class TenantForm : Form
    {
        public void LoadTenantName() {

            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    // Query to get available rooms
                    string query = "SELECT Name FROM Tenant";

                    // Initialize SqlCommand
                    SqlCommand cmd = new SqlCommand(query, cnn);

                    // Execute and read data
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear existing items in the ComboBox
                    cbName.Items.Clear();

                    while (reader.Read())
                    {
                        // Add each available DormUnit to the ComboBox
                        cbName.Items.Add(reader["Name"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching available rooms: " + ex.Message);
                }
            }

        }
        public void LoadDormUnit() {

            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    // Query to get available rooms
                    string query = "SELECT DormUnit FROM DormList WHERE DormUnit NOT IN (SELECT DormUnit FROM Tenant WHERE DormUnit IS NOT NULL)";

                    // Initialize SqlCommand
                    SqlCommand cmd = new SqlCommand(query, cnn);

                    // Execute and read data
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear existing items in the ComboBox
                    cbDorm.Items.Clear();

                    while (reader.Read())
                    {
                        // Add each available DormUnit to the ComboBox
                        cbDorm.Items.Add(reader["DormUnit"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching available rooms: " + ex.Message);
                }
            }


        }
        private void LoadTenantData()
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                cnn.Open();

                // Query to fetch data from Tenant
                string query = "SELECT * FROM [Tenant];";

                // Initialize SqlDataAdapter
                SqlDataAdapter sqlDA = new SqlDataAdapter(query, cnn);

                // Fill the DataTable
                DataTable dt = new DataTable();
                sqlDA.Fill(dt);

                // Bind the data to the DataGridView
                DataGridView1.DataSource = dt;

                // Optional: Auto-resize columns for better display
                DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                // Handle or log the error
                MessageBox.Show("An error occurred while loading data: " + ex.Message);
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

        public TenantForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNewTenantFrame frm = new AddNewTenantFrame();
            frm.Show();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void TenantForm_Load(object sender, EventArgs e)
        {
            DataGridView1.ReadOnly = true;
            LoadDormUnit();
            LoadTenantName();
            LoadTenantData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                cnn.Open();

                // Retrieve data from combobox
                string DormUnit = cbDorm.Text.Trim();
                string TenantName = cbName.Text.Trim();

                // Step 1: Get the MonthlyRentRate for the selected DormUnit
                decimal monthlyRentRate = 0;
                string getRentRateQuery = "SELECT MonthlyRentRate FROM DormList WHERE DormUnit = @DormUnit";
                using (SqlCommand rentCmd = new SqlCommand(getRentRateQuery, cnn))
                {
                    rentCmd.Parameters.Add("@DormUnit", SqlDbType.Int).Value = Convert.ToInt32(DormUnit);
                    object result = rentCmd.ExecuteScalar();
                    if (result != null)
                    {
                        monthlyRentRate = Convert.ToDecimal(result);
                    }
                    else
                    {
                        MessageBox.Show("Invalid DormUnit selected.");
                        return;
                    }
                }

                // Step 2: Update the Tenant's DormUnit
                string updateTenantQuery = "UPDATE Tenant SET DormUnit = @DormUnit WHERE Name = @Name";
                using (SqlCommand tenantCmd = new SqlCommand(updateTenantQuery, cnn))
                {
                    tenantCmd.Parameters.Add("@DormUnit", SqlDbType.Int).Value = Convert.ToInt32(DormUnit);
                    tenantCmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = TenantName;

                    int rowsAffected = tenantCmd.ExecuteNonQuery();
                    if (rowsAffected <= 0)
                    {
                        MessageBox.Show("No record found to update for Tenant.");
                        return;
                    }
                }

                // Step 3: Update the Billing table's MonthlyRentRate for the affected Tenant
                string updateBillingQuery = "UPDATE Billing SET MonthlyRentRate = @MonthlyRentRate WHERE TenantID = (SELECT TenantID FROM Tenant WHERE Name = @Name)";
                using (SqlCommand billingCmd = new SqlCommand(updateBillingQuery, cnn))
                {
                    billingCmd.Parameters.Add("@MonthlyRentRate", SqlDbType.Decimal).Value = monthlyRentRate;
                    billingCmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = TenantName;

                    int billingRowsAffected = billingCmd.ExecuteNonQuery();
                    if (billingRowsAffected > 0)
                    {
                        MessageBox.Show("Tenant details updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No tenant record found to update.");
                    }
                }

                // Refresh the data in DataGridView
                LoadDormUnit();
                LoadTenantName();
                LoadTenantData();
            }
            catch (Exception ex)
            {
                // Handle or log the error
                MessageBox.Show("An error occurred while updating data: " + ex.Message);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                cnn.Open();

                // Retrieve the selected tenant name
                string tenantName = cbName.Text.Trim();

                if (string.IsNullOrEmpty(tenantName))
                {
                    MessageBox.Show("Please select a tenant name to delete.");
                    return;
                }

                // Confirm deletion
                DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete the tenant '{tenantName}'?", "Confirm Deletion", MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes)
                {
                    return;
                }

                // Step 1: Delete records from ReportOfPayment
                string deletePaymentsQuery = "DELETE FROM ReportOfPayment WHERE TenantID = (SELECT TenantID FROM Tenant WHERE Name = @Name)";
                using (SqlCommand paymentCmd = new SqlCommand(deletePaymentsQuery, cnn))
                {
                    paymentCmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = tenantName;
                    paymentCmd.ExecuteNonQuery();
                }

                // Step 2: Delete records from Billing
                string deleteBillingQuery = "DELETE FROM Billing WHERE TenantID = (SELECT TenantID FROM Tenant WHERE Name = @Name)";
                using (SqlCommand billingCmd = new SqlCommand(deleteBillingQuery, cnn))
                {
                    billingCmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = tenantName;
                    billingCmd.ExecuteNonQuery();
                }

                // Step 3: Delete the Tenant record
                string deleteTenantQuery = "DELETE FROM Tenant WHERE Name = @Name";
                using (SqlCommand tenantCmd = new SqlCommand(deleteTenantQuery, cnn))
                {
                    tenantCmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = tenantName;
                    int rowsAffected = tenantCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Tenant deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No tenant record found to delete.");
                    }
                }

                // Refresh the data in DataGridView and ComboBoxes
                LoadDormUnit();
                LoadTenantName();
                LoadTenantData();
            }
            catch (Exception ex)
            {
                // Handle or log the error
                MessageBox.Show("An error occurred while deleting data: " + ex.Message);
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
    }
}
