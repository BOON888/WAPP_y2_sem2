using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class my_post : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/sign_in.aspx");
            }

            if (!IsPostBack)
            {
                LoadMyPosts();
            }
        }

        private void LoadMyPosts()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DataTable dtPosts = new DataTable();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT CP.Id, CP.UserId, U.FullName, U.Role, CP.PostContent, CP.PostDateTime
                                 FROM CommunityPost CP
                                 JOIN Users U ON CP.UserId = U.Id
                                 WHERE CP.UserId = @UserId
                                 ORDER BY CP.PostDateTime DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@UserId", userId);
                da.Fill(dtPosts);
            }

            // ✅ Add Replies column before loop
            if (!dtPosts.Columns.Contains("Replies"))
                dtPosts.Columns.Add("Replies", typeof(object));

            foreach (DataRow row in dtPosts.Rows)
            {
                int postId = Convert.ToInt32(row["Id"]);
                row["Replies"] = GetReplies(postId);
            }

            if (dtPosts.Rows.Count == 0)
            {
                lblNoPosts.Visible = true;
            }

            rptMyPosts.DataSource = dtPosts;
            rptMyPosts.DataBind();
        }

        private DataTable GetReplies(int postId)
        {
            DataTable dtReplies = new DataTable();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT R.Id, R.PostId, U.FullName, R.ReplyContent, R.ReplyDateTime
                                 FROM Reply R
                                 JOIN Users U ON R.UserId = U.Id
                                 WHERE R.PostId = @PostId
                                 ORDER BY R.ReplyDateTime ASC";


                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@PostId", postId);
                da.Fill(dtReplies);
            }

            return dtReplies;
        }

        protected void btnDelete_PreRender(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            btn.Attributes["data-uniqueid"] = btn.UniqueID;
            btn.Visible = true; // Make it visible after assigning the attribute
        }


        protected void rptMyPosts_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeletePost")
            {
                int postId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Delete replies first to maintain foreign key integrity
                    string deleteReplies = "DELETE FROM Reply WHERE PostId = @PostId";
                    using (SqlCommand cmd = new SqlCommand(deleteReplies, conn))
                    {
                        cmd.Parameters.AddWithValue("@PostId", postId);
                        cmd.ExecuteNonQuery();
                    }

                    // ✅ Delete the main post now
                    string deletePost = "DELETE FROM CommunityPost WHERE Id = @PostId";
                    using (SqlCommand cmd = new SqlCommand(deletePost, conn))
                    {
                        cmd.Parameters.AddWithValue("@PostId", postId);
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                // Reload updated list
                LoadMyPosts();
            }
        }
    }
}
