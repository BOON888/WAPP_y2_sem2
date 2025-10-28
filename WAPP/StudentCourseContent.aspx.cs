using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class StudentCourseContent : System.Web.UI.Page
    {
        private string connString => ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
        private int ProgressPercent
        {
            get { return ViewState["ProgressPercent"] == null ? 0 : (int)ViewState["ProgressPercent"]; }
            set { ViewState["ProgressPercent"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null || Session["Role"]?.ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                string idParam = Request.QueryString["courseId"];
                if (string.IsNullOrEmpty(idParam) || !int.TryParse(idParam, out int courseId))
                {
                    lblError.Text = "Invalid or missing course ID.";
                    lblError.Visible = true;
                    return;
                }

                Session["SelectedCourseId"] = courseId;
                LoadCourseInfo(courseId);
                LoadLessons(courseId);
                LoadQuizzes(courseId);
            }

            rptQuizzes.ItemCommand += rptQuizzes_ItemCommand;
        }

        private void LoadCourseInfo(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                    SELECT c.Title, c.CourseType, ISNULL(c.Coin, 0) AS Coin, 
                           c.Status, e.EducationQualification AS EducatorName
                    FROM Course c
                    JOIN Educator e ON c.EducatorId = e.Id
                    WHERE c.Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", courseId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblCourseTitle.Text = dr["Title"].ToString();
                    lblEducator.Text = dr["EducatorName"].ToString();
                    lblCourseType.Text = dr["CourseType"].ToString();
                    lblStatus.Text = dr["Status"].ToString();
                    lblCoin.Text = dr["Coin"].ToString();
                }
                else
                {
                    lblError.Text = "Course not found.";
                    lblError.Visible = true;
                }
                dr.Close();
            }
        }

        private void LoadLessons(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT Id, LessonNumber, LessonTitle, ContentType, ContentFilePath 
                      FROM Lesson WHERE CourseId=@cid ORDER BY LessonNumber ASC", conn);
                da.SelectCommand.Parameters.AddWithValue("@cid", courseId);

                DataTable dt = new DataTable();
                da.Fill(dt);
                rptLessons.DataSource = dt;
                rptLessons.DataBind();
            }
        }

        private void LoadQuizzes(int courseId)
        {
            if (Session["StudentID"] == null || !int.TryParse(Session["StudentID"].ToString(), out int studentId))
            {
                Response.Redirect("sign_in.aspx");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                    SELECT q.Id, q.QuizRewardCoins, l.LessonTitle, l.LessonNumber,
                           ISNULL(p.Status, 'NotStarted') AS Status
                    FROM Quiz q
                    JOIN Lesson l ON q.LessonId = l.Id
                    LEFT JOIN StudentQuizProgress p ON p.QuizId = q.Id AND p.StudentId = @sid
                    WHERE l.CourseId = @cid
                    ORDER BY l.LessonNumber ASC", conn);
                cmd.Parameters.AddWithValue("@cid", courseId);
                cmd.Parameters.AddWithValue("@sid", studentId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("ButtonText", typeof(string));
                dt.Columns.Add("ButtonClass", typeof(string));
                dt.Columns.Add("ButtonEnabled", typeof(bool));

                bool previousQuizCompleted = true;
                int completedCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string status = row["Status"].ToString();
                    bool isCompleted = status.Equals("Completed", StringComparison.OrdinalIgnoreCase);

                    if (isCompleted)
                    {
                        row["ButtonText"] = "Completed";
                        row["ButtonClass"] = "btn-view btn-disabled";
                        row["ButtonEnabled"] = false;
                        completedCount++;
                        previousQuizCompleted = true;
                    }
                    else if (previousQuizCompleted)
                    {
                        row["ButtonText"] = "Start Quiz";
                        row["ButtonClass"] = "btn-view";
                        row["ButtonEnabled"] = true;
                        previousQuizCompleted = false;
                    }
                    else
                    {
                        row["ButtonText"] = "Locked";
                        row["ButtonClass"] = "btn-view btn-disabled";
                        row["ButtonEnabled"] = false;
                    }
                }

                int total = dt.Rows.Count;
                ProgressPercent = (total == 0) ? 0 : (int)((completedCount / (double)total) * 100);

                lblProgressPercent.Text = ProgressPercent + "%";
                progressFill.Style["width"] = ProgressPercent + "%";
                progressFill.InnerText = ProgressPercent + "%";

                btnCompleteCourse.Enabled = (ProgressPercent == 100);

                rptQuizzes.DataSource = dt;
                rptQuizzes.DataBind();
            }
        }

        protected void btnBackDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("StudentDashboard.aspx");
        }

        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "OpenQuiz")
            {
                string quizId = e.CommandArgument.ToString();
                Response.Redirect($"StudentQuiz.aspx?quizId={quizId}");
            }
        }

        //Complete Course Button Click
        protected void btnCompleteCourse_Click(object sender, EventArgs e)
        {
            // Ensure student session is valid
            if (Session["StudentID"] == null)
            {
                lblError.Text = "Session expired. Please log in again.";
                lblError.Visible = true;
                return;
            }

            int studentId = Convert.ToInt32(Session["StudentID"]);
            int courseId = Convert.ToInt32(Session["SelectedCourseId"]);

            // Check if course progress is 100%
            if (ProgressPercent < 100)
            {
                lblError.Text = "You must complete all quizzes before finishing the course.";
                lblError.Visible = true;
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Update StudentCourseProgress status + award badge
                string badgeTitle = "Distinction in " + lblCourseTitle.Text;

                SqlCommand updateCmd = new SqlCommand(@"
            UPDATE StudentCourseProgress
            SET Status = 'Completed', BadgeEarned = @badge
            WHERE StudentId = @sid AND CourseId = @cid", conn);

                updateCmd.Parameters.AddWithValue("@sid", studentId);
                updateCmd.Parameters.AddWithValue("@cid", courseId);
                updateCmd.Parameters.AddWithValue("@badge", badgeTitle);

                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Increment student's total badges
                    SqlCommand updateBadgeCount = new SqlCommand(@"
                UPDATE Student
                SET BadgesEarned = ISNULL(BadgesEarned, 0) + 1
                WHERE Id = @sid", conn);

                    updateBadgeCount.Parameters.AddWithValue("@sid", studentId);
                    updateBadgeCount.ExecuteNonQuery();

                    lblError.Visible = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        $"alert('Course completed! You earned the badge: {badgeTitle}'); window.location='StudentDashboard.aspx';", true);
                }
                else
                {
                    lblError.Text = "Could not update course completion. Please try again.";
                    lblError.Visible = true;
                }
            }
        }


    }
}
