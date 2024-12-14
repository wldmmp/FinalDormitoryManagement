using System;
using System.Windows.Forms;

namespace DormitoryManagement
{
    public partial class MainFrame : Form
    {
        public MainFrame()
        {
            InitializeComponent();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainFrame_Load(object sender, EventArgs e)
        {
            DashboardForm frm = new DashboardForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            AboutForm frm = new AboutForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void btnTenant_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            TenantForm frm = new TenantForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            BillingForm frm = new BillingForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            SendingForm frm = new SendingForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            ReportForm frm = new ReportForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Do you want to logout?", "Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Dispose();
                LoginFrame loginFrame = new LoginFrame();
                loginFrame.Show();
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnDorm_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            DormForm frm = new DormForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            DashboardForm frm = new DashboardForm();
            frm.TopLevel = false;
            mainpanel.Controls.Add(frm);
            frm.Show();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
