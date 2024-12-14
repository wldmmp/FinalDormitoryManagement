using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DormitoryManagement
{
    public partial class DashboardForm : Form
    {
        public static DashboardForm Instance;
        public Label room;
        public Label dormUnit;
        public Label CashtoCollect;
        public DashboardForm()
        {
            InitializeComponent();
            Instance = this;
            room = lblRoom;
            dormUnit = lblDormUnit;
            
        }
        //counting all dorms
        public void UpdateRoomCount(string roomCount)
        {
            lblDormUnit.Text = roomCount;
        }

        public void UpdateOccupiedCount(string dormunit)
        {
            lblRoom.Text = dormunit;
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.lblTimeAndDate.Text = dateTime.ToString();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            DormUnit.Instance.UpdateDormUnitCountInDashboard();
            OccupiedRoom.Instance.UpdateOccupiedCountInDashboard();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to exit program?", "Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblTimeAndDate_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
