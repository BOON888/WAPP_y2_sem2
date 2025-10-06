using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class PublicCourse : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string keyword = Request.QueryString["keyword"];
                if (!string.IsNullOrEmpty(keyword))
                {
                    txtSearch.Text = keyword;
                    LoadCourses(keyword);
                }
                else
                {
                    LoadCourses();
                }
            }
        }

        private void LoadCourses(string keyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT c.Id, c.Title, e.EducationQualification AS EducatorName
                    FROM Course c 
                    JOIN Educator e ON c.EducatorId = e.Id
                    WHERE c.CourseType='Public'";

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " AND (c.Title LIKE @kw OR e.EducationQualification LIKE @kw)";
                }

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(keyword))
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptPublicCourses.DataSource = dt;
                rptPublicCourses.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            LoadCourses(keyword);
        }
    }
}
