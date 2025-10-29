using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class edit_course : System.Web.UI.Page
    {
        // --- Page_Load, LoadCourseTitle, LoadLessonList are UNCHANGED ---

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string courseId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(courseId))
                {
                    LoadCourseTitle(courseId);
                    LoadLessonList(courseId);
                }
                else
                {
                    Response.Redirect("educator_dashboard.aspx");
                }
            }
        }

        private void LoadCourseTitle(string courseId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Title FROM Course WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", courseId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                lblCourseTitle.Text = result != null ? result.ToString() : "Course not found.";
            }
        }

        private void LoadLessonList(string courseId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Id, LessonNumber, LessonTitle FROM Lesson WHERE CourseId = @CourseId ORDER BY LessonNumber";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CourseId", courseId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptLessons.DataSource = dt;
                rptLessons.DataBind();
            }
        }

        // --- rptLessons_ItemCommand and DeleteLesson are UNCHANGED ---

        protected void rptLessons_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string lessonId = e.CommandArgument.ToString();

            if (e.CommandName == "Delete")
            {
                DeleteLesson(lessonId);
                string courseId = Request.QueryString["id"];
                LoadLessonList(courseId);
                pnlLessonEdit.Visible = false;
            }
            else if (e.CommandName == "Edit")
            {
                pnlLessonEdit.Visible = true;
                LoadLessonDetails(lessonId); // MODIFIED
                LoadQuizQuestions(lessonId);
            }
        }

        private void DeleteLesson(string lessonId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    DELETE q FROM Question q INNER JOIN Quiz z ON q.QuizId = z.Id WHERE z.LessonId = @LessonId;
                    DELETE FROM Quiz WHERE LessonId = @LessonId;
                    DELETE FROM Lesson WHERE Id = @LessonId;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LessonId", lessonId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// [MODIFIED] Loads lesson details based on the 'CreateCourse' storage pattern.
        /// Loads ContentFile (text OR original name) into the textbox.
        /// Stores the actual file path and type in hidden fields.
        /// </summary>
        private void LoadLessonDetails(string lessonId)
        {
            hfLessonId.Value = lessonId;
            hfExistingFilePath.Value = ""; // Clear hidden fields
            hfExistingContentType.Value = "";

            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT LessonTitle, ContentFilePath, ContentFile, ContentType FROM Lesson WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", lessonId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtLessonTitle.Text = reader["LessonTitle"].ToString();

                    string filePath = reader["ContentFilePath"].ToString();
                    string fileContent = reader["ContentFile"].ToString(); // This is text OR original name

                    // ALWAYS load ContentFile into the textbox
                    txtLessonContentPath.Text = fileContent;

                    // If a file path exists, store the path and type in hidden fields
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        hfExistingFilePath.Value = filePath;
                        hfExistingContentType.Value = reader["ContentType"].ToString();
                    }
                }
            }
        }

        /// <summary>
        /// [MODIFIED] Updates the lesson using the 'CreateCourse' storage pattern.
        /// Handles 4 scenarios:
        /// 1. New file uploaded: Replaces old content, stores GUID_path and original_name.
        /// 2. Was text, now new text: Updates ContentFile.
        /// 3. Was file, kept file: Keeps old GUID_path, updates ContentFile (display name).
        /// 4. Was file, now text: Clears file path/type, saves text to ContentFile.
        /// </summary>
        protected void btnUpdateLesson_Click(object sender, EventArgs e)
        {
            string lessonId = hfLessonId.Value;
            if (string.IsNullOrEmpty(lessonId)) return;

            string title = txtLessonTitle.Text.Trim();
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // This dynamic query is safer than updating all columns
                string query = "UPDATE Lesson SET LessonTitle = @Title";

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Id", lessonId);

                if (fuLessonFile.HasFile)
                {
                    // --- PATH 1: A new file is being uploaded (matches CreateCourse) ---
                    string originalFileName = Path.GetFileName(fuLessonFile.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + originalFileName;
                    string savePath = Server.MapPath("~/Uploads/" + uniqueFileName);
                    string filePath = "~/Uploads/" + uniqueFileName;
                    string contentType = fuLessonFile.PostedFile.ContentType;

                    // Save the new physical file
                    fuLessonFile.SaveAs(savePath);

                    // Add file columns to the query
                    query += ", ContentFilePath = @FilePath, ContentFile = @File, ContentType = @Type";
                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                    cmd.Parameters.AddWithValue("@File", originalFileName); // Store ORIGINAL name
                    cmd.Parameters.AddWithValue("@Type", contentType);
                }
                else
                {
                    // --- PATH 2: No new file uploaded ---
                    string textInBox = txtLessonContentPath.Text.Trim();
                    string oldFilePath = hfExistingFilePath.Value;
                    string oldContentType = hfExistingContentType.Value;

                    if (string.IsNullOrEmpty(oldFilePath))
                    {
                        // SCENARIO 2.1: It was text, and is still text.
                        // Save the new text from the box.
                        query += ", ContentFilePath = @FilePath, ContentFile = @File, ContentType = @Type";
                        cmd.Parameters.AddWithValue("@FilePath", (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@File", textInBox);
                        cmd.Parameters.AddWithValue("@Type", (object)DBNull.Value);
                    }
                    else
                    {
                        // It was a file.
                        if (string.IsNullOrEmpty(textInBox))
                        {
                            // SCENARIO 2.2: It was a file, but user deleted the name.
                            // Convert to empty text content.
                            query += ", ContentFilePath = @FilePath, ContentFile = @File, ContentType = @Type";
                            cmd.Parameters.AddWithValue("@FilePath", (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@File", (object)DBNull.Value); // Save NULL
                            cmd.Parameters.AddWithValue("@Type", (object)DBNull.Value);
                        }
                        else
                        {
                            // SCENARIO 2.3: It was a file, and user is keeping it.
                            // We keep the old path/type, but update the display name (ContentFile).
                            query += ", ContentFilePath = @FilePath, ContentFile = @File, ContentType = @Type";
                            cmd.Parameters.AddWithValue("@FilePath", oldFilePath); // Keep old GUID path
                            cmd.Parameters.AddWithValue("@File", textInBox);       // Save new display name
                            cmd.Parameters.AddWithValue("@Type", oldContentType);  // Keep old type
                        }
                    }
                }

                query += " WHERE Id = @Id"; // Add the WHERE clause at the end
                cmd.CommandText = query;
                cmd.Connection = conn;

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            string courseId = Request.QueryString["id"];
            LoadLessonList(courseId);
            pnlLessonEdit.Visible = false;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Lesson updated successfully!');", true);
        }

        // --- ALL OTHER METHODS (Quiz, Questions, etc.) are UNCHANGED ---
        // --- They are not included here for brevity but are in your original file ---

        private void LoadQuizQuestions(string lessonId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT q.Id, q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectAnswer
                    FROM Quiz z
                    INNER JOIN Question q ON q.QuizId = z.Id
                    WHERE z.LessonId = @LessonId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LessonId", lessonId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.Columns.Add("Status", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    row["Status"] = "";
                }
                ViewState["QuizQuestions"] = dt;
                BindQuizRepeater();
            }
        }
        private void BindQuizRepeater()
        {
            if (ViewState["QuizQuestions"] != null)
            {
                DataTable dt = (DataTable)ViewState["QuizQuestions"];
                rptQuizQuestions.DataSource = dt;
                rptQuizQuestions.DataBind();
            }
        }
        protected void rptQuizQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rowView = (DataRowView)e.Item.DataItem;
                if (rowView["Status"].ToString() == "Deleted")
                {
                    e.Item.Visible = false;
                    return;
                }
                DropDownList ddlCorrectAnswer = (DropDownList)e.Item.FindControl("ddlCorrectAnswer");
                if (ddlCorrectAnswer != null)
                {
                    string correctAnswer = rowView["CorrectAnswer"].ToString().Trim();
                    ListItem li = ddlCorrectAnswer.Items.FindByValue(correctAnswer);
                    if (li != null)
                    {
                        ddlCorrectAnswer.SelectedValue = correctAnswer;
                    }
                }
            }
        }
        protected void rptQuizQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteQuestion")
            {
                string questionId = e.CommandArgument.ToString();
                DataTable dt = (DataTable)ViewState["QuizQuestions"];
                DataRow rowToToggle = dt.AsEnumerable().FirstOrDefault(r => r["Id"].ToString() == questionId);
                if (rowToToggle != null)
                {
                    if (rowToToggle["Status"].ToString() == "New")
                    {
                        dt.Rows.Remove(rowToToggle);
                    }
                    else
                    {
                        rowToToggle["Status"] = "Deleted";
                    }
                }
                ViewState["QuizQuestions"] = dt;
                BindQuizRepeater();
            }
        }
        protected void btnUpdateQuiz_Click(object sender, EventArgs e)
        {
            int quizId = GetOrCreateQuizId(hfLessonId.Value);
            DataTable dt = (DataTable)ViewState["QuizQuestions"];
            if (dt == null) return;

            foreach (RepeaterItem item in rptQuizQuestions.Items)
            {
                if (item.Visible && (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem))
                {
                    HiddenField hfQuestionId = (HiddenField)item.FindControl("hfQuestionId");
                    string qId = hfQuestionId.Value;
                    DataRow row = dt.AsEnumerable().FirstOrDefault(r => r["Id"].ToString() == qId);
                    if (row != null)
                    {
                        row["QuestionText"] = ((TextBox)item.FindControl("txtQuestionText")).Text.Trim();
                        row["OptionA"] = ((TextBox)item.FindControl("txtOptionA")).Text.Trim();
                        row["OptionB"] = ((TextBox)item.FindControl("txtOptionB")).Text.Trim();
                        row["OptionC"] = ((TextBox)item.FindControl("txtOptionC")).Text.Trim();
                        row["OptionD"] = ((TextBox)item.FindControl("txtOptionD")).Text.Trim();
                        row["CorrectAnswer"] = ((DropDownList)item.FindControl("ddlCorrectAnswer")).SelectedValue;
                    }
                }
            }
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                foreach (DataRow row in dt.Rows)
                {
                    string status = row["Status"].ToString();
                    if (status == "New")
                    {
                        InsertQuestion(conn, quizId, row);
                    }
                    else if (status == "Deleted")
                    {
                        DeleteQuestion(conn, row["Id"].ToString());
                    }
                    else
                    {
                        UpdateQuestion(conn, row);
                    }
                }
            }
            LoadQuizQuestions(hfLessonId.Value);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Quiz questions updated successfully!');", true);
        }
        private void InsertQuestion(SqlConnection conn, int quizId, DataRow row)
        {
            string query = @"
                INSERT INTO Question (QuizId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer)
                VALUES (@QuizId, @Text, @OptA, @OptB, @OptC, @OptD, @Correct)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                cmd.Parameters.AddWithValue("@Text", row["QuestionText"]);
                cmd.Parameters.AddWithValue("@OptA", row["OptionA"]);
                cmd.Parameters.AddWithValue("@OptB", row["OptionB"]);
                cmd.Parameters.AddWithValue("@OptC", row["OptionC"]);
                cmd.Parameters.AddWithValue("@OptD", row["OptionD"]);
                cmd.Parameters.AddWithValue("@Correct", row["CorrectAnswer"]);
                cmd.ExecuteNonQuery();
            }
        }
        private void UpdateQuestion(SqlConnection conn, DataRow row)
        {
            string query = @"
                UPDATE Question
                SET QuestionText = @QuestionText, OptionA = @OptionA, OptionB = @OptionB,
                    OptionC = @OptionC, OptionD = @OptionD, CorrectAnswer = @CorrectAnswer
                WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@QuestionText", row["QuestionText"]);
                cmd.Parameters.AddWithValue("@OptionA", row["OptionA"]);
                cmd.Parameters.AddWithValue("@OptionB", row["OptionB"]);
                cmd.Parameters.AddWithValue("@OptionC", row["OptionC"]);
                cmd.Parameters.AddWithValue("@OptionD", row["OptionD"]);
                cmd.Parameters.AddWithValue("@CorrectAnswer", row["CorrectAnswer"]);
                cmd.Parameters.AddWithValue("@Id", row["Id"]);
                cmd.ExecuteNonQuery();
            }
        }
        private void DeleteQuestion(SqlConnection conn, string questionId)
        {
            string query = "DELETE FROM Question WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", questionId);
                cmd.ExecuteNonQuery();
            }
        }
        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)ViewState["QuizQuestions"];
            if (dt == null) return;
            if (string.IsNullOrEmpty(txtNewQuestionText.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('New question text cannot be empty.');", true);
                return;
            }
            DataRow newRow = dt.NewRow();
            int tempId = (dt.Rows.Count > 0) ? (int)dt.AsEnumerable().Min(r => Convert.ToInt32(r["Id"])) : 0;
            newRow["Id"] = (tempId >= 0) ? -1 : (tempId - 1);
            newRow["QuestionText"] = txtNewQuestionText.Text.Trim();
            newRow["OptionA"] = txtNewOptionA.Text.Trim();
            newRow["OptionB"] = txtNewOptionB.Text.Trim();
            newRow["OptionC"] = txtNewOptionC.Text.Trim();
            newRow["OptionD"] = txtNewOptionD.Text.Trim();
            newRow["CorrectAnswer"] = ddlNewCorrectAnswer.SelectedValue;
            newRow["Status"] = "New";
            dt.Rows.Add(newRow);
            ViewState["QuizQuestions"] = dt;
            BindQuizRepeater();
            ClearNewQuestionForm();
        }
        private int GetOrCreateQuizId(string lessonId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string selectQuery = "SELECT Id FROM Quiz WHERE LessonId = @LessonId";
                SqlCommand cmdSelect = new SqlCommand(selectQuery, conn);
                cmdSelect.Parameters.AddWithValue("@LessonId", lessonId);
                object result = cmdSelect.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    string insertQuery = "INSERT INTO Quiz (LessonId) VALUES (@LessonId); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdInsert = new SqlCommand(insertQuery, conn);
                    cmdInsert.Parameters.AddWithValue("@LessonId", lessonId);
                    return Convert.ToInt32(cmdInsert.ExecuteScalar());
                }
            }
        }
        private void ClearNewQuestionForm()
        {
            txtNewQuestionText.Text = string.Empty;
            txtNewOptionA.Text = string.Empty;
            txtNewOptionB.Text = string.Empty;
            txtNewOptionC.Text = string.Empty;
            txtNewOptionD.Text = string.Empty;
            ddlNewCorrectAnswer.SelectedIndex = 0;
        }
    }
}