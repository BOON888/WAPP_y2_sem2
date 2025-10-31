using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class admin_communitymanagement : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // No session check - anyone can access this management page
            if (!IsPostBack)
            {
                LoadStatistics();
                LoadPosts();
            }
        }

        private void LoadStatistics()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Total Posts only
                string postsQuery = "SELECT COUNT(*) FROM CommunityPost";
                SqlCommand postsCmd = new SqlCommand(postsQuery, conn);
                lblTotalPosts.Text = postsCmd.ExecuteScalar().ToString();
            }
        }

        private void LoadPosts()
        {
            DataTable dtPosts = new DataTable();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT CP.Id, CP.UserId, U.FullName, U.Role, CP.PostContent, CP.PostDateTime
                         FROM CommunityPost CP
                         JOIN Users U ON CP.UserId = U.Id
                         ORDER BY CP.PostDateTime DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dtPosts);
            }

            // Add Replies column
            if (!dtPosts.Columns.Contains("Replies"))
            {
                dtPosts.Columns.Add("Replies", typeof(object));
            }

            foreach (DataRow row in dtPosts.Rows)
            {
                int postId = Convert.ToInt32(row["Id"]);
                row["Replies"] = GetReplies(postId);
            }

            rptPosts.DataSource = dtPosts;
            rptPosts.DataBind();

            // Show no posts message if empty
            pnlNoPosts.Visible = dtPosts.Rows.Count == 0;

            // Update statistics after operations
            LoadStatistics();
        }

        private List<object> GetReplies(int postId)
        {
            List<object> replies = new List<object>();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT R.Id as ReplyId, R.PostId, R.ReplyContent, R.ReplyDateTime, U.FullName
                                 FROM Reply R
                                 JOIN Users U ON R.UserId = U.Id
                                 WHERE R.PostId = @PostId
                                 ORDER BY R.ReplyDateTime ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PostId", postId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    replies.Add(new
                    {
                        ReplyId = reader["ReplyId"],
                        PostId = reader["PostId"],
                        FullName = reader["FullName"].ToString(),
                        ReplyContent = reader["ReplyContent"].ToString(),
                        ReplyDateTime = Convert.ToDateTime(reader["ReplyDateTime"])
                    });
                }
            }

            return replies;
        }

        protected void rptPosts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeletePost")
            {
                int postId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Step 1: Delete all replies linked to this post
                    string deleteReplies = "DELETE FROM Reply WHERE PostId = @PostId";
                    using (SqlCommand cmdReplies = new SqlCommand(deleteReplies, conn))
                    {
                        cmdReplies.Parameters.AddWithValue("@PostId", postId);
                        cmdReplies.ExecuteNonQuery();
                    }

                    // Step 2: Delete the post itself
                    string deletePost = "DELETE FROM CommunityPost WHERE Id = @PostId";
                    using (SqlCommand cmdPost = new SqlCommand(deletePost, conn))
                    {
                        cmdPost.Parameters.AddWithValue("@PostId", postId);
                        cmdPost.ExecuteNonQuery();
                    }
                }

                // Reload data without showing success message
                LoadPosts();
            }
        }

        protected void rptReplies_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteReply")
            {
                int replyId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string deleteReply = "DELETE FROM Reply WHERE Id = @ReplyId";
                    using (SqlCommand cmd = new SqlCommand(deleteReply, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReplyId", replyId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reload data without showing success message
                LoadPosts();
            }
        }

        protected void btnDeletePost_PreRender(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            btn.Attributes["data-uniqueid"] = btn.UniqueID;
        }

        protected void btnDeleteReply_PreRender(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            btn.Attributes["data-uniqueid"] = btn.UniqueID;
        }

        protected void rptPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // All posts are visible, delete buttons always shown
            }
        }

        protected void rptReplies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // All reply delete buttons are visible
            }
        }
    }
}