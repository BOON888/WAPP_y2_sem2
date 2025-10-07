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
                // If your login sets these sessions, use them; else fallback demo
                if (Session["EducatorID"] != null)
                    int.TryParse(Session["EducatorID"].ToString(), out _educatorId);
                else if (Session["UserID"] != null)
                {
                    int userId;
                    if (int.TryParse(Session["UserID"].ToString(), out userId))
                        _educatorId = LookupEducatorIdByUserId(userId);
                }

                if (_educatorId <= 0)
                {
                    _educatorId = 3000;
                    Session["EducatorID"] = _educatorId;
                    Session["EducatorName"] = "Educator";
                }

                lblEducatorName.Text = Session["EducatorName"] != null ? Session["EducatorName"].ToString() : "Educator";

                BindDashboardStats();
                BindCourses();
            }
        }

        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string courseId = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(courseId)) return;

            if (e.CommandName == "View")
                Response.Redirect($"~/ViewCourse.aspx?id={Server.UrlEncode(courseId)}");
            else if (e.CommandName == "Edit")
                Response.Redirect($"~/EditCourse.aspx?id={Server.UrlEncode(courseId)}");
        }

        protected void btnCreateCourse_Click(object sender, EventArgs e)
        {
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
