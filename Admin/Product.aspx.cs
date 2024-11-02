using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace FoodShop.Admin
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Product";

            }
            lblMsg.Visible = false;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;
            int productId = Convert.ToInt32(hdnId.Value);
            con = new SqlConnection(Connection.GetConnectionString());
            cmd = new SqlCommand("Product_Crud", con);
            cmd.Parameters.AddWithValue("@Action", productId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text.Trim());
            cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());
            cmd.Parameters.AddWithValue("@CategoryId", ddlCategory.Text.Trim());
            cmd.Parameters.AddWithValue("@IsActive", cbIsActive.Checked);
            if (fuProductImage.HasFile)
            {
                if (Utils.IsValidExpression(fuProductImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExtension = Path.GetExtension(fuProductImage.FileName);
                    imagePath = "Images/Product/" + obj.ToString() + fileExtension;
                    fuProductImage.PostedFile.SaveAs(Server.MapPath("~/Images/Product/") + obj.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue("@ImageUrl", imagePath);
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
                    actionName = productId == 0 ? "inserted" : "updated";
                    lblMsg.Visible = true;
                    lblMsg.Text = "Product " + actionName + " successfully.";
                    lblMsg.CssClass = "alert alert-success";
                    getProducts();
                    clear();
                }
                catch (Exception ex)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "ERROR : " + ex.Message.ToString();
                    lblMsg.CssClass = "alert alert-danger";
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void getProducts()
        {
            con = new SqlConnection(Connection.ConnectionString());
            cmd = new SqlCommand("Product_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.CommandType= CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            rProduct.DataSource = dt;
            rProduct.DataBind();
        }

        private void clear()
        {
            txtName.Text = txtDescription.Text = txtQuantity.Text = txtPrice.Text = imgProduct.ImageUrl = string.Empty;
            ddlCategory.ClearSelection();
            cbIsActive.Checked = false;
            hdnId.Value = "0";
            btnAddOrUpdate.Text = "Add";
            lblMsg.Visible = false;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {

        }

        protected void rProduct_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void rProduct_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }
    }
}