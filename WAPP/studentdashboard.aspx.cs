using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class StudentDashboard : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Ensure valid student session
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                // Ensure StudentID exists
                if (Session["StudentID"] == null)
                {
                    LoadStudentID();
                }

                if (Session["StudentID"] == null)
                {
                    lblStudentName.Text = "Unknown Student";
                    lblCoins.Text = "0";
                    lblBadges.Text = "0";
                    lblCoursesCompleted.Text = "0";
                    return;
                }

                lblStudentName.Text = Session["FullName"]?.ToString() ?? "Unnamed";
                LoadStudentStats();
                LoadCourses();
            }
        }

        private void LoadStudentID()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id FROM Student WHERE UserId = @uid", conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    Session["StudentID"] = Convert.ToInt32(result);
                    Session["StudentName"] = Session["FullName"];
                }
            }
        }

        private void LoadStudentStats()
        {
            int studentId = Convert.ToInt32(Session["StudentID"]);

            using (SqlConnection conn = new SqlConnection(connString))
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

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // In Progress Courses
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

                // Public
                SqlDataAdapter daPublic = new SqlDataAdapter(
                    @"SELECT c.Id, c.Title, e.EducationQualification AS EducatorName 
                      FROM Course c 
                      JOIN Educator e ON c.EducatorId=e.Id 
                      WHERE c.CourseType='Public'", conn);
                DataTable dtPublic = new DataTable();
                daPublic.Fill(dtPublic);
                rptPublicCourses.DataSource = dtPublic;
                rptPublicCourses.DataBind();

                // Private
                SqlDataAdapter daPrivate = new SqlDataAdapter(
                    @"SELECT c.Id, c.Title, e.EducationQualification AS EducatorName, 50 AS CoinReward 
                      FROM Course c 
                      JOIN Educator e ON c.EducatorId=e.Id 
                      WHERE c.CourseType='Private'", conn);
                DataTable dtPrivate = new DataTable();
                daPrivate.Fill(dtPrivate);
                rptPrivateCourses.DataSource = dtPrivate;
                rptPrivateCourses.DataBind();

                // Completed
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

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    //Step 1: Check if already joined this course
                    SqlCommand checkExist = new SqlCommand(
                        "SELECT COUNT(*) FROM StudentCourseProgress WHERE StudentId=@sid AND CourseId=@cid", conn);
                    checkExist.Parameters.AddWithValue("@sid", studentId);
                    checkExist.Parameters.AddWithValue("@cid", courseId);

                    int exists = Convert.ToInt32(checkExist.ExecuteScalar());
                    if (exists > 0)
                    {
                        //Already joined
                        Response.Write("<script>alert('You have already joined this course!');</script>");
                        return;
                    }

                    //Step 2: Check current coins
                    SqlCommand getCoins = new SqlCommand("SELECT Coins FROM Student WHERE Id=@id", conn);
                    getCoins.Parameters.AddWithValue("@id", studentId);
                    int coins = Convert.ToInt32(getCoins.ExecuteScalar());

                    int cost = 50; // default cost per course
                    if (coins >= cost)
                    {
                        //Deduct coins
                        SqlCommand updateCoins = new SqlCommand("UPDATE Student SET Coins=Coins-@cost WHERE Id=@id", conn);
                        updateCoins.Parameters.AddWithValue("@cost", cost);
                        updateCoins.Parameters.AddWithValue("@id", studentId);
                        updateCoins.ExecuteNonQuery();

                        //Enroll course
                        SqlCommand enroll = new SqlCommand(
                            "INSERT INTO StudentCourseProgress (StudentId, CourseId, Status) VALUES (@sid, @cid, 'In Progress')", conn);
                        enroll.Parameters.AddWithValue("@sid", studentId);
                        enroll.Parameters.AddWithValue("@cid", courseId);
                        enroll.ExecuteNonQuery();

                        Response.Write("<script>alert('Successfully subscribed to the course!');window.location='StudentDashboard.aspx';</script>");
                    }
                    else
                    {
                        //Not enough coins
                        Response.Write("<script>alert('Not enough coins to subscribe.');</script>");
                    }
                }
            }
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

                    //Step 1: Check if already joined this course
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

                    //Step 2: Insert new progress record (auto join)
                    SqlCommand enroll = new SqlCommand(
                        "INSERT INTO StudentCourseProgress (StudentId, CourseId, Status) VALUES (@sid, @cid, 'In Progress')", conn);
                    enroll.Parameters.AddWithValue("@sid", studentId);
                    enroll.Parameters.AddWithValue("@cid", courseId);
                    enroll.ExecuteNonQuery();

                    //Step 3: Redirect to content page
                    Response.Write("<script>alert('Course started successfully!');window.location='StudentCourseContent.aspx?courseId=" + courseId + "';</script>");
                }
            }
        }

        protected void btnSearchPublic_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchPublic.Text.Trim();
            Response.Redirect("PublicCourse.aspx" + (!string.IsNullOrEmpty(keyword) ? "?keyword=" + Server.UrlEncode(keyword) : ""));
        }

        protected void btnSearchPrivate_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchPrivate.Text.Trim();
            Response.Redirect("PrivateCourse.aspx" + (!string.IsNullOrEmpty(keyword) ? "?keyword=" + Server.UrlEncode(keyword) : ""));
        }
    }
}
