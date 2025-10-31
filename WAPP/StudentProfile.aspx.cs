using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace WAPP
{
    public partial class StudentProfile : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                LoadProfile();
                LoadStatsAndBadges();
            }
        }

        private void LoadProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        s.Id AS StudentId,      
                        u.FullName, 
                        u.Email, 
                        u.Age, 
                        u.Gender, 
                        u.ProfilePicture,
                        s.School, 
                        s.InterestSubject, 
                        s.Coins, 
                        s.BadgesEarned
                    FROM Users u
                    JOIN Student s ON u.Id = s.UserId
                    WHERE u.Id = @uid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    
                    lblUserName.Text = dr["FullName"].ToString();
                    lblStudentId.Text = dr["StudentId"].ToString();
                    txtFullName.Text = dr["FullName"].ToString();
                    txtEmail.Text = dr["Email"].ToString();
                    txtSchool.Text = dr["School"].ToString();
                    txtInterestSubject.Text = dr["InterestSubject"].ToString();
                    txtAge.Text = dr["Age"].ToString();

                    string gender = dr["Gender"].ToString();
                    if (!string.IsNullOrEmpty(gender) && ddlGender.Items.FindByValue(gender) != null)
                        ddlGender.SelectedValue = gender;

                    lblCoins.Text = dr["Coins"].ToString();
                    lblBadges.Text = dr["BadgesEarned"].ToString();

                    
                    string profilePic = dr["ProfilePicture"].ToString();
                    imgProfile.ImageUrl = string.IsNullOrEmpty(profilePic)
                        ? ResolveUrl("~/Image/default_profile2.png")
                        : ResolveUrl("~/Image/" + profilePic);

                    
                    Session["StudentID"] = dr["StudentId"].ToString();
                    Session["FullName"] = dr["FullName"].ToString();
                    Session["ProfilePicture"] = dr["ProfilePicture"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string fullName = txtFullName.Text.Trim();
            string school = txtSchool.Text.Trim();
            string subject = txtInterestSubject.Text.Trim();
            string gender = ddlGender.SelectedValue;
            int.TryParse(txtAge.Text.Trim(), out int age);

            string fileName = null;
            if (fileUploadProfile.HasFile)
            {
                
                string extension = Path.GetExtension(fileUploadProfile.FileName);
                fileName = "profile_" + userId + "_" + DateTime.Now.Ticks + extension;

                string folderPath = Server.MapPath("~/Image/");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                fileUploadProfile.SaveAs(Path.Combine(folderPath, fileName));
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                
                string queryUser = @"
                    UPDATE Users 
                    SET FullName = @name, Age = @age, Gender = @gender
                        " + (fileName != null ? ", ProfilePicture = @pic" : "") + @"
                    WHERE Id = @uid";

                SqlCommand cmdUser = new SqlCommand(queryUser, conn);
                cmdUser.Parameters.AddWithValue("@name", fullName);
                cmdUser.Parameters.AddWithValue("@age", age);
                cmdUser.Parameters.AddWithValue("@gender", gender);
                cmdUser.Parameters.AddWithValue("@uid", userId);
                if (fileName != null)
                    cmdUser.Parameters.AddWithValue("@pic", fileName);
                cmdUser.ExecuteNonQuery();

                
                string queryStudent = @"
                    UPDATE Student 
                    SET School = @school, InterestSubject = @subject 
                    WHERE UserId = @uid";

                SqlCommand cmdStudent = new SqlCommand(queryStudent, conn);
                cmdStudent.Parameters.AddWithValue("@school", school);
                cmdStudent.Parameters.AddWithValue("@subject", subject);
                cmdStudent.Parameters.AddWithValue("@uid", userId);
                cmdStudent.ExecuteNonQuery();

                
                Session["FullName"] = fullName;
                if (fileName != null)
                    Session["ProfilePicture"] = fileName;

                lblMessage.Text = "Profile updated successfully!";
                LoadProfile();
            }
        }

        private void LoadStatsAndBadges()
        {
            int studentId = 0;
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                SqlCommand getSid = new SqlCommand("SELECT Id FROM Student WHERE UserId = @uid", conn);
                getSid.Parameters.AddWithValue("@uid", userId);
                object sidObj = getSid.ExecuteScalar();
                if (sidObj != null)
                    studentId = Convert.ToInt32(sidObj);

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT p.BadgeEarned, c.Title AS CourseTitle
                    FROM StudentCourseProgress p
                    JOIN Course c ON p.CourseId = c.Id
                    WHERE p.StudentId = @sid AND p.BadgeEarned IS NOT NULL", conn);

                da.SelectCommand.Parameters.AddWithValue("@sid", studentId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptBadges.DataSource = dt;
                rptBadges.DataBind();
            }
        }

        
        protected void btnBackDashboard_Click(object sender, EventArgs e)
        {
            // make sure sessions are still active
            if (Session["UserId"] != null && Session["Role"]?.ToString().ToLower() == "student")
            {
                Response.Redirect("~/StudentDashboard.aspx");
            }
            else
            {
                Response.Redirect("~/sign_in.aspx");
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            txtFullName.ReadOnly = false;
            txtAge.ReadOnly = false;
            txtSchool.ReadOnly = false;
            txtInterestSubject.ReadOnly = false;
            ddlGender.Enabled = true;
            fileUploadProfile.Visible = true;

            btnEdit.Visible = false;
            btnSaveProfile.Visible = true;
            btnCancel.Visible = true;
            lblMessage.Text = "";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            txtFullName.ReadOnly = true;
            txtAge.ReadOnly = true;
            txtSchool.ReadOnly = true;
            txtInterestSubject.ReadOnly = true;
            ddlGender.Enabled = false;
            fileUploadProfile.Visible = false;

            btnEdit.Visible = true;
            btnSaveProfile.Visible = false;
            btnCancel.Visible = false;

            LoadProfile();
            lblMessage.Text = "Changes canceled.";
            lblMessage.ForeColor = System.Drawing.Color.Gray;
        }
    }
}
