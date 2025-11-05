using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class EducatorCourseStudents : System.Web.UI.Page
    {
        // Connects to database
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If no CourseId is passed, go back to the dashboard
                if (Request.QueryString["CourseId"] == null)
                {
                    Response.Redirect("Educator_dashboard.aspx");
                    return;
                }
                // Get CourseId
                int courseId = Convert.ToInt32(Request.QueryString["CourseId"]);

                // Load the course title for display at the top
                LoadCourseTitle(courseId);

                // Load the list of students with "Incomplete" status
                LoadStudents(courseId, "Incomplete");

                // Set the status label to indicate which students are being shown
                lblStatusMessage.Text = "Showing students with incomplete progress.";
            }
        }

        private void LoadCourseTitle(int courseId)
        {
            // Gets the course title from the database using the CourseId
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Title FROM Course WHERE Id=@cid", conn);
                cmd.Parameters.AddWithValue("@cid", courseId);

                // If no title found, show "Untitled Course"
                lblCourseTitle.Text = cmd.ExecuteScalar()?.ToString() ?? "Untitled Course";
            }
        }

        private void LoadStudents(int courseId, string statusFilter)
        {
            // Loads student data (Completed or Incomplete) for the selected course
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Base SQL query: joins StudentCourseProgress, Student, and Users tables
                string sql = @"
                    SELECT u.FullName, u.Email, 
                           ISNULL(scp.Status, 'Incomplete') AS Status
                    FROM StudentCourseProgress scp
                    JOIN Student s ON scp.StudentId = s.Id
                    JOIN Users u ON s.UserId = u.Id
                    WHERE scp.CourseId = @cid";

                // Filter based on button clicked
                if (statusFilter == "Completed")
                    sql += " AND scp.Status = 'Completed'";
                else
                    sql += " AND (scp.Status IS NULL OR scp.Status <> 'Completed')";

                // Run the query and fill results into a DataTable
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@cid", courseId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Bind the student list to the Repeater (rptStudents)
                rptStudents.DataSource = dt;
                rptStudents.DataBind();

                // Show a message if no students found
                lblNoData.Visible = dt.Rows.Count == 0;
                lblNoData.Text = dt.Rows.Count == 0
                    ? "No students found for this category."
                    : "";
            }
        }

        protected void btnIncomplete_Click(object sender, EventArgs e)
        {
            // When "Incomplete" tab clicked:
            btnIncomplete.CssClass = "tab-button active";
            btnComplete.CssClass = "tab-button";

            int courseId = Convert.ToInt32(Request.QueryString["CourseId"]);

            // Reload students with Incomplete status
            LoadStudents(courseId, "Incomplete");
            lblStatusMessage.Text = "Showing students with incomplete progress.";
        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            // When "Completed" tab clicked:
            btnIncomplete.CssClass = "tab-button";
            btnComplete.CssClass = "tab-button active";

            int courseId = Convert.ToInt32(Request.QueryString["CourseId"]);

            // Reload students with Completed status
            LoadStudents(courseId, "Completed");
            lblStatusMessage.Text = "Showing students who have completed this course.";
        }
    }
}
