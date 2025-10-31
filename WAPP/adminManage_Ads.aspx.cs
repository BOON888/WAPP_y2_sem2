using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
                    int totalCount = Convert.ToInt32(countCmd.ExecuteScalar());
                    lblTotalAnnouncements.Text = totalCount.ToString();

                    // Get announcements
                    string query = @"
                        SELECT 
                            Id, 
                            AdContent,
                            AdImage,
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
                                row["AdImage"] != DBNull.Value ? row["AdImage"].ToString() : null,
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

        private void CreateAnnouncementCard(string id, string content, string imagePath, DateTime? startDate, DateTime? endDate)
        {
            string title = "Announcement";
            string announcementType = "promotion"; // default type
            string announcementContent = content;

            // Parse content format: "Title\n\nContent\n\n[type]"
            string[] sections = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (sections.Length >= 1)
            {
                title = sections[0].Trim();

                if (sections.Length >= 2)
                {
                    // Check if last section is the type in brackets
                    string lastSection = sections[sections.Length - 1].Trim();
                    if (lastSection.StartsWith("[") && lastSection.EndsWith("]"))
                    {
                        announcementType = lastSection.Substring(1, lastSection.Length - 2).ToLower();

                        // Rebuild content without the type section and title
                        if (sections.Length > 2)
                        {
                            announcementContent = string.Join("\n\n", sections, 1, sections.Length - 2);
                        }
                        else
                        {
                            announcementContent = ""; // Only title and type, no content
                        }
                    }
                    else
                    {
                        // No type found, use all sections after title as content
                        announcementContent = string.Join("\n\n", sections, 1, sections.Length - 1);
                    }
                }
                else
                {
                    // Only title exists, no content or type
                    announcementContent = "";
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

            // Image (if exists)
            if (!string.IsNullOrEmpty(imagePath))
            {
                Panel imagePanel = new Panel();
                imagePanel.CssClass = "announcement-image";

                Image img = new Image();

                // Fix: Use ResolveUrl to get correct image path
                string imageUrl = ResolveUrl("~/Image/" + imagePath);
                img.ImageUrl = imageUrl;
                img.AlternateText = title;
                img.CssClass = "announcement-img";

                imagePanel.Controls.Add(img);
                announcementCard.Controls.Add(imagePanel);

                System.Diagnostics.Debug.WriteLine($"Image URL for announcement {id}: {imageUrl}");
            }

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

            string imagePath = null;

            // Handle image upload from hidden fields
            string fileName = hdnFileName.Value;
            string fileData = hdnFileData.Value;

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(fileData))
            {
                try
                {
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    // Validate file type
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".gif")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Please upload only JPG, PNG, or GIF images.');", true);
                        return;
                    }

                    // Ensure Image directory exists
                    string imageDir = Server.MapPath("~/Image/");
                    if (!Directory.Exists(imageDir))
                    {
                        Directory.CreateDirectory(imageDir);
                        System.Diagnostics.Debug.WriteLine($"Created directory: {imageDir}");
                    }

                    // Generate unique filename to avoid conflicts
                    string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    string fullPath = Path.Combine(imageDir, uniqueFileName);

                    // Convert base64 string to image file
                    byte[] imageBytes = Convert.FromBase64String(fileData.Split(',')[1]); // Remove data:image/xxx;base64, prefix
                    File.WriteAllBytes(fullPath, imageBytes);

                    // Set the image path for database - store only filename
                    imagePath = uniqueFileName;

                    System.Diagnostics.Debug.WriteLine($"Image saved successfully: {fullPath}");
                    System.Diagnostics.Debug.WriteLine($"Database will store filename: {imagePath}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error uploading image: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Error uploading image. Please try again.');", true);
                    return;
                }
            }

            string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO Advertisement (AdminId, AdContent, AdImage, StartDate, EndDate)
                        VALUES (@AdminId, @AdContent, @AdImage, GETDATE(), DATEADD(month, 1, GETDATE()))";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AdminId", ADMIN_ID);
                    cmd.Parameters.AddWithValue("@AdContent", formattedContent);

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        cmd.Parameters.AddWithValue("@AdImage", imagePath);
                        System.Diagnostics.Debug.WriteLine($"Saving to database with image: {imagePath}");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@AdImage", DBNull.Value);
                        System.Diagnostics.Debug.WriteLine("Saving to database without image");
                    }

                    int result = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine($"Database insert result: {result} rows affected");

                    if (result > 0)
                    {
                        // Hide modal and refresh announcements
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "hideModal();", true);

                        // Clear form fields
                        txtTitle.Text = "";
                        txtContent.Text = "";
                        ddlType.SelectedIndex = 0;
                        hdnFileName.Value = "";
                        hdnFileData.Value = "";

                        // Reload announcements - THIS WILL UPDATE THE COUNT
                        LoadAnnouncements();

                        // Update both update panels to ensure count refreshes
                        upAnnouncements.Update();
                        upModal.Update();

                        // Show success message
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Announcement added successfully!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save announcement to database.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding announcement: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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

                    // First get the image path to delete the file
                    string getImageQuery = "SELECT AdImage FROM Advertisement WHERE Id = @Id AND AdminId = @AdminId";
                    SqlCommand getCmd = new SqlCommand(getImageQuery, conn);
                    getCmd.Parameters.AddWithValue("@Id", announcementId);
                    getCmd.Parameters.AddWithValue("@AdminId", ADMIN_ID);

                    object imagePathObj = getCmd.ExecuteScalar();

                    // Delete the announcement
                    string deleteQuery = "DELETE FROM Advertisement WHERE Id = @Id AND AdminId = @AdminId";
                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", announcementId);
                    cmd.Parameters.AddWithValue("@AdminId", ADMIN_ID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Delete the image file from Image folder
                        if (imagePathObj != null && imagePathObj != DBNull.Value)
                        {
                            string imagePath = imagePathObj.ToString();
                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                string physicalPath = Server.MapPath("~/Image/" + imagePath);
                                if (File.Exists(physicalPath))
                                {
                                    File.Delete(physicalPath);
                                    System.Diagnostics.Debug.WriteLine($"Deleted image file: {physicalPath}");
                                }
                            }
                        }

                        // Reload announcements to update count and list
                        LoadAnnouncements();
                        upAnnouncements.Update();

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Announcement deleted successfully!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to delete announcement.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting announcement: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Error deleting announcement. Please try again.');", true);
            }
        }
    }
}