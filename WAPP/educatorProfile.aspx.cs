using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WAPP
{
    public partial class educatorProfile : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ✅ Ensure educator is logged in
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString().ToLower() != "educator")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                LoadEducatorProfile();
                LoadTeachingStats();
            }
        }

        private void LoadEducatorProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        u.FullName, u.Email, u.Age, u.Gender, 
                        e.EducationQualification, e.GraduatedUniversity
                    FROM Users u
                    JOIN Educator e ON u.Id = e.UserId
                    WHERE u.Id = @uid";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblFullName.Text = dr["FullName"].ToString();
                    lblEmail.Text = dr["Email"].ToString();

                    txtFullName.Text = dr["FullName"].ToString();
                    txtEmail.Text = dr["Email"].ToString();
                    txtAge.Text = dr["Age"].ToString();

                    string gender = dr["Gender"].ToString();
                    if (!string.IsNullOrEmpty(gender) && ddlGender.Items.FindByValue(gender) != null)
                        ddlGender.SelectedValue = gender;

                    string qual = dr["EducationQualification"].ToString();
                    if (!string.IsNullOrEmpty(qual) && ddlQualification.Items.FindByValue(qual) != null)
                        ddlQualification.SelectedValue = qual;

                    txtUniversity.Text = dr["GraduatedUniversity"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            txtFullName.ReadOnly = false;
            txtEmail.ReadOnly = false;      // ✅ Make email editable
            txtAge.ReadOnly = false;
            txtUniversity.ReadOnly = false;
            ddlGender.Enabled = true;
            ddlQualification.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = true;
            lblMsg.Text = "";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            txtFullName.ReadOnly = true;
            txtEmail.ReadOnly = true;       // ✅ Re-disable email on cancel
            txtAge.ReadOnly = true;
            txtUniversity.ReadOnly = true;
            ddlGender.Enabled = false;
            ddlQualification.Enabled = false;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnCancel.Visible = false;

            LoadEducatorProfile();
            lblMsg.Text = "Changes canceled.";
            lblMsg.ForeColor = System.Drawing.Color.Gray;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();     // ✅ Allow updating email
            string gender = ddlGender.SelectedValue;
            string qualification = ddlQualification.SelectedValue;
            string university = txtUniversity.Text.Trim();

            int.TryParse(txtAge.Text.Trim(), out int age);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // ✅ Update Users table (including Email now)
                    string sqlUser = @"UPDATE Users 
                                       SET FullName=@name, Email=@mail, Age=@age, Gender=@gender
                                       WHERE Id=@uid";
                    SqlCommand cmdU = new SqlCommand(sqlUser, conn, tran);
                    cmdU.Parameters.AddWithValue("@name", fullName);
                    cmdU.Parameters.AddWithValue("@mail", email);
                    cmdU.Parameters.AddWithValue("@age", age);
                    cmdU.Parameters.AddWithValue("@gender", gender);
                    cmdU.Parameters.AddWithValue("@uid", userId);
                    cmdU.ExecuteNonQuery();

                    // ✅ Update Educator table
                    string sqlEdu = @"UPDATE Educator 
                                      SET EducationQualification=@qual, GraduatedUniversity=@uni
                                      WHERE UserId=@uid";
                    SqlCommand cmdE = new SqlCommand(sqlEdu, conn, tran);
                    cmdE.Parameters.AddWithValue("@qual", qualification);
                    cmdE.Parameters.AddWithValue("@uni", university);
                    cmdE.Parameters.AddWithValue("@uid", userId);
                    cmdE.ExecuteNonQuery();

                    tran.Commit();

                    lblMsg.Text = "✅ Profile updated successfully!";
                    lblMsg.ForeColor = System.Drawing.Color.Green;

                    LoadEducatorProfile();

                    txtFullName.ReadOnly = true;
                    txtEmail.ReadOnly = true;
                    txtAge.ReadOnly = true;
                    txtUniversity.ReadOnly = true;
                    ddlGender.Enabled = false;
                    ddlQualification.Enabled = false;

                    btnEdit.Visible = true;
                    btnSave.Visible = false;
                    btnCancel.Visible = false;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    lblMsg.Text = "❌ Error updating profile: " + ex.Message;
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void LoadTeachingStats()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            int educatorId = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 1️⃣ Get EducatorId based on UserId
                SqlCommand getEid = new SqlCommand("SELECT Id FROM Educator WHERE UserId=@uid", conn);
                getEid.Parameters.AddWithValue("@uid", userId);
                object eidObj = getEid.ExecuteScalar();

                if (eidObj == null)
                {
                    lblCoursesCreated.Text = "0";
                    lblTotalStudents.Text = "0";
                    lblCompletions.Text = "0";
                    return;
                }

                educatorId = Convert.ToInt32(eidObj);

                // 2️⃣ Courses Created
                SqlCommand cmdCourses = new SqlCommand("SELECT COUNT(*) FROM Course WHERE EducatorId=@eid", conn);
                cmdCourses.Parameters.AddWithValue("@eid", educatorId);
                lblCoursesCreated.Text = cmdCourses.ExecuteScalar()?.ToString() ?? "0";

                // 3️⃣ Total Students (distinct)
                SqlCommand cmdStudents = new SqlCommand(@"
                    SELECT COUNT(DISTINCT scp.StudentId)
                    FROM StudentCourseProgress scp
                    INNER JOIN Course c ON scp.CourseId = c.Id
                    WHERE c.EducatorId = @eid", conn);
                        cmdStudents.Parameters.AddWithValue("@eid", educatorId);
                lblTotalStudents.Text = cmdStudents.ExecuteScalar()?.ToString() ?? "0";

                // 4️⃣ Completed Courses
                SqlCommand cmdCompleted = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM StudentCourseProgress scp
                    INNER JOIN Course c ON scp.CourseId = c.Id
                    WHERE c.EducatorId = @eid AND scp.Status = 'Completed'", conn);
                        cmdCompleted.Parameters.AddWithValue("@eid", educatorId);
                lblCompletions.Text = cmdCompleted.ExecuteScalar()?.ToString() ?? "0";
            }
        }

    }
}
