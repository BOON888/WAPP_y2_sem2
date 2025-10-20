using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCourses();
            }
        }

        private void LoadCourses()
        {
            // 🔹 Replace with your actual connection string name in Web.config
            string connStr = ConfigurationManager.ConnectionStrings["YourConnectionStringName"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT CourseID, CourseName, Instructor, LessonCount, CourseImage FROM Courses";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                RepeaterCourses.DataSource = dt;
                RepeaterCourses.DataBind();
            }
        }

        protected void RepeaterCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string courseId = e.CommandArgument.ToString();

            if (e.CommandName == "View")
            {
                // Redirect to a course details page
                Response.Redirect("CourseDetails.aspx?id=" + courseId);
            }
            else if (e.CommandName == "Delete")
            {
                DeleteCourse(courseId);
                LoadCourses(); // refresh list after deletion
            }
        }

        private void DeleteCourse(string courseId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["YourConnectionStringName"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string deleteQuery = "DELETE FROM Courses WHERE CourseID = @CourseID";
                SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
