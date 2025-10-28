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
                // 🔹 Temporarily disabled login/session check for UI testing
                LoadEducatorProfile();
                LoadTeachingStats();
            }
        }

        private void LoadEducatorProfile()
        {
            // ⚠️ For now, hardcode a test user ID
            int userId = 1000; // Example: use an existing user in your DB

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

                    if (!string.IsNullOrEmpty(dr["Gender"].ToString()))
                        ddlGender.SelectedValue = dr["Gender"].ToString();

                    if (!string.IsNullOrEmpty(dr["EducationQualification"].ToString()))
                        ddlQualification.SelectedValue = dr["EducationQualification"].ToString();

                    txtUniversity.Text = dr["GraduatedUniversity"].ToString();
                }

                dr.Close();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            txtFullName.ReadOnly = false;
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
            // ⚠️ Use the same hardcoded user ID for now
            int userId = 1000;
            string fullName = txtFullName.Text.Trim();
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
                    string sqlUser = @"UPDATE Users 
                                       SET FullName=@name, Age=@age, Gender=@gender
                                       WHERE Id=@uid";
                    SqlCommand cmdU = new SqlCommand(sqlUser, conn, tran);
                    cmdU.Parameters.AddWithValue("@name", fullName);
                    cmdU.Parameters.AddWithValue("@age", age);
                    cmdU.Parameters.AddWithValue("@gender", gender);
                    cmdU.Parameters.AddWithValue("@uid", userId);
                    cmdU.ExecuteNonQuery();

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
            int educatorId = 3000; // ⚠️ Hardcoded for testing
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmdCourses = new SqlCommand("SELECT COUNT(*) FROM Course WHERE EducatorId=@eid", conn);
                cmdCourses.Parameters.AddWithValue("@eid", educatorId);
                lblCoursesCreated.Text = cmdCourses.ExecuteScalar().ToString();

                SqlCommand cmdStudents = new SqlCommand(@"
                    SELECT COUNT(DISTINCT scp.StudentId)
                    FROM StudentCourseProgress scp
                    JOIN Course c ON scp.CourseId = c.Id
                    WHERE c.EducatorId=@eid", conn);
                cmdStudents.Parameters.AddWithValue("@eid", educatorId);
                lblTotalStudents.Text = cmdStudents.ExecuteScalar()?.ToString() ?? "0";

                SqlCommand cmdCompletions = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM StudentCourseProgress scp
                    JOIN Course c ON scp.CourseId = c.Id
                    WHERE c.EducatorId=@eid AND scp.Status='Completed'", conn);
                cmdCompletions.Parameters.AddWithValue("@eid", educatorId);
                lblCompletions.Text = cmdCompletions.ExecuteScalar()?.ToString() ?? "0";
            }
        }
    }
}
