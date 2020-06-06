using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace CrudINGridView
{
    public partial class Default : System.Web.UI.Page
    {
        string connectionString = 
            @"Server = .\SQLEXPRESS; " +
            "Database = PhoneBookDB; " +
            "Trusted_Connection = true; " +
            "MultipleActiveResultSets = true";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateGridView();
        }

        void PopulateGridView()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            DataTable dt = new DataTable();

            string query = "select * from phonebook";

            using (conn)
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            if (dt.Rows.Count > 0)
            {
                gvPhoneBook.DataSource = dt;
                gvPhoneBook.DataBind();
            } else
            {
                dt.Rows.Add(dt.NewRow());

                gvPhoneBook.DataSource = dt;
                gvPhoneBook.DataBind();

                gvPhoneBook.Rows[0].Cells.Clear();
                gvPhoneBook.Rows[0].Cells.Add(new TableCell());
                
                gvPhoneBook.Rows[0].Cells[0].ColumnSpan = dt.Columns.Count;
                gvPhoneBook.Rows[0].Cells[0].Text = "No data found ...";
                gvPhoneBook.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
            
        }

        protected void gvPhoneBook_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string query = 
                "insert into phonebook (FirstName, LastName, Contact, Email) " +
                "values " +
                "(@FirstName, @LastName, @Contact, @Email)";

            try
            {
                if (e.CommandName.Equals("AddNew"))
                {
                    SqlConnection conn = new SqlConnection(connectionString);

                    using (conn)
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@FirstName",
                            (gvPhoneBook.FooterRow.FindControl("txtFirstNameFooter") as TextBox).Text.Trim());

                        cmd.Parameters.AddWithValue("@LastName",
                            (gvPhoneBook.FooterRow.FindControl("txtLastNameFooter") as TextBox).Text.Trim());

                        cmd.Parameters.AddWithValue("@Contact",
                            (gvPhoneBook.FooterRow.FindControl("txtContactFooter") as TextBox).Text.Trim());

                        cmd.Parameters.AddWithValue("@Email",
                            (gvPhoneBook.FooterRow.FindControl("txtEmailFooter") as TextBox).Text.Trim());

                        cmd.ExecuteNonQuery();

                        PopulateGridView();
                        lbldonemsg.Text = "New record has been added";
                        lblwrongmsg.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lbldonemsg.Text = "";
                lblwrongmsg.Text = ex.Message;
            }
        }

        protected void gvPhoneBook_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvPhoneBook.EditIndex = e.NewEditIndex;
            PopulateGridView();
        }

        protected void gvPhoneBook_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPhoneBook.EditIndex = -1;
            PopulateGridView();
        }

        protected void gvPhoneBook_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string query =
                "update phonebook set " +
                "FirstName = @FirstName, " +
                "LastName = @LastName, " +
                "Contact =  @Contact, " +
                "Email = @Email " +
                "where Id = @Id ";

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);

                using (conn)
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@FirstName",
                        (gvPhoneBook.Rows[e.RowIndex].FindControl("txtFirstName") as TextBox).Text.Trim());

                    cmd.Parameters.AddWithValue("@LastName",
                        (gvPhoneBook.Rows[e.RowIndex].FindControl("txtLastName") as TextBox).Text.Trim());

                    cmd.Parameters.AddWithValue("@Contact",
                        (gvPhoneBook.Rows[e.RowIndex].FindControl("txtContact") as TextBox).Text.Trim());

                    cmd.Parameters.AddWithValue("@Email",
                        (gvPhoneBook.Rows[e.RowIndex].FindControl("txtEmail") as TextBox).Text.Trim());

                    cmd.Parameters.AddWithValue("@Id", 
                        Convert.ToInt32(gvPhoneBook.DataKeys[e.RowIndex].Value.ToString()));

                    cmd.ExecuteNonQuery();

                    gvPhoneBook.EditIndex = -1;
                    PopulateGridView();

                    lbldonemsg.Text = "Record has been updated";
                    lblwrongmsg.Text = "";
                }
            }
            catch (Exception ex)
            {
                lbldonemsg.Text = "";
                lblwrongmsg.Text = ex.Message;
            }
        }

        protected void gvPhoneBook_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string query =
                "delete from phonebook where id = @id ";

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);

                using (conn)
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Id",
                        Convert.ToInt32(gvPhoneBook.DataKeys[e.RowIndex].Value.ToString()));

                    cmd.ExecuteNonQuery();

                    PopulateGridView();

                    lbldonemsg.Text = "Record has been deleted";
                    lblwrongmsg.Text = "";
                }
            }
            catch (Exception ex)
            {
                lbldonemsg.Text = "";
                lblwrongmsg.Text = ex.Message;
            }
        }
    }
}