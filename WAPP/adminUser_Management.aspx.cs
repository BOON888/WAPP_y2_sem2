using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class user_management : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardStats();
                LoadUsers();
            }

            // Handle status change commands
            if (Request["__EVENTTARGET"] == "DisableUser")
            {
                string userId = Request["__EVENTARGUMENT"];
                UpdateUserStatus(userId, "Inactive");
            }
            else if (Request["__EVENTTARGET"] == "EnableUser")
            {
                string userId = Request["__EVENTARGUMENT"];
                UpdateUserStatus(userId, "Active");
            }
        }

        private void LoadDashboardStats()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Total Users
                    string totalUsersQuery = "SELECT COUNT(*) FROM Users";
                    SqlCommand totalCmd = new SqlCommand(totalUsersQuery, conn);
                    lblTotalUsers.Text = totalCmd.ExecuteScalar().ToString();

                    // Active Users
                    string activeUsersQuery = "SELECT COUNT(*) FROM Users WHERE Status = 'Active'";
                    SqlCommand activeCmd = new SqlCommand(activeUsersQuery, conn);
                    lblActiveUsers.Text = activeCmd.ExecuteScalar().ToString();

                    // Total Students
                    string studentsQuery = "SELECT COUNT(*) FROM Users WHERE Role = 'student'";
                    SqlCommand studentsCmd = new SqlCommand(studentsQuery, conn);
                    lblTotalStudents.Text = studentsCmd.ExecuteScalar().ToString();

                    // Total Educators
                    string educatorsQuery = "SELECT COUNT(*) FROM Users WHERE Role = 'educator'";
                    SqlCommand educatorsCmd = new SqlCommand(educatorsQuery, conn);
                    lblTotalEducators.Text = educatorsCmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading dashboard statistics: " + ex.Message);
            }
        }

        private void LoadUsers()
        {
            string searchTerm = txtSearch.Text.Trim();
            string roleFilter = ViewState["CurrentRole"] as string ?? "All";

            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            Id, 
                            FullName, 
                            Email, 
                            Role, 
                            Age, 
                            Gender,
                            Status
                        FROM Users 
                        WHERE 1=1";

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " AND (FullName LIKE @SearchTerm OR Email LIKE @SearchTerm)";
                    }

                    if (roleFilter != "All")
                    {
                        query += " AND Role = @Role";
                    }

                    query += " ORDER BY FullName";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }

                    if (roleFilter != "All")
                    {
                        cmd.Parameters.AddWithValue("@Role", roleFilter);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    pnlUserContainer.Controls.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            CreateUserCard(
                                row["Id"].ToString(),
                                row["FullName"].ToString(),
                                row["Email"].ToString(),
                                row["Role"].ToString(),
                                row["Age"] != DBNull.Value ? row["Age"].ToString() : "N/A",
                                row["Gender"] != DBNull.Value ? row["Gender"].ToString() : "N/A",
                                row["Status"].ToString()
                            );
                        }
                        lblNoUsers.Visible = false;
                        lblUsersShown.Text = dt.Rows.Count.ToString();
                    }
                    else
                    {
                        lblNoUsers.Visible = true;
                        lblUsersShown.Text = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading users: " + ex.Message);
                lblNoUsers.Visible = true;
            }
        }

        private void CreateUserCard(string id, string fullName, string email, string role, string age, string gender, string status)
        {
            Panel userCard = new Panel();
            userCard.CssClass = "user-card";

            // Main content
            Panel mainContent = new Panel();
            mainContent.CssClass = "user-main-content";

            // Header
            Panel header = new Panel();
            header.CssClass = "user-header";

            // Avatar
            Panel avatar = new Panel();
            avatar.CssClass = "user-avatar";
            avatar.Controls.Add(new LiteralControl(fullName.Substring(0, 1).ToUpper()));
            header.Controls.Add(avatar);

            // User info
            Panel userInfo = new Panel();
            userInfo.CssClass = "user-info";

            Label lblName = new Label();
            lblName.Text = $"<div class='user-name'>{Server.HtmlEncode(fullName)}</div>";
            userInfo.Controls.Add(lblName);

            Label lblRole = new Label();
            string roleClass = role.ToLower() == "student" ? "student-role" : "educator-role";
            lblRole.Text = $"<span class='user-role {roleClass}'>{Server.HtmlEncode(role)}</span>";
            userInfo.Controls.Add(lblRole);

            header.Controls.Add(userInfo);
            mainContent.Controls.Add(header);

            // User details
            Panel details = new Panel();
            details.CssClass = "user-details";

            // Email
            Panel emailDetail = new Panel();
            emailDetail.CssClass = "user-detail";
            emailDetail.Controls.Add(new LiteralControl($"<i>📧</i> {Server.HtmlEncode(email)}"));
            details.Controls.Add(emailDetail);

            // Age
            Panel ageDetail = new Panel();
            ageDetail.CssClass = "user-detail";
            ageDetail.Controls.Add(new LiteralControl($"<i>👤</i> Age: {Server.HtmlEncode(age)}"));
            details.Controls.Add(ageDetail);

            // Gender
            Panel genderDetail = new Panel();
            genderDetail.CssClass = "user-detail";
            genderDetail.Controls.Add(new LiteralControl($"<i>⚧️</i> Gender: {Server.HtmlEncode(gender)}"));
            details.Controls.Add(genderDetail);

            mainContent.Controls.Add(details);
            userCard.Controls.Add(mainContent);

            // Status section
            Panel statusSection = new Panel();
            statusSection.CssClass = "user-status-section";

            // Status text
            Label lblStatus = new Label();
            lblStatus.Text = $"<div class='status-text {(status == "Active" ? "" : "disabled")}'>{status}</div>";
            statusSection.Controls.Add(lblStatus);

            // Action buttons
            Panel actions = new Panel();
            actions.CssClass = "user-actions";

            if (status == "Active")
            {
                Button btnDisable = new Button();
                btnDisable.Text = "Disable";
                btnDisable.CssClass = "disable-btn";
                btnDisable.OnClientClick = $"return confirmDisable('{id}', '{fullName.Replace("'", "\\'")}');";
                actions.Controls.Add(btnDisable);
            }
            else
            {
                Button btnEnable = new Button();
                btnEnable.Text = "Enable";
                btnEnable.CssClass = "enable-btn";
                btnEnable.OnClientClick = $"return confirmEnable('{id}', '{fullName.Replace("'", "\\'")}');";
                actions.Controls.Add(btnEnable);
            }

            statusSection.Controls.Add(actions);
            userCard.Controls.Add(statusSection);

            pnlUserContainer.Controls.Add(userCard);
        }

        private void UpdateUserStatus(string userId, string status)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = "UPDATE Users SET Status = @Status WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Id", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowSuccess($"User status updated to {status} successfully!");
                        LoadDashboardStats();
                        LoadUsers();
                    }
                    else
                    {
                        ShowError("User not found or update failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error updating user status: " + ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadUsers();
        }

        protected void btnTab_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string role = btn.CommandArgument;

            // Update active tab
            btnAllUsers.CssClass = "tab";
            btnStudents.CssClass = "tab";
            btnEducators.CssClass = "tab";
            btn.CssClass = "tab active";

            ViewState["CurrentRole"] = role;
            LoadUsers();
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = message;
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;
        }
    }
}