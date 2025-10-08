using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WAPP
{
    public partial class forgot_password : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string father = txtFather.Text.Trim();
            string mother = txtMother.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (newPassword != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match!";
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND father = @Father AND mother = @Mother";
                SqlCommand cmdCheck = new SqlCommand(checkQuery, conn);
                cmdCheck.Parameters.AddWithValue("@Email", email);
                cmdCheck.Parameters.AddWithValue("@Father", father);
                cmdCheck.Parameters.AddWithValue("@Mother", mother);

                int count = (int)cmdCheck.ExecuteScalar();

                if (count == 1)
                {
                    string updateQuery = "UPDATE Users SET Password = @Password WHERE Email = @Email";
                    SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn);
                    cmdUpdate.Parameters.AddWithValue("@Password", newPassword);
                    cmdUpdate.Parameters.AddWithValue("@Email", email);
                    cmdUpdate.ExecuteNonQuery();

                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "Password successfully updated!";
                }
                else
                {
                    lblMessage.Text = "Invalid details! Please check your email, father, and mother name.";
                }
            }
        }
    }
}
