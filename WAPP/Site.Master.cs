using System;

namespace WAPP
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string currentPage = System.IO.Path.GetFileNameWithoutExtension(Request.Path).ToLower();

            // 🔹 Control top navigation (Sign In / Sign Up / Profile / Logout)
            if (currentPage == "public_dashboard" || currentPage == "sign_in" || currentPage == "sign_up" || currentPage == "forgot_password")
            {
                navSignIn.Visible = true;
                navSignUp.Visible = true;
                navLogout.Visible = false;
                navUserProfile.Visible = false;
                menuBar.Visible = false;
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

            if (Session["Role"] != null)
            {
                string role = Session["Role"].ToString().ToLower();

                switch (role)
                {
                    case "student":
                        navDashboardLink.HRef = "~/StudentDashboard.aspx";
                        break;
                    case "educator":
                        navDashboardLink.HRef = "~/educatordashboard.aspx";
                        break;
                    case "admin":
                        navDashboardLink.HRef = "~/admin_dashboard.aspx";
                        break;
                    default:
                        navDashboardLink.HRef = "~/public_dashboard.aspx";
                        break;
                }
            }
            else
            {
                // User not logged in
                navDashboardLink.HRef = "~/public_dashboard.aspx";
            }

            studentMenu.Visible = false;
            educatorMenu.Visible = false;
            adminMenu.Visible = false;

            string userRole = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";

            if ((currentPage == "studentdashboard" ||
                 currentPage == "feedback" ||
                 currentPage == "leaderboard" ||
                 currentPage == "privatecourse" ||
                 currentPage == "publiccourse" ||
                 currentPage == "studentprofile" ||
                 currentPage == "studentcoursecontent" ||
                 currentPage == "studentlesson" ||
                 currentPage == "studentquiz" ||
                 currentPage == "studentquizresult" ||
                 (currentPage == "community" && userRole == "student")||
                 (currentPage == "my_post" && userRole == "student")))
            {
                studentMenu.Visible = true;
            }
            else if ((currentPage == "educatordashboard" ||
                      currentPage == "educatorprofile" ||
                      currentPage == "createcourse" ||
                      currentPage == "educatorcoursestudents" ||
                      (currentPage == "community" && userRole == "educator")||
                      (currentPage == "my_post" && userRole == "educator")))
            {
                educatorMenu.Visible = true;
            }
            else if (currentPage == "admin_dashboard" ||
                     currentPage == "adminmanage_ads" ||
                     currentPage == "adminuser_management" ||
                     currentPage == "admin_communitymanagement" ||
                     currentPage == "admin_feedbackmanagement")
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

