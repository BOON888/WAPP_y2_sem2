using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WAPP
{
    public partial class sing_in : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAdvertisements();
            }
        }

        private void LoadAdvertisements()
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT AdContent, AdImage FROM Advertisement WHERE GETDATE() BETWEEN StartDate AND EndDate";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptAds.DataSource = dt;
                rptAds.DataBind();
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SeaLearnerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Count students
                SqlCommand cmdStudents = new SqlCommand("SELECT COUNT(*) FROM student", conn);
                int studentCount = (int)cmdStudents.ExecuteScalar();

                // Count educators
                SqlCommand cmdEducators = new SqlCommand("SELECT COUNT(*) FROM educator", conn);
                int educatorCount = (int)cmdEducators.ExecuteScalar();

                // Count courses
                SqlCommand cmdCourses = new SqlCommand("SELECT COUNT(*) FROM course", conn);
                int courseCount = (int)cmdCourses.ExecuteScalar();

                // Display in labels
                lblStudentCount.Text = studentCount.ToString("N0"); // formatted with commas
                lblEducatorCount.Text = educatorCount.ToString("N0");
                lblCourseCount.Text = courseCount.ToString("N0");
            }
        }
    }
}
