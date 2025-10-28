using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class EditCourse : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
        int courseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] == null)
            {
                Response.Redirect("Educator_dashboard.aspx");
                return;
            }

            courseId = Convert.ToInt32(Request.QueryString["id"]);

            if (!IsPostBack)
            {
                LoadCourseInfo();
                BindLessons();
            }
        }

        private void LoadCourseInfo()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT Title, CourseType, Coin FROM Course WHERE Id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", courseId);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtCourseTitle.Text = dr["Title"].ToString();
                    rblCourseType.SelectedValue = dr["CourseType"].ToString().ToLower();
                    txtCoursePrice.Text = dr["Coin"] != DBNull.Value ? dr["Coin"].ToString() : "";
                }
                dr.Close();
            }
        }

        private void BindLessons()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"SELECT Id, LessonNumber, LessonTitle FROM Lesson WHERE CourseId=@cid ORDER BY LessonNumber";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", courseId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            lvLessons.DataSource = dt;
            lvLessons.DataBind();
        }

        protected void btnAddLesson_Click(object sender, EventArgs e)
        {
            string title = txtLessonTitle.Text.Trim();
            string content = txtLessonContent.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                lblLessonMsg.Text = "⚠ Please enter a lesson title.";
                lblLessonMsg.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string filePath = null;
            if (fuLessonFile.HasFile)
            {
                string folder = Server.MapPath("~/uploads/");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fuLessonFile.FileName;
                fuLessonFile.SaveAs(Path.Combine(folder, fileName));
                filePath = "~/uploads/" + fileName;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sqlNum = "SELECT ISNULL(MAX(LessonNumber),0)+1 FROM Lesson WHERE CourseId=@cid";
                SqlCommand cmdNum = new SqlCommand(sqlNum, conn);
                cmdNum.Parameters.AddWithValue("@cid", courseId);
                int lessonNum = Convert.ToInt32(cmdNum.ExecuteScalar());

                string sql = @"INSERT INTO Lesson (CourseId, LessonNumber, LessonTitle, ContentType, ContentFilePath, ContentFile)
                               VALUES (@cid, @num, @title, 'text', @path, @content)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", courseId);
                cmd.Parameters.AddWithValue("@num", lessonNum);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@path", (object)filePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@content", string.IsNullOrEmpty(content) ? DBNull.Value : (object)content);
                cmd.ExecuteNonQuery();
            }

            lblLessonMsg.Text = "✅ Lesson added successfully!";
            lblLessonMsg.ForeColor = System.Drawing.Color.Green;
            txtLessonTitle.Text = "";
            txtLessonContent.Text = "";
            BindLessons();
        }

        protected void lvLessons_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            int lessonId = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "EditLesson")
            {
                LoadLessonForEditing(lessonId);
            }
            else if (e.CommandName == "DeleteLesson")
            {
                DeleteLesson(lessonId);
                BindLessons();
            }
        }

        private void LoadLessonForEditing(int lessonId)
        {
            hfLessonId.Value = lessonId.ToString();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT LessonTitle, ContentFile FROM Lesson WHERE Id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", lessonId);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtLessonTitle.Text = dr["LessonTitle"].ToString();
                    txtLessonContent.Text = dr["ContentFile"].ToString();
                }
                dr.Close();
            }
            btnAddLesson.Visible = false;
            btnUpdateLesson.Visible = true;
        }

        protected void btnUpdateLesson_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfLessonId.Value)) return;
            int lessonId = Convert.ToInt32(hfLessonId.Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"UPDATE Lesson SET LessonTitle=@title, ContentFile=@content WHERE Id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", txtLessonTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@content", txtLessonContent.Text.Trim());
                cmd.Parameters.AddWithValue("@id", lessonId);
                cmd.ExecuteNonQuery();
            }

            lblLessonMsg.Text = "✅ Lesson updated successfully!";
            lblLessonMsg.ForeColor = System.Drawing.Color.Green;
            btnAddLesson.Visible = true;
            btnUpdateLesson.Visible = false;
            txtLessonTitle.Text = "";
            txtLessonContent.Text = "";
            BindLessons();
        }

        private void DeleteLesson(int lessonId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"DELETE FROM Question WHERE QuizId IN (SELECT Id FROM Quiz WHERE LessonId=@lid);
                               DELETE FROM Quiz WHERE LessonId=@lid;
                               DELETE FROM Lesson WHERE Id=@lid;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@lid", lessonId);
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnSaveCourse_Click(object sender, EventArgs e)
        {
            string title = txtCourseTitle.Text.Trim();
            string type = rblCourseType.SelectedValue;
            int price = 0;
            int.TryParse(txtCoursePrice.Text.Trim(), out price);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"UPDATE Course SET Title=@title, CourseType=@type, Coin=@price WHERE Id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@id", courseId);
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "✅ Course updated successfully!";
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
    }
}
