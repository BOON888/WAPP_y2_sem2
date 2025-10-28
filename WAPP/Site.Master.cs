using System;

namespace WAPP
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string currentPage = System.IO.Path.GetFileNameWithoutExtension(Request.Path).ToLower();

            // 🔹 Control top navigation (Sign In / Sign Up / Profile / Logout)
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

                    // Display user's name and role
                    lblUserFullName.Text = Session["FullName"].ToString();

                    string role = Session["Role"].ToString().ToLower();
                    lblRole.Text = char.ToUpper(role[0]) + role.Substring(1).ToLower();

                    // Set profile image
                    if (Session["ProfilePicture"] != null && !string.IsNullOrEmpty(Session["ProfilePicture"].ToString()))
                    {
                        string fileName = Session["ProfilePicture"].ToString();
                        imgProfileHeader.ImageUrl = "~/image/" + fileName;
                    }
                    else
                    {
                        imgProfileHeader.ImageUrl = "~/image/default_profile.png"; // default placeholder image
                    }

                    // ✅ Set correct profile link based on role
                    if (role == "educator")
                    {
                        lnkProfile.HRef = "~/educatorProfile.aspx";
                    }
                    else if (role == "student")
                    {
                        lnkProfile.HRef = "~/studentProfile.aspx";
                    }
                    else
                    {
                        // Default fallback (optional)
                        lnkProfile.HRef = "~/profile.aspx";
                    }
                }
                else
                {
                    navUserProfile.Visible = false;
                    navLogout.Visible = false;
                }
            }

            studentMenu.Visible = false;
            educatorMenu.Visible = false;
            adminMenu.Visible = false;

            currentPage = currentPage.ToLower();

            if (currentPage == "studentdashboard" ||
                currentPage == "feedback" ||
                currentPage == "leaderboard" ||
                currentPage == "privatecourse" ||
                currentPage == "publiccourse" ||
                currentPage == "studentcoursecontent" ||
                currentPage == "studentlesson" ||
                currentPage == "studentquiz" ||
                currentPage == "studentquizresult")
            {
                studentMenu.Visible = true;
            }
            else if (currentPage == "educatordashboard")
            {
                educatorMenu.Visible = true;
            }
            else if (currentPage == "admin_dashboard")
            {
                adminMenu.Visible = true;
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

