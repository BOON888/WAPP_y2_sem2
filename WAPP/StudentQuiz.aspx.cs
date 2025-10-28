using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls; // Added for completeness, if used

namespace WAPP
{
    public partial class StudentQuiz : System.Web.UI.Page
    {
        private string connString => ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            
            rptQuestions.ItemDataBound += rptQuestions_ItemDataBound;

            
            if (Session["UserId"] == null || Session["Role"]?.ToString().ToLower() != "student")
            {
                Response.Redirect("sign_in.aspx");
                return;
            }

            if (!IsPostBack)
            {
                
                string quizParam = Request.QueryString["quizId"];
                if (string.IsNullOrEmpty(quizParam) || !int.TryParse(quizParam, out int quizId))
                {
                    lblError.Text = "Invalid or missing quiz ID.";
                    lblError.Visible = true;
                    return;
                }

                Session["CurrentQuizId"] = quizId;
                LoadQuiz(quizId);
            }
        }

        
        private int GetStudentIdFromUserId(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                string query = "SELECT Id FROM Student WHERE UserId = @userId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                object result = cmd.ExecuteScalar();

                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

        
        private void InsertPassingProgress(int qId, int sId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string insertQuery = @"
                    INSERT INTO StudentQuizProgress (StudentId, QuizId, Status)
                    VALUES (@studentId, @quizId, @status)";

                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@studentId", sId);
                insertCmd.Parameters.AddWithValue("@quizId", qId);

                
                insertCmd.Parameters.AddWithValue("@status", "Completed");

                insertCmd.ExecuteNonQuery();
            }
        }

        private void LoadQuiz(int quizId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                SqlCommand cmdQuiz = new SqlCommand(
                    @"SELECT q.Id, q.LessonId, q.QuizRewardCoins, q.TotalQuestions, l.LessonTitle 
                      FROM Quiz q
                      JOIN Lesson l ON q.LessonId = l.Id
                      WHERE q.Id=@qid", conn);
                cmdQuiz.Parameters.AddWithValue("@qid", quizId);
                SqlDataReader dr = cmdQuiz.ExecuteReader();

                if (dr.Read())
                {
                    lblQuizTitle.Text = "Quiz for Lesson: " + dr["LessonTitle"].ToString();
                    ViewState["QuizRewardCoins"] = dr["QuizRewardCoins"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QuizRewardCoins"]);
                    ViewState["QuizPassingGrade"] = 70f;
                }
                else
                {
                    lblError.Text = "Quiz not found.";
                    lblError.Visible = true;
                    return;
                }

                int totalQuestionsFromDB = dr["TotalQuestions"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalQuestions"]);
                dr.Close();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer FROM Question WHERE QuizId=@qid", conn);
                da.SelectCommand.Parameters.AddWithValue("@qid", quizId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                rptQuestions.DataSource = dt;
                rptQuestions.DataBind();
                ViewState["QuizData"] = dt;

                if (totalQuestionsFromDB == 0 && dt.Rows.Count > 0)
                {
                    UpdateTotalQuestions(quizId, dt.Rows.Count, conn);
                }
            }
        }

        private void UpdateTotalQuestions(int qId, int count, SqlConnection conn)
        {
            if (count > 0)
            {
                string updateQuery = "UPDATE Quiz SET TotalQuestions = @count WHERE Id = @quizId";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@count", count);
                updateCmd.Parameters.AddWithValue("@quizId", qId);
                updateCmd.ExecuteNonQuery();
            }
        }

        protected void rptQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                RadioButtonList rbl = (RadioButtonList)e.Item.FindControl("rblOptions");

                if (rbl != null)
                {
                    rbl.Items.Clear();

                    if (!string.IsNullOrEmpty(row["OptionA"].ToString()))
                        rbl.Items.Add(new ListItem(row["OptionA"].ToString(), "A"));
                    if (!string.IsNullOrEmpty(row["OptionB"].ToString()))
                        rbl.Items.Add(new ListItem(row["OptionB"].ToString(), "B"));
                    if (!string.IsNullOrEmpty(row["OptionC"].ToString()))
                        rbl.Items.Add(new ListItem(row["OptionC"].ToString(), "C"));
                    if (!string.IsNullOrEmpty(row["OptionD"].ToString()))
                        rbl.Items.Add(new ListItem(row["OptionD"].ToString(), "D"));
                }
            }
        }

        
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                lblError.Text = "You must be logged in.";
                lblError.Visible = true;
                return;
            }

            
            foreach (RepeaterItem item in rptQuestions.Items)
            {
                RadioButtonList rbl = (RadioButtonList)item.FindControl("rblOptions");
                if (rbl == null || string.IsNullOrEmpty(rbl.SelectedValue))
                {
                    lblError.Text = "Please answer all questions before submitting the quiz.";
                    lblError.Visible = true;
                    return;
                }
            }
            lblError.Visible = false;

            
            int userId = Convert.ToInt32(Session["UserId"]);
            int studentIdForFK = GetStudentIdFromUserId(userId);

            if (studentIdForFK <= 0)
            {
                lblError.Text = "Error: Cannot find a valid Student record for this user. Foreign Key conflict is likely.";
                lblError.Visible = true;
                return;
            }
            


            int quizId = Convert.ToInt32(Session["CurrentQuizId"]);
            DataTable dt = ViewState["QuizData"] as DataTable;
            float passingGrade = 70f;

            if (dt == null)
            {
                lblError.Text = "Something went wrong loading quiz data.";
                lblError.Visible = true;
                return;
            }

            int totalQuestions = dt.Rows.Count;
            int correctCount = 0;

            // Grade answers... 
            for (int i = 0; i < rptQuestions.Items.Count; i++)
            {
                RepeaterItem item = rptQuestions.Items[i];
                RadioButtonList rbl = (RadioButtonList)item.FindControl("rblOptions");
                string correctAnswer = dt.Rows[i]["CorrectAnswer"].ToString().Trim();

                if (rbl.SelectedValue.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                    correctCount++;
            }

            
            double percentageScore = (totalQuestions == 0) ? 0 : (correctCount / (double)totalQuestions) * 100;
            bool passed = percentageScore >= passingGrade;

            
            if (passed)
            {
                
                InsertPassingProgress(quizId, studentIdForFK);
            }
           
            Response.Redirect($"StudentQuizResult.aspx?quizId={quizId}&score={correctCount}&total={totalQuestions}&passGrade={passingGrade}");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Session["SelectedCourseId"] != null)
                Response.Redirect($"StudentCourseContent.aspx?courseId={Session["SelectedCourseId"]}");
            else
                Response.Redirect("StudentDashboard.aspx");
        }
    }
}