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
    public partial class LoginFrame : Form
    {
        public LoginFrame()
        {
            InitializeComponent();
        }

     

        
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                cnn.Open();
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                string query = "SELECT * FROM [User] WHERE Username = @Username AND Password = @Password";
                SqlDataAdapter sqlDA = new SqlDataAdapter(query, cnn);

                sqlDA.SelectCommand.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                sqlDA.SelectCommand.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;

                DataTable dt = new DataTable();
                sqlDA.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Log in Successfully!");
                    this.Hide();
                    MainFrame mainFrame = new MainFrame();
                    mainFrame.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                    txtUsername.Clear();
                    txtPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void LoginFrame_Load(object sender, EventArgs e)
        {
      
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to exit program?", "Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void kryptonPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
