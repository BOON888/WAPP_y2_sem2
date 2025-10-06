using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WAPP
{
    public partial class sign_in : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Optional: clear any existing session if returning to login
            if (!IsPostBack)
            {
                Session.Clear();
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter both email and password.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // ✅ Store user info in session
                    Session["UserId"] = reader["Id"].ToString();
                    Session["FullName"] = reader["FullName"].ToString();
                    Session["Email"] = reader["Email"].ToString();
                    Session["Role"] = reader["Role"].ToString();
                    Session["ProfilePicture"] = reader["ProfilePicture"].ToString();

                    string role = reader["Role"].ToString().ToLower();

                    // ✅ Redirect based on role
                    if (role == "student")
                    {
                        Response.Redirect("~/studentdashboard");
                    }
                    else if (role == "educator")
                    {
                        Response.Redirect("~/educatordashboard");
                    }
                    else if (role == "admin")
                    {
                        Response.Redirect("~/admin_dashboard");
                    }
                    else
                    {
                        lblError.Text = "Unknown role detected.";
                        lblError.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    lblError.Text = "Invalid email or password.";
                    lblError.ForeColor = System.Drawing.Color.Red;
                }

                reader.Close();
            }
        }
    }
}
