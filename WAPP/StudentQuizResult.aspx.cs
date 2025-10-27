using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace WAPP
{
    public partial class StudentQuizResult : System.Web.UI.Page
    {
        private string connString => ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        // Use a private field for the mapped Student.Id (The actual FK value)
        private int studentTableId;

        // Use private fields to store parsed query string parameters
        private int quizId;
        private int score;
        private int total;
        private float passingGrade;

        // Controls are assumed to be defined in the .designer.cs
        protected HtmlGenericControl pnlResult;
        protected HtmlGenericControl statusIcon;
        protected Panel pnlReward;
        protected Button btnRetry;
        protected Button btnBackToCourse;
        protected Label lblQuizTitle;
        protected Label lblStatus;
        protected Label lblError;
        protected Literal litScore;
        protected Literal litPercentage;
        protected Literal litRewardMessage;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString().ToLower() != "student")
            {
                Response.Redirect("sign_in.aspx");
                return;
            }

            // --- CRITICAL FIX: MAP SESSION ID TO STUDENT TABLE ID ---
            int userId = Convert.ToInt32(Session["UserId"]);
            this.studentTableId = GetStudentIdFromUserId(userId); // <--- Use the MAPPED ID

            if (this.studentTableId <= 0)
            {
                lblError.Text = "⚠️ Security Error: Student record not found. Cannot process results.";
                lblError.Visible = true;
                return;
            }
            // --- END CRITICAL FIX ---

            if (!ValidateQueryParams())
            {
                if (pnlResult != null) pnlResult.Visible = false;
                lblError.Text = "⚠️ Invalid or missing quiz result data.";
                lblError.Visible = true;
                return;
            }

            if (!IsPostBack)
            {
                DisplayResult();
            }
        }

        private bool ValidateQueryParams()
        {
            if (!int.TryParse(Request.QueryString["quizId"], out quizId) ||
                !int.TryParse(Request.QueryString["score"], out score) ||
                !int.TryParse(Request.QueryString["total"], out total) ||
                !float.TryParse(Request.QueryString["passGrade"], out passingGrade))
            {
                return false;
            }
            return true;
        }

        // ============================================
        // Helper Functions (ID Mapping, Progress Check, Coin Logic)
        // ============================================

        // *** Function to map Users.Id (from session) to Student.Id (for FK) ***
        private int GetStudentIdFromUserId(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                // Query to find the Student.Id where the Student.UserId matches the Session ID
                string query = "SELECT Id FROM Student WHERE UserId = @userId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                object result = cmd.ExecuteScalar();

                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

        // This function is kept but NO LONGER USED in DisplayResult, as requested
        private bool CheckIfQuizPassedBefore(int qId, int sId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT 1 
                    FROM StudentQuizProgress 
                    WHERE StudentId = @sId AND QuizId = @qId AND Status = 'Completed'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sId", sId);
                cmd.Parameters.AddWithValue("@qId", qId);

                return cmd.ExecuteScalar() != null;
            }
        }

        private void AddCoinsToStudent(int studentId, int coins)
        {
            if (coins <= 0) return;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // 1. Define the command within its own using block
                using (SqlCommand cmd = new SqlCommand("UPDATE Student SET Coins = ISNULL(Coins,0) + @coins WHERE Id=@sid", conn))
                {
                    conn.Open();

                    // 2. Use the correct Student Id (which is the mapped ID)
                    cmd.Parameters.AddWithValue("@coins", coins);
                    cmd.Parameters.AddWithValue("@sid", studentId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============================================
        // Display Logic (MODIFIED for Unconditional Reward)
        // ============================================

        private void DisplayResult()
        {
            if (total == 0) return;

            double percentageScore = (score / (double)total) * 100;
            bool passed = percentageScore >= passingGrade;

            litScore.Text = $"{score}/{total}";
            litPercentage.Text = $"{Math.Round(percentageScore, 2)}%";

            int rewardCoins = 0;
            int courseId = 0;

            // 1. Get Quiz/Lesson details
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT q.QuizRewardCoins, l.CourseId, l.LessonTitle 
                    FROM Quiz q 
                    JOIN Lesson l ON q.LessonId = l.Id 
                    WHERE q.Id = @quizId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@quizId", quizId);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        rewardCoins = dr["QuizRewardCoins"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QuizRewardCoins"]);
                        courseId = dr["CourseId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CourseId"]);
                        lblQuizTitle.Text = $"Quiz Results for: {dr["LessonTitle"].ToString()}";
                        Session["SelectedCourseId"] = courseId;
                    }
                    dr.Close();

                    btnBackToCourse.CommandArgument = courseId.ToString();
                }
            }

            // 2. Apply styling and reward logic
            HtmlGenericControl statusSection = statusIcon.Parent is HtmlGenericControl ? (HtmlGenericControl)statusIcon.Parent : null;

            if (passed)
            {
                if (statusSection != null) statusSection.Attributes["class"] += " passed";
                statusIcon.InnerHtml = "✅";
                lblStatus.Text = "Congratulations! You passed!";

                if (rewardCoins > 0)
                {
                    // --- MODIFIED LOGIC: NO CHECK ---
                    // Award coins using the correct Student ID (this.studentTableId) on every pass
                    AddCoinsToStudent(this.studentTableId, rewardCoins);
                    litRewardMessage.Text = $"🏆 You earned **{rewardCoins}** Coins!";
                    // --- END MODIFIED LOGIC ---

                    pnlReward.Visible = true;
                }
                else
                {
                    litRewardMessage.Text = "No coin reward assigned for this quiz.";
                    pnlReward.Visible = true;
                }

                btnRetry.Visible = false;

            }
            else // Failed
            {
                if (statusSection != null) statusSection.Attributes["class"] += " failed";
                statusIcon.InnerHtml = "❌";
                lblStatus.Text = "Oops! You did not meet the passing grade.";
                litRewardMessage.Text = $"The passing grade is {passingGrade}%. You scored {Math.Round(percentageScore, 2)}%.";
                pnlReward.Visible = true;

                btnRetry.Visible = true;
                btnRetry.CommandArgument = quizId.ToString();
            }
        }


        protected void btnBackToCourse_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string courseId = btn.CommandArgument;
            Response.Redirect($"StudentCourseContent.aspx?courseId={courseId}");
        }

        protected void btnRetry_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string quizId = btn.CommandArgument;
            Response.Redirect($"StudentQuiz.aspx?quizId={quizId}");
        }
    }
}