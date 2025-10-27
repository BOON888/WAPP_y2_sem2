using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls; // For RepeaterCommandEventArgs

namespace WAPP
{
    public partial class StudentCourseContent : System.Web.UI.Page
    {
        private string connString => ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ✅ Authorization check
                if (Session["UserId"] == null || Session["Role"]?.ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                // ✅ Course ID validation
                string idParam = Request.QueryString["courseId"];
                if (string.IsNullOrEmpty(idParam) || !int.TryParse(idParam, out int courseId))
                {
                    lblError.Text = "⚠️ Invalid or missing course ID.";
                    lblError.Visible = true;
                    return;
                }

                Session["SelectedCourseId"] = courseId;

                // ✅ Load all data
                LoadCourseInfo(courseId);
                LoadLessons(courseId);
                LoadQuizzes(courseId);
            }

            // Ensure handler is bound on every load
            rptQuizzes.ItemCommand += rptQuizzes_ItemCommand;
            
        }

        // =============================
        //  COURSE INFORMATION
        // =============================
        private void LoadCourseInfo(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT c.Title, c.CourseType, 
                           ISNULL(c.Coin, 0) AS Coin, 
                           c.Status, e.EducationQualification AS EducatorName
                    FROM Course c
                    JOIN Educator e ON c.EducatorId = e.Id
                    WHERE c.Id = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", courseId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblCourseTitle.Text = dr["Title"].ToString();
                    lblEducator.Text = dr["EducatorName"].ToString();
                    lblCourseType.Text = dr["CourseType"].ToString();
                    lblStatus.Text = dr["Status"].ToString();

                    // Change "Coins Needed"
                    int coins = dr["Coin"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Coin"]);
                    lblCoin.Text = coins.ToString();
                }
                else
                {
                    lblError.Text = "⚠️ Course not found.";
                    lblError.Visible = true;
                }

                dr.Close();
            }
        }

        // =============================
        //  LESSON LIST
        // =============================
        private void LoadLessons(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    // ⭐ Make sure 'Id' is included here
                    @"SELECT Id, LessonNumber, LessonTitle, ContentType, ContentFilePath
              FROM Lesson WHERE CourseId=@cid ORDER BY LessonNumber ASC", conn);
                da.SelectCommand.Parameters.AddWithValue("@cid", courseId);

                DataTable dt = new DataTable();
                da.Fill(dt);
                rptLessons.DataSource = dt;
                rptLessons.DataBind();
            }
        }

        // =============================
        //  QUIZ LIST + PROGRESS BAR
        // =============================
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
                string query = @"
                    SELECT q.Id, q.QuizRewardCoins, l.LessonTitle, l.LessonNumber,
                           ISNULL(p.Status, 'NotStarted') AS Status
                    FROM Quiz q
                    JOIN Lesson l ON q.LessonId = l.Id
                    LEFT JOIN StudentQuizProgress p ON p.QuizId = q.Id AND p.StudentId = @sid
                    WHERE l.CourseId = @cid
                    ORDER BY l.LessonNumber ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cid", courseId);
                cmd.Parameters.AddWithValue("@sid", studentId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Add temporary fields for UI
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
                        // Completed quiz
                        row["ButtonText"] = "Completed";
                        row["ButtonClass"] = "btn-view btn-disabled";
                        row["ButtonEnabled"] = false;
                        completedCount++;
                        previousQuizCompleted = true;
                    }
                    else if (previousQuizCompleted)
                    {
                        // Next available quiz
                        row["ButtonText"] = "Start Quiz";
                        row["ButtonClass"] = "btn-view";
                        row["ButtonEnabled"] = true;
                        previousQuizCompleted = false;
                    }
                    else
                    {
                        // Locked quiz
                        row["ButtonText"] = "Locked 🔒";
                        row["ButtonClass"] = "btn-view btn-disabled";
                        row["ButtonEnabled"] = false;
                    }
                }

                // Calculate course progress
                int total = dt.Rows.Count;
                int percent = (total == 0) ? 0 : (int)((completedCount / (double)total) * 100);

                string progressClass;
                if (percent < 30)
                    progressClass = "progress-low";
                else if (percent < 70)
                    progressClass = "progress-medium";
                else
                    progressClass = "progress-high";

                lblProgressPercent.Text = percent + "%";
                progressFill.Style["width"] = percent + "%";
                progressFill.InnerText = percent + "%";
                progressFill.Attributes["class"] = $"progress-fill {progressClass}";

                rptQuizzes.DataSource = dt;
                rptQuizzes.DataBind();
            }
        }

        // =============================
        //  BUTTON HANDLERS
        // =============================

        // 🔹 Back to Dashboard
        protected void btnBackDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("StudentDashboard.aspx");
        }

        // 🔹 Quiz Click Event
        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "OpenQuiz")
            {
                string quizId = e.CommandArgument.ToString();
                Response.Redirect($"StudentQuiz.aspx?quizId={quizId}");
            }
        }

        


    }
}
