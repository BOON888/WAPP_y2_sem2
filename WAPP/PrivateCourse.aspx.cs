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
                LoadCourses();
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
    }
}
