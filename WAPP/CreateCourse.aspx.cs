using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace WAPP
{
    public partial class CreateCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        // Temp storage of questions for the lesson currently being built (ViewState)
        private DataTable TempQuestionTable
        {
            get
            {
                if (ViewState["TempQuestionTable"] == null)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("QuestionText", typeof(string));
                    dt.Columns.Add("OptionA", typeof(string));
                    dt.Columns.Add("OptionB", typeof(string));
                    dt.Columns.Add("OptionC", typeof(string));
                    dt.Columns.Add("OptionD", typeof(string));
                    dt.Columns.Add("CorrectAnswer", typeof(string));
                    ViewState["TempQuestionTable"] = dt;
                }
                return (DataTable)ViewState["TempQuestionTable"];
            }
            set { ViewState["TempQuestionTable"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ensure educator session exists (demo fallback)
                if (Session["EducatorID"] == null)
                {
                    Session["EducatorID"] = 3000;
                    Session["EducatorName"] = "Educator";
                }
                lblEducatorName.Text = Session["EducatorName"].ToString();

                // create draft course if needed
                if (Session["NewCourseId"] == null)
                {
                    int newCourseId = CreateDraftCourse();
                    Session["NewCourseId"] = newCourseId;
                }

                LoadCourseInfoToUI();
                BindLessons();
                pnlQuiz.Visible = false;
            }
        }

        private int CreateDraftCourse()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"INSERT INTO Course (Title, EducatorId, CourseType, Status) 
                       VALUES (@title, @eid, @type, @status); 
                       SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Add all parameters before executing
                    cmd.Parameters.AddWithValue("@title", ""); // empty string instead of "Enter Course Title"
                    cmd.Parameters.AddWithValue("@eid", Convert.ToInt32(Session["EducatorID"]));
                    cmd.Parameters.AddWithValue("@type", "public");
                    cmd.Parameters.AddWithValue("@status", "Draft");

                    // Execute and return new Course ID
                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    return id;
                }
            }
        }


        private void LoadCourseInfoToUI()
        {
            if (Session["NewCourseId"] == null) return;
            int cid = Convert.ToInt32(Session["NewCourseId"]);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Title, CourseType FROM Course WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", cid);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            txtCourseTitle.Text = dr["Title"]?.ToString();
                            string type = dr["CourseType"]?.ToString();
                            rblCourseType.SelectedValue = string.IsNullOrEmpty(type) ? "public" : type.ToLower();
                        }
                    }
                }
            }
        }

        

        protected void btnOpenQuiz_Click(object sender, EventArgs e)
        {
            pnlQuiz.Visible = true;
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            string q = txtQuestionText.Text.Trim();
            if (string.IsNullOrEmpty(q))
            {
                lblAddLessonMsg.Text = "Please enter a question.";
                lblAddLessonMsg.ForeColor = System.Drawing.Color.Red;
                return;
            }

            DataTable dt = TempQuestionTable;
            DataRow r = dt.NewRow();
            r["QuestionText"] = q;
            r["OptionA"] = txtOptA.Text.Trim();
            r["OptionB"] = txtOptB.Text.Trim();
            r["OptionC"] = txtOptC.Text.Trim();
            r["OptionD"] = txtOptD.Text.Trim();
            r["CorrectAnswer"] = rblCorrect.SelectedValue ?? "A";
            dt.Rows.Add(r);
            TempQuestionTable = dt;

            rptTempQuestions.DataSource = dt;
            rptTempQuestions.DataBind();

            // clear inputs
            txtQuestionText.Text = "";
            txtOptA.Text = "";
            txtOptB.Text = "";
            txtOptC.Text = "";
            txtOptD.Text = "";
            rblCorrect.ClearSelection();
            lblAddLessonMsg.Text = "";
        }

        protected void btnDoneQuiz_Click(object sender, EventArgs e)
        {
            pnlQuiz.Visible = false;
        }

        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            if (Session["NewCourseId"] == null) return;
            int courseId = Convert.ToInt32(Session["NewCourseId"]);

            string lessonTitle = txtNewLessonTitle.Text.Trim();
            string lessonContent = txtNewLessonContent.Text.Trim();
            if (string.IsNullOrEmpty(lessonTitle))
            {
                lblAddLessonMsg.Text = "Please enter a lesson title.";
                lblAddLessonMsg.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string contentType = "text";
            string contentPath = null;

            // handle file upload if present
            if (fuLessonFile.HasFile)
            {
                string uploads = Server.MapPath("~/uploads/");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                string unique = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileName(fuLessonFile.FileName);
                string full = Path.Combine(uploads, unique);
                fuLessonFile.SaveAs(full);
                contentType = "file";
                contentPath = "~/uploads/" + unique;
            }
            else if (!string.IsNullOrEmpty(lessonContent))
            {
                // save text to txt file to keep file-path column consistent
                string uploads = Server.MapPath("~/uploads/");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                string unique = "lesson_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                string full = Path.Combine(uploads, unique);
                File.WriteAllText(full, lessonContent);
                contentType = "text";
                contentPath = "~/uploads/" + unique;
            }

            DataTable qdt = TempQuestionTable; // may be empty

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // compute next lesson number
                    int lessonNumber = 1;
                    using (SqlCommand cmdNum = new SqlCommand("SELECT ISNULL(MAX(LessonNumber),0) FROM Lesson WHERE CourseId=@cid", conn, tran))
                    {
                        cmdNum.Parameters.AddWithValue("@cid", courseId);
                        lessonNumber = Convert.ToInt32(cmdNum.ExecuteScalar()) + 1;
                    }

                    // insert lesson
                    string insertLesson = @"INSERT INTO Lesson (CourseId, LessonNumber, LessonTitle, ContentType, ContentFilePath)
                                            VALUES (@cid, @lnum, @ltitle, @ctype, @cpath);
                                            SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    int lessonId;
                    using (SqlCommand cmdL = new SqlCommand(insertLesson, conn, tran))
                    {
                        cmdL.Parameters.AddWithValue("@cid", courseId);
                        cmdL.Parameters.AddWithValue("@lnum", lessonNumber);
                        cmdL.Parameters.AddWithValue("@ltitle", lessonTitle);
                        cmdL.Parameters.AddWithValue("@ctype", contentType);
                        if (string.IsNullOrEmpty(contentPath)) cmdL.Parameters.AddWithValue("@cpath", DBNull.Value);
                        else cmdL.Parameters.AddWithValue("@cpath", contentPath);
                        lessonId = Convert.ToInt32(cmdL.ExecuteScalar());
                    }

                    // if questions exist, insert quiz and questions
                    if (qdt != null && qdt.Rows.Count > 0)
                    {
                        int coins = 0;
                        int.TryParse(txtQuizCoins.Text.Trim(), out coins);
                        int totalQ = qdt.Rows.Count;

                        string insertQuiz = @"INSERT INTO Quiz (LessonId, QuizRewardCoins, TotalQuestions)
                                              VALUES (@lid, @coins, @total);
                                              SELECT CAST(SCOPE_IDENTITY() AS INT);";
                        int quizId;
                        using (SqlCommand cmdQ = new SqlCommand(insertQuiz, conn, tran))
                        {
                            cmdQ.Parameters.AddWithValue("@lid", lessonId);
                            cmdQ.Parameters.AddWithValue("@coins", coins);
                            cmdQ.Parameters.AddWithValue("@total", totalQ);
                            quizId = Convert.ToInt32(cmdQ.ExecuteScalar());
                        }

                        string insertQuestion = @"INSERT INTO Question (QuizId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer)
                                                  VALUES (@qid, @qtext, @a, @b, @c, @d, @corr);";
                        using (SqlCommand cmdQQ = new SqlCommand(insertQuestion, conn, tran))
                        {
                            cmdQQ.Parameters.Add(new SqlParameter("@qid", System.Data.SqlDbType.Int));
                            cmdQQ.Parameters.Add(new SqlParameter("@qtext", System.Data.SqlDbType.NVarChar));
                            cmdQQ.Parameters.Add(new SqlParameter("@a", System.Data.SqlDbType.NVarChar));
                            cmdQQ.Parameters.Add(new SqlParameter("@b", System.Data.SqlDbType.NVarChar));
                            cmdQQ.Parameters.Add(new SqlParameter("@c", System.Data.SqlDbType.NVarChar));
                            cmdQQ.Parameters.Add(new SqlParameter("@d", System.Data.SqlDbType.NVarChar));
                            cmdQQ.Parameters.Add(new SqlParameter("@corr", System.Data.SqlDbType.NVarChar));

                            foreach (DataRow row in qdt.Rows)
                            {
                                cmdQQ.Parameters["@qid"].Value = quizId;
                                cmdQQ.Parameters["@qtext"].Value = row["QuestionText"].ToString();
                                cmdQQ.Parameters["@a"].Value = row["OptionA"].ToString();
                                cmdQQ.Parameters["@b"].Value = row["OptionB"].ToString();
                                cmdQQ.Parameters["@c"].Value = row["OptionC"].ToString();
                                cmdQQ.Parameters["@d"].Value = row["OptionD"].ToString();
                                cmdQQ.Parameters["@corr"].Value = row["CorrectAnswer"].ToString();
                                cmdQQ.ExecuteNonQuery();
                            }
                        }
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    lblAddLessonMsg.Text = "Error saving lesson: " + ex.Message;
                    lblAddLessonMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            // clear UI
            txtNewLessonTitle.Text = "";
            txtNewLessonContent.Text = "";
            if (fuLessonFile.HasFile) { /* file already saved */ }
            ViewState["TempQuestionTable"] = null;
            rptTempQuestions.DataSource = null;
            rptTempQuestions.DataBind();
            pnlQuiz.Visible = false;
            lblAddLessonMsg.Text = "Lesson added.";
            lblAddLessonMsg.ForeColor = System.Drawing.Color.Green;

            // refresh lesson list
            BindLessons();
        }

        private void BindLessons()
        {
            if (Session["NewCourseId"] == null) return;
            int courseId = Convert.ToInt32(Session["NewCourseId"]);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"SELECT l.Id, l.LessonNumber, l.LessonTitle,
                               CASE WHEN EXISTS(SELECT 1 FROM Quiz q WHERE q.LessonId = l.Id) THEN 1 ELSE 0 END AS HasQuiz
                               FROM Lesson l
                               WHERE l.CourseId = @cid
                               ORDER BY l.LessonNumber";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", courseId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            lvLessons.DataSource = dt;
            lvLessons.DataBind();
        }

        protected void lvLessons_ItemCommand(object sender, System.Web.UI.WebControls.ListViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteLesson")
            {
                int lessonId = Convert.ToInt32(e.CommandArgument);
                DeleteLesson(lessonId);
                BindLessons();
            }
            else if (e.CommandName == "EditLesson")
            {
                int lessonId = Convert.ToInt32(e.CommandArgument);
                LoadLessonForEdit(lessonId);
            }
        }

        private void DeleteLesson(int lessonId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"DELETE FROM Question WHERE QuizId IN (SELECT Id FROM Quiz WHERE LessonId=@lid);
                               DELETE FROM Quiz WHERE LessonId=@lid;
                               DELETE FROM Lesson WHERE Id=@lid;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@lid", lessonId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadLessonForEdit(int lessonId)
        {
            // load lesson details
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT LessonTitle, ContentType, ContentFilePath FROM Lesson WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", lessonId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            txtNewLessonTitle.Text = dr["LessonTitle"].ToString();
                            // Not loading file content into textarea — educator may re-upload or edit text manually
                        }
                    }
                }

                // load quiz questions if exists
                using (SqlCommand cmdQ = new SqlCommand("SELECT q.Id FROM Quiz q WHERE q.LessonId=@lid", conn))
                {
                    cmdQ.Parameters.AddWithValue("@lid", lessonId);
                    object qidObj = cmdQ.ExecuteScalar();
                    if (qidObj != null)
                    {
                        int qid = Convert.ToInt32(qidObj);
                        DataTable qdt = new DataTable();
                        using (SqlCommand cmdQuestions = new SqlCommand("SELECT QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer FROM Question WHERE QuizId=@qid", conn))
                        {
                            cmdQuestions.Parameters.AddWithValue("@qid", qid);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmdQuestions))
                            {
                                da.Fill(qdt);
                            }
                        }
                        TempQuestionTable = qdt;
                        rptTempQuestions.DataSource = qdt;
                        rptTempQuestions.DataBind();
                        pnlQuiz.Visible = true;
                    }
                    else
                    {
                        ViewState["TempQuestionTable"] = null;
                        rptTempQuestions.DataSource = null;
                        rptTempQuestions.DataBind();
                        pnlQuiz.Visible = false;
                    }
                }
            }

            // keep editing id so Save/Edit can update later (optional: implement update logic)
            // we'll store the lesson Id in hidden field to allow update in future
            hfEditingLessonId.Value = lessonId.ToString();
            lblAddLessonMsg.Text = "Loaded lesson for editing. Make changes and click Add Lesson to save as new (or implement update logic).";
            lblAddLessonMsg.ForeColor = System.Drawing.Color.Blue;
        }

        protected void btnCreateCourse_Click(object sender, EventArgs e)
        {
            if (Session["NewCourseId"] == null) return;
            int cid = Convert.ToInt32(Session["NewCourseId"]);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Course SET Status=@status, Title=@title, CourseType=@type WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@status", "Published");
                    cmd.Parameters.AddWithValue("@title", txtCourseTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@type", rblCourseType.SelectedValue ?? "public");
                    cmd.Parameters.AddWithValue("@id", cid);
                    cmd.ExecuteNonQuery();
                }
            }

            // clear session draft so next time a new draft is created
            Session.Remove("NewCourseId");
            Response.Redirect("Educator_dashboard.aspx");
        }
    }
}
