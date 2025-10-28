using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WAPP
{
    public partial class Feedback : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SeaLearnerConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Ensure valid student session
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString().ToLower() != "student")
                {
                    Response.Redirect("sign_in.aspx");
                    return;
                }

                //Ensure StudentID exists
                if (Session["StudentID"] == null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT Id FROM Student WHERE UserId = @uid", conn);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            Session["StudentID"] = Convert.ToInt32(result);
                        else
                        {
                            Response.Redirect("sign_in.aspx");
                            return;
                        }
                    }
                }

                LoadFeedbackHistory();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCategory.SelectedValue) ||
                string.IsNullOrEmpty(ddlPriority.SelectedValue) ||
                string.IsNullOrEmpty(txtSubject.Text.Trim()) ||
                string.IsNullOrEmpty(txtDescription.Text.Trim()))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please fill in all fields before submitting.";
                return;
            }

            int studentId = Convert.ToInt32(Session["StudentID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Feedback (StudentId, Category, Priority, Subject, Description, Status, CreatedAt)
                    VALUES (@sid, @cat, @pri, @sub, @desc, 'Pending', GETDATE())", conn);

                cmd.Parameters.AddWithValue("@sid", studentId);
                cmd.Parameters.AddWithValue("@cat", ddlCategory.SelectedValue);
                cmd.Parameters.AddWithValue("@pri", ddlPriority.SelectedValue);
                cmd.Parameters.AddWithValue("@sub", txtSubject.Text.Trim());
                cmd.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());
                cmd.ExecuteNonQuery();
            }

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Feedback submitted successfully!";
            txtSubject.Text = "";
            txtDescription.Text = "";
            ddlCategory.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 0;

            LoadFeedbackHistory();
        }

        private void LoadFeedbackHistory()
        {
            int studentId = Convert.ToInt32(Session["StudentID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT Category, Priority, Subject, Description, Status, CreatedAt
                    FROM Feedback
                    WHERE StudentId=@id
                    ORDER BY CreatedAt DESC", conn);
                da.SelectCommand.Parameters.AddWithValue("@id", studentId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptFeedbackHistory.DataSource = dt;
                rptFeedbackHistory.DataBind();
            }
        }
    }
}
