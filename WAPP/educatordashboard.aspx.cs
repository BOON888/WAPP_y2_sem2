using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class Educator_dashboard : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
        private int _educatorId = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ✅ Must be logged in as educator
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString().ToLower() != "educator")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                // ✅ Load EducatorID if not already set
                if (Session["EducatorID"] == null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT Id FROM Educator WHERE UserId = @uid", conn);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            Session["EducatorID"] = Convert.ToInt32(result);
                            Session["EducatorName"] = Session["FullName"];
                        }
                        else
                        {
                            // optional: create educator profile if not found
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

                // ✅ Use the real EducatorID now
                _educatorId = Convert.ToInt32(Session["EducatorID"]);
                lblEducatorName.Text = Session["FullName"]?.ToString() ?? "Educator";

                // Load stats and courses
                BindDashboardStats();
                BindCourses();
            }

        }
        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string courseId = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(courseId)) return;

            if (e.CommandName == "View")
            {
                // Navigate to the new EducatorCourseStudents page
                Response.Redirect($"~/EducatorCourseStudents.aspx?CourseId={Server.UrlEncode(courseId)}");
            }
            else if (e.CommandName == "Edit")
            {
                // Keep your original Edit logic
                Response.Redirect($"~/EditCourse.aspx?id={Server.UrlEncode(courseId)}");
            }
        }


        protected void btnCreateCourse_Click(object sender, EventArgs e)
        {
            Session["NewCourseId"] = null; // Clear any leftover draft ID
            Response.Redirect("~/CreateCourse.aspx");
        }

        private int LookupEducatorIdByUserId(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Id FROM educator WHERE userId = @uid", conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    var res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
                }
            }
            return -1;
        }

        private void BindDashboardStats()
        {
            int coursesCreated = 0;
            int totalStudents = 0;
            int courseCompletions = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // courses created
                using (SqlCommand c1 = new SqlCommand("SELECT COUNT(*) FROM course WHERE educatorId = @eid", conn))
                {
                    c1.Parameters.AddWithValue("@eid", _educatorId);
                    coursesCreated = Convert.ToInt32(c1.ExecuteScalar());
                }

                // distinct students enrolled in any of educator's courses
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

                // completions
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

            lblCoursesCreated.Text = coursesCreated.ToString();
            lblTotalStudents.Text = totalStudents.ToString();
            lblCourseCompletions.Text = courseCompletions.ToString();
        }

        private void BindCourses()
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

            rptCourses.DataSource = dt;
            rptCourses.DataBind();
        }
    }
}
