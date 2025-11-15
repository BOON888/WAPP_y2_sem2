using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WAPP
{
    // Make sure to change the class name to match the file
    public partial class editCourse : System.Web.UI.Page
    {
        private static List<Lesson> lessonList = new List<Lesson>();
        private static List<Question> tempQuestionList = new List<Question>(); // For building a quiz

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear all temporary data on initial page load
                lessonList.Clear();
                tempQuestionList.Clear();

                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int courseId))
                {
                    // Store the CourseId to use it in the Update button click
                    ViewState["CourseId"] = courseId;

                    // Load all existing data from the DB into the static lists
                    LoadCourseData(courseId);
                }
                else
                {
                    // No ID provided, show error and hide content
                    lblMainStatus.ForeColor = System.Drawing.Color.Red;
                    lblMainStatus.Text = "Error: No course ID provided or invalid ID. Please go back and select a course to edit.";
                    btnUpdateCourse.Enabled = false;
                    return;
                }

                // Bind all grids and dropdowns with the loaded data
                BindLessonGrid();
                UpdateLessonCountLabel();
                BindQuizLessonDropdown();
                BindTempQuestionGrid();
            }
        }

        private void LoadCourseData(int courseId)
        {
            // 1. Get EducatorId from Session to verify ownership
            int educatorId = 0;
            if (Session["UserId"] != null)
            {
                educatorId = GetEducatorId(Convert.ToInt32(Session["UserId"]));
            }

            if (educatorId == 0)
            {
                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "Error: Could not find your educator profile.";
                btnUpdateCourse.Enabled = false;
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 2. Load Course Details and verify ownership
                string courseQuery = "SELECT Title, CourseType, Coin FROM Course WHERE Id = @CourseId AND EducatorId = @EducatorId";
                using (SqlCommand cmdCourse = new SqlCommand(courseQuery, con))
                {
                    cmdCourse.Parameters.AddWithValue("@CourseId", courseId);
                    cmdCourse.Parameters.AddWithValue("@EducatorId", educatorId);

                    using (SqlDataReader reader = cmdCourse.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtCourseTitle.Text = reader["Title"].ToString();
                            ddlCourseType.SelectedValue = reader["CourseType"].ToString();
                            txtCourseCoin.Text = reader["Coin"].ToString();

                            if (ddlCourseType.SelectedValue == "Private")
                            {
                                coinDiv.Visible = true;
                            }
                        }
                        else
                        {
                            // Course not found or user does not own it
                            lblMainStatus.ForeColor = System.Drawing.Color.Red;
                            lblMainStatus.Text = "Error: Course not found or you do not have permission to edit it.";
                            btnUpdateCourse.Enabled = false;
                            reader.Close();
                            return;
                        }
                    }
                }

                // 3. Load Lessons
                string lessonQuery = "SELECT Id, LessonNumber, LessonTitle, ContentType, ContentFilePath, ContentFile FROM Lesson WHERE CourseId = @CourseId ORDER BY LessonNumber";
                Dictionary<int, Lesson> lessonsById = new Dictionary<int, Lesson>(); // To map LessonID to Lesson object

                using (SqlCommand cmdLesson = new SqlCommand(lessonQuery, con))
                {
                    cmdLesson.Parameters.AddWithValue("@CourseId", courseId);
                    using (SqlDataReader lessonReader = cmdLesson.ExecuteReader())
                    {
                        while (lessonReader.Read())
                        {
                            Lesson lesson = new Lesson
                            {
                                LessonNumber = Convert.ToInt32(lessonReader["LessonNumber"]),
                                LessonTitle = lessonReader["LessonTitle"].ToString(),
                                ContentType = lessonReader["ContentType"].ToString(),
                                ContentFilePath = lessonReader["ContentFilePath"].ToString(),
                                ContentFile = lessonReader["ContentFile"].ToString(),
                                LessonQuiz = null
                            };
                            lessonList.Add(lesson);
                            lessonsById.Add(Convert.ToInt32(lessonReader["Id"]), lesson); // Map DB Id to the object
                        }
                    }
                }

                // 4. Load Quizzes and Questions
                string quizQuery = @"
                    SELECT 
                        q.Id AS QuizId, q.LessonId, q.QuizRewardCoins,
                        qn.Id AS QuestionId, qn.QuestionText, 
                        qn.OptionA, qn.OptionB, qn.OptionC, qn.OptionD, qn.CorrectAnswer
                    FROM Quiz q
                    LEFT JOIN Question qn ON q.Id = qn.QuizId
                    WHERE q.LessonId IN (SELECT Id FROM Lesson WHERE CourseId = @CourseId)
                    ORDER BY q.LessonId, qn.Id"; // Order by the Question's primary key

                using (SqlCommand cmdQuiz = new SqlCommand(quizQuery, con))
                {
                    cmdQuiz.Parameters.AddWithValue("@CourseId", courseId);
                    using (SqlDataReader quizReader = cmdQuiz.ExecuteReader())
                    {
                        Quiz currentQuiz = null;
                        int lastLessonId = -1;

                        while (quizReader.Read())
                        {
                            int lessonId = Convert.ToInt32(quizReader["LessonId"]);

                            // Check if this row is for a new quiz
                            if (lastLessonId != lessonId)
                            {
                                lastLessonId = lessonId;
                                currentQuiz = new Quiz();
                                if (quizReader["QuizRewardCoins"] != DBNull.Value)
                                    currentQuiz.QuizRewardCoins = Convert.ToInt32(quizReader["QuizRewardCoins"]);

                                // Find the lesson object this quiz belongs to
                                if (lessonsById.ContainsKey(lessonId))
                                {
                                    lessonsById[lessonId].LessonQuiz = currentQuiz;
                                }
                            }

                            // Add question to the current quiz (if one exists)
                            if (currentQuiz != null && quizReader["QuestionId"] != DBNull.Value)
                            {
                                Question question = new Question
                                {
                                    QuestionNumber = currentQuiz.Questions.Count + 1,
                                    QuestionText = quizReader["QuestionText"].ToString(),
                                    OptionA = quizReader["OptionA"].ToString(),
                                    OptionB = quizReader["OptionB"].ToString(),
                                    OptionC = quizReader["OptionC"].ToString(),
                                    OptionD = quizReader["OptionD"].ToString(),
                                    CorrectAnswer = quizReader["CorrectAnswer"].ToString()
                                };
                                currentQuiz.Questions.Add(question);
                            }
                        }
                    }
                }
            }
        }

        private int GetEducatorId(int userId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string findEducatorQuery = "SELECT Id FROM Educator WHERE UserId = @UserId";
                using (SqlCommand findCmd = new SqlCommand(findEducatorQuery, con))
                {
                    findCmd.Parameters.AddWithValue("@UserId", userId);
                    object result = findCmd.ExecuteScalar();
                    if (result != null)
                        return Convert.ToInt32(result);
                    else
                        return 0; // Not found
                }
            }
        }

        #region Course Methods
        // This is kept to ensure the coinDiv visibility is set correctly on load
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
                if (fuContentFile.HasFile && fuContentFile.PostedFile.ContentLength > 52428800) // 50MB
                {
                    lblMainStatus.ForeColor = System.Drawing.Color.Red;
                    lblMainStatus.Text = "Lesson File: Size exceeds 50MB limit.";
                    return;
                }
                string lessonTitle = txtLessonTitle.Text.Trim();
                if (string.IsNullOrEmpty(lessonTitle))
                {
                    lblMainStatus.ForeColor = System.Drawing.Color.Red;
                    lblMainStatus.Text = "Please enter a lesson title.";
                    return;
                }

                string contentType = "Document"; 
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
                        lblMainStatus.ForeColor = System.Drawing.Color.Red;
                        lblMainStatus.Text = "Error uploading file: " + ex.Message;
                        return;
                    }
                }
                // End of file upload logic

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

                lblMainStatus.ForeColor = System.Drawing.Color.Green;
                lblMainStatus.Text = "Lesson added to list. Remember to click 'Update Course' to save all changes.";
            }
            catch (Exception ex)
            {
                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "Error adding lesson: " + ex.Message;
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

        protected void gvLessons_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvLessons.EditIndex = e.NewEditIndex;
            BindLessonGrid();
        }

        protected void gvLessons_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
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
                        if (fuEditFile.PostedFile.ContentLength > 52428800)
                        {
                            lblMainStatus.ForeColor = System.Drawing.Color.Red;
                            lblMainStatus.Text = "File size exceeds 50MB limit.";
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
                lblMainStatus.ForeColor = System.Drawing.Color.Green;
                lblMainStatus.Text = "Lesson updated in list. Remember to click 'Update Course' to save.";
            }
            catch (Exception ex)
            {
                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "Error updating lesson: " + ex.Message;
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
                lblMainStatus.ForeColor = System.Drawing.Color.Green;
                lblMainStatus.Text = "Lesson deleted from list. Remember to click 'Update Course' to save.";
            }
            catch (Exception ex)
            {
                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "Error deleting lesson: " + ex.Message;
            }
        }

        // Show Quiz Status in Lesson Grid
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

        // Handle "Edit Quiz" / "Delete Quiz" from Lesson Grid
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
                    lblMainStatus.Text = $"Editing quiz for: {lesson.LessonTitle}";
                    lblMainStatus.ForeColor = System.Drawing.Color.Blue;
                }
                else if (e.CommandName == "DeleteQuiz")
                {
                    // Confirm deletion
                    lesson.LessonQuiz = null;
                    BindLessonGrid(); // Re-binds lesson grid, RowDataBound updates status
                    lblMainStatus.Text = "Quiz removed from lesson. Remember to click 'Update Course' to save.";
                    lblMainStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
        }
        #endregion

        #region Quiz & Question Methods
        
        private void BindQuizLessonDropdown()
        {
            ddlQuizLesson.DataSource = lessonList;
            ddlQuizLesson.DataTextField = "LessonTitle";
            ddlQuizLesson.DataValueField = "LessonNumber";
            ddlQuizLesson.DataBind();
            ddlQuizLesson.Items.Insert(0, new ListItem("Select Lesson...", ""));
        }

        // Binds the temporary question list grid
        private void BindTempQuestionGrid()
        {
            gvTempQuestions.DataKeyNames = new string[] { "QuestionNumber" };
            gvTempQuestions.DataSource = tempQuestionList;
            gvTempQuestions.DataBind();
        }

        // Clears the question input form
        private void ClearQuestionForm()
        {
            txtQuestionText.Text = "";
            txtOptionA.Text = "";
            txtOptionB.Text = "";
            txtOptionC.Text = "";
            txtOptionD.Text = "";
            ddlCorrectAnswer.SelectedIndex = 0;
        }

        // Adds a question to the temporary list
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

        // Saves the temp question list as a Quiz object on the selected Lesson
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
                    lblMainStatus.Text = "Quiz updated successfully! Remember to click 'Update Course' to save.";
                    lblMainStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblMainStatus.Text = "Quiz saved to lesson successfully! Remember to click 'Update Course' to save.";
                    lblMainStatus.ForeColor = System.Drawing.Color.Green;
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

        // Button to clear the quiz form
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

        #region Update Course Button (Main Save)
        protected void btnUpdateCourse_Click(object sender, EventArgs e)
        {
            // VALIDATION
            if (lessonList.Count == 0)
            {
                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "You cannot save a course with zero lessons.";
                return;
            }
            // END VALIDATION

            // Get the CourseId and EducatorId to update
            if (ViewState["CourseId"] == null || Session["UserId"] == null)
            {
                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "Error: Session expired or course ID lost. Please log in and try again.";
                return;
            }

            int courseId = (int)ViewState["CourseId"];
            int educatorId = GetEducatorId(Convert.ToInt32(Session["UserId"]));

            if (educatorId == 0)
            {
                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "Error: Could not find your educator profile.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            SqlConnection con = null;
            SqlTransaction transaction = null;

            try
            {
                using (con = new SqlConnection(connStr))
                {
                    con.Open();
                    transaction = con.BeginTransaction(); // Start a transaction

                    // 1. Verify ownership again (security check)
                    string checkOwnerQuery = "SELECT COUNT(*) FROM Course WHERE Id = @CourseId AND EducatorId = @EducatorId";
                    using (SqlCommand checkCmd = new SqlCommand(checkOwnerQuery, con, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CourseId", courseId);
                        checkCmd.Parameters.AddWithValue("@EducatorId", educatorId);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count == 0)
                        {
                            throw new Exception("Access Denied. You do not own this course.");
                        }
                    }

                    // 2. Delete ALL existing children of this course
                    
                    // Delete StudentQuizProgress first
                    string deleteProgressQuery = @"
                        DELETE sqp FROM StudentQuizProgress sqp
                        INNER JOIN Quiz q ON sqp.QuizId = q.Id
                        INNER JOIN Lesson l ON q.LessonId = l.Id
                        WHERE l.CourseId = @CourseId";
                    using (SqlCommand delProgCmd = new SqlCommand(deleteProgressQuery, con, transaction))
                    {
                        delProgCmd.Parameters.AddWithValue("@CourseId", courseId);
                        delProgCmd.ExecuteNonQuery();
                    }

                    // Delete Questions
                    string deleteQuestionsQuery = @"
                        DELETE qn FROM Question qn
                        INNER JOIN Quiz q ON qn.QuizId = q.Id
                        INNER JOIN Lesson l ON q.LessonId = l.Id
                        WHERE l.CourseId = @CourseId";
                    using (SqlCommand delQnCmd = new SqlCommand(deleteQuestionsQuery, con, transaction))
                    {
                        delQnCmd.Parameters.AddWithValue("@CourseId", courseId);
                        delQnCmd.ExecuteNonQuery();
                    }

                    // Delete Quizzes
                    string deleteQuizQuery = @"
                        DELETE q FROM Quiz q
                        INNER JOIN Lesson l ON q.LessonId = l.Id
                        WHERE l.CourseId = @CourseId";
                    using (SqlCommand delQuizCmd = new SqlCommand(deleteQuizQuery, con, transaction))
                    {
                        delQuizCmd.Parameters.AddWithValue("@CourseId", courseId);
                        delQuizCmd.ExecuteNonQuery();
                    }

                    // Delete Lessons
                    string deleteLessonQuery = "DELETE FROM Lesson WHERE CourseId = @CourseId";
                    using (SqlCommand delLessonCmd = new SqlCommand(deleteLessonQuery, con, transaction))
                    {
                        delLessonCmd.Parameters.AddWithValue("@CourseId", courseId);
                        delLessonCmd.ExecuteNonQuery();
                    }


                    // 3. Re-Insert Lessons, Quizzes, and Questions from the 'lessonList'
                    
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
                            cmdLesson.Parameters.AddWithValue("@CourseId", courseId); // Use existing CourseId
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

                    lblMainStatus.ForeColor = System.Drawing.Color.Green;
                    lblMainStatus.Text = "Course, lessons, and quizzes updated successfully!";

                    BindLessonGrid();
                    BindTempQuestionGrid();
                    BindQuizLessonDropdown();
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

                lblMainStatus.ForeColor = System.Drawing.Color.Red;
                lblMainStatus.Text = "Error updating course: " + ex.Message;
            }
        }
        #endregion

        #region Data Models (Classes)
        // COPIED IDENTICALLY
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