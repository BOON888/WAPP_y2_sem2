using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class manage_ads : System.Web.UI.Page
    {
        private const int ADMIN_ID = 4000;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAnnouncements();
            }

            // Handle delete command
            if (Request["__EVENTTARGET"] == "DeleteAnnouncement")
            {
                string announcementId = Request["__EVENTARGUMENT"];
                DeleteAnnouncement(announcementId);
            }
        }

        private void LoadAnnouncements()
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Get total count
                    string countQuery = "SELECT COUNT(*) FROM Advertisement WHERE AdminId = @AdminId";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    countCmd.Parameters.AddWithValue("@AdminId", ADMIN_ID);
                    lblTotalAnnouncements.Text = countCmd.ExecuteScalar().ToString();

                    // Get announcements
                    string query = @"
                        SELECT 
                            Id, 
                            AdContent,
                            StartDate,
                            EndDate
                        FROM Advertisement 
                        WHERE AdminId = @AdminId
                        ORDER BY Id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AdminId", ADMIN_ID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    pnlAnnouncements.Controls.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            CreateAnnouncementCard(
                                row["Id"].ToString(),
                                row["AdContent"].ToString(),
                                row["StartDate"] != DBNull.Value ? Convert.ToDateTime(row["StartDate"]) : (DateTime?)null,
                                row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null
                            );
                        }
                        lblNoAnnouncements.Visible = false;
                    }
                    else
                    {
                        lblNoAnnouncements.Text = "No announcements found. Click 'Add New Announcement' to create your first announcement.";
                        lblNoAnnouncements.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading announcements: {ex.Message}");
                lblNoAnnouncements.Text = "Error loading announcements. Please try again.";
                lblNoAnnouncements.Visible = true;
            }
        }

        private void CreateAnnouncementCard(string id, string content, DateTime? startDate, DateTime? endDate)
        {
            // Parse the content to extract title, content, and type
            string title = "Announcement";
            string announcementType = "promotion"; // default type
            string announcementContent = content;

            // Parse content format: "Title\n\nContent\n\n[type]"
            string[] sections = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (sections.Length > 0)
            {
                title = sections[0].Trim();

                if (sections.Length > 1)
                {
                    // Check if last section is the type in brackets
                    string lastSection = sections[sections.Length - 1].Trim();
                    if (lastSection.StartsWith("[") && lastSection.EndsWith("]"))
                    {
                        announcementType = lastSection.Substring(1, lastSection.Length - 2).ToLower();
                        // Rebuild content without the type section
                        announcementContent = string.Join("\n\n", sections, 1, sections.Length - 2);
                    }
                    else
                    {
                        announcementContent = string.Join("\n\n", sections, 1, sections.Length - 1);
                    }
                }
            }

            // Ensure type is one of the three valid types
            if (announcementType != "promotion" && announcementType != "lesson" && announcementType != "partner")
            {
                announcementType = "promotion"; // default to promotion if invalid type
            }

            Panel announcementCard = new Panel();
            announcementCard.CssClass = "announcement-card";

            // Header
            Panel header = new Panel();
            header.CssClass = "announcement-header";

            Label lblTitle = new Label();
            lblTitle.Text = $"<div class='announcement-title'>{Server.HtmlEncode(title)}</div>";
            header.Controls.Add(lblTitle);

            Label lblType = new Label();
            lblType.Text = $"<span class='announcement-type type-{announcementType}'>{announcementType}</span>";
            header.Controls.Add(lblType);

            announcementCard.Controls.Add(header);

            // Content
            if (!string.IsNullOrEmpty(announcementContent))
            {
                Label lblContent = new Label();
                lblContent.Text = $"<div class='announcement-content'>{Server.HtmlEncode(announcementContent).Replace("\n", "<br/>")}</div>";
                announcementCard.Controls.Add(lblContent);
            }

            // Footer
            Panel footer = new Panel();
            footer.CssClass = "announcement-footer";

            // ID and dates
            Panel infoPanel = new Panel();
            infoPanel.Style.Add("display", "flex");
            infoPanel.Style.Add("flex-direction", "column");
            infoPanel.Style.Add("gap", "5px");

            Label lblId = new Label();
            lblId.Text = $"ID: {id}";
            lblId.Style.Add("font-weight", "500");
            infoPanel.Controls.Add(lblId);

            if (startDate.HasValue)
            {
                Label lblStartDate = new Label();
                lblStartDate.Text = $"Started: {startDate.Value.ToString("MMM dd, yyyy")}";
                lblStartDate.CssClass = "announcement-date";
                infoPanel.Controls.Add(lblStartDate);
            }

            if (endDate.HasValue)
            {
                Label lblEndDate = new Label();
                lblEndDate.Text = $"Expires: {endDate.Value.ToString("MMM dd, yyyy")}";
                lblEndDate.CssClass = "announcement-date";
                infoPanel.Controls.Add(lblEndDate);
            }

            footer.Controls.Add(infoPanel);

            // Actions
            Panel actions = new Panel();
            actions.CssClass = "announcement-actions";

            // Delete button
            Button btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.CssClass = "btn-danger";
            btnDelete.OnClientClick = $"confirmDelete('{id}', '{title.Replace("'", "\\'")}'); return false;";
            actions.Controls.Add(btnDelete);

            footer.Controls.Add(actions);
            announcementCard.Controls.Add(footer);

            pnlAnnouncements.Controls.Add(announcementCard);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();
            string type = ddlType.SelectedValue;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                // Show error message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Please fill in all fields.');", true);
                return;
            }

            // Format content: Title + Content + [Type]
            string formattedContent = $"{title}\n\n{content}\n\n[{type}]";

            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO Advertisement (AdminId, AdContent, StartDate, EndDate)
                        VALUES (@AdminId, @AdContent, GETDATE(), DATEADD(month, 1, GETDATE()))";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AdminId", ADMIN_ID);
                    cmd.Parameters.AddWithValue("@AdContent", formattedContent);

                    cmd.ExecuteNonQuery();

                    // Hide modal and refresh announcements
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "hideModal();", true);

                    // Clear form fields
                    txtTitle.Text = "";
                    txtContent.Text = "";
                    ddlType.SelectedIndex = 0;

                    // Reload announcements
                    LoadAnnouncements();

                    // Update the update panel
                    upAnnouncements.Update();

                    // Show success message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Announcement added successfully!');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding announcement: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Error adding announcement. Please try again.');", true);
            }
        }

        private void DeleteAnnouncement(string announcementId)
        {
            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = "DELETE FROM Advertisement WHERE Id = @Id AND AdminId = @AdminId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", announcementId);
                    cmd.Parameters.AddWithValue("@AdminId", ADMIN_ID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        LoadAnnouncements();
                        upAnnouncements.Update();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Announcement deleted successfully!');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting announcement: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Error deleting announcement. Please try again.');", true);
            }
        }
    }
}