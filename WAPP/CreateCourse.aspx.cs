using System;
namespace SeaLearner
{
    public partial class CreateCourse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCreateCourse_Click(object sender, EventArgs e)
        {
            string title = txtCourseTitle.Text.Trim();
            string type = rbPublic.Checked ? "Public" : rbPrivate.Checked ? "Private" : "None";

            // For now, just redirect back to dashboard (you can later save course data to DB)
            if (!string.IsNullOrEmpty(title) && type != "None")
            {
                // Temporary: redirect after "creation"
                Response.Redirect("EducatorDashboard.aspx");
            }
        }
    }
}
