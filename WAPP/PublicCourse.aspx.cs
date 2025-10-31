using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class PublicCourse : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                if (Session["UserId"] == null || Session["Role"]?.ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                string keyword = Request.QueryString["keyword"];
                if (!string.IsNullOrEmpty(keyword))
                {
                    txtSearch.Text = keyword;
                    LoadCourses(keyword);
                }
                else
                {
                    LoadCourses();
                }
            }
        }

        private void LoadCourses(string keyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT c.Id, c.Title, e.EducationQualification AS EducatorName
                    FROM Course c 
                    JOIN Educator e ON c.EducatorId = e.Id
                    WHERE c.CourseType = 'Public'";

                if (!string.IsNullOrEmpty(keyword))
                    query += " AND (c.Title LIKE @kw OR e.EducationQualification LIKE @kw)";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(keyword))
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptPublicCourses.DataSource = dt;
                rptPublicCourses.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            LoadCourses(keyword);
        }

        protected void rptPublicCourses_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "StartCourse")
            {
                int studentId = Convert.ToInt32(Session["StudentID"]);
                int courseId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    
                    SqlCommand checkExist = new SqlCommand(
                        "SELECT COUNT(*) FROM StudentCourseProgress WHERE StudentId=@sid AND CourseId=@cid", conn);
                    checkExist.Parameters.AddWithValue("@sid", studentId);
                    checkExist.Parameters.AddWithValue("@cid", courseId);

                    int exists = Convert.ToInt32(checkExist.ExecuteScalar());
                    if (exists > 0)
                    {
                        Response.Write("<script>alert('You have already joined this course!');</script>");
                        return;
                    }

                    
                    SqlCommand enroll = new SqlCommand(
                        "INSERT INTO StudentCourseProgress (StudentId, CourseId, Status) VALUES (@sid, @cid, 'In Progress')", conn);
                    enroll.Parameters.AddWithValue("@sid", studentId);
                    enroll.Parameters.AddWithValue("@cid", courseId);
                    enroll.ExecuteNonQuery();

                    
                    Response.Write("<script>alert('Course started successfully!');window.location='StudentCourseContent.aspx?courseId=" + courseId + "';</script>");
                }
            }
        }
    }
}
