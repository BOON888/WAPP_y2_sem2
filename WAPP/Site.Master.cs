using System;

namespace WAPP
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the current page name (e.g. "sign_in.aspx")
            string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath).ToLower();

            // Only show Sign-In and Sign-Up on specific pages
            if (currentPage == "public_dashboard" || currentPage == "sign_in" || currentPage == "sign_up")
            {
                navSignIn.Visible = true;
                navSignUp.Visible = true;
            }
            else
            {
                navSignIn.Visible = false;
                navSignUp.Visible = false;
            }
        }
    }
}
