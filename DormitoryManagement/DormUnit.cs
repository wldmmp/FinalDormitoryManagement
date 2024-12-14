using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DormitoryManagement
{
    internal class DormUnit
    {
        // Singleton pattern for the DormUnit instance
        private static DormUnit _instance;
        public static DormUnit Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DormUnit();
                }
                return _instance;
            }
        }

        private string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

        // Private constructor to prevent direct instantiation
        private DormUnit() { }

        // Method to get available dorm count
        public int GetAvailableDormCount()
        {
            int availableDorms = 0;

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    // Query to count available dorms
                    string query = @"
                        SELECT COUNT(*) AS TotalRooms
                        FROM DormList;";

                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        availableDorms = (int)cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }

            return availableDorms;
        }

        // Method to update the room count in DashboardForm
        public void UpdateDormUnitCountInDashboard()
        {
            int availableDorms = GetAvailableDormCount();
            string avail = availableDorms.ToString();
            DashboardForm.Instance.UpdateRoomCount(avail);
        }
    }
}
