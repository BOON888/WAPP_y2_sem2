using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace WAPP
{
    public partial class educatorProfile : System.Web.UI.Page
    {
        // Database connect
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Ensure educator is logged in
                if (Session["UserId"] == null || Session["Role"] == null ||
                    Session["Role"].ToString().ToLower() != "educator")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                LoadEducatorProfile(); // Load educator personal details
                LoadTeachingStats(); // Load teaching statistics
            }
        }

        private void LoadEducatorProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                // Get educator profile info from Users and Educator tables
                string sql = @"
                    SELECT 
                        e.Id AS EducatorId,
                        u.FullName, u.Age, u.Gender, u.ProfilePicture, u.Email,
                        e.EducationQualification, e.GraduatedUniversity
                    FROM Users u
                    JOIN Educator e ON u.Id = e.UserId
                    WHERE u.Id = @uid";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    // Load profile picture
                    string profilePic = dr["ProfilePicture"]?.ToString();
                    imgProfile.ImageUrl = string.IsNullOrEmpty(profilePic)
                        ? ResolveUrl("~/Image/default_profile2.png")
                        : ResolveUrl("~/Image/" + profilePic);

                    // Display educator info
                    lblFullName.Text = dr["FullName"].ToString();
                    txtFullName.Text = dr["FullName"].ToString();
                    txtAge.Text = dr["Age"].ToString();
                    lblEducatorId.Text = dr["EducatorId"].ToString();

                    // Email (read-only)
                    txtEmail.Text = dr["Email"].ToString();

                    // Gender
                    string gender = dr["Gender"].ToString();
                    if (!string.IsNullOrEmpty(gender) && ddlGender.Items.FindByValue(gender) != null)
                        ddlGender.SelectedValue = gender;

                    // Qualification
                    string qual = dr["EducationQualification"].ToString();
                    if (!string.IsNullOrEmpty(qual) && ddlQualification.Items.FindByValue(qual) != null)
                        ddlQualification.SelectedValue = qual;

                    // University
                    txtUniversity.Text = dr["GraduatedUniversity"].ToString();

                    // Update session with latest educator data
                    Session["EducatorID"] = dr["EducatorId"].ToString();
                    Session["FullName"] = dr["FullName"].ToString();
                    Session["ProfilePicture"] = dr["ProfilePicture"].ToString();
                }
                dr.Close();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            // Make profile fields editable
            txtFullName.ReadOnly = false;
            txtAge.ReadOnly = false;
            txtUniversity.ReadOnly = false;
            ddlGender.Enabled = true;
            ddlQualification.Enabled = true;
            fileUploadProfile.Visible = true;

            // Show Save/Cancel buttons
            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = true;
            lblMsg.Text = "";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Revert to read-only mode
            txtFullName.ReadOnly = true;
            txtAge.ReadOnly = true;
            txtUniversity.ReadOnly = true;
            ddlGender.Enabled = false;
            ddlQualification.Enabled = false;
            fileUploadProfile.Visible = false;

            // Show Edit button again
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnCancel.Visible = false;

            LoadEducatorProfile(); // Reload profile from database
            lblMsg.Text = "Changes canceled.";
            lblMsg.ForeColor = System.Drawing.Color.Gray;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string fullName = txtFullName.Text.Trim();
            string gender = ddlGender.SelectedValue;
            string qualification = ddlQualification.SelectedValue;
            string university = txtUniversity.Text.Trim();
            int.TryParse(txtAge.Text.Trim(), out int age);

            string fileName = null;

            bool fileUploadedSuccessfully = false; // Track file upload success

            // --- File Upload ---
            if (fileUploadProfile.HasFile)
            {
                try
                {
                    // Generate a unique file name for uploaded profile picture
                    string extension = Path.GetExtension(fileUploadProfile.FileName);
                    fileName = "educator_" + userId + "_" + DateTime.Now.Ticks + extension;

                    string folderPath = Server.MapPath("~/Image/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // Save the uploaded file to Image folder
                    fileUploadProfile.SaveAs(Path.Combine(folderPath, fileName));
                    fileUploadedSuccessfully = true; 
                }
                catch (Exception fileEx)
                {
                    // Display upload error message
                    lblMsg.Text = "File Upload Failed: " + fileEx.Message + ". Please check folder permissions.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            // --- Update Database ---
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction(); // Start a transaction

                try
                {
                    // Keep old picture if no new upload
                    if (!fileUploadedSuccessfully)
                    {
                        fileName = Session["ProfilePicture"]?.ToString();
                    }

                    // Update Users table
                    string sqlUser = @"UPDATE Users 
                               SET FullName=@name, Age=@age, Gender=@gender"
                                       + (fileUploadedSuccessfully ? ", ProfilePicture=@pic" : "")
                                       + " WHERE Id=@uid";

                    SqlCommand cmdU = new SqlCommand(sqlUser, conn, tran);
                    cmdU.Parameters.AddWithValue("@name", fullName);
                    cmdU.Parameters.AddWithValue("@age", age);
                    cmdU.Parameters.AddWithValue("@gender", gender);
                    cmdU.Parameters.AddWithValue("@uid", userId);

                    if (fileUploadedSuccessfully)
                        cmdU.Parameters.AddWithValue("@pic", fileName);

                    cmdU.ExecuteNonQuery();

                    // Update Educator table
                    string sqlEdu = @"UPDATE Educator 
                              SET EducationQualification=@qual, GraduatedUniversity=@uni
                              WHERE UserId=@uid";

                    SqlCommand cmdE = new SqlCommand(sqlEdu, conn, tran);
                    cmdE.Parameters.AddWithValue("@qual", qualification);
                    cmdE.Parameters.AddWithValue("@uni", university);
                    cmdE.Parameters.AddWithValue("@uid", userId);
                    cmdE.ExecuteNonQuery();

                    tran.Commit(); // Commit changes

                    lblMsg.Text = "Profile updated successfully!";
                    lblMsg.ForeColor = System.Drawing.Color.Green;

                    // Refresh and lock the form again
                    LoadEducatorProfile();
                    txtFullName.ReadOnly = true;
                    txtAge.ReadOnly = true;
                    txtUniversity.ReadOnly = true;
                    ddlGender.Enabled = false;
                    ddlQualification.Enabled = false;
                    fileUploadProfile.Visible = false;

                    btnEdit.Visible = true;
                    btnSave.Visible = false;
                    btnCancel.Visible = false;
                }
                catch (Exception dbEx)
                {
                    tran.Rollback(); // Undo if error occurs
                    lblMsg.Text = "Database Error: " + dbEx.Message;
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

                // Get educator ID
                SqlCommand getEid = new SqlCommand("SELECT Id FROM Educator WHERE UserId=@uid", conn);
                getEid.Parameters.AddWithValue("@uid", userId);
                object eidObj = getEid.ExecuteScalar();

                if (eidObj == null)
                {
                    // No data then show zeros
                    lblCoursesCreated.Text = "0";
                    lblTotalStudents.Text = "0";
                    lblCompletions.Text = "0";
                    return;
                }

                educatorId = Convert.ToInt32(eidObj);

                // Count courses created
                SqlCommand cmdCourses = new SqlCommand("SELECT COUNT(*) FROM Course WHERE EducatorId=@eid", conn);
                cmdCourses.Parameters.AddWithValue("@eid", educatorId);
                lblCoursesCreated.Text = cmdCourses.ExecuteScalar()?.ToString() ?? "0";

                // Count total students enrolled
                SqlCommand cmdStudents = new SqlCommand(@"
                    SELECT COUNT(DISTINCT scp.StudentId)
                    FROM StudentCourseProgress scp
                    INNER JOIN Course c ON scp.CourseId = c.Id
                    WHERE c.EducatorId = @eid", conn);
                cmdStudents.Parameters.AddWithValue("@eid", educatorId);
                lblTotalStudents.Text = cmdStudents.ExecuteScalar()?.ToString() ?? "0";

                // Count completed courses
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
