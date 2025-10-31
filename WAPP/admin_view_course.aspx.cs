using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class admin_view_course : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["courseId"] != null)
                {
                    string courseId = Request.QueryString["courseId"];
                    LoadCourseDetails(courseId);
                    LoadCourseLessons(courseId);
                    LoadCourseQuizzes(courseId);
                }
                else
                {
                    ShowMessage("No course ID provided.", "error");
                }
            }
        }

        private void LoadCourseDetails(string courseId)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT Id, Title, EducatorId, CourseType, CoursePicture, Status, Coin 
                        FROM Course 
                        WHERE Id = @CourseId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        // Set course details
                        lblCourseId.Text = row["Id"].ToString();
                        lblCourseTitle.Text = row["Title"].ToString();
                        lblEducatorId.Text = row["EducatorId"].ToString();
                        lblCoins.Text = row["Coin"] != DBNull.Value ? row["Coin"].ToString() + " coins" : "0 coins";

                        // Set course type badge
                        string courseType = row["CourseType"] != DBNull.Value ? row["CourseType"].ToString() : "Free";
                        lblCourseType.Text = courseType;
                        lblCourseType.CssClass = courseType.ToLower().Contains("premium") ? "badge badge-type-premium" : "badge badge-type-free";

                        // Set status badge
                        string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Active";
                        lblStatus.Text = status;
                        lblStatus.CssClass = status.ToLower().Contains("active") ? "badge badge-status-active" : "badge badge-status-inactive";

                        // Set course image
                        if (row["CoursePicture"] != DBNull.Value && !string.IsNullOrEmpty(row["CoursePicture"].ToString()))
                        {
                            imgCourse.ImageUrl = row["CoursePicture"].ToString();
                        }
                        else
                        {
                            imgCourse.ImageUrl = "~/Image/default-course.jpg";
                            imgCourse.AlternateText = "Default Course Image";
                        }
                    }
                    else
                    {
                        ShowMessage("Course not found.", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading course details: " + ex.Message, "error");
            }
        }

        private void LoadCourseLessons(string courseId)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT Id, LessonNumber, LessonTitle, ContentType, ContentFilePath, ContentFile 
                        FROM Lesson 
                        WHERE CourseId = @CourseId 
                        ORDER BY LessonNumber";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        rptLessons.DataSource = dt;
                        rptLessons.DataBind();
                        lblLessonsCount.Text = dt.Rows.Count.ToString();
                        pnlNoLessons.Visible = false;
                    }
                    else
                    {
                        rptLessons.DataSource = null;
                        rptLessons.DataBind();
                        lblLessonsCount.Text = "0";
                        pnlNoLessons.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading lessons: " + ex.Message, "error");
                pnlNoLessons.Visible = true;
            }
        }

        private void LoadCourseQuizzes(string courseId)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            qz.Id as QuizId,
                            qz.QuizRewardCoins,
                            qz.TotalQuestions,
                            l.LessonNumber,
                            l.LessonTitle
                        FROM Quiz qz
                        INNER JOIN Lesson l ON qz.LessonId = l.Id
                        WHERE l.CourseId = @CourseId
                        ORDER BY l.LessonNumber";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        rptQuizzes.DataSource = dt;
                        rptQuizzes.DataBind();
                        lblQuizzesCount.Text = dt.Rows.Count.ToString();
                        pnlNoQuizzes.Visible = false;
                    }
                    else
                    {
                        rptQuizzes.DataSource = null;
                        rptQuizzes.DataBind();
                        lblQuizzesCount.Text = "0";
                        pnlNoQuizzes.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading quizzes: " + ex.Message, "error");
                pnlNoQuizzes.Visible = true;
            }
        }

        protected void rptLessons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // You can add additional lesson item binding logic here if needed
            }
        }

        protected void rptQuizzes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RepeaterItem item = e.Item;
                DataRowView rowView = (DataRowView)item.DataItem;

                string quizId = rowView["QuizId"].ToString();

                // Find the questions repeater in the current quiz item
                Repeater rptQuestions = (Repeater)item.FindControl("rptQuestions");

                if (rptQuestions != null)
                {
                    LoadQuizQuestions(quizId, rptQuestions);
                }
            }
        }

        private void LoadQuizQuestions(string quizId, Repeater rptQuestions)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            QuestionText, 
                            OptionA, 
                            OptionB, 
                            OptionC, 
                            OptionD, 
                            CorrectAnswer 
                        FROM Question 
                        WHERE QuizId = @QuizId 
                        ORDER BY Id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@QuizId", quizId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        rptQuestions.DataSource = dt;
                        rptQuestions.DataBind();
                    }
                    else
                    {
                        rptQuestions.DataSource = null;
                        rptQuestions.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent fail for questions - don't break the entire page
            }
        }

        // Helper methods for content display
        public string GetContentMaterial(object contentType, object contentFile)
        {
            string type = contentType?.ToString()?.ToLower() ?? "";
            string file = contentFile?.ToString() ?? "";

            if (string.IsNullOrEmpty(file))
                return "No content available";

            if (type.Contains("video"))
                return "Video material";
            else if (type.Contains("pdf"))
                return "PDF document";
            else if (type.Contains("text") || type.Contains("document"))
                return "Text material";
            else
                return "Learning material";
        }

        public bool ShowMaterialPreview(object contentType, object contentFile)
        {
            string type = contentType?.ToString()?.ToLower() ?? "";
            string file = contentFile?.ToString() ?? "";

            return !string.IsNullOrEmpty(file);
        }

        public string GetMaterialIcon(object contentType)
        {
            string type = contentType?.ToString()?.ToLower() ?? "";

            if (type.Contains("video")) return "🎬";
            else if (type.Contains("pdf")) return "📄";
            else if (type.Contains("text") || type.Contains("document")) return "📝";
            else return "📚";
        }

        public string GetMaterialText(object contentType, object contentFile)
        {
            string type = contentType?.ToString()?.ToLower() ?? "";
            string file = contentFile?.ToString() ?? "";

            if (string.IsNullOrEmpty(file))
                return "No file attached";

            if (type.Contains("video")) return "Video File";
            else if (type.Contains("pdf")) return "PDF Document";
            else if (type.Contains("text") || type.Contains("document")) return "Text Document";
            else return "Learning Material";
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;

            if (type == "success")
            {
                pnlMessage.CssClass = "message-container message-success";
            }
            else
            {
                pnlMessage.CssClass = "message-container message-error";
            }
        }
    }
}