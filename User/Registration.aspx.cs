using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FoodShop.User
{
    public partial class Registration : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;
            int userId = Convert.ToInt32(Request.QueryString["id"]);
            con= new SqlConnection(Connection.GetConnectionString());
            cmd=new SqlCommand("User_Crud", con);
            cmd.Parameters.AddWithValue("@Action", userId==0? "INSERT": "UPDATE");
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
            cmd.Parameters.AddWithValue("@Mobile", txtPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@Postcode", txtPostcode.Text.Trim());
            cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());

            if (fuUserImage.HasFile)
            {
                if (Utils.IsValidExtension(fuUserImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExtension = Path.GetExtension(fuUserImage.FileName);
                    imagePath = "Images/User/" + obj.ToString() + fileExtension;
                    fuUserImage.PostedFile.SaveAs(Server.MapPath("~/Images/User/") + obj.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue("@ImageUrl",imagePath);
                    isValidToExecute = true;
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Please select .jpg, .jpeg, .png file only.";
                    lblMsg.CssClass = "alert alert-danger";
                    isValidToExecute = false;
                }
            }
            else
            {
                isValidToExecute = true;
            }

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    actionName = userId == 0 ?
                        "registered successfully! <b><a href='Login.aspx'>Click here</a></b> to log in":
                        "details updated successfully! <b><a href='Profile.aspx'>Click here</a></b> to check for profiles";
                    lblMsg.Visible = true;
                    lblMsg.Text = "<b>"+txtName.Text.Trim()+"</b> " + actionName + " successfully.";
                    lblMsg.CssClass = "alert alert-success";
                    if(userId!=0)
                    {
                        Response.AddHeader("REFRESH", "2;URL=Profile.aspx");
                    }
                    clear();
                }

                catch (Exception ex)
                {
                    if(ex.Message.Contains("Violation of PRIMARY KEY"))
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Username already exists.";
                        lblMsg.CssClass = "alert alert-danger";
                    }
                    else
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Error-" + ex.Message;
                        lblMsg.CssClass = "alert alert-danger";
                    }
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void clear()
        {
            txtName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPostcode.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }
    }
}