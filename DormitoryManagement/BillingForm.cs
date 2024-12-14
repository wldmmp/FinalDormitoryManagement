using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DormitoryManagement
{
    public partial class BillingForm : Form
    {
        // Connection string for database
        private string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

        // Class-level variables to hold dorm unit and monthly rent rate
        public static string dormUnit;
        public static string monthlyRentRate;

        public void LoadBillingData()
        {
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                cnn.Open();

                // Modified query to include TotalOutstandingBalance from ReportOfBalance table
                string query = @"
            SELECT 
                b.BillID,
                t.Name AS TenantName,
                b.DateRegistered,
                b.DueDate,
                b.UtilityDueDate,
                b.MonthlyRentRate,
                b.ElectricBill,
                b.WaterBill,
                r.TotalOutstandingBalance -- Include outstanding balance
            FROM 
                Billing b
            JOIN 
                Tenant t ON b.TenantID = t.TenantID
            LEFT JOIN 
                ReportOfBalance r ON b.TenantID = r.TenantID;"; // Join ReportOfBalance table

                // Initialize SqlDataAdapter
                SqlDataAdapter sqlDA = new SqlDataAdapter(query, cnn);

                // Fill the DataTable
                DataTable dt = new DataTable();
                sqlDA.Fill(dt);

                // Add Due Date warning
                dt.Columns.Add("Due Date Warning", typeof(string)); // Add new column for warning

                foreach (DataRow row in dt.Rows)
                {
                    DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
                    if (dueDate < DateTime.Now) // Check if the due date is in the past
                    {
                        row["Due Date Warning"] = "Due Date Passed!"; // Add warning for overdue bills
                    }
                    else
                    {
                        row["Due Date Warning"] = "Payment Pending"; // No warning if it's not overdue
                    }
                }

                // Bind the data to the DataGridView
                dataGridView1.DataSource = dt;

                // Optional: Auto-resize columns for better display
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                // Handle or log the error
                MessageBox.Show("An error occurred while loading billing data: " + ex.Message);
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
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Due Date Warning" && e.Value != null)
            {
                if (e.Value.ToString() == "Due Date Passed!")
                {
                    e.CellStyle.BackColor = Color.Red;  // Set background color to red for overdue
                    e.CellStyle.ForeColor = Color.White;  // Set text color to white
                }
                else
                {
                    e.CellStyle.BackColor = Color.Yellow;  // Set background color to yellow for pending
                }
            }
        }
        public BillingForm()
        {
            InitializeComponent();
        }

        // Define a class for ComboBox items
        private class ComboBoxItem
        {
            public string Text { get; set; } // Display name
            public int Value { get; set; }  // Tenant ID

            // Override ToString to display only the Text property
            public override string ToString()
            {
                return Text;
            }
        }

        // Method to load the tenant names into ComboBox
        public void LoadTenantName()
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    // Query to get tenant names
                    string query = "SELECT Name, TenantID FROM Tenant";

                    SqlCommand cmd = new SqlCommand(query, cnn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear existing items in ComboBox
                    cbName.Items.Clear();

                    while (reader.Read())
                    {
                        // Add tenant name and ID to ComboBox using ComboBoxItem class
                        cbName.Items.Add(new ComboBoxItem
                        {
                            Text = reader["Name"].ToString(),
                            Value = Convert.ToInt32(reader["TenantID"])
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching tenant names: " + ex.Message);
                }
            }
        }

        // Method to set the data based on the selected tenant
        public void SetData()
        {
            if (cbName.SelectedItem is ComboBoxItem selectedTenant)
            {
                int tenantID = selectedTenant.Value;

                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    try
                    {
                        cnn.Open();

                        // Query to get dorm unit and monthly rent rate based on the selected tenant
                        string query = @"
                            SELECT d.DormUnit, d.MonthlyRentRate
                            FROM Tenant t
                            INNER JOIN DormList d ON t.DormUnit = d.DormUnit
                            WHERE t.TenantID = @TenantID";

                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@TenantID", tenantID);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            dormUnit = reader["DormUnit"].ToString();
                            monthlyRentRate = reader["MonthlyRentRate"].ToString();

                            // Update labels or textboxes with the fetched data
                            lblDormUnit.Text = dormUnit;
                            lblMonthlyRentRate.Text = monthlyRentRate;
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error fetching dorm unit and rent rate: " + ex.Message);
                    }
                }
            }
        }

        // Event handler when a tenant name is selected from ComboBox
        private void cbName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetData(); // Fetch and display the dorm unit and rent rate
        }

        // Event handler when the form loads
        private void BillingForm_Load(object sender, EventArgs e)
        {
            LoadTenantName(); // Load tenant names when form loads
            LoadBillingData(); // Load tenant bills to datagridview
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (cbName.SelectedItem is ComboBoxItem selectedTenant)
            {
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    SqlTransaction transaction = null;

                    try
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        int tenantID = selectedTenant.Value;

                        // Retrieve tenant's monthly rent rate and dorm unit
                        double monthlyRentRate;
                        if (!double.TryParse(lblMonthlyRentRate.Text.Trim(), out monthlyRentRate))
                        {
                            MessageBox.Show("Invalid Monthly Rent Rate.");
                            return;
                        }

                        int dormUnit;
                        if (!int.TryParse(lblDormUnit.Text.Trim(), out dormUnit))
                        {
                            MessageBox.Show("Invalid Dorm Unit.");
                            return;
                        }

                        // Calculate utility bills based on user input
                        double consumedWater;
                        if (!double.TryParse(txtConsumedWater.Text.Trim(), out consumedWater))
                        {
                            MessageBox.Show("Invalid water consumption input.");
                            return;
                        }

                        double ratePerCM;
                        if (!double.TryParse(txtRateCM.Text.Trim(), out ratePerCM))
                        {
                            MessageBox.Show("Invalid water rate per CM.");
                            return;
                        }

                        double waterBill = consumedWater * ratePerCM;

                        double consumedKW;
                        if (!double.TryParse(txtConsumedKW.Text.Trim(), out consumedKW))
                        {
                            MessageBox.Show("Invalid electricity consumption input.");
                            return;
                        }

                        double ratePerKW;
                        if (!double.TryParse(txtRateKW.Text.Trim(), out ratePerKW))
                        {
                            MessageBox.Show("Invalid electricity rate per KW.");
                            return;
                        }

                        double electricBill = consumedKW * ratePerKW;

                        // Retrieve the previous outstanding balance (if any)
                        double previousOutstandingBalance = 0;
                        string checkBalanceQuery = @"
                    SELECT TotalOutstandingBalance 
                    FROM ReportOfBalance 
                    WHERE TenantID = @TenantID";

                        using (SqlCommand checkCmd = new SqlCommand(checkBalanceQuery, cnn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@TenantID", tenantID);
                            var result = checkCmd.ExecuteScalar();
                            if (result != DBNull.Value)
                            {
                                previousOutstandingBalance = Convert.ToDouble(result);
                            }
                        }

                        // Update data in the Billing table with the new calculated values
                        string updateBillingQuery = @"
                    UPDATE Billing
                    SET ElectricBill = @ElectricBill, WaterBill = @WaterBill
                    WHERE TenantID = @TenantID";

                        using (SqlCommand billingCmd = new SqlCommand(updateBillingQuery, cnn, transaction))
                        {
                            billingCmd.Parameters.AddWithValue("@TenantID", tenantID);
                            billingCmd.Parameters.AddWithValue("@ElectricBill", electricBill);
                            billingCmd.Parameters.AddWithValue("@WaterBill", waterBill);

                            int rowsAffected = billingCmd.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                MessageBox.Show("No billing record found to update.");
                                return;
                            }
                        }

                        // Calculate the total bill for the current month (new rent + new utility bills)
                        double totalBillForCurrentMonth = monthlyRentRate + electricBill + waterBill;


                        // Calculate the new outstanding balance
                        double newOutstandingBalance = (previousOutstandingBalance + totalBillForCurrentMonth);

                        string updateBalanceQuery = @"
                    UPDATE ReportOfBalance
                    SET 
                        MonthlyRentRate = @MonthlyRentRate,
                        ElectricBill = @ElectricBill,
                        WaterBill = @WaterBill,
                        Paid = @Paid,
                        TotalOutstandingBalance = @TotalOutstandingBalance
                    WHERE TenantID = @TenantID";

                        using (SqlCommand updateCmd = new SqlCommand(updateBalanceQuery, cnn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@TenantID", tenantID);
                            updateCmd.Parameters.AddWithValue("@MonthlyRentRate", monthlyRentRate);
                            updateCmd.Parameters.AddWithValue("@ElectricBill", electricBill);
                            updateCmd.Parameters.AddWithValue("@WaterBill", waterBill);
                            updateCmd.Parameters.AddWithValue("@Paid", 0);
                            updateCmd.Parameters.AddWithValue("@TotalOutstandingBalance", newOutstandingBalance);

                            updateCmd.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();

                        // Reload billing data to reflect updates
                        LoadBillingData();
                        MessageBox.Show("Billing and outstanding balance updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of error
                        transaction?.Rollback();
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("Error processing billing: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a tenant to calculate.");
            }
        }


        //no more adjustment
        private void btnPay_Click(object sender, EventArgs e)
        {
            if (cbName.SelectedItem is ComboBoxItem selectedTenant)
            {
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    SqlTransaction transaction = null;

                    try
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        // Retrieve input values
                        int tenantID = selectedTenant.Value;
                        decimal paymentAmount;
                        if (!decimal.TryParse(txtAmount.Text.Trim(), out paymentAmount) || paymentAmount <= 0)
                        {
                            MessageBox.Show("Please enter a valid positive payment amount.");
                            return;
                        }

                        DateTime paymentDate = DateTime.Now;

                        // Update DueDate and UtilityDueDate in Billing table
                        string updateQuery = @"
                UPDATE Billing
                SET 
                    DueDate = DATEADD(MONTH, 1, DueDate), 
                    UtilityDueDate = DATEADD(MONTH, 1, UtilityDueDate)
                WHERE TenantID = @TenantID";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, cnn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@TenantID", tenantID);
                            updateCmd.ExecuteNonQuery();
                        }

                        // Insert payment into ReportOfPayment table
                        string paymentQuery = @"
                INSERT INTO ReportOfPayment (TenantID, DormUnit, Date, Amount) 
                VALUES (@TenantID, @DormUnit, @Date, @Amount)";

                        using (SqlCommand paymentCmd = new SqlCommand(paymentQuery, cnn, transaction))
                        {
                            paymentCmd.Parameters.AddWithValue("@TenantID", tenantID);
                            paymentCmd.Parameters.AddWithValue("@DormUnit", Convert.ToInt32(lblDormUnit.Text.Trim()));
                            paymentCmd.Parameters.AddWithValue("@Date", paymentDate);
                            paymentCmd.Parameters.AddWithValue("@Amount", paymentAmount);

                            paymentCmd.ExecuteNonQuery();
                        }

                        // Update Paid amount and recompute outstanding balance in ReportOfBalance
                        string updateBalanceQuery = @"
                UPDATE ReportOfBalance
                SET 
                    Paid = Paid + @PaymentAmount,
                    TotalOutstandingBalance = TotalOutstandingBalance - @PaymentAmount, 
                    LastPayment = @PaymentDate
                WHERE TenantID = @TenantID";

                        using (SqlCommand balanceCmd = new SqlCommand(updateBalanceQuery, cnn, transaction))
                        {
                            balanceCmd.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
                            balanceCmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                            balanceCmd.Parameters.AddWithValue("@TenantID", tenantID);

                            balanceCmd.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();

                        MessageBox.Show("Payment recorded successfully, balances updated, and due dates extended!");

                        // Refresh data to reflect changes
                        LoadBillingData();

                        // Clear input fields after successful payment
                        txtAmount.Clear();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of error
                        transaction?.Rollback();
                        MessageBox.Show("Error recording payment: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a tenant to record the payment.");
            }
        }

    }
}