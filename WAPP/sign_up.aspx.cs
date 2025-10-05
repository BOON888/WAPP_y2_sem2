using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WAPP
{
    public partial class sign_up : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void rblRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            studentSection.Visible = rblRole.SelectedValue == "student";
            educatorSection.Visible = rblRole.SelectedValue == "educator";
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblMessage.Text = "⚠ Passwords do not match!";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMessage.Text = "⚠ Please fill in all required fields!";
                return;
            }
  
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            bool success = false;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string insertUserQuery = @"INSERT INTO Users (FullName, Email, Password, Role, Age, Gender)
                                   OUTPUT INSERTED.Id
                                   VALUES (@FullName, @Email, @Password, @Role, @Age, @Gender)";
                    SqlCommand cmdUser = new SqlCommand(insertUserQuery, conn, transaction);
                    cmdUser.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmdUser.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmdUser.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    cmdUser.Parameters.AddWithValue("@Role", rblRole.SelectedValue);

                    int age = 0;
                    string gender = "";

                    if (rblRole.SelectedValue == "student")
                    {
                        int.TryParse(txtAge.Text, out age);
                        gender = ddlGender.SelectedValue;
                    }
                    else if (rblRole.SelectedValue == "educator")
                    {
                        int.TryParse(txtAgeEdu.Text, out age);
                        gender = ddlGenderEdu.SelectedValue;
                    }

                    cmdUser.Parameters.AddWithValue("@Age", age);
                    cmdUser.Parameters.AddWithValue("@Gender", gender);

                    // Insert into Users table and get new UserId
                    int userId = Convert.ToInt32(cmdUser.ExecuteScalar());

                    // Insert into Student or Educator table
                    if (rblRole.SelectedValue == "student")
                    {
                        string insertStudentQuery = @"INSERT INTO Student (UserId, School, InterestSubject)
                                      VALUES (@UserId, @School, @InterestSubject)";
                        SqlCommand cmdStudent = new SqlCommand(insertStudentQuery, conn, transaction);
                        cmdStudent.Parameters.AddWithValue("@UserId", userId);
                        cmdStudent.Parameters.AddWithValue("@School", txtSchool.Text.Trim());
                        cmdStudent.Parameters.AddWithValue("@InterestSubject", ddlSubject.SelectedValue);
                        cmdStudent.ExecuteNonQuery();
                    }
                    else if (rblRole.SelectedValue == "educator")
                    {
                        string insertEducatorQuery = @"INSERT INTO Educator (UserId, EducationQualification, GraduatedUniversity)
                                       VALUES (@UserId, @EducationQualification, @GraduatedUniversity)";
                        SqlCommand cmdEducator = new SqlCommand(insertEducatorQuery, conn, transaction);
                        cmdEducator.Parameters.AddWithValue("@UserId", userId);
                        cmdEducator.Parameters.AddWithValue("@EducationQualification", ddlQualification.SelectedValue);
                        cmdEducator.Parameters.AddWithValue("@GraduatedUniversity", txtUniversity.Text.Trim());
                        cmdEducator.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    success = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    lblMessage.Text = "Error: " + ex.Message;
                    success = false;
                }
            }

            // Redirect only after the transaction is properly completed and disposed
            if (success)
            {
                Response.Redirect("sign_in.aspx?message=AccountCreated");
            }
        }
    }
}
