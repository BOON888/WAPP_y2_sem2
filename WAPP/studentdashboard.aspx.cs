using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class StudentDashboard : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["StudentID"] == null)
                {
                    Session["StudentID"] = 2000;              // matches student.id   //temp
                    Session["StudentName"] = "Alice Tan";     // matches users.fullName  //temp
                    //Response.Redirect("Login.aspx");
                    //return;
                }

                lblStudentName.Text = Session["StudentName"].ToString();
                LoadStudentStats();
                LoadCourses();
            }
        }

        private void LoadStudentStats()
        {
            int studentId = Convert.ToInt32(Session["StudentID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT Coins, BadgesEarned FROM Student WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblCoins.Text = dr["Coins"].ToString();
                    lblBadges.Text = dr["BadgesEarned"].ToString();
                }
                dr.Close();

                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM StudentCourseProgress WHERE StudentId=@id AND Status='Completed'", conn);
                cmd2.Parameters.AddWithValue("@id", studentId);
                lblCoursesCompleted.Text = cmd2.ExecuteScalar().ToString();
            }
        }

        private void LoadCourses()
        {
            int studentId = Convert.ToInt32(Session["StudentID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 🔹 Incomplete Courses
                SqlDataAdapter daIncomplete = new SqlDataAdapter(
                    @"SELECT c.Id, c.Title, e.EducationQualification AS EducatorName
                      FROM Course c 
                      JOIN Educator e ON c.EducatorId=e.Id
                      JOIN StudentCourseProgress p ON p.CourseId=c.Id
                      WHERE p.StudentId=@sid AND p.Status='In Progress'", conn);
                daIncomplete.SelectCommand.Parameters.AddWithValue("@sid", studentId);
                DataTable dtIncomplete = new DataTable();
                daIncomplete.Fill(dtIncomplete);
                rptIncompleteCourses.DataSource = dtIncomplete;
                rptIncompleteCourses.DataBind();

                // Public Courses
                SqlDataAdapter daPublic = new SqlDataAdapter(
                    "SELECT c.Id, c.Title, e.EducationQualification AS EducatorName FROM Course c JOIN Educator e ON c.EducatorId=e.Id WHERE c.CourseType='Public'", conn);
                DataTable dtPublic = new DataTable();
                daPublic.Fill(dtPublic);
                rptPublicCourses.DataSource = dtPublic;
                rptPublicCourses.DataBind();

                // Private Courses
                SqlDataAdapter daPrivate = new SqlDataAdapter(
                    "SELECT c.Id, c.Title, e.EducationQualification AS EducatorName, 50 AS CoinReward FROM Course c JOIN Educator e ON c.EducatorId=e.Id WHERE c.CourseType='Private'", conn);
                DataTable dtPrivate = new DataTable();
                daPrivate.Fill(dtPrivate);
                rptPrivateCourses.DataSource = dtPrivate;
                rptPrivateCourses.DataBind();

                // Completed Courses
                SqlDataAdapter daCompleted = new SqlDataAdapter(
                    @"SELECT c.Id, c.Title, e.EducationQualification AS EducatorName
                      FROM Course c 
                      JOIN Educator e ON c.EducatorId=e.Id
                      JOIN StudentCourseProgress p ON p.CourseId=c.Id
                      WHERE p.StudentId=@sid AND p.Status='Completed'", conn);
                daCompleted.SelectCommand.Parameters.AddWithValue("@sid", studentId);
                DataTable dtCompleted = new DataTable();
                daCompleted.Fill(dtCompleted);
                rptCompletedCourses.DataSource = dtCompleted;
                rptCompletedCourses.DataBind();
            }
        }

        protected void rptPrivateCourses_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SubscribeCourse")
            {
                int studentId = Convert.ToInt32(Session["StudentID"]);
                int courseId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand getCoins = new SqlCommand("SELECT Coins FROM Student WHERE Id=@id", conn);
                    getCoins.Parameters.AddWithValue("@id", studentId);
                    int coins = Convert.ToInt32(getCoins.ExecuteScalar());

                    int cost = 50;
                    if (coins >= cost)
                    {
                        SqlCommand updateCoins = new SqlCommand("UPDATE Student SET Coins=Coins-@cost WHERE Id=@id", conn);
                        updateCoins.Parameters.AddWithValue("@cost", cost);
                        updateCoins.Parameters.AddWithValue("@id", studentId);
                        updateCoins.ExecuteNonQuery();

                        SqlCommand enroll = new SqlCommand(
                            "INSERT INTO StudentCourseProgress(StudentId, CourseId, Status) VALUES(@sid,@cid,'In Progress')", conn);
                        enroll.Parameters.AddWithValue("@sid", studentId);
                        enroll.Parameters.AddWithValue("@cid", courseId);
                        enroll.ExecuteNonQuery();

                        Response.Write("<script>alert('Successfully subscribed to the course!');window.location='StudentDashboard.aspx';</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Not enough coins to subscribe.');</script>");
                    }
                }
            }
        }
        protected void btnSearchPublic_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchPublic.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                Response.Redirect("SearchResults.aspx?type=public&keyword=" + Server.UrlEncode(keyword));
            }
        }

        protected void btnSearchPrivate_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchPrivate.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                Response.Redirect("SearchResults.aspx?type=private&keyword=" + Server.UrlEncode(keyword));
            }
        }
    }
}
