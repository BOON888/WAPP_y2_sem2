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

            // Handle delete command
            if (Request["__EVENTTARGET"] == "DeleteFeedback")
            {
                string feedbackId = Request["__EVENTARGUMENT"];
                DeleteFeedback(feedbackId);
            }

            // Handle status update command
            if (Request["__EVENTTARGET"] == "UpdateStatus")
            {
                string[] args = Request["__EVENTARGUMENT"].Split('|');
                if (args.Length == 2)
                {
                    UpdateFeedbackStatus(args[0], args[1]);
                }
            }
        }

        private void LoadFeedbackStats()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Get total feedback count
                    string totalQuery = "SELECT COUNT(*) FROM Feedback";
                    SqlCommand totalCmd = new SqlCommand(totalQuery, conn);
                    int totalCount = Convert.ToInt32(totalCmd.ExecuteScalar());
                    lblTotalFeedback.Text = totalCount.ToString();

                    // Get pending count
                    string pendingQuery = "SELECT COUNT(*) FROM Feedback WHERE Status = 'pending' OR Status IS NULL";
                    SqlCommand pendingCmd = new SqlCommand(pendingQuery, conn);
                    int pendingCount = Convert.ToInt32(pendingCmd.ExecuteScalar());
                    lblPendingFeedback.Text = pendingCount.ToString();

                    // Get completed count
                    string completedQuery = "SELECT COUNT(*) FROM Feedback WHERE Status = 'completed'";
                    SqlCommand completedCmd = new SqlCommand(completedQuery, conn);
                    int completedCount = Convert.ToInt32(completedCmd.ExecuteScalar());
                    lblCompletedFeedback.Text = completedCount.ToString();

                    // Get technical issues count
                    string technicalQuery = "SELECT COUNT(*) FROM Feedback WHERE Category = 'technical'";
                    SqlCommand technicalCmd = new SqlCommand(technicalQuery, conn);
                    int technicalCount = Convert.ToInt32(technicalCmd.ExecuteScalar());
                    lblTechnicalFeedback.Text = technicalCount.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading feedback stats: {ex.Message}");
                // Set default values if there's an error
                lblTotalFeedback.Text = "0";
                lblPendingFeedback.Text = "0";
                lblCompletedFeedback.Text = "0";
                lblTechnicalFeedback.Text = "0";
            }
        }

        private void LoadFeedback()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
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
                            s.FirstName,
                            s.LastName,
                            s.Email
                        FROM Feedback f
                        INNER JOIN Student s ON f.StudentId = s.Id
                        WHERE 1=1";

                    // Apply filters
                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        query += " AND f.Status = @Status";
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

                    // Add parameters if filters are selected
                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);

                    if (!string.IsNullOrEmpty(ddlPriority.SelectedValue))
                        cmd.Parameters.AddWithValue("@Priority", ddlPriority.SelectedValue);

                    if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                        cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    pnlFeedback.Controls.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            CreateFeedbackCard(
                                row["Id"].ToString(),
                                row["Subject"].ToString(),
                                row["Description"].ToString(),
                                row["Category"] != DBNull.Value ? row["Category"].ToString() : "general",
                                row["Priority"] != DBNull.Value ? row["Priority"].ToString() : "medium",
                                row["Status"] != DBNull.Value ? row["Status"].ToString() : "pending",
                                Convert.ToDateTime(row["CreatedAt"]),
                                row["FirstName"].ToString(),
                                row["LastName"].ToString(),
                                row["Email"].ToString()
                            );
                        }
                        lblNoFeedback.Visible = false;
                    }
                    else
                    {
                        lblNoFeedback.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading feedback: {ex.Message}");
                lblNoFeedback.Text = "Error loading feedback. Please try again.";
                lblNoFeedback.Visible = true;
            }
        }

        private void CreateFeedbackCard(string id, string subject, string description, string category, string priority, string status, DateTime createdAt, string firstName, string lastName, string email)
        {
            Panel feedbackCard = new Panel();
            feedbackCard.CssClass = "feedback-card";

            // Header
            Panel header = new Panel();
            header.CssClass = "feedback-header";

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = $"<div class='feedback-title'>{Server.HtmlEncode(subject)}</div>";
            header.Controls.Add(lblTitle);

            // Meta information (priority, category, status)
            Panel metaPanel = new Panel();
            metaPanel.CssClass = "feedback-meta";

            // Priority badge
            Label lblPriority = new Label();
            lblPriority.Text = $"<span class='feedback-priority priority-{priority}'>{priority} priority</span>";
            metaPanel.Controls.Add(lblPriority);

            // Category badge
            Label lblCategory = new Label();
            lblCategory.Text = $"<span class='feedback-category category-{category}'>{category}</span>";
            metaPanel.Controls.Add(lblCategory);

            // Status badge
            Label lblStatus = new Label();
            string statusDisplay = status == "completed" ? "✔ Completed" : status;
            lblStatus.Text = $"<span class='feedback-status status-{status}'>{statusDisplay}</span>";
            metaPanel.Controls.Add(lblStatus);

            header.Controls.Add(metaPanel);
            feedbackCard.Controls.Add(header);

            // Content
            if (!string.IsNullOrEmpty(description))
            {
                Label lblContent = new Label();
                lblContent.Text = $"<div class='feedback-content'>{Server.HtmlEncode(description).Replace("\n", "<br/>")}</div>";
                feedbackCard.Controls.Add(lblContent);
            }

            // Footer
            Panel footer = new Panel();
            footer.CssClass = "feedback-footer";

            // User and date info
            Panel infoPanel = new Panel();
            infoPanel.Style.Add("display", "flex");
            infoPanel.Style.Add("flex-direction", "column");
            infoPanel.Style.Add("gap", "5px");

            Label lblUser = new Label();
            string displayName = $"{firstName} {lastName}";
            lblUser.Text = $"<div class='feedback-user'>Submitted by: {Server.HtmlEncode(displayName)} ({email})</div>";
            infoPanel.Controls.Add(lblUser);

            Label lblDate = new Label();
            lblDate.Text = $"<div class='feedback-date'>{createdAt.ToString("MM/dd/yyyy hh:mm tt")}</div>";
            infoPanel.Controls.Add(lblDate);

            footer.Controls.Add(infoPanel);

            // Actions
            Panel actions = new Panel();
            actions.CssClass = "feedback-actions";

            // Update status button
            Button btnUpdateStatus = new Button();
            btnUpdateStatus.Text = "Update Status";
            btnUpdateStatus.CssClass = "btn-sm btn-info";
            btnUpdateStatus.OnClientClick = $"updateStatus('{id}', '{status}'); return false;";
            actions.Controls.Add(btnUpdateStatus);

            // Delete button
            Button btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.CssClass = "btn-sm btn-danger";
            btnDelete.OnClientClick = $"confirmDelete('{id}', '{subject.Replace("'", "\\'")}'); return false;";
            actions.Controls.Add(btnDelete);

            footer.Controls.Add(actions);
            feedbackCard.Controls.Add(footer);

            pnlFeedback.Controls.Add(feedbackCard);
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
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string deleteQuery = "DELETE FROM Feedback WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", feedbackId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        LoadFeedbackStats();
                        LoadFeedback();
                        upFeedback.Update();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Feedback deleted successfully!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to delete feedback.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting feedback: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Error deleting feedback. Please try again.');", true);
            }
        }

        private void UpdateFeedbackStatus(string feedbackId, string newStatus)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string updateQuery = "UPDATE Feedback SET Status = @Status WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@Id", feedbackId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        LoadFeedbackStats();
                        LoadFeedback();
                        upFeedback.Update();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess",
                            $"alert('Feedback status updated to {newStatus} successfully!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to update feedback status.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating feedback status: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Error updating feedback status. Please try again.');", true);
            }
        }
    }
}