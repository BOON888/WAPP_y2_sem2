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

                // Replace line breaks with <br> tags for HTML display
                foreach (DataRow row in dt.Rows)
                {
                    if (row["AdContent"] != DBNull.Value)
                    {
                        row["AdContent"] = row["AdContent"].ToString()
                            .Replace("\r\n", "<br>")
                            .Replace("\n", "<br>")
                            .Replace("\r", "<br>");
                    }
                }

                rptAds.DataSource = dt;
                rptAds.DataBind();
            }

            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmdStudents = new SqlCommand("SELECT COUNT(*) FROM student", conn);
                int studentCount = (int)cmdStudents.ExecuteScalar();
                SqlCommand cmdEducators = new SqlCommand("SELECT COUNT(*) FROM educator", conn);
                int educatorCount = (int)cmdEducators.ExecuteScalar();
                SqlCommand cmdCourses = new SqlCommand("SELECT COUNT(*) FROM course", conn);
                int courseCount = (int)cmdCourses.ExecuteScalar();
                lblStudentCount.Text = studentCount.ToString("N0"); 
                lblEducatorCount.Text = educatorCount.ToString("N0");
                lblCourseCount.Text = courseCount.ToString("N0");
            }
        }
    }
}
