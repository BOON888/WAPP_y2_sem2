using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
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
                    string query = "SELECT COUNT(*) FROM Student";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    lblTotalStudents.Text = cmd.ExecuteScalar().ToString();
                }

                // Total Educators
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT COUNT(*) FROM Educator";
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
                    string query = "SELECT COUNT(*) FROM Feedback WHERE Status = 'Pending'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    lblPendingFeedback.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                // Silent fail - don't show error messages for stats
            }
        }

        private void LoadCourses()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
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

                    gvCourses.DataSource = dt;
                    gvCourses.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading courses. Please try again.", "error");
                gvCourses.DataSource = null;
                gvCourses.DataBind();
            }
        }

        private int GetTotalCourseCount(string whereSql, System.Collections.Generic.Dictionary<string, object> parameters)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = $"SELECT COUNT(*) FROM Course {whereSql}";
                SqlCommand cmd = new SqlCommand(query, conn);

                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // GridView Events
        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCourse")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                string courseId = args[0];
                string courseTitle = args.Length > 1 ? args[1] : "Unknown Course";

                try
                {
                    bool deleted = DeleteCourseAndAllRelatedData(courseId);
                    if (deleted)
                    {
                        ShowMessage($"Course '{courseTitle}' (ID: {courseId}) has been successfully deleted.", "success");
                        LoadCourses();
                        LoadDashboardStats();
                    }
                    else
                    {
                        ShowMessage($"Course '{courseTitle}' (ID: {courseId}) was not found or could not be deleted.", "error");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error deleting course: {ex.Message}", "error");
                }
            }
            else if (e.CommandName == "ViewCourseContent")
            {
                string courseId = e.CommandArgument.ToString();
                // Redirect to StudentCourseContent.aspx with the course ID
                Response.Redirect($"StudentCourseContent.aspx?courseId={courseId}");
            }
        }

        // Clean method to delete course and all related data
        private bool DeleteCourseAndAllRelatedData(string courseId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Delete in correct order to respect foreign key constraints
                    string[] deleteQueries = {
                        // Delete from tables that reference other tables first
                        "DELETE FROM StudentQuizProgress WHERE QuizId IN (SELECT Id FROM Quiz WHERE LessonId IN (SELECT Id FROM Lesson WHERE CourseId = @CourseID))",
                        "DELETE FROM Question WHERE QuizId IN (SELECT Id FROM Quiz WHERE LessonId IN (SELECT Id FROM Lesson WHERE CourseId = @CourseID))",
                        
                        // Then delete from intermediate tables
                        "DELETE FROM Quiz WHERE LessonId IN (SELECT Id FROM Lesson WHERE CourseId = @CourseID)",
                        "DELETE FROM StudentCourseProgress WHERE CourseId = @CourseID",
                        "DELETE FROM Lesson WHERE CourseId = @CourseID",
                        
                        // Finally delete the course
                        "DELETE FROM Course WHERE Id = @CourseID"
                    };

                    foreach (string query in deleteQueries)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CourseID", courseId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected void gvCourses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Additional row binding logic if needed
        }

        // Helper methods for GridView templates
        public string GetCourseTypeClass(string courseType)
        {
            if (string.IsNullOrEmpty(courseType) || courseType == "Not Set")
                return "free";
            return courseType.ToLower().Contains("free") ? "free" : "premium";
        }

        public string GetStatusClass(string status)
        {
            if (string.IsNullOrEmpty(status) || status == "Not Set")
                return "active";
            return status.ToLower().Contains("active") ? "active" : "inactive";
        }

        private (string whereSql, System.Collections.Generic.Dictionary<string, object> parameters) BuildWhereClauseWithParameters()
        {
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (string.IsNullOrEmpty(SearchValue) || FilterType == "All")
                return ("", parameters);

            string whereSql = "WHERE ";
            switch (FilterType)
            {
                case "CourseID":
                    if (int.TryParse(SearchValue, out int courseId))
                    {
                        whereSql += "Id = @SearchValue";
                        parameters.Add("@SearchValue", courseId);
                    }
                    else
                    {
                        whereSql += "1 = 0";
                    }
                    break;
                case "EducatorID":
                    if (int.TryParse(SearchValue, out int educatorId))
                    {
                        whereSql += "EducatorId = @SearchValue";
                        parameters.Add("@SearchValue", educatorId);
                    }
                    else
                    {
                        whereSql += "1 = 0";
                    }
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

        // Message display method
        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;

            if (type == "success")
            {
                pnlMessage.CssClass = "message-container message-success";
            }
            else
            {
                pnlMessage.CssClass = "message-container message-error";
            }

            // Auto-hide success messages after 5 seconds
            if (type == "success")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideMessage",
                    "setTimeout(function() { document.getElementById('" + pnlMessage.ClientID + "').style.display = 'none'; }, 5000);", true);
            }
        }
    }
}