using System;

namespace WAPP
{
    public partial class profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["FullName"] != null)
                {
                    lblFullName.Text = Session["FullName"].ToString();
                }
                else
                {
                    lblFullName.Text = "Guest";
                }

                if (Session["ProfilePicture"] != null && !string.IsNullOrEmpty(Session["ProfilePicture"].ToString()))
                {
                    // If only filename is stored, add folder path
                    imgProfile.ImageUrl = "~/image/" + Session["ProfilePicture"].ToString();
                }
                else
                {
                    imgProfile.ImageUrl = "~/image/default_profile.png"; // default image
                }
            }
        }
    }
}
