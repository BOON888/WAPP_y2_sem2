using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class EducatorCourseStudents : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["CourseId"] == null)
                {
                    Response.Redirect("Educator_dashboard.aspx");
                    return;
                }

                int courseId = Convert.ToInt32(Request.QueryString["CourseId"]);
                LoadCourseTitle(courseId);
                LoadStudents(courseId, "Incomplete");
                lblStatusMessage.Text = "Showing students with incomplete progress.";
            }
        }

        private void LoadCourseTitle(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Title FROM Course WHERE Id=@cid", conn);
                cmd.Parameters.AddWithValue("@cid", courseId);
                lblCourseTitle.Text = cmd.ExecuteScalar()?.ToString() ?? "Untitled Course";
            }
        }

        private void LoadStudents(int courseId, string statusFilter)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT u.FullName, u.Email, 
                           ISNULL(scp.Status, 'Incomplete') AS Status
                    FROM StudentCourseProgress scp
                    JOIN Student s ON scp.StudentId = s.Id
                    JOIN Users u ON s.UserId = u.Id
                    WHERE scp.CourseId = @cid";

                if (statusFilter == "Completed")
                    sql += " AND scp.Status = 'Completed'";
                else
                    sql += " AND (scp.Status IS NULL OR scp.Status <> 'Completed')";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@cid", courseId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptStudents.DataSource = dt;
                rptStudents.DataBind();

                lblNoData.Visible = dt.Rows.Count == 0;
                lblNoData.Text = dt.Rows.Count == 0
                    ? "No students found for this category."
                    : "";
            }
        }

        protected void btnIncomplete_Click(object sender, EventArgs e)
        {
            btnIncomplete.CssClass = "tab-button active";
            btnComplete.CssClass = "tab-button";
            int courseId = Convert.ToInt32(Request.QueryString["CourseId"]);
            LoadStudents(courseId, "Incomplete");
            lblStatusMessage.Text = "Showing students with incomplete progress.";
        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            btnIncomplete.CssClass = "tab-button";
            btnComplete.CssClass = "tab-button active";
            int courseId = Convert.ToInt32(Request.QueryString["CourseId"]);
            LoadStudents(courseId, "Completed");
            lblStatusMessage.Text = "Showing students who have completed this course.";
        }
    }
}
