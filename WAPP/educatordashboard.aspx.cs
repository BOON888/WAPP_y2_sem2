using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WAPP
{
	public partial class educatordashboard1 : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/sign_in");
            }
            lblWelcome.Text = "Welcome, " + Session["FullName"];
        }

    }
}