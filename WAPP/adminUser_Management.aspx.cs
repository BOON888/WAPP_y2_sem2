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
        private string currentFilter = "All";
        private string searchTerm = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Store initial values in ViewState
                ViewState["CurrentFilter"] = currentFilter;
                ViewState["SearchTerm"] = searchTerm;

                LoadUserStats();
                LoadUsers();
            }
            else
            {
                // Retrieve values from ViewState
                currentFilter = ViewState["CurrentFilter"]?.ToString() ?? "All";
                searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Rebuild controls on every postback to maintain state
            if (IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUserStats()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Total Users
                    string query = "SELECT COUNT(*) FROM Users";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    lblTotalUsers.Text = cmd.ExecuteScalar().ToString();

                    // Active Users
                    query = "SELECT COUNT(*) FROM Users WHERE Status = 'Active'";
                    cmd = new SqlCommand(query, conn);
                    lblActiveUsers.Text = cmd.ExecuteScalar().ToString();

                    // Total Students
                    query = "SELECT COUNT(*) FROM Users WHERE Role = 'student'";
                    cmd = new SqlCommand(query, conn);
                    lblTotalStudents.Text = cmd.ExecuteScalar().ToString();

                    // Total Educators
                    query = "SELECT COUNT(*) FROM Users WHERE Role = 'educator'";
                    cmd = new SqlCommand(query, conn);
                    lblTotalEducators.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user stats: {ex.Message}");
            }
        }

        private void LoadUsers()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string userQuery = @"
                        SELECT 
                            Id, 
                            FullName, 
                            Email, 
                            Role, 
                            Status
                        FROM Users 
                        WHERE (@SearchTerm = '' OR FullName LIKE '%' + @SearchTerm + '%' OR Email LIKE '%' + @SearchTerm + '%')
                        AND (@RoleFilter = 'All' OR Role = @RoleFilter)
                        ORDER BY FullName";

                    SqlCommand userCmd = new SqlCommand(userQuery, conn);
                    userCmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? "" : searchTerm);
                    userCmd.Parameters.AddWithValue("@RoleFilter", currentFilter);

                    SqlDataAdapter da = new SqlDataAdapter(userCmd);
                    DataTable userDt = new DataTable();
                    da.Fill(userDt);

                    pnlUserContainer.Controls.Clear();
                    lblUsersShown.Text = userDt.Rows.Count.ToString();

                    if (userDt.Rows.Count > 0)
                    {
                        foreach (DataRow userRow in userDt.Rows)
                        {
                            string userId = userRow["Id"].ToString();
                            string role = userRow["Role"].ToString();

                            if (role == "student")
                            {
                                CreateStudentCard(userId, userRow);
                            }
                            else if (role == "educator")
                            {
                                CreateEducatorCard(userId, userRow);
                            }
                        }
                        lblNoUsers.Visible = false;
                    }
                    else
                    {
                        lblNoUsers.Text = "No users found.";
                        lblNoUsers.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
                lblNoUsers.Text = "Error loading users. Please try again.";
                lblNoUsers.Visible = true;
            }
        }

        private void CreateStudentCard(string userId, DataRow userRow)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            string school = "Not specified";
            string coins = "0";
            string badges = "0";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string studentQuery = @"
                        SELECT 
                            ISNULL(School, 'Not specified') as School,
                            ISNULL(Coins, 0) as Coins,
                            ISNULL(BadgesEarned, 0) as BadgesEarned
                        FROM Student 
                        WHERE UserId = @UserId";

                    SqlCommand studentCmd = new SqlCommand(studentQuery, conn);
                    studentCmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataReader reader = studentCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        school = reader["School"].ToString();
                        coins = reader["Coins"].ToString();
                        badges = reader["BadgesEarned"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Continue with default values
            }

            // Create the student card
            Panel userCard = new Panel();
            userCard.CssClass = "user-card";

            // Main content container
            Panel mainContent = new Panel();
            mainContent.CssClass = "user-main-content";

            // User header with avatar and info
            Panel userHeader = new Panel();
            userHeader.CssClass = "user-header";

            // Avatar
            Panel avatar = new Panel();
            avatar.CssClass = "user-avatar";
            string firstLetter = userRow["FullName"].ToString().Length > 0 ?
                userRow["FullName"].ToString()[0].ToString().ToUpper() : "U";
            avatar.Controls.Add(new LiteralControl(firstLetter));

            // User info
            Panel userInfo = new Panel();
            userInfo.CssClass = "user-info";

            Label lblName = new Label();
            lblName.Text = $"<div class='user-name'>{userRow["FullName"]}</div>";
            userInfo.Controls.Add(lblName);

            Label lblRole = new Label();
            lblRole.Text = $"<span class='user-role student-role'>Student</span>";
            userInfo.Controls.Add(lblRole);

            userHeader.Controls.Add(avatar);
            userHeader.Controls.Add(userInfo);
            mainContent.Controls.Add(userHeader);

            // User details
            Panel userDetails = new Panel();
            userDetails.CssClass = "user-details";

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = $"<div class='user-detail'><i>📧</i>{userRow["Email"]}</div>";
            userDetails.Controls.Add(lblEmail);

            // School
            Label lblSchool = new Label();
            lblSchool.Text = $"<div class='user-detail'><i>🏫</i>{school}</div>";
            userDetails.Controls.Add(lblSchool);

            mainContent.Controls.Add(userDetails);

            // Stats
            Panel userStats = new Panel();
            userStats.CssClass = "user-stats";

            // Coins
            Panel coinsStat = new Panel();
            coinsStat.CssClass = "stat-item";
            coinsStat.Controls.Add(new LiteralControl($"<div class='stat-value'>{coins}</div><div class='stat-label'>Coins</div>"));
            userStats.Controls.Add(coinsStat);

            // Badges
            Panel badgesStat = new Panel();
            badgesStat.CssClass = "stat-item";
            badgesStat.Controls.Add(new LiteralControl($"<div class='stat-value'>{badges}</div><div class='stat-label'>Badges</div>"));
            userStats.Controls.Add(badgesStat);

            // Completed
            Panel completedStat = new Panel();
            completedStat.CssClass = "stat-item";
            completedStat.Controls.Add(new LiteralControl($"<div class='stat-value'>0</div><div class='stat-label'>Completed</div>"));
            userStats.Controls.Add(completedStat);

            mainContent.Controls.Add(userStats);
            userCard.Controls.Add(mainContent);

            // Status section
            Panel statusSection = new Panel();
            statusSection.CssClass = "user-status-section";

            bool isActive = userRow["Status"].ToString() == "Active";

            // Create toggle button
            Button btnToggle = new Button();
            btnToggle.ID = "btnToggle_" + userId;
            btnToggle.Text = isActive ? "Disable User" : "Enable User";
            btnToggle.CssClass = isActive ? "disable-btn" : "enable-btn";
            btnToggle.CommandArgument = userId + "|" + userRow["Status"].ToString();
            btnToggle.Click += BtnToggle_Click;

            statusSection.Controls.Add(btnToggle);
            userCard.Controls.Add(statusSection);

            pnlUserContainer.Controls.Add(userCard);
        }

        private void CreateEducatorCard(string userId, DataRow userRow)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            string university = "Not specified";
            string courseCount = "0";
            string students = "0";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Get educator university
                    string educatorQuery = @"
                        SELECT ISNULL(GraduatedUniversity, 'Not specified') as University
                        FROM Educator 
                        WHERE UserId = @UserId";

                    SqlCommand educatorCmd = new SqlCommand(educatorQuery, conn);
                    educatorCmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataReader reader = educatorCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        university = reader["University"].ToString();
                    }
                    reader.Close();

                    // Get course count
                    string courseQuery = @"
                        SELECT COUNT(*) as CourseCount 
                        FROM Course 
                        WHERE EducatorId IN (SELECT Id FROM Educator WHERE UserId = @UserId)";

                    SqlCommand courseCmd = new SqlCommand(courseQuery, conn);
                    courseCmd.Parameters.AddWithValue("@UserId", userId);
                    var courseResult = courseCmd.ExecuteScalar();
                    if (courseResult != null && courseResult != DBNull.Value)
                    {
                        courseCount = courseResult.ToString();
                    }

                    // Get total students count
                    string studentsQuery = "SELECT COUNT(*) FROM Student";
                    SqlCommand studentsCmd = new SqlCommand(studentsQuery, conn);
                    var studentsResult = studentsCmd.ExecuteScalar();
                    if (studentsResult != null && studentsResult != DBNull.Value)
                    {
                        students = studentsResult.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Continue with default values
            }

            // Create the educator card
            Panel userCard = new Panel();
            userCard.CssClass = "user-card";

            // Main content container
            Panel mainContent = new Panel();
            mainContent.CssClass = "user-main-content";

            // User header with avatar and info
            Panel userHeader = new Panel();
            userHeader.CssClass = "user-header";

            // Avatar
            Panel avatar = new Panel();
            avatar.CssClass = "user-avatar";
            string firstLetter = userRow["FullName"].ToString().Length > 0 ?
                userRow["FullName"].ToString()[0].ToString().ToUpper() : "U";
            avatar.Controls.Add(new LiteralControl(firstLetter));

            // User info
            Panel userInfo = new Panel();
            userInfo.CssClass = "user-info";

            Label lblName = new Label();
            lblName.Text = $"<div class='user-name'>{userRow["FullName"]}</div>";
            userInfo.Controls.Add(lblName);

            Label lblRole = new Label();
            lblRole.Text = $"<span class='user-role educator-role'>Educator</span>";
            userInfo.Controls.Add(lblRole);

            userHeader.Controls.Add(avatar);
            userHeader.Controls.Add(userInfo);
            mainContent.Controls.Add(userHeader);

            // User details
            Panel userDetails = new Panel();
            userDetails.CssClass = "user-details";

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = $"<div class='user-detail'><i>📧</i>{userRow["Email"]}</div>";
            userDetails.Controls.Add(lblEmail);

            // University
            Label lblUniversity = new Label();
            lblUniversity.Text = $"<div class='user-detail'><i>🎓</i>{university}</div>";
            userDetails.Controls.Add(lblUniversity);

            mainContent.Controls.Add(userDetails);

            // Stats
            Panel userStats = new Panel();
            userStats.CssClass = "user-stats";

            // Courses
            Panel coursesStat = new Panel();
            coursesStat.CssClass = "stat-item";
            coursesStat.Controls.Add(new LiteralControl($"<div class='stat-value'>{courseCount}</div><div class='stat-label'>Courses</div>"));
            userStats.Controls.Add(coursesStat);

            // Students
            Panel studentsStat = new Panel();
            studentsStat.CssClass = "stat-item";
            studentsStat.Controls.Add(new LiteralControl($"<div class='stat-value'>{students}</div><div class='stat-label'>Students</div>"));
            userStats.Controls.Add(studentsStat);

            // Empty stat for alignment
            Panel emptyStat = new Panel();
            emptyStat.CssClass = "stat-item";
            emptyStat.Controls.Add(new LiteralControl($"<div class='stat-value'>-</div><div class='stat-label'>&nbsp;</div>"));
            userStats.Controls.Add(emptyStat);

            mainContent.Controls.Add(userStats);
            userCard.Controls.Add(mainContent);

            // Status section
            Panel statusSection = new Panel();
            statusSection.CssClass = "user-status-section";

            bool isActive = userRow["Status"].ToString() == "Active";

            // Create toggle button
            Button btnToggle = new Button();
            btnToggle.ID = "btnToggle_" + userId;
            btnToggle.Text = isActive ? "Disable User" : "Enable User";
            btnToggle.CssClass = isActive ? "disable-btn" : "enable-btn";
            btnToggle.CommandArgument = userId + "|" + userRow["Status"].ToString();
            btnToggle.Click += BtnToggle_Click;

            statusSection.Controls.Add(btnToggle);
            userCard.Controls.Add(statusSection);

            pnlUserContainer.Controls.Add(userCard);
        }

        protected void BtnToggle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] args = btn.CommandArgument.Split('|');
            string userId = args[0];
            string currentStatus = args[1];

            string newStatus = currentStatus == "Active" ? "Inactive" : "Active";

            // Update database
            bool success = UpdateUserStatus(userId, newStatus);

            if (success)
            {
                ShowSuccessMessage($"User status updated to {newStatus} successfully!");
                LoadUserStats(); // Refresh stats
            }
            else
            {
                ShowErrorMessage("Error updating user status. Please try again.");
            }
        }

        protected void btnTab_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            currentFilter = btn.CommandArgument;

            // Store in ViewState
            ViewState["CurrentFilter"] = currentFilter;

            // Update active tab
            btnAllUsers.CssClass = btnAllUsers.CommandArgument == currentFilter ? "tab active" : "tab";
            btnStudents.CssClass = btnStudents.CommandArgument == currentFilter ? "tab active" : "tab";
            btnEducators.CssClass = btnEducators.CommandArgument == currentFilter ? "tab active" : "tab";

            LoadUsers();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchTerm = txtSearch.Text.Trim();
            ViewState["SearchTerm"] = searchTerm;
            LoadUsers();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            searchTerm = "";
            currentFilter = "All";

            // Store in ViewState
            ViewState["SearchTerm"] = searchTerm;
            ViewState["CurrentFilter"] = currentFilter;

            // Reset tabs
            btnAllUsers.CssClass = "tab active";
            btnStudents.CssClass = "tab";
            btnEducators.CssClass = "tab";

            LoadUsers();
        }

        private bool UpdateUserStatus(string userId, string status)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "UPDATE Users SET Status = @Status WHERE Id = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"SUCCESS: Updated user {userId} to {status}");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"FAILED: No user found with ID {userId}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR updating user status: {ex.Message}");
                return false;
            }
        }

        private void ShowSuccessMessage(string message)
        {
            pnlSuccess.Visible = true;
            lblSuccess.Text = message;
            pnlError.Visible = false;
        }

        private void ShowErrorMessage(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;
        }
    }
}