using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class CreateCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

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

            // ✅ Restore unsaved lesson data on every postback
            if (ViewState["UnsavedLessonTitle"] != null)
            {
                txtNewLessonTitle.Text = ViewState["UnsavedLessonTitle"].ToString();
            }
            if (ViewState["UnsavedLessonContent"] != null)
            {
                txtNewLessonContent.Text = ViewState["UnsavedLessonContent"].ToString();
            }
            if (ViewState["UnsavedQuizCoins"] != null)
            {
                txtQuizCoins.Text = ViewState["UnsavedQuizCoins"].ToString();
            }
            // ✅ Restore dynamic quiz questions if validation failed
            if (IsPostBack && !string.IsNullOrEmpty(hfQuizData.Value))
            {
                string json = hfQuizData.Value;
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var questions = js.Deserialize<List<Dictionary<string, string>>>(json);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                int qnum = 0;
                foreach (var q in questions)
                {
                    qnum++;
                    sb.Append($@"
                        <div class='question-item' style='padding:16px; border:1px solid #e5e7eb; border-radius:8px; margin-bottom:20px;'>
                        <div style='font-weight:600; margin-bottom:10px;'>Question {qnum}</div>
                        <input type='text' name='q{qnum}_text' class='input' value='{q["text"]}' placeholder='Enter question text' required />
                        <input type='text' name='q{qnum}_a' class='input' value='{q["a"]}' placeholder='Option A' required />
                        <input type='text' name='q{qnum}_b' class='input' value='{q["b"]}' placeholder='Option B' required />
                        <input type='text' name='q{qnum}_c' class='input' value='{q["c"]}' placeholder='Option C' required />
                        <input type='text' name='q{qnum}_d' class='input' value='{q["d"]}' placeholder='Option D' required />
                        <label style='margin-top:10px;'>Correct Answer</label>
                        <div style='margin-bottom:6px;'>
                            <label style='margin-right:10px;'><input type='radio' name='q{qnum}_correct' value='A' {(q["correct"] == "A" ? "checked" : "")} /> A</label>
                            <label style='margin-right:10px;'><input type='radio' name='q{qnum}_correct' value='B' {(q["correct"] == "B" ? "checked" : "")} /> B</label>
                            <label style='margin-right:10px;'><input type='radio' name='q{qnum}_correct' value='C' {(q["correct"] == "C" ? "checked" : "")} /> C</label>
                            <label><input type='radio' name='q{qnum}_correct' value='D' {(q["correct"] == "D" ? "checked" : "")} /> D</label>
                         </div>
                    </div>");
                }
                questionsContainer.InnerHtml = sb.ToString();
                pnlQuiz.Visible = true;
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
                    cmd.Parameters.AddWithValue("@title", "");
                    cmd.Parameters.AddWithValue("@eid", Convert.ToInt32(Session["EducatorID"]));
                    cmd.Parameters.AddWithValue("@type", "public");
                    cmd.Parameters.AddWithValue("@status", "Draft");

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
                using (SqlCommand cmd = new SqlCommand("SELECT Title, CourseType, Coin FROM Course WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", cid);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            txtCourseTitle.Text = dr["Title"]?.ToString();
                            string type = dr["CourseType"]?.ToString();
                            rblCourseType.SelectedValue = string.IsNullOrEmpty(type) ? "public" : type.ToLower();

                            if (type?.ToLower() == "private" && dr["Coin"] != DBNull.Value)
                            {
                                txtCoursePrice.Text = dr["Coin"].ToString();
                            }
                        }
                    }
                }
            }
        }

        protected void btnOpenQuiz_Click(object sender, EventArgs e)
        {
            pnlQuiz.Visible = true;
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

                // ✅ Keep quiz panel open
                pnlQuiz.Visible = true;

                // ✅ Save what the educator has entered so far
                ViewState["UnsavedLessonTitle"] = txtNewLessonTitle.Text;
                ViewState["UnsavedLessonContent"] = txtNewLessonContent.Text;
                ViewState["UnsavedQuizCoins"] = txtQuizCoins.Text;

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
                string uploads = Server.MapPath("~/uploads/");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                string unique = "lesson_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                string full = Path.Combine(uploads, unique);
                File.WriteAllText(full, lessonContent);
                contentType = "text";
                contentPath = "~/uploads/" + unique;
            }

            // ✅ Collect all dynamic questions from Request
            List<(string QText, string A, string B, string C, string D, string Correct)> questions = new List<(string, string, string, string, string, string)>();
            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("q") && key.Contains("_text"))
                {
                    string prefix = key.Split('_')[0]; // e.g. "q1"
                    string qtext = Request.Form[key].Trim();
                    string a = Request.Form[prefix + "_a"];
                    string b = Request.Form[prefix + "_b"];
                    string c = Request.Form[prefix + "_c"];
                    string d = Request.Form[prefix + "_d"];
                    string correct = Request.Form[prefix + "_correct"];

                    if (!string.IsNullOrEmpty(qtext))
                        questions.Add((qtext, a, b, c, d, correct));
                }
            }

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
                    string insertLesson = @"INSERT INTO Lesson 
                        (CourseId, LessonNumber, LessonTitle, ContentType, ContentFilePath, ContentFile)
                        VALUES (@cid, @lnum, @ltitle, @ctype, @cpath, @contentFile);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int lessonId;
                    using (SqlCommand cmdL = new SqlCommand(insertLesson, conn, tran))
                    {
                        cmdL.Parameters.AddWithValue("@cid", courseId);
                        cmdL.Parameters.AddWithValue("@lnum", lessonNumber);
                        cmdL.Parameters.AddWithValue("@ltitle", lessonTitle);
                        cmdL.Parameters.AddWithValue("@ctype", contentType);
                        if (fuLessonFile.HasFile)
                        {
                            // User uploaded a file
                            cmdL.Parameters.AddWithValue("@cpath", (object)contentPath ?? DBNull.Value);
                            cmdL.Parameters.AddWithValue("@contentFile", DBNull.Value);
                        }
                        else
                        {
                            // User entered text content
                            string textContent = txtNewLessonContent.Text.Trim();
                            cmdL.Parameters.AddWithValue("@cpath", DBNull.Value);
                            cmdL.Parameters.AddWithValue("@contentFile", string.IsNullOrEmpty(textContent) ? DBNull.Value : (object)textContent);
                        }

                        lessonId = Convert.ToInt32(cmdL.ExecuteScalar());
                    }

                    // insert quiz + questions if any exist
                    if (questions.Count > 0)
                    {
                        int coins = 0;
                        int.TryParse(txtQuizCoins.Text.Trim(), out coins);
                        int totalQ = questions.Count;

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
                            cmdQQ.Parameters.Add("@qid", SqlDbType.Int);
                            cmdQQ.Parameters.Add("@qtext", SqlDbType.NVarChar);
                            cmdQQ.Parameters.Add("@a", SqlDbType.NVarChar);
                            cmdQQ.Parameters.Add("@b", SqlDbType.NVarChar);
                            cmdQQ.Parameters.Add("@c", SqlDbType.NVarChar);
                            cmdQQ.Parameters.Add("@d", SqlDbType.NVarChar);
                            cmdQQ.Parameters.Add("@corr", SqlDbType.NVarChar);

                            foreach (var q in questions)
                            {
                                cmdQQ.Parameters["@qid"].Value = quizId;
                                cmdQQ.Parameters["@qtext"].Value = q.QText;
                                cmdQQ.Parameters["@a"].Value = q.A;
                                cmdQQ.Parameters["@b"].Value = q.B;
                                cmdQQ.Parameters["@c"].Value = q.C;
                                cmdQQ.Parameters["@d"].Value = q.D;
                                cmdQQ.Parameters["@corr"].Value = q.Correct;
                                cmdQQ.ExecuteNonQuery();
                            }
                        }
                    }

                    tran.Commit();

                    // ✅ Only clear when successfully added
                    txtNewLessonTitle.Text = "";
                    txtNewLessonContent.Text = "";
                    ViewState["TempQuestionTable"] = null;
                    pnlQuiz.Visible = false;

                    lblAddLessonMsg.Text = "Lesson added successfully.";
                    lblAddLessonMsg.ForeColor = System.Drawing.Color.Green;

                    // ✅ Clear any temporarily stored unsaved data
                    ViewState["UnsavedLessonTitle"] = null;
                    ViewState["UnsavedLessonContent"] = null;
                    ViewState["UnsavedQuizCoins"] = null;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    lblAddLessonMsg.Text = "Error saving lesson: " + ex.Message;
                    lblAddLessonMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }
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

        protected void lvLessons_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteLesson")
            {
                int lessonId = Convert.ToInt32(e.CommandArgument);
                DeleteLesson(lessonId); // calls your existing function
                BindLessons(); // refresh the list
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

        protected void btnCreateCourse_Click(object sender, EventArgs e)
        {
            if (Session["NewCourseId"] == null)
            {
                lblMessage.Text = "⚠️ Course session not found. Please refresh and try again.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            int cid = Convert.ToInt32(Session["NewCourseId"]);
            string courseTitle = txtCourseTitle.Text.Trim();
            string courseType = rblCourseType.SelectedValue ?? "public";
            int coursePrice = 0;

            // 🧠 1️⃣ Validate course title
            if (string.IsNullOrEmpty(courseTitle))
            {
                lblMessage.Text = "⚠️ Please enter a course title before creating the course.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // 🧠 2️⃣ Validate course price if private
            if (courseType == "private")
            {
                if (!int.TryParse(txtCoursePrice.Text.Trim(), out coursePrice) || coursePrice < 10 || coursePrice > 250)
                {
                    lblMessage.Text = "⚠️ Please enter a valid whole number between 10 and 250 for the course price.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            // 🧠 3️⃣ Save course info to database (update existing draft)
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"UPDATE Course
                           SET Title=@title,
                               CourseType=@type,
                               Coin=@coin,
                               Status='Published'
                           WHERE Id=@id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", courseTitle);
                        cmd.Parameters.AddWithValue("@type", courseType);
                        cmd.Parameters.AddWithValue("@coin", coursePrice);
                        cmd.Parameters.AddWithValue("@id", cid);
                        cmd.ExecuteNonQuery();
                    }
                }

                // ✅ Feedback message
                lblMessage.Text = "Course created and published successfully!";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error saving course: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

    }
}
