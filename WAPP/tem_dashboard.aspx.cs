using System;

namespace WAPP
{
    public partial class tem_dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCommunity_Click(object sender, EventArgs e)
        {
            // Redirect user to community page
            Response.Redirect("community.aspx");
        }
    }
}
