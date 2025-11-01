using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WAPP
{
    public partial class sign_in : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                conn.Open();

                string userQuery = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
                SqlCommand userCmd = new SqlCommand(userQuery, conn);
                userCmd.Parameters.AddWithValue("@Email", email);
                userCmd.Parameters.AddWithValue("@Password", password);

                SqlDataReader reader = userCmd.ExecuteReader();

                if (reader.Read())
                {
                    Session["UserId"] = reader["Id"].ToString();
                    Session["FullName"] = reader["FullName"].ToString();  
                    Session["Email"] = reader["Email"].ToString();
                    Session["Role"] = reader["Role"].ToString().ToLower();
                    Session["ProfilePicture"] = reader["ProfilePicture"].ToString();

                    string role = reader["Role"].ToString().ToLower();
                    reader.Close();

                    // Redirect by role
                    if (role == "student")
                        Response.Redirect("~/studentdashboard.aspx");
                    else if (role == "educator")
                        Response.Redirect("~/educatordashboard.aspx");
                    else
                        lblError.Text = "Unknown role detected.";
                }
                else
                {
                    reader.Close();

                    // 🔹 Step 2: If not found in Users, check Admin table
                    string adminQuery = "SELECT * FROM Admin WHERE Email = @Email AND Password = @Password";
                    SqlCommand adminCmd = new SqlCommand(adminQuery, conn);
                    adminCmd.Parameters.AddWithValue("@Email", email);
                    adminCmd.Parameters.AddWithValue("@Password", password);

                    SqlDataReader adminReader = adminCmd.ExecuteReader();

                    if (adminReader.Read())
                    {
                        Session["UserId"] = adminReader["Id"].ToString();
                        Session["Email"] = adminReader["Email"].ToString();
                        Session["Role"] = "admin";
                        Session["FullName"] = "Administrator"; 

                        adminReader.Close();

                        Response.Redirect("~/admin_dashboard.aspx");
                    }
                    else
                    {
                        adminReader.Close();
                        lblError.Text = "Invalid email or password.";
                        lblError.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
    }
}
