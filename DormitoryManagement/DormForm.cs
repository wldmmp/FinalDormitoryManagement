using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DormitoryManagement
{
    public partial class DormForm : Form
    {
        public DormForm()
        {
            InitializeComponent();
        }
        private void LoadDormData()
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                cnn.Open();

                // Query to fetch data from DormList
                string query = "SELECT * FROM [DormList];";

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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    // Open connection
                    cnn.Open();

                    // Retrieve data from textboxes
                    int dormUnit = int.Parse(txtDormUnit.Text.Trim()); // Assuming DormUnit is an INT
                    string description = txtDescription.Text.Trim();
                    decimal price = decimal.Parse(txtPrice.Text.Trim()); // Assuming Price is DECIMAL(10,2)

                    // Insert query with column names specified
                    string query = "INSERT INTO DormList (DormUnit, Description, MonthlyRentRate) VALUES (@DormUnit, @Description, @Price)";

                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@DormUnit", dormUnit);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);

                        // Execute query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Dorm unit added successfully!");

                            // Clear input fields
                            txtDormUnit.Clear();
                            txtDescription.Clear();
                            txtPrice.Clear();

                            // Refresh DataGridView (call a method to reload data from the database)
                            LoadDormData();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add the dorm unit.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Display error without clearing user input
                    MessageBox.Show("An error occurred: " + "Please enter the information correctly");
                }
            }
        }

        private void DormForm_Load(object sender, EventArgs e)
        {
            DataGridView1.ReadOnly = true;
            LoadDormData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDormData();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                cnn.Open();

                // Retrieve data from textboxes
                string DormUnit = txtDormUnit.Text.Trim();
                string Description = txtDescription.Text.Trim();
                double Price = Convert.ToDouble(txtPrice.Text.Trim());

                // Use UPDATE statement
                string query = "UPDATE DormList SET Description = @Description, MonthlyRentRate = @Price WHERE DormUnit = @DormUnit";

                // Initialize SqlCommand
                using (SqlCommand cmd = new SqlCommand(query, cnn))
                {
                    // Add parameters with explicit data types
                    cmd.Parameters.Add("@DormUnit", SqlDbType.VarChar).Value = DormUnit;
                    cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                    cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = Price;

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Show success message
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Dorm details updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No record found to update.");
                    }
                }

                // Refresh the data in DataGridView
                LoadDormData();
                txtDormUnit.Clear();
                txtDescription.Clear();
                txtPrice.Clear();
            }
            catch (Exception ex)
            {
                // Handle or log the error
                MessageBox.Show("An error occurred: " + "Please enter the information correctly");
                txtDormUnit.Clear();
                txtDescription.Clear();
                txtPrice.Clear();
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

        private void txtDormUnit_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}



