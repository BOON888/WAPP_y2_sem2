using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class PrivateCourse : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                if (Session["StudentID"] == null)
                {
                    LoadStudentID();
                }

                LoadCourses();
            }
        }

        private void LoadStudentID()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id FROM Student WHERE UserId = @uid", conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    Session["StudentID"] = Convert.ToInt32(result);
                    Session["StudentName"] = Session["FullName"];
                }
            }
        }

        private void LoadCourses(string keyword = "", string coinFilter = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        SELECT c.Id, c.Title, e.EducationQualification AS EducatorName,
                               ISNULL(c.Coin, 50) AS Coin
                        FROM Course c
                        JOIN Educator e ON c.EducatorId = e.Id
                        WHERE c.CourseType = 'Private'";

                    if (!string.IsNullOrEmpty(keyword))
                        query += " AND (c.Title LIKE @kw OR e.EducationQualification LIKE @kw)";

                    if (!string.IsNullOrEmpty(coinFilter))
                        query += " AND ISNULL(c.Coin, 50) <= @coinFilter";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(keyword))
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                    if (!string.IsNullOrEmpty(coinFilter))
                        cmd.Parameters.AddWithValue("@coinFilter", Convert.ToInt32(coinFilter));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptPrivateCourses.DataSource = dt;
                    rptPrivateCourses.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message.Replace("'", "") + "');</script>");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            string coinFilter = ddlCoinFilter.SelectedValue;
            LoadCourses(keyword, coinFilter);
        }

        protected void rptPrivateCourses_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SubscribeCourse")
            {
                int studentId = Convert.ToInt32(Session["StudentID"]);
                int courseId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    
                    SqlCommand checkExist = new SqlCommand(
                        "SELECT COUNT(*) FROM StudentCourseProgress WHERE StudentId=@sid AND CourseId=@cid", conn);
                    checkExist.Parameters.AddWithValue("@sid", studentId);
                    checkExist.Parameters.AddWithValue("@cid", courseId);

                    int exists = Convert.ToInt32(checkExist.ExecuteScalar());
                    if (exists > 0)
                    {
                        Response.Write("<script>alert('You have already joined this course!');</script>");
                        return;
                    }

                    
                    SqlCommand getCoins = new SqlCommand("SELECT Coins FROM Student WHERE Id=@id", conn);
                    getCoins.Parameters.AddWithValue("@id", studentId);
                    int coins = Convert.ToInt32(getCoins.ExecuteScalar());

                    
                    SqlCommand getCost = new SqlCommand("SELECT ISNULL(Coin, 50) FROM Course WHERE Id=@cid", conn);
                    getCost.Parameters.AddWithValue("@cid", courseId);
                    int cost = Convert.ToInt32(getCost.ExecuteScalar());

                    
                    if (coins >= cost)
                    {
                        SqlCommand updateCoins = new SqlCommand("UPDATE Student SET Coins=Coins-@cost WHERE Id=@id", conn);
                        updateCoins.Parameters.AddWithValue("@cost", cost);
                        updateCoins.Parameters.AddWithValue("@id", studentId);
                        updateCoins.ExecuteNonQuery();

                        SqlCommand enroll = new SqlCommand(
                            "INSERT INTO StudentCourseProgress (StudentId, CourseId, Status) VALUES (@sid, @cid, 'In Progress')", conn);
                        enroll.Parameters.AddWithValue("@sid", studentId);
                        enroll.Parameters.AddWithValue("@cid", courseId);
                        enroll.ExecuteNonQuery();

                        Response.Write("<script>alert('Successfully subscribed to the course!');window.location='PrivateCourse.aspx';</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Not enough coins to subscribe.');</script>");
                    }
                }
            }
        }
    }
}
