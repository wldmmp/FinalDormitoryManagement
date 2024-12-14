using System;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Windows.Forms;

namespace DormitoryManagement
{
    public partial class AddNewTenantFrame : Form
    {
        private readonly string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

        public AddNewTenantFrame()
        {
            InitializeComponent();
        }

        private void AddNewTenantFrame_Load(object sender, EventArgs e)
        {
            cbGender.Text = "Gender";
            LoadAvailableRooms();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();

                    // Insert Tenant and get TenantID
                    int tenantID = InsertTenant(cnn);

                    // Insert Billing
                    InsertBilling(cnn, tenantID);

                    // Insert Report Balance
                    InsertReportBalance(cnn, tenantID);

                    MessageBox.Show("Tenant added successfully!");
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            if (txtPhone.Text.Length > 11)
            {
                txtPhone.Text = txtPhone.Text.Substring(0, 11);
                txtPhone.SelectionStart = txtPhone.Text.Length;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void LoadAvailableRooms()
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    string query = "SELECT DormUnit FROM DormList WHERE DormUnit NOT IN (SELECT DormUnit FROM Tenant WHERE DormUnit IS NOT NULL)";
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cbRoom.Items.Clear();
                        while (reader.Read())
                        {
                            cbRoom.Items.Add(reader["DormUnit"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching available rooms: " + ex.Message);
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                cbGender.SelectedIndex == -1 ||
                cbRoom.SelectedIndex == -1)
            {
                MessageBox.Show("All fields must be filled.");
                return false;
            }

            if (!int.TryParse(cbRoom.Text.Trim(), out _))
            {
                MessageBox.Show("Please select a valid room number.");
                return false;
            }

            if (txtPhone.Text.Length != 11)
            {
                MessageBox.Show("Phone number must be exactly 11 digits.");
                return false;
            }

            if (!IsValidEmail(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid email address.");
                return false;
            }

            return true;
        }

        private int InsertTenant(SqlConnection cnn)
        {
            string tenantQuery = "INSERT INTO Tenant (Name, Gender, DormUnit, StartingDate, Email, Phone) " +
                                 "OUTPUT INSERTED.TenantID " +
                                 "VALUES (@Name, @Gender, @DormUnit, @StartingDate, @Email, @Phone)";

            using (SqlCommand tenantCmd = new SqlCommand(tenantQuery, cnn))
            {
                tenantCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                tenantCmd.Parameters.AddWithValue("@Gender", cbGender.Text.Trim());
                tenantCmd.Parameters.AddWithValue("@DormUnit", int.Parse(cbRoom.Text.Trim()));
                tenantCmd.Parameters.AddWithValue("@StartingDate", DateTime.Now);
                tenantCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                tenantCmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());

                return (int)tenantCmd.ExecuteScalar();
            }
        }

        private void InsertBilling(SqlConnection cnn, int tenantID)
        {
            string billingQuery = "INSERT INTO Billing (TenantID, DateRegistered, DueDate, UtilityDueDate, MonthlyRentRate, ElectricBill, WaterBill) " +
                                  "VALUES (@TenantID, @DateRegistered, @DueDate, @UtilityDueDate, @MonthlyRentRate, @ElectricBill, @WaterBill)";

            decimal monthlyRentRate = GetMonthlyRentRate(cnn, int.Parse(cbRoom.Text.Trim()));
            DateTime startingDate = DateTime.Now;
            DateTime dueDate = startingDate.AddMonths(2);
            DateTime utilityDueDate = startingDate.AddMonths(1);

            using (SqlCommand billingCmd = new SqlCommand(billingQuery, cnn))
            {
                billingCmd.Parameters.AddWithValue("@TenantID", tenantID);
                billingCmd.Parameters.AddWithValue("@DateRegistered", startingDate);
                billingCmd.Parameters.AddWithValue("@DueDate", dueDate);
                billingCmd.Parameters.AddWithValue("@UtilityDueDate", utilityDueDate);
                billingCmd.Parameters.AddWithValue("@MonthlyRentRate", monthlyRentRate);
                billingCmd.Parameters.AddWithValue("@ElectricBill", 0);
                billingCmd.Parameters.AddWithValue("@WaterBill", 0);

                billingCmd.ExecuteNonQuery();
            }
        }

        private void InsertReportBalance(SqlConnection cnn, int tenantID)
        {
            string balanceQuery = "INSERT INTO ReportOfBalance (TenantID, DormUnit, MonthlyRentRate, ElectricBill, WaterBill, Paid, TotalOutstandingBalance, LastPayment) " +
                                  "VALUES (@TenantID, @DormUnit, @MonthlyRentRate, @ElectricBill, @WaterBill, @Paid, @TotalOutstandingBalance, @LastPayment)";

            decimal monthlyRentRate = GetMonthlyRentRate(cnn, int.Parse(cbRoom.Text.Trim()));
            DateTime lastPayment = DateTime.Now;

            using (SqlCommand balanceCmd = new SqlCommand(balanceQuery, cnn))
            {
                balanceCmd.Parameters.AddWithValue("@TenantID", tenantID);
                balanceCmd.Parameters.AddWithValue("@DormUnit", int.Parse(cbRoom.Text.Trim()));
                balanceCmd.Parameters.AddWithValue("@MonthlyRentRate", monthlyRentRate);
                balanceCmd.Parameters.AddWithValue("@ElectricBill", 0);
                balanceCmd.Parameters.AddWithValue("@WaterBill", 0);
                balanceCmd.Parameters.AddWithValue("@Paid", 0);
                balanceCmd.Parameters.AddWithValue("@TotalOutstandingBalance", 0);
                balanceCmd.Parameters.AddWithValue("@LastPayment", lastPayment);

                balanceCmd.ExecuteNonQuery();
            }
        }

        private decimal GetMonthlyRentRate(SqlConnection cnn, int dormUnit)
        {
            string rentQuery = "SELECT MonthlyRentRate FROM DormList WHERE DormUnit = @DormUnit";

            using (SqlCommand rentCmd = new SqlCommand(rentQuery, cnn))
            {
                rentCmd.Parameters.AddWithValue("@DormUnit", dormUnit);
                return (decimal)rentCmd.ExecuteScalar();
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
