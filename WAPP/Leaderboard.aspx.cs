using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WAPP
{
    public partial class Leaderboard : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["StudentID"] == null)
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                LoadLeaderboard();
            }
        }

        private void LoadLeaderboard()
        {
            int currentId = Convert.ToInt32(Session["StudentID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
                    SELECT s.Id AS StudentId, u.FullName, s.BadgesEarned,
                           (SELECT COUNT(*) FROM StudentCourseProgress p 
                            WHERE p.StudentId = s.Id AND p.Status='Completed') AS CoursesCompleted,
                           s.School, s.InterestSubject
                    FROM Student s
                    JOIN Users u ON s.UserId = u.Id
                    ORDER BY s.BadgesEarned DESC, CoursesCompleted DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptLeaderboard.DataSource = dt;
                rptLeaderboard.DataBind();

                // 🔹 Top Stats Section
                lblTotalStudents.Text = dt.Rows.Count.ToString();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dt.Rows[i]["StudentId"]) == currentId)
                    {
                        lblRank.Text = (i + 1).ToString();
                        lblBadges.Text = dt.Rows[i]["BadgesEarned"].ToString();
                        lblCourses.Text = dt.Rows[i]["CoursesCompleted"].ToString();
                        break;
                    }
                }
            }
        }

        protected void rptLeaderboard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                int rank = e.Item.ItemIndex + 1;
                int currentId = Convert.ToInt32(Session["StudentID"]);
                bool isCurrentUser = Convert.ToInt32(drv["StudentId"]) == currentId;

                Literal litYou = (Literal)e.Item.FindControl("litYou");
                Literal litBadgeLabel = (Literal)e.Item.FindControl("litBadgeLabel");
                Literal litSchool = (Literal)e.Item.FindControl("litSchool");
                Literal litSubject = (Literal)e.Item.FindControl("litSubject");
                Panel pnlRow = (Panel)e.Item.FindControl("pnlRow");

                // Highlight current user
                if (isCurrentUser)
                {
                    pnlRow.CssClass += " highlighted";
                    litYou.Text = " <span style='color:#001eff;'>(You)</span>";
                }

                // Top 3 badge tags
                if (rank == 1)
                    litBadgeLabel.Text = " <span class='badge-label champion'>🥇 Champion</span>";
                else if (rank == 2)
                    litBadgeLabel.Text = " <span class='badge-label runner'>🥈 Runner-up</span>";
                else if (rank == 3)
                    litBadgeLabel.Text = " <span class='badge-label third'>🥉 Third Place</span>";

                // Student details
                litSchool.Text = string.IsNullOrEmpty(drv["School"].ToString()) ? "—" : drv["School"].ToString();
                litSubject.Text = string.IsNullOrEmpty(drv["InterestSubject"].ToString()) ? "—" : drv["InterestSubject"].ToString();
            }
        }

        // Avatar color gradient per rank
        public string GetAvatarColor(int rank)
        {
            switch (rank)
            {
                case 1: return "background:linear-gradient(135deg,#facc15,#fbbf24);";
                case 2: return "background:linear-gradient(135deg,#d1d5db,#9ca3af);";
                case 3: return "background:linear-gradient(135deg,#fcd34d,#fb923c);";
                default: return "background:linear-gradient(135deg,#001eff,#4f46e5);";
            }
        }
    }
}
