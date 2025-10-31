using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class admin_feedbackmanagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFeedbackStats();
                LoadFeedback();
            }

            // Handle postback commands
            if (Request["__EVENTTARGET"] == "DeleteFeedback")
            {
                string feedbackId = Request["__EVENTARGUMENT"];
                DeleteFeedback(feedbackId);
            }

            if (Request["__EVENTTARGET"] == "CompleteFeedback")
            {
                string feedbackId = Request["__EVENTARGUMENT"];
                CompleteFeedback(feedbackId);
            }
        }

        private string GetConnectionString()
        {
            // Try to get connection string from web.config
            ConnectionStringSettings connStringSettings = ConfigurationManager.ConnectionStrings["MyConnectionString"];

            if (connStringSettings != null)
            {
                return connStringSettings.ConnectionString;
            }

            // If not found, try common names
            string[] possibleNames = { "SeaLearnerConnection", "DefaultConnection", "ConnectionString" };

            foreach (string name in possibleNames)
            {
                connStringSettings = ConfigurationManager.ConnectionStrings[name];
                if (connStringSettings != null)
                {
                    return connStringSettings.ConnectionString;
                }
            }

            throw new Exception("No database connection string found in web.config");
        }

        private void LoadFeedbackStats()
        {
            try
            {
                string connString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Total feedback
                    string totalQuery = "SELECT COUNT(*) FROM Feedback";
                    SqlCommand totalCmd = new SqlCommand(totalQuery, conn);
                    lblTotalFeedback.Text = totalCmd.ExecuteScalar().ToString();

                    // Pending feedback
                    string pendingQuery = "SELECT COUNT(*) FROM Feedback WHERE Status IS NULL OR Status = 'pending' OR Status = 'inprogress'";
                    SqlCommand pendingCmd = new SqlCommand(pendingQuery, conn);
                    lblPendingFeedback.Text = pendingCmd.ExecuteScalar().ToString();

                    // Completed feedback
                    string completedQuery = "SELECT COUNT(*) FROM Feedback WHERE Status = 'completed'";
                    SqlCommand completedCmd = new SqlCommand(completedQuery, conn);
                    lblCompletedFeedback.Text = completedCmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading statistics: " + ex.Message);
            }
        }

        private void LoadFeedback()
        {
            try
            {
                string connString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            f.Id,
                            f.Subject,
                            f.Description,
                            f.Category,
                            f.Priority,
                            f.Status,
                            f.CreatedAt,
                            u.FullName,
                            u.Email
                        FROM Feedback f
                        INNER JOIN Student s ON f.StudentId = s.Id
                        INNER JOIN Users u ON s.UserId = u.Id
                        WHERE 1=1";

                    // Build query based on filters
                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        if (ddlStatus.SelectedValue == "pending")
                        {
                            query += " AND (f.Status IS NULL OR f.Status = 'pending' OR f.Status = 'inprogress')";
                        }
                        else
                        {
                            query += " AND f.Status = @Status";
                        }
                    }

                    if (!string.IsNullOrEmpty(ddlPriority.SelectedValue))
                    {
                        query += " AND f.Priority = @Priority";
                    }

                    if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                    {
                        query += " AND f.Category = @Category";
                    }

                    query += " ORDER BY f.CreatedAt DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Add parameters
                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue) && ddlStatus.SelectedValue != "pending")
                    {
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    }

                    if (!string.IsNullOrEmpty(ddlPriority.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@Priority", ddlPriority.SelectedValue);
                    }

                    if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    DisplayFeedback(dt);
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading feedback: " + ex.Message);
            }
        }

        private void DisplayFeedback(DataTable dt)
        {
            pnlFeedback.Controls.Clear();
            lblNoFeedback.Visible = false;
            lblError.Visible = false;

            if (dt.Rows.Count == 0)
            {
                lblNoFeedback.Visible = true;
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                string id = row["Id"].ToString();
                string subject = GetSafeString(row["Subject"]);
                string description = GetSafeString(row["Description"]);
                string category = GetSafeString(row["Category"], "course-content");
                string priority = GetSafeString(row["Priority"], "medium");
                string status = GetSafeString(row["Status"], "pending");
                DateTime createdAt = row["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(row["CreatedAt"]) : DateTime.Now;
                string fullName = GetSafeString(row["FullName"], "Unknown User");
                string email = GetSafeString(row["Email"], "No email");

                // Create feedback card
                Panel card = new Panel();
                card.CssClass = "feedback-card";

                // Header
                Panel header = new Panel();
                header.CssClass = "feedback-header";

                // Title
                Literal title = new Literal();
                title.Text = $"<div class='feedback-title'>{Server.HtmlEncode(subject)}</div>";
                header.Controls.Add(title);

                // Meta info
                Panel meta = new Panel();
                meta.CssClass = "feedback-meta";

                // Priority
                Literal priorityBadge = new Literal();
                priorityBadge.Text = $"<span class='feedback-priority priority-{priority}'>{priority} priority</span>";
                meta.Controls.Add(priorityBadge);

                // Category
                Literal categoryBadge = new Literal();
                string categoryDisplay = category.Replace("-", " ");
                categoryBadge.Text = $"<span class='feedback-category category-{category}'>{categoryDisplay}</span>";
                meta.Controls.Add(categoryBadge);

                // Status
                Literal statusBadge = new Literal();
                string statusDisplay = status == "completed" ? "✔ Completed" : "Pending";
                string statusClass = status == "completed" ? "status-completed" : "status-pending";
                statusBadge.Text = $"<span class='feedback-status {statusClass}'>{statusDisplay}</span>";
                meta.Controls.Add(statusBadge);

                header.Controls.Add(meta);
                card.Controls.Add(header);

                // Description
                if (!string.IsNullOrEmpty(description) && description != "No description provided.")
                {
                    Literal desc = new Literal();
                    desc.Text = $"<div class='feedback-content'>{Server.HtmlEncode(description).Replace("\n", "<br/>")}</div>";
                    card.Controls.Add(desc);
                }

                // Footer
                Panel footer = new Panel();
                footer.CssClass = "feedback-footer";

                // User info
                Literal userInfo = new Literal();
                userInfo.Text = $@"
                    <div style='display: flex; flex-direction: column; gap: 5px;'>
                        <div class='feedback-user'>Submitted by: {Server.HtmlEncode(fullName)} ({email})</div>
                        <div class='feedback-date'>{createdAt.ToString("MM/dd/yyyy hh:mm tt")}</div>
                    </div>";
                footer.Controls.Add(userInfo);

                // Actions - Only show Complete button if not already completed
                Panel actions = new Panel();
                actions.CssClass = "feedback-actions";

                if (status != "completed")
                {
                    // Complete button
                    Button btnComplete = new Button();
                    btnComplete.Text = "Complete";
                    btnComplete.CssClass = "btn-sm btn-success";
                    btnComplete.OnClientClick = $"completeFeedback('{id}'); return false;";
                    actions.Controls.Add(btnComplete);
                }

                // Delete button
                Button btnDelete = new Button();
                btnDelete.Text = "Delete";
                btnDelete.CssClass = "btn-sm btn-danger";
                btnDelete.OnClientClick = $"confirmDelete('{id}', '{subject.Replace("'", "''")}'); return false;";
                actions.Controls.Add(btnDelete);

                footer.Controls.Add(actions);
                card.Controls.Add(footer);

                pnlFeedback.Controls.Add(card);
            }
        }

        private string GetSafeString(object value, string defaultValue = "")
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            return value.ToString();
        }

        protected void FilterChanged(object sender, EventArgs e)
        {
            LoadFeedback();
            upFeedback.Update();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            ddlStatus.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 0;
            ddlCategory.SelectedIndex = 0;
            LoadFeedback();
            upFeedback.Update();
        }

        private void DeleteFeedback(string feedbackId)
        {
            try
            {
                string connString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "DELETE FROM Feedback WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", feedbackId);
                    cmd.ExecuteNonQuery();
                }

                LoadFeedbackStats();
                LoadFeedback();
                upFeedback.Update();

                ScriptManager.RegisterStartupScript(this, GetType(), "deleteSuccess",
                    "alert('Feedback deleted successfully!');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "deleteError",
                    "alert('Error deleting feedback: " + ex.Message + "');", true);
            }
        }

        private void CompleteFeedback(string feedbackId)
        {
            try
            {
                string connString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "UPDATE Feedback SET Status = 'completed' WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", feedbackId);
                    cmd.ExecuteNonQuery();
                }

                LoadFeedbackStats();
                LoadFeedback();
                upFeedback.Update();

                ScriptManager.RegisterStartupScript(this, GetType(), "completeSuccess",
                    "alert('Feedback marked as completed!');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "completeError",
                    "alert('Error completing feedback: " + ex.Message + "');", true);
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
            lblNoFeedback.Visible = false;
        }
    }
}