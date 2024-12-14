using Bunifu.UI.WinForms;
using Newtonsoft.Json.Bson;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace DormitoryManagement
{
    public partial class SendingForm : Form
    {
        private string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

        public static string DueDate;
        public static double AmountDue;
        public static string tenantName;
        public static int tenantID;

        // Load tenant details based on email
        public void LoadTenantName()
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();
                    string checkBalanceQuery = @"
                        SELECT Name, TenantID 
                        FROM Tenant 
                        WHERE Email = @Email";

                    using (SqlCommand checkCmd = new SqlCommand(checkBalanceQuery, cnn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", cbEmail.Text.Trim());
                        SqlDataReader reader = checkCmd.ExecuteReader();

                        if (reader.Read())
                        {
                            tenantName = Convert.ToString(reader["Name"]);
                            tenantID = Convert.ToInt32(reader["TenantID"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching tenant name: " + ex.Message);
                }
            }
        }

        // Load due date for the tenant
        public void LoadDueDate()
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();
                    string checkBalanceQuery = @"
                        SELECT DueDate 
                        FROM [Billing] 
                        WHERE TenantID = @TenantID";

                    using (SqlCommand checkCmd = new SqlCommand(checkBalanceQuery, cnn))
                    {
                        checkCmd.Parameters.AddWithValue("@TenantID", tenantID);
                        var result = checkCmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            DueDate = Convert.ToString(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching due date: " + ex.Message);
                }
            }
        }

        // Load the amount due for the tenant
        public void LoadAmountDue()
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();
                    string checkBalanceQuery = @"
                        SELECT TotalOutstandingBalance 
                        FROM [ReportOfBalance] 
                        WHERE TenantID = @TenantID";

                    using (SqlCommand checkCmd = new SqlCommand(checkBalanceQuery, cnn))
                    {
                        checkCmd.Parameters.AddWithValue("@TenantID", tenantID);
                        var result = checkCmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            AmountDue = Convert.ToDouble(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching outstanding balance: " + ex.Message);
                }
            }
        }

        // Confirmation message for payment receipt
        public void BillsConfirmation()
        {
            LoadTenantName();
            string message = "Dear " + tenantName + "\r\n\r\nI hope this message finds you well. I am confirming that we have received your payment for your utility bill and rent." +
                "\r\n\r\nThank you for your prompt payment. If you have any questions or need further assistance, please don’t hesitate to contact us.\r\n\r\n" +
                "Best regards,\r\n" +
                "– Owner";
            txtMessage.Text = message;
        }

        // Reminder message for outstanding bill
        public void TenantBills()
        {
            LoadDueDate();
            LoadAmountDue();
            LoadTenantName();
            string message = "Dear " + tenantName + "," + "\r\n\r\nI hope this message finds you well. " +
                "This is a friendly reminder regarding your outstanding utility bill balance. " +
                "Please make the payment as soon as possible to avoid any disruption in service." +
                "\r\n\r\nPlease find the payment details below:\r\n" +
                "Amount Due: " + AmountDue.ToString("C2") + // Formatted to show currency
                "\r\n" +
                "Due Date: " + DueDate +
                "\r\n\r\n" +
                "If you have any questions or need assistance, feel free to contact us.\r\n" +
                "Thank you for your prompt attention to this matter.\r\nBest regards,\r\n\r\n- Owner";
            txtMessage.Text = message;
        }

        // Load email addresses of tenants into ComboBox
        public void LoadEmails()
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();
                    string query = "SELECT Email FROM Tenant WHERE Email IS NOT NULL;";
                    SqlCommand cmd = new SqlCommand(query, cnn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    cbEmail.Items.Clear();

                    while (reader.Read())
                    {
                        cbEmail.Items.Add(reader["Email"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching emails: " + ex.Message);
                }
            }
        }

        // SendingForm constructor
        public SendingForm()
        {
            InitializeComponent();
        }

        // Cancel button click (empty for now)
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // You can implement functionality here if needed
        }

        // Send email button click
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // Set up email details
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                string to = cbEmail.Text.Trim();
                string body = txtMessage.Text.Trim();

                msg.From = new MailAddress("dormitorymanagement5@gmail.com");
                msg.To.Add(to);
                msg.ReplyToList.Add(new MailAddress(to));

                // Handle email subject based on selected index
                if (comboBox1.SelectedIndex == 0)
                {
                    msg.Subject = "Payment Confirmation for Utility Bill";
                }
                else
                {
                    msg.Subject = "Reminder: Outstanding Utility Bill Balance";
                }

                msg.Body = body;

                // SMTP configuration
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("dormitorymanagement5@gmail.com", "vtaa cpdm fjqv nbxq"); // Replace with secure password handling
                smtp.EnableSsl = true;

                // Send the email
                smtp.Send(msg);
                MessageBox.Show("Email sent successfully!");
            }
            catch (Exception ex)
            {
                // Handle errors
                MessageBox.Show("Error sending email: " + ex.Message);
            }
        }

        // Form load event to load emails
        private void SendingForm_Load(object sender, EventArgs e)
        {
            LoadEmails();
        }

        // ComboBox selection changed to update message
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                BillsConfirmation();
            }
            else
            {
                TenantBills();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
