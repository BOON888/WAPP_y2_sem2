using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WAPP
{
    public partial class sign_in : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter both email and password.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Role FROM users WHERE Email = @Email AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                object roleObj = cmd.ExecuteScalar();

                if (roleObj != null)
                {
                    string role = roleObj.ToString().ToLower();

                    if (role == "student")
                    {
                        Response.Redirect("~/studentdashboard");
                    }
                    else if (role == "educator")
                    {
                        Response.Redirect("~/educatordashboard");
                    }
                    else
                    {
                        lblError.Text = "Unknown role detected.";
                    }
                }
                else
                {
                    lblError.Text = "Invalid email or password.";
                }
            }
        }
    }
}
