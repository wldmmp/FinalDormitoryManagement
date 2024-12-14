using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DormitoryManagement
{
    internal class OccupiedRoom
    {
        // Singleton pattern for the DormUnit instance
        private static OccupiedRoom _instance;
        public static OccupiedRoom Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OccupiedRoom();
                }
                return _instance;
            }
        }

        private string connectionString = "Data Source=JUSTINHOWARD\\SQLEXPRESS; Initial Catalog=DBDormitoryManagement; User ID=JUSTINHOWARD\\kyle; Trusted_Connection=true";

        // Private constructor to prevent direct instantiation
        private OccupiedRoom() { }

        // Method to get the count of occupied rooms
        public int GetOccupiedRoomCount()
        {
            int occupiedRooms = 0;

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();

                    // Query to count occupied rooms
                    string query = @"
                        SELECT COUNT(DISTINCT t.DormUnit) AS OccupiedRooms
                        FROM Tenant t;";

                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        occupiedRooms = (int)cmd.ExecuteScalar();
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Database error: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred: " + ex.Message);
                }
            }

            return occupiedRooms;
        }

        // Method to update the room count in DashboardForm
        public void UpdateOccupiedCountInDashboard()
        {
            int occupiedRooms = GetOccupiedRoomCount();
            string occupiedCount = occupiedRooms.ToString();
            DashboardForm.Instance.UpdateOccupiedCount(occupiedCount);
        }
    }
}