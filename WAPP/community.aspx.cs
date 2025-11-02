using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class community : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("sign_in.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPosts();
            }
        }

        protected void btnMyPosts_Click(object sender, EventArgs e)
        {
            Response.Redirect("my_post.aspx");
        }

        protected void btnAsk_Click(object sender, EventArgs e)
        {
            string content = txtQuestion.Text.Trim();

            if (string.IsNullOrEmpty(content))
            {
                lblMessage.Text = "Please enter your question.";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO CommunityPost (UserId, PostContent, PostDateTime) VALUES (@UserId, @PostContent, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", Session["UserId"]);
                cmd.Parameters.AddWithValue("@PostContent", content);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            txtQuestion.Text = "";
            LoadPosts();
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
        }


        private List<object> GetReplies(int postId)
        {
            List<object> replies = new List<object>();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT R.ReplyContent, R.ReplyDateTime, U.FullName
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
            int postId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "AddReply")
            {
                TextBox txtReply = (TextBox)e.Item.FindControl("txtReply");
                string replyText = txtReply.Text.Trim();

                if (replyText != "")
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = "INSERT INTO Reply (PostId, UserId, ReplyContent, ReplyDateTime) VALUES (@PostId, @UserId, @ReplyContent, GETDATE())";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@PostId", postId);
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"]);
                        cmd.Parameters.AddWithValue("@ReplyContent", replyText);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    LoadPosts();
                }
            }

            if (e.CommandName == "DeletePost")
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string deleteReplies = "DELETE FROM Reply WHERE PostId = @PostId";
                    using (SqlCommand cmdReplies = new SqlCommand(deleteReplies, conn))
                    {
                        cmdReplies.Parameters.AddWithValue("@PostId", postId);
                        cmdReplies.ExecuteNonQuery();
                    }
                    string deletePost = "DELETE FROM CommunityPost WHERE Id = @PostId AND UserId = @UserId";
                    using (SqlCommand cmdPost = new SqlCommand(deletePost, conn))
                    {
                        cmdPost.Parameters.AddWithValue("@PostId", postId);
                        cmdPost.Parameters.AddWithValue("@UserId", Session["UserId"]);
                        cmdPost.ExecuteNonQuery();
                    }
                }

                LoadPosts();
            }
        }
        protected void btnDelete_PreRender(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            btn.Attributes["data-uniqueid"] = btn.UniqueID;
        }

        protected void rptPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LinkButton btnDelete = (LinkButton)e.Item.FindControl("btnDelete");
                DataRowView drv = (DataRowView)e.Item.DataItem;
                int postUserId = Convert.ToInt32(drv["UserId"]);
                int currentUserId = Convert.ToInt32(Session["UserId"]);

                // Only owner sees delete
                if (postUserId == currentUserId)
                {
                    btnDelete.Visible = true;
                }
            }
        }
    }
}
