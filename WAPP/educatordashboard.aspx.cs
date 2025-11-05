using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class Educator_dashboard : System.Web.UI.Page
    {
        // Connect to the database
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        // Stores the educator’s ID after login
        private int _educatorId = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Ensure user is logged in as an educator
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString().ToLower() != "educator")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                // Load educator ID if not already in session
                if (Session["EducatorID"] == null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT Id FROM Educator WHERE UserId = @uid", conn);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        object result = cmd.ExecuteScalar();

                        // If educator exists, store ID in session
                        if (result != null)
                        {
                            Session["EducatorID"] = Convert.ToInt32(result);
                            Session["EducatorName"] = Session["FullName"];
                        }
                        else
                        {
                            // If educator not found, create a new record
                            SqlCommand insert = new SqlCommand(
                                "INSERT INTO Educator (UserId, EducationQualification) VALUES (@uid, 'Not Set'); SELECT SCOPE_IDENTITY();",
                                conn
                            );

                            insert.Parameters.AddWithValue("@uid", userId);
                            object newId = insert.ExecuteScalar();

                            Session["EducatorID"] = Convert.ToInt32(newId);
                            Session["EducatorName"] = Session["FullName"];
                        }
                    }
                }

                // Set educator ID and display their name
                _educatorId = Convert.ToInt32(Session["EducatorID"]);
                lblEducatorName.Text = Session["FullName"]?.ToString() ?? "Educator";

                // Load dashboard data
                BindDashboardStats();
                BindCourses();
            }

        }
        
        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e) // Handles View and Edit button actions in the course list
        {
            string courseId = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(courseId)) return;

            if (e.CommandName == "View")
            {
                // Go to course student progress page
                Response.Redirect($"~/EducatorCourseStudents.aspx?CourseId={Server.UrlEncode(courseId)}");
            }
            else if (e.CommandName == "Edit")
            {
                // Go to edit course page
                Response.Redirect($"~/EditCourse.aspx?id={Server.UrlEncode(courseId)}");
            }
        }

        protected void btnCreateCourse_Click(object sender, EventArgs e) // Redirects to create course page

        {
            Session["NewCourseId"] = null; // Clear any leftover draft ID
            Response.Redirect("~/CreateCourse.aspx");
        }

        private void BindDashboardStats() // Loads dashboard statistics: total courses, students, and completions

        {
            int coursesCreated = 0;
            int totalStudents = 0;
            int courseCompletions = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Count total courses created
                using (SqlCommand c1 = new SqlCommand("SELECT COUNT(*) FROM course WHERE educatorId = @eid", conn))
                {
                    c1.Parameters.AddWithValue("@eid", _educatorId);
                    coursesCreated = Convert.ToInt32(c1.ExecuteScalar());
                }

                // Count students enrolled
                using (SqlCommand c2 = new SqlCommand(
                    @"SELECT COUNT(DISTINCT scp.studentId)
                      FROM studentCourseProgress scp
                      JOIN course c ON scp.courseId = c.id
                      WHERE c.educatorId = @eid", conn))
                {
                    c2.Parameters.AddWithValue("@eid", _educatorId);
                    var val = c2.ExecuteScalar();
                    if (val != null && val != DBNull.Value) totalStudents = Convert.ToInt32(val);
                }

                // Count total student course completions
                using (SqlCommand c3 = new SqlCommand(
                    @"SELECT COUNT(*) 
                      FROM studentCourseProgress scp
                      JOIN course c ON scp.courseId = c.id
                      WHERE c.educatorId = @eid AND scp.status = 'Completed'", conn))
                {
                    c3.Parameters.AddWithValue("@eid", _educatorId);
                    var val = c3.ExecuteScalar();
                    if (val != null && val != DBNull.Value) courseCompletions = Convert.ToInt32(val);
                }
            }

            // Display stats on the dashboard
            lblCoursesCreated.Text = coursesCreated.ToString();
            lblTotalStudents.Text = totalStudents.ToString();
            lblCourseCompletions.Text = courseCompletions.ToString();
        }

        private void BindCourses() // Loads educator's courses into the repeater (list view)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string sql =
                    @"SELECT c.id, c.title, c.courseType,
                        ISNULL((SELECT COUNT(*) FROM lesson l WHERE l.courseId = c.id),0) AS LessonCount,
                        ISNULL((SELECT COUNT(*) FROM studentCourseProgress scp WHERE scp.courseId = c.id),0) AS StudentCount
                      FROM course c
                      WHERE c.educatorId = @eid
                      ORDER BY c.title";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@eid", _educatorId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            // Bind the data to the Repeater for display
            rptCourses.DataSource = dt;
            rptCourses.DataBind();
        }
    }
}
