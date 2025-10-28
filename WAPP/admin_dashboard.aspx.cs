using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private int currentPage = 1;
        private const int pageSize = 6;

        // Properties to persist filter values in ViewState
        private string FilterType
        {
            get { return ViewState["FilterType"] as string ?? "All"; }
            set { ViewState["FilterType"] = value; }
        }

        private string SearchValue
        {
            get { return ViewState["SearchValue"] as string ?? ""; }
            set { ViewState["SearchValue"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                currentPage = 1;
                ViewState["CurrentPage"] = currentPage;
                FilterType = "All";
                SearchValue = "";
                LoadDashboardStats();
                LoadCourses();
            }
            else
            {
                currentPage = (int)(ViewState["CurrentPage"] ?? 1);
            }

            // Update the search box with persisted value
            if (!IsPostBack)
            {
                txtSearchValue.Text = SearchValue;
                ddlFilterType.SelectedValue = FilterType;
            }
        }

        private void LoadDashboardStats()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                // Total Students
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT COUNT(*) FROM Student"; // Adjust table name as needed
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    lblTotalStudents.Text = cmd.ExecuteScalar().ToString();
                }

                // Total Educators
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT COUNT(*) FROM Educator"; // Adjust table name as needed
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    lblTotalTeachers.Text = cmd.ExecuteScalar().ToString();
                }

                // Total Courses
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT COUNT(*) FROM Course";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    lblTotalCourses.Text = cmd.ExecuteScalar().ToString();
                }

                // Pending Feedback
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT COUNT(*) FROM Feedback WHERE Status = 'Pending'"; // Adjust table/column names
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    lblPendingFeedback.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                // Handle database errors - you might want to show a message
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard stats: {ex.Message}");
            }
        }

        private void LoadCourses()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                string whereClause = BuildWhereClause();
                var (whereSql, parameters) = BuildWhereClauseWithParameters();

                // Get total count for pagination
                int totalRecords = GetTotalCourseCount(whereSql, parameters);
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Update pagination controls
                lblCurrentPage.Text = currentPage.ToString();
                lblTotalPages.Text = totalPages.ToString();
                btnPrev.Enabled = currentPage > 1;
                btnNext.Enabled = currentPage < totalPages;

                // Calculate pagination
                int startIndex = (currentPage - 1) * pageSize;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = $@"
                        SELECT Id, EducatorId, CourseType, Status, Title 
                        FROM Course 
                        {whereSql}
                        ORDER BY Id 
                        OFFSET {startIndex} ROWS 
                        FETCH NEXT {pageSize} ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Add parameters to prevent SQL injection
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    pnlCourseContainer.Controls.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            AddCourseRow(
                                row["Id"].ToString(),
                                row["Title"] != DBNull.Value ? row["Title"].ToString() : "No Title",
                                row["EducatorId"].ToString(),
                                row["CourseType"].ToString(),
                                row["Status"].ToString()
                            );
                        }
                        lblNoCourses.Visible = false;
                    }
                    else
                    {
                        lblNoCourses.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle database errors
                System.Diagnostics.Debug.WriteLine($"Error loading courses: {ex.Message}");
                lblNoCourses.Text = "Error loading courses. Please try again.";
                lblNoCourses.Visible = true;
            }
        }

        private int GetTotalCourseCount(string whereSql, System.Collections.Generic.Dictionary<string, object> parameters)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = $"SELECT COUNT(*) FROM Course {whereSql}";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Add parameters to prevent SQL injection
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private string BuildWhereClause()
        {
            if (string.IsNullOrEmpty(SearchValue) || FilterType == "All")
                return "";

            switch (FilterType)
            {
                case "CourseID":
                    return $"WHERE Id = '{SearchValue}'";
                case "EducatorID":
                    return $"WHERE EducatorId = '{SearchValue}'";
                case "CourseType":
                    return $"WHERE CourseType LIKE '%{SearchValue}%'";
                case "Status":
                    return $"WHERE Status LIKE '%{SearchValue}%'";
                default:
                    return "";
            }
        }

        // New method with parameterized queries to prevent SQL injection
        private (string whereSql, System.Collections.Generic.Dictionary<string, object> parameters) BuildWhereClauseWithParameters()
        {
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (string.IsNullOrEmpty(SearchValue) || FilterType == "All")
                return ("", parameters);

            string whereSql = "WHERE ";
            switch (FilterType)
            {
                case "CourseID":
                    whereSql += "Id = @SearchValue";
                    parameters.Add("@SearchValue", SearchValue);
                    break;
                case "EducatorID":
                    whereSql += "EducatorId = @SearchValue";
                    parameters.Add("@SearchValue", SearchValue);
                    break;
                case "CourseType":
                    whereSql += "CourseType LIKE @SearchValue";
                    parameters.Add("@SearchValue", "%" + SearchValue + "%");
                    break;
                case "Status":
                    whereSql += "Status LIKE @SearchValue";
                    parameters.Add("@SearchValue", "%" + SearchValue + "%");
                    break;
                default:
                    return ("", parameters);
            }

            return (whereSql, parameters);
        }

        private void AddCourseRow(string courseId, string title, string educatorId, string courseType, string status)
        {
            Panel courseRow = new Panel();
            courseRow.CssClass = "course-row";

            // Course ID
            Label lblId = new Label();
            lblId.Text = $"<div class='course-id'>{courseId}</div>";
            courseRow.Controls.Add(lblId);

            // Course Title
            Label lblTitle = new Label();
            lblTitle.Text = $"<div class='course-title'>{title}</div>";
            courseRow.Controls.Add(lblTitle);

            // Educator ID
            Label lblEducator = new Label();
            lblEducator.Text = $"<div>{educatorId}</div>";
            courseRow.Controls.Add(lblEducator);

            // Course Type
            Label lblType = new Label();
            string typeClass = GetCourseTypeClass(courseType);
            lblType.Text = $"<div class='course-type {typeClass}'>{courseType}</div>";
            courseRow.Controls.Add(lblType);

            // Status
            Label lblStatus = new Label();
            string statusClass = GetStatusClass(status);
            lblStatus.Text = $"<div class='course-status {statusClass}'>{status}</div>";
            courseRow.Controls.Add(lblStatus);

            // Actions
            Panel actionsPanel = new Panel();
            actionsPanel.CssClass = "course-actions";

            Button btnView = new Button();
            btnView.Text = "View";
            btnView.CssClass = "btn btn-view";
            btnView.CommandArgument = courseId;
            btnView.Click += btnView_Click;
            actionsPanel.Controls.Add(btnView);

            Button btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.CssClass = "btn btn-delete";
            btnDelete.CommandArgument = courseId;
            btnDelete.Click += btnDelete_Click;
            btnDelete.OnClientClick = "return confirm('Are you sure you want to delete this course?');";
            actionsPanel.Controls.Add(btnDelete);

            courseRow.Controls.Add(actionsPanel);
            pnlCourseContainer.Controls.Add(courseRow);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string courseId = btn.CommandArgument;
            Response.Redirect($"CourseDetails.aspx?id={courseId}");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string courseId = btn.CommandArgument;
            DeleteCourse(courseId);
            LoadCourses(); // Refresh the course list
            LoadDashboardStats(); // Refresh stats
        }

        private void DeleteCourse(string courseId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string deleteQuery = "DELETE FROM Course WHERE Id = @CourseID";
                SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Filter and Search Events
        protected void ddlFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterType = ddlFilterType.SelectedValue;
            currentPage = 1;
            ViewState["CurrentPage"] = currentPage;
            LoadCourses();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchValue = txtSearchValue.Text.Trim();
            currentPage = 1;
            ViewState["CurrentPage"] = currentPage;
            LoadCourses();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchValue.Text = "";
            SearchValue = "";
            ddlFilterType.SelectedIndex = 0;
            FilterType = "All";
            currentPage = 1;
            ViewState["CurrentPage"] = currentPage;
            LoadCourses();
        }

        // Pagination Events
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                ViewState["CurrentPage"] = currentPage;
                LoadCourses();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            var (whereSql, parameters) = BuildWhereClauseWithParameters();
            int totalRecords = GetTotalCourseCount(whereSql, parameters);
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            if (currentPage < totalPages)
            {
                currentPage++;
                ViewState["CurrentPage"] = currentPage;
                LoadCourses();
            }
        }

        // Helper methods
        private string GetCourseTypeClass(string courseType)
        {
            if (string.IsNullOrEmpty(courseType))
                return "free";
            return courseType.ToLower().Contains("free") ? "free" : "premium";
        }

        private string GetStatusClass(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "active";
            return status.ToLower().Contains("active") ? "active" : "inactive";
        }
    }
}