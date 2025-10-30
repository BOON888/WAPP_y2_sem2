using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class createcourse : System.Web.UI.Page
    {
        // Static lists to hold data before saving to DB
        private static List<Lesson> lessonList = new List<Lesson>();
        private static List<Question> tempQuestionList = new List<Question>(); // For building a quiz

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear all temporary data on initial page load
                lessonList.Clear();
                tempQuestionList.Clear();

                BindLessonGrid();
                UpdateLessonCountLabel();
                BindQuizLessonDropdown();
                BindTempQuestionGrid();
            }
        }

        #region Course Methods
        protected void ddlCourseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            coinDiv.Visible = ddlCourseType.SelectedValue == "Private";
        }
        #endregion

        #region Lesson Methods
        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            try
            {
                // ... (File size check and lesson title check - same as your original code) ...
                if (fuContentFile.HasFile && fuContentFile.PostedFile.ContentLength > 52428800) // 50MB
                {
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "Lesson File: Size exceeds 50MB limit.";
                    return;
                }
                string lessonTitle = txtLessonTitle.Text.Trim();
                if (string.IsNullOrEmpty(lessonTitle))
                {
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "Please enter a lesson title.";
                    return;
                }

                // ... (File upload logic - same as your original code) ...
                string contentType = "Document"; // You might want to detect this based on file type
                string contentFile = txtTextContent.Text.Trim();
                string filePath = "";

                if (fuContentFile.HasFile)
                {
                    try
                    {
                        string fileName = System.IO.Path.GetFileName(fuContentFile.FileName);
                        string uploadsFolder = Server.MapPath("~/Uploads/");
                        if (!System.IO.Directory.Exists(uploadsFolder))
                        {
                            System.IO.Directory.CreateDirectory(uploadsFolder);
                        }
                        string savePath = System.IO.Path.Combine(uploadsFolder, fileName);
                        fuContentFile.SaveAs(savePath);
                        filePath = "~/Uploads/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "Error uploading file: " + ex.Message;
                        return;
                    }
                }
                // ... (End of file upload logic) ...

                int lessonNumber = lessonList.Count + 1;
                lessonList.Add(new Lesson
                {
                    LessonNumber = lessonNumber,
                    LessonTitle = lessonTitle,
                    ContentType = contentType,
                    ContentFilePath = filePath,
                    ContentFile = contentFile,
                    LessonQuiz = null // New lessons have no quiz initially
                });

                BindLessonGrid();
                UpdateLessonCountLabel();
                BindQuizLessonDropdown(); // Update the quiz dropdown

                // Clear lesson form
                txtLessonTitle.Text = "";
                txtTextContent.Text = "";
                // fuContentFile.Dispose(); // Note: Dispose might not clear the selection

                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Lesson added successfully!";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error adding lesson: " + ex.Message;
            }
        }

        private void BindLessonGrid()
        {
            gvLessons.DataKeyNames = new string[] { "LessonNumber" };
            gvLessons.DataSource = lessonList;
            gvLessons.DataBind();
        }

        private void UpdateLessonCountLabel()
        {
            lblLessonCount.Text = lessonList.Count.ToString();
        }

        // --- gvLessons GridView Events (Edit, Update, Cancel, Delete) ---
        // ... (gvLessons_RowEditing, gvLessons_RowUpdating, gvLessons_RowCancelingEdit are same as your code) ...

        protected void gvLessons_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvLessons.EditIndex = e.NewEditIndex;
            BindLessonGrid();
        }

        protected void gvLessons_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // ... (Same as your original code) ...
            try
            {
                int lessonNumber = Convert.ToInt32(gvLessons.DataKeys[e.RowIndex].Value);
                Lesson lessonToUpdate = lessonList.FirstOrDefault(l => l.LessonNumber == lessonNumber);
                if (lessonToUpdate != null)
                {
                    TextBox txtEditTitle = (TextBox)gvLessons.Rows[e.RowIndex].FindControl("txtEditLessonTitle");
                    TextBox txtEditContent = (TextBox)gvLessons.Rows[e.RowIndex].FindControl("txtEditContentFile");
                    FileUpload fuEditFile = (FileUpload)gvLessons.Rows[e.RowIndex].FindControl("fuEditContentFile");

                    lessonToUpdate.LessonTitle = txtEditTitle.Text.Trim();
                    lessonToUpdate.ContentFile = txtEditContent.Text.Trim();

                    if (fuEditFile.HasFile)
                    {
                        // ... (file validation and save logic - same as your code) ...
                        if (fuEditFile.PostedFile.ContentLength > 52428800)
                        {
                            lblStatus.ForeColor = System.Drawing.Color.Red;
                            lblStatus.Text = "File size exceeds 50MB limit.";
                            return;
                        }
                        string fileName = System.IO.Path.GetFileName(fuEditFile.FileName);
                        string uploadsFolder = Server.MapPath("~/Uploads/");
                        if (!System.IO.Directory.Exists(uploadsFolder))
                        {
                            System.IO.Directory.CreateDirectory(uploadsFolder);
                        }
                        string savePath = System.IO.Path.Combine(uploadsFolder, fileName);
                        fuEditFile.SaveAs(savePath);
                        lessonToUpdate.ContentFilePath = "~/Uploads/" + fileName;
                    }
                }
                gvLessons.EditIndex = -1;
                BindLessonGrid();
                BindQuizLessonDropdown(); // Update dropdown in case title changed
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Lesson updated successfully!";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error updating lesson: " + ex.Message;
            }
        }

        protected void gvLessons_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvLessons.EditIndex = -1;
            BindLessonGrid();
        }

        protected void gvLessons_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int lessonNumber = Convert.ToInt32(gvLessons.DataKeys[e.RowIndex].Value);
                lessonList.RemoveAll(l => l.LessonNumber == lessonNumber);

                // Re-number remaining lessons
                for (int i = 0; i < lessonList.Count; i++)
                {
                    lessonList[i].LessonNumber = i + 1;
                }

                BindLessonGrid();
                UpdateLessonCountLabel();
                BindQuizLessonDropdown(); // Update quiz dropdown
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Lesson deleted successfully!";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error deleting lesson: " + ex.Message;
            }
        }

        // --- NEW --- Show Quiz Status in Lesson Grid
        protected void gvLessons_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Lesson lesson = (Lesson)e.Row.DataItem;
                Label lblQuizStatus = (Label)e.Row.FindControl("lblQuizStatus");

                if (lesson.LessonQuiz != null && lesson.LessonQuiz.Questions.Count > 0)
                {
                    lblQuizStatus.Text = $"Yes ({lesson.LessonQuiz.Questions.Count} Qs)";
                    lblQuizStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblQuizStatus.Text = "No";
                    lblQuizStatus.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }

        // --- NEW --- Handle "Edit Quiz" / "Delete Quiz" from Lesson Grid
        protected void gvLessons_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditQuiz" || e.CommandName == "DeleteQuiz")
            {
                int lessonNumber = Convert.ToInt32(e.CommandArgument);
                Lesson lesson = lessonList.FirstOrDefault(l => l.LessonNumber == lessonNumber);
                if (lesson == null) return;

                if (e.CommandName == "EditQuiz")
                {
                    // Load quiz data into the quiz form
                    ddlQuizLesson.SelectedValue = lesson.LessonNumber.ToString();
                    txtQuizRewardCoins.Text = lesson.LessonQuiz?.QuizRewardCoins?.ToString() ?? "";

                    // Copy questions to temp list
                    tempQuestionList = (lesson.LessonQuiz != null) ? new List<Question>(lesson.LessonQuiz.Questions) : new List<Question>();
                    BindTempQuestionGrid();

                    // Set edit mode
                    ViewState["QuizEditLessonNumber"] = lessonNumber;
                    btnSaveQuiz.Text = "Update Quiz";
                    ddlQuizLesson.Enabled = false; // Don't allow changing lesson while editing quiz
                    lblStatus.Text = $"Editing quiz for: {lesson.LessonTitle}";
                    lblStatus.ForeColor = System.Drawing.Color.Blue;
                }
                else if (e.CommandName == "DeleteQuiz")
                {
                    // Confirm deletion (already done by OnClientClick)
                    lesson.LessonQuiz = null;
                    BindLessonGrid(); // Re-binds lesson grid, RowDataBound updates status
                    lblStatus.Text = "✅ Quiz removed from lesson.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
        }
        #endregion

        #region Quiz & Question Methods

        // --- NEW --- Populates the "Select Lesson" dropdown
        private void BindQuizLessonDropdown()
        {
            ddlQuizLesson.DataSource = lessonList;
            ddlQuizLesson.DataTextField = "LessonTitle";
            ddlQuizLesson.DataValueField = "LessonNumber";
            ddlQuizLesson.DataBind();
            ddlQuizLesson.Items.Insert(0, new ListItem("Select Lesson...", ""));
        }

        // --- NEW --- Binds the temporary question list grid
        private void BindTempQuestionGrid()
        {
            gvTempQuestions.DataKeyNames = new string[] { "QuestionNumber" };
            gvTempQuestions.DataSource = tempQuestionList;
            gvTempQuestions.DataBind();
        }

        // --- NEW --- Clears the question input form
        private void ClearQuestionForm()
        {
            txtQuestionText.Text = "";
            txtOptionA.Text = "";
            txtOptionB.Text = "";
            txtOptionC.Text = "";
            txtOptionD.Text = "";
            ddlCorrectAnswer.SelectedIndex = 0;
        }

        // --- NEW --- Adds a question to the temporary list
        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtQuestionText.Text) ||
                string.IsNullOrWhiteSpace(txtOptionA.Text) ||
                string.IsNullOrWhiteSpace(txtOptionB.Text) ||
                ddlCorrectAnswer.SelectedValue == "")
            {
                lblQuizStatus.ForeColor = System.Drawing.Color.Red;
                lblQuizStatus.Text = "Please fill in Question Text, Option A, Option B, and select a Correct Answer.";
                return;
            }

            try
            {
                Question newQ = new Question
                {
                    QuestionNumber = tempQuestionList.Count + 1,
                    QuestionText = txtQuestionText.Text.Trim(),
                    OptionA = txtOptionA.Text.Trim(),
                    OptionB = txtOptionB.Text.Trim(),
                    OptionC = txtOptionC.Text.Trim(),
                    OptionD = txtOptionD.Text.Trim(),
                    CorrectAnswer = ddlCorrectAnswer.SelectedValue
                };

                tempQuestionList.Add(newQ);
                BindTempQuestionGrid();
                ClearQuestionForm();
                lblQuizStatus.Text = "Question added to temp list.";
                lblQuizStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblQuizStatus.ForeColor = System.Drawing.Color.Red;
                lblQuizStatus.Text = "Error adding question: " + ex.Message;
            }
        }

        // --- NEW --- Saves the temp question list as a Quiz object on the selected Lesson
        protected void btnSaveQuiz_Click(object sender, EventArgs e)
        {
            int lessonNumber;

            // Check if we are in edit mode or add mode
            if (ViewState["QuizEditLessonNumber"] != null)
            {
                lessonNumber = (int)ViewState["QuizEditLessonNumber"];
            }
            else
            {
                if (ddlQuizLesson.SelectedValue == "")
                {
                    lblQuizStatus.ForeColor = System.Drawing.Color.Red;
                    lblQuizStatus.Text = "Please select a lesson to attach this quiz to.";
                    return;
                }
                lessonNumber = Convert.ToInt32(ddlQuizLesson.SelectedValue);
            }

            if (tempQuestionList.Count == 0)
            {
                lblQuizStatus.ForeColor = System.Drawing.Color.Red;
                lblQuizStatus.Text = "Please add at least one question to the quiz.";
                return;
            }

            try
            {
                // Find the target lesson
                Lesson lessonToUpdate = lessonList.FirstOrDefault(l => l.LessonNumber == lessonNumber);
                if (lessonToUpdate == null)
                {
                    lblQuizStatus.ForeColor = System.Drawing.Color.Red;
                    lblQuizStatus.Text = "Error: Lesson not found.";
                    return;
                }

                // Create new Quiz object
                Quiz newQuiz = new Quiz();
                if (!string.IsNullOrWhiteSpace(txtQuizRewardCoins.Text))
                {
                    newQuiz.QuizRewardCoins = Convert.ToInt32(txtQuizRewardCoins.Text);
                }

                // Assign the questions from the temp list
                newQuiz.Questions = new List<Question>(tempQuestionList);

                // Attach quiz to the lesson
                lessonToUpdate.LessonQuiz = newQuiz;

                // Clear form and temp list
                ClearQuestionForm();
                tempQuestionList.Clear();
                BindTempQuestionGrid();
                txtQuizRewardCoins.Text = "";
                ddlQuizLesson.SelectedIndex = 0;

                // Reset edit mode if we were in it
                if (ViewState["QuizEditLessonNumber"] != null)
                {
                    ViewState["QuizEditLessonNumber"] = null;
                    btnSaveQuiz.Text = "Save Quiz to Lesson";
                    ddlQuizLesson.Enabled = true;
                    lblStatus.Text = "Quiz updated successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblStatus.Text = "Quiz saved to lesson successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }

                lblQuizStatus.Text = "";
                BindLessonGrid(); // Rebind lesson grid to update quiz status
            }
            catch (Exception ex)
            {
                lblQuizStatus.ForeColor = System.Drawing.Color.Red;
                lblQuizStatus.Text = "Error saving quiz: " + ex.Message;
            }
        }

        // --- NEW --- Button to clear the quiz form
        protected void btnCancelQuizEdit_Click(object sender, EventArgs e)
        {
            ClearQuestionForm();
            tempQuestionList.Clear();
            BindTempQuestionGrid();
            txtQuizRewardCoins.Text = "";
            ddlQuizLesson.SelectedIndex = 0;

            ViewState["QuizEditLessonNumber"] = null;
            btnSaveQuiz.Text = "Save Quiz to Lesson";
            ddlQuizLesson.Enabled = true;

            lblQuizStatus.Text = "Quiz creation/edit cancelled.";
            lblQuizStatus.ForeColor = System.Drawing.Color.Gray;
        }


        // --- gvTempQuestions GridView Events (Edit, Update, Cancel, Delete) ---
        // --- NEW ---
        protected void gvTempQuestions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvTempQuestions.EditIndex = e.NewEditIndex;
            BindTempQuestionGrid();
        }

        protected void gvTempQuestions_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int qNumber = Convert.ToInt32(gvTempQuestions.DataKeys[e.RowIndex].Value);
                Question qToUpdate = tempQuestionList.FirstOrDefault(q => q.QuestionNumber == qNumber);

                if (qToUpdate != null)
                {
                    qToUpdate.QuestionText = ((TextBox)gvTempQuestions.Rows[e.RowIndex].FindControl("txtEditQuestionText")).Text.Trim();
                    qToUpdate.OptionA = ((TextBox)gvTempQuestions.Rows[e.RowIndex].FindControl("txtEditOptionA")).Text.Trim();
                    qToUpdate.OptionB = ((TextBox)gvTempQuestions.Rows[e.RowIndex].FindControl("txtEditOptionB")).Text.Trim();
                    qToUpdate.OptionC = ((TextBox)gvTempQuestions.Rows[e.RowIndex].FindControl("txtEditOptionC")).Text.Trim();
                    qToUpdate.OptionD = ((TextBox)gvTempQuestions.Rows[e.RowIndex].FindControl("txtEditOptionD")).Text.Trim();
                    qToUpdate.CorrectAnswer = ((DropDownList)gvTempQuestions.Rows[e.RowIndex].FindControl("ddlEditCorrectAnswer")).SelectedValue;
                }

                gvTempQuestions.EditIndex = -1;
                BindTempQuestionGrid();
                lblQuizStatus.Text = "Question updated in temp list.";
                lblQuizStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblQuizStatus.Text = "Error updating question: " + ex.Message;
                lblQuizStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void gvTempQuestions_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvTempQuestions.EditIndex = -1;
            BindTempQuestionGrid();
        }

        protected void gvTempQuestions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int qNumber = Convert.ToInt32(gvTempQuestions.DataKeys[e.RowIndex].Value);
                tempQuestionList.RemoveAll(q => q.QuestionNumber == qNumber);

                // Re-number
                for (int i = 0; i < tempQuestionList.Count; i++)
                {
                    tempQuestionList[i].QuestionNumber = i + 1;
                }

                BindTempQuestionGrid();
                lblQuizStatus.Text = "Question removed from temp list.";
                lblQuizStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblQuizStatus.Text = "Error deleting question: " + ex.Message;
                lblQuizStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Create Course Button (Main Save)
        protected void btnCreateCourse_Click(object sender, EventArgs e)
        {
            // --- VALIDATION ---
            // (Add client-side validation for this too)
            if (string.IsNullOrWhiteSpace(txtCourseTitle.Text) || ddlCourseType.SelectedValue == "" || lessonList.Count == 0)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Please provide a Course Title, Type, and add at least one Lesson.";
                return;
            }
            if (ddlCourseType.SelectedValue == "Private" && string.IsNullOrWhiteSpace(txtCourseCoin.Text))
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Private courses require a Coin value.";
                return;
            }
            // --- END VALIDATION ---


            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            SqlConnection con = null;
            SqlTransaction transaction = null;

            try
            {
                using (con = new SqlConnection(connStr))
                {
                    con.Open();
                    transaction = con.BeginTransaction(); // Start a transaction

                    // ✅ 1. Find EducatorId using Session["UserId"]
                    int userId = Convert.ToInt32(Session["UserId"]);
                    int educatorId = 0;
                    string findEducatorQuery = "SELECT Id FROM Educator WHERE UserId = @UserId";
                    using (SqlCommand findCmd = new SqlCommand(findEducatorQuery, con, transaction))
                    {
                        findCmd.Parameters.AddWithValue("@UserId", userId);
                        object result = findCmd.ExecuteScalar();
                        if (result != null)
                            educatorId = Convert.ToInt32(result);
                        else
                            throw new Exception("Educator record not found for this user.");
                    }

                    // ✅ 2. Insert Course
                    string insertCourseQuery = @"
                        INSERT INTO Course (Title, EducatorId, CourseType, CoursePicture, Status, Coin)
                        OUTPUT INSERTED.Id
                        VALUES (@Title, @EducatorId, @CourseType, @CoursePicture, @Status, @Coin)";

                    int coin = 0;
                    if (ddlCourseType.SelectedValue == "Private")
                        coin = Convert.ToInt32(txtCourseCoin.Text);

                    int courseId;
                    using (SqlCommand cmdCourse = new SqlCommand(insertCourseQuery, con, transaction))
                    {
                        cmdCourse.Parameters.AddWithValue("@Title", txtCourseTitle.Text.Trim());
                        cmdCourse.Parameters.AddWithValue("@EducatorId", educatorId);
                        cmdCourse.Parameters.AddWithValue("@CourseType", ddlCourseType.SelectedValue);
                        cmdCourse.Parameters.AddWithValue("@CoursePicture", DBNull.Value); // Placeholder
                        cmdCourse.Parameters.AddWithValue("@Status", "Active");
                        cmdCourse.Parameters.AddWithValue("@Coin", coin);
                        courseId = (int)cmdCourse.ExecuteScalar();
                    }

                    // ✅ 3. Insert Lessons, Quizzes, and Questions
                    foreach (Lesson lesson in lessonList)
                    {
                        // 3a. Insert Lesson
                        string insertLessonQuery = @"
                            INSERT INTO Lesson (CourseId, LessonNumber, LessonTitle, ContentType, ContentFilePath, ContentFile)
                            OUTPUT INSERTED.Id
                            VALUES (@CourseId, @LessonNumber, @LessonTitle, @ContentType, @ContentFilePath, @ContentFile)";

                        int lessonId;
                        using (SqlCommand cmdLesson = new SqlCommand(insertLessonQuery, con, transaction))
                        {
                            cmdLesson.Parameters.AddWithValue("@CourseId", courseId);
                            cmdLesson.Parameters.AddWithValue("@LessonNumber", lesson.LessonNumber);
                            cmdLesson.Parameters.AddWithValue("@LessonTitle", lesson.LessonTitle);
                            cmdLesson.Parameters.AddWithValue("@ContentType", (object)lesson.ContentType ?? DBNull.Value);
                            cmdLesson.Parameters.AddWithValue("@ContentFilePath", (object)lesson.ContentFilePath ?? DBNull.Value);
                            cmdLesson.Parameters.AddWithValue("@ContentFile", (object)lesson.ContentFile ?? DBNull.Value);
                            lessonId = (int)cmdLesson.ExecuteScalar(); // Get the new Lesson ID
                        }

                        // 3b. Check for and Insert Quiz
                        if (lesson.LessonQuiz != null && lesson.LessonQuiz.Questions.Count > 0)
                        {
                            // 3c. Insert Quiz
                            string insertQuizQuery = @"
                                INSERT INTO Quiz (LessonId, QuizRewardCoins, TotalQuestions)
                                OUTPUT INSERTED.Id
                                VALUES (@LessonId, @QuizRewardCoins, @TotalQuestions)";

                            int quizId;
                            using (SqlCommand cmdQuiz = new SqlCommand(insertQuizQuery, con, transaction))
                            {
                                cmdQuiz.Parameters.AddWithValue("@LessonId", lessonId);
                                cmdQuiz.Parameters.AddWithValue("@QuizRewardCoins", (object)lesson.LessonQuiz.QuizRewardCoins ?? DBNull.Value);
                                cmdQuiz.Parameters.AddWithValue("@TotalQuestions", lesson.LessonQuiz.Questions.Count);
                                quizId = (int)cmdQuiz.ExecuteScalar(); // Get the new Quiz ID
                            }

                            // 3d. Insert Questions
                            foreach (Question question in lesson.LessonQuiz.Questions)
                            {
                                string insertQuestionQuery = @"
                                    INSERT INTO Question (QuizId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer)
                                    VALUES (@QuizId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer)";

                                using (SqlCommand cmdQuestion = new SqlCommand(insertQuestionQuery, con, transaction))
                                {
                                    cmdQuestion.Parameters.AddWithValue("@QuizId", quizId);
                                    cmdQuestion.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                                    cmdQuestion.Parameters.AddWithValue("@OptionA", (object)question.OptionA ?? DBNull.Value);
                                    cmdQuestion.Parameters.AddWithValue("@OptionB", (object)question.OptionB ?? DBNull.Value);
                                    cmdQuestion.Parameters.AddWithValue("@OptionC", (object)question.OptionC ?? DBNull.Value);
                                    cmdQuestion.Parameters.AddWithValue("@OptionD", (object)question.OptionD ?? DBNull.Value);
                                    cmdQuestion.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer);
                                    cmdQuestion.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    // If all successful, commit the transaction
                    transaction.Commit();

                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    lblStatus.Text = "✅ Course, lessons, and quizzes saved successfully!";

                    // Clear everything
                    lessonList.Clear();
                    tempQuestionList.Clear();
                    BindLessonGrid();
                    BindTempQuestionGrid();
                    BindQuizLessonDropdown();
                    UpdateLessonCountLabel();
                    txtCourseTitle.Text = "";
                    ddlCourseType.SelectedIndex = 0;
                    coinDiv.Visible = false;
                    txtCourseCoin.Text = "";
                }
            }
            catch (Exception ex)
            {
                // Something went wrong, roll back
                try
                {
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch (Exception rbEx)
                {
                    // Log rollback exception
                }

                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error saving course: " + ex.Message;
            }
        }
        #endregion

        #region Data Models (Classes)
        public class Lesson
        {
            public int LessonNumber { get; set; }
            public string LessonTitle { get; set; }
            public string ContentType { get; set; }
            public string ContentFilePath { get; set; }
            public string ContentFile { get; set; }
            public Quiz LessonQuiz { get; set; } // Can hold one quiz
        }

        public class Quiz
        {
            public int? QuizRewardCoins { get; set; }
            public List<Question> Questions { get; set; }

            public Quiz()
            {
                Questions = new List<Question>();
            }
        }

        public class Question
        {
            public int QuestionNumber { get; set; } // Temporary ID for the list
            public string QuestionText { get; set; }
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public string CorrectAnswer { get; set; } // "A", "B", "C", or "D"
        }
        #endregion
    }
}