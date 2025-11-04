using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class StudentLesson : System.Web.UI.Page
    {
        private string connString => ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
        private int courseId;
        private int currentLessonNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["SelectedCourseId"] != null && int.TryParse(Session["SelectedCourseId"].ToString(), out courseId))
            {
                
            }
            else
            {
                courseId = 0;
            }

            if (!IsPostBack)
            {
                
                if (Session["UserId"] == null || Session["Role"]?.ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                string idParam = Request.QueryString["lessonId"];
                if (string.IsNullOrEmpty(idParam) || !int.TryParse(idParam, out int lessonId))
                {
                    lblError.Text = "Invalid or missing lesson ID.";
                    lblError.Visible = true;
                    return;
                }

                LoadLessonContent(lessonId);
            }
        }

        private void LoadLessonContent(int lessonId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                
                string lessonQuery = @"
                    SELECT LessonNumber, LessonTitle, ContentType, ContentFilePath, ContentFile, CourseId
                    FROM Lesson WHERE Id = @lessonId";

                SqlCommand cmdLesson = new SqlCommand(lessonQuery, conn);
                cmdLesson.Parameters.AddWithValue("@lessonId", lessonId);
                SqlDataReader dr = cmdLesson.ExecuteReader();

                if (dr.Read())
                {
                    // Update Literals and class fields
                    currentLessonNumber = Convert.ToInt32(dr["LessonNumber"]);
                    litLessonNumber.Text = currentLessonNumber.ToString();
                    litLessonTitle.Text = dr["LessonTitle"].ToString();
                    litContentType.Text = dr["ContentType"].ToString();

                    if (courseId == 0)
                    {
                        if (dr["CourseId"] != DBNull.Value)
                        {
                            courseId = Convert.ToInt32(dr["CourseId"]);
                            Session["SelectedCourseId"] = courseId;
                        }
                    }

                    string contentType = dr["ContentType"].ToString();
                    string filePath = ResolveUrl(dr["ContentFilePath"]?.ToString() ?? "");
                    string fileText = dr["ContentFile"]?.ToString() ?? "";

                    dr.Close();

                    

                    // 1. Handle ContentFile Text (Transcript/Summary)
                    litContentFileText.Text = fileText.Replace("\n", "<br/>");
                    pnlTextContent.Visible = !string.IsNullOrEmpty(fileText);

                    // 2. Handle Media Embed (Video/PDF)
                    contentEmbed.InnerHtml = "";

                    if (contentType.Equals("Video", StringComparison.OrdinalIgnoreCase))
                    {
                        contentEmbed.InnerHtml = $"<iframe src='{filePath}' frameborder='0' allowfullscreen></iframe>";
                    }
                    else if (contentType.Equals("PDF", StringComparison.OrdinalIgnoreCase) || contentType.Equals("Document", StringComparison.OrdinalIgnoreCase))
                    {
                        contentEmbed.InnerHtml = $"<iframe src='{filePath}' style='width:100%; height:500px;' frameborder='0'></iframe>";
                    }
                    else if (contentType.Equals("Text", StringComparison.OrdinalIgnoreCase))
                    {
                        
                    }
                    else
                    {
                        lblError.Text = "Unsupported content type or missing file path.";
                        lblError.Visible = true;
                    }

                    // 3. Check for Next Lesson
                    CheckForNextLesson(conn);

                }
                else
                {
                    dr.Close();
                    lblError.Text = "Lesson not found.";
                    lblError.Visible = true;
                }
            }
        }

        private void CheckForNextLesson(SqlConnection conn)
        {
            //Look for the next sequential lesson
            string nextLessonQuery = @"
                SELECT TOP 1 Id AS NextLessonId
                FROM Lesson
                WHERE CourseId = @cid AND LessonNumber > @currentNum
                ORDER BY LessonNumber ASC";

            SqlCommand cmdNext = new SqlCommand(nextLessonQuery, conn);
            cmdNext.Parameters.AddWithValue("@cid", courseId);
            cmdNext.Parameters.AddWithValue("@currentNum", currentLessonNumber);

            object nextLessonIdObj = cmdNext.ExecuteScalar();

            if (nextLessonIdObj != null)
            {
                // Next lesson found
                btnNextLesson.Visible = true;
                btnNextLesson.CommandArgument = nextLessonIdObj.ToString();
            }
            else
            {
                // No more lessons in this course
                btnNextLesson.Visible = false;
            }
        }


        protected void btnNextLesson_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string nextLessonId = btn.CommandArgument;

            // Redirect to the next lesson
            if (!string.IsNullOrEmpty(nextLessonId))
            {
                Response.Redirect($"StudentLesson.aspx?lessonId={nextLessonId}");
            }
        }

        protected void btnBackToCourse_Click(object sender, EventArgs e)
        {
            // Redirect back to the course content page
            if (courseId > 0)
            {
                Response.Redirect($"StudentCourseContent.aspx?courseId={courseId}");
            }
            else
            {
                Response.Redirect("StudentDashboard.aspx");
            }
        }
    }
}