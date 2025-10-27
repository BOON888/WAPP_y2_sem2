using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls; // Required for RepeaterCommandEventArgs

namespace WAPP
{
    public partial class StudentCourseContent : System.Web.UI.Page
    {
        private string connString => ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Authorization check
                if (Session["UserId"] == null || Session["Role"]?.ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                // Course ID validation
                string idParam = Request.QueryString["courseId"];
                if (string.IsNullOrEmpty(idParam) || !int.TryParse(idParam, out int courseId))
                {
                    lblError.Text = "⚠️ Invalid or missing course ID.";
                    lblError.Visible = true;
                    return;
                }

                Session["SelectedCourseId"] = courseId;
                
                // Load course data
                LoadCourseInfo(courseId);
                LoadLessons(courseId);
                LoadQuizzes(courseId);
            }

            // Must register the ItemCommand handler every time (PostBack or not)
            rptQuizzes.ItemCommand += rptQuizzes_ItemCommand;
        }

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
                    lblCoin.Text = dr["Coin"].ToString();
                    lblStatus.Text = dr["Status"].ToString();
                }
                else
                {
                    lblError.Text = "⚠️ Course not found.";
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
                    @"SELECT LessonNumber, LessonTitle, ContentType, ContentFilePath
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

                // Add TEMPORARY columns for front-end rendering logic
                dt.Columns.Add("ButtonText", typeof(string));
                dt.Columns.Add("ButtonClass", typeof(string));
                dt.Columns.Add("ButtonEnabled", typeof(bool));
                
                // --- Sequential Quiz Unlocking Logic ---
                bool previousQuizCompleted = true;
                int completedCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string status = row["Status"].ToString();
                    bool isCompleted = status.Equals("Completed", StringComparison.OrdinalIgnoreCase);

                    if (isCompleted)
                    {
                        // Case 1: Quiz is COMPLETED
                        row["ButtonText"] = "Completed";
                        row["ButtonClass"] = "btn-view btn-disabled";
                        row["ButtonEnabled"] = false;
                        completedCount++;
                        previousQuizCompleted = true;
                    }
                    else if (previousQuizCompleted)
                    {
                        // Case 2: Quiz is UNLOCKED (The next one in the sequence)
                        row["ButtonText"] = "Start Quiz";
                        row["ButtonClass"] = "btn-view";
                        row["ButtonEnabled"] = true;
                        previousQuizCompleted = false; // Locks all quizzes that follow
                    }
                    else
                    {
                        // Case 3: Quiz is LOCKED
                        row["ButtonText"] = "Locked 🔒";
                        row["ButtonClass"] = "btn-view btn-disabled";
                        row["ButtonEnabled"] = false;
                    }
                }
                // --- End of Sequential Quiz Unlocking Logic ---


                // Compute Progress
                int total = dt.Rows.Count;
                int percent = (total == 0) ? 0 : (int)((completedCount / (double)total) * 100);
                
                // --- PROGRESS BAR COLOR SELECTION LOGIC ---
                string progressClass;
                if (percent < 30)
                {
                    progressClass = "progress-low";    // Red
                }
                else if (percent < 70)
                {
                    progressClass = "progress-medium"; // Orange
                }
                else
                {
                    progressClass = "progress-high";   // Green
                }

                lblProgressPercent.Text = percent + "%";
                progressFill.Style["width"] = percent + "%";
                progressFill.InnerText = percent + "%";
                // Apply the new class to change color
                progressFill.Attributes["class"] = $"progress-fill {progressClass}";
                // --- END PROGRESS BAR COLOR LOGIC ---


                rptQuizzes.DataSource = dt;
                rptQuizzes.DataBind();
            }
        }

        protected void btnBackDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("StudentDashboard.aspx");
        }
        
        // Handler for the Quiz Repeater Button Clicks
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