using System;

namespace WAPP
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string currentPage = System.IO.Path.GetFileNameWithoutExtension(Request.Path);

            if (currentPage == "public_dashboard" || currentPage == "sign_in" || currentPage == "sign_up")
            {
                navSignIn.Visible = true;
                navSignUp.Visible = true;
                navLogout.Visible = false;
                navUserProfile.Visible = false;
            }
            else
            {
                navSignIn.Visible = false;
                navSignUp.Visible = false;

                // Check if user is logged in
                if (Session["UserId"] != null)
                {
                    navUserProfile.Visible = true;
                    navLogout.Visible = true;

                    // Display user's name and profile picture
                    lblUserFullName.Text = Session["FullName"].ToString();

                    if (Session["ProfilePicture"] != null && !string.IsNullOrEmpty(Session["ProfilePicture"].ToString()))
                    {
                        string fileName = Session["ProfilePicture"].ToString();
                        imgProfileHeader.ImageUrl = "~/image/" + fileName;
                    }
                    else
                    {
                        imgProfileHeader.ImageUrl = "~/image/default_profile.png"; // default placeholder image
                    }
                }
                else
                {
                    navUserProfile.Visible = false;
                    navLogout.Visible = false;
                }
            }
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session.Clear();           // Remove all session data
            Session.Abandon();         // End the session
            Response.Redirect("~/public_dashboard");
        }

    }
}
