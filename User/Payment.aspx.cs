using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FoodShop.User
{
    public partial class Payment : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader dr, drl;
        DataTable dt;
        SqlTransaction trans = null;
        string _name = string.Empty; string _cardNo = string.Empty; string _expDate = string.Empty; string _cvv = string.Empty;
        string _address = string.Empty; string _paymentMode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void lbCardSubmit_Click(object sender, EventArgs e)
        {
            _name = txtName.Text.Trim();
            _cardNo = txtCardNo.Text.Trim();
            _cardNo = string.Format("************{0}", txtCardNo.Text.Trim().Substring(12, 4));
            _expDate = txtExpMonth.Text.Trim() + "/" + txtExpYear.Text.Trim();
            _cvv = txtCvv.Text.Trim();
            _address = txtAddress.Text.Trim();
            _paymentMode = "Card";
            if (Session["userId"] != null)
            {
                OrderPayment(_name, _cardNo, _expDate, _cvv, _address, _paymentMode);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void lbCodSubmit_Click(object sender, EventArgs e)
        {
            _address = txtCODAddress.Text.Trim();
            _paymentMode = "COD";
            if (Session["userId"] != null)
            {
                OrderPayment(_address,_paymentMode);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        void OrderPayment(string name, string cardNo, string expDate, string cvv, string address, string paymentMode)
        {
            int paymentId; int productId; int quantity;
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[7]
            {
        new DataColumn("OrderNo", typeof(string)),
        new DataColumn("ProductId", typeof(int)),
        new DataColumn("Quantity", typeof(int)),
        new DataColumn("UserId", typeof(int)),
        new DataColumn("Status", typeof (string)),
        new DataColumn("PaymentId", typeof(int)),
        new DataColumn("OrderDate", typeof(DateTime))
            });
            con = new SqlConnection(Connection.GetConnectionString());
            con.Open();
            #region Sql Transaction
            trans = con.BeginTransaction();
            try
            {
                // Save Payment
                cmd = new SqlCommand("Save_Payment", con, trans);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                cmd.Parameters.AddWithValue("@ExpDate", expDate);
                cmd.Parameters.AddWithValue("@Cvv", cvv);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);
                cmd.Parameters.Add("@InsertedId", SqlDbType.Int);
                cmd.Parameters["@InsertedId"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                paymentId = Convert.ToInt32(cmd.Parameters["@InsertedId"].Value);

                // Debugging statement
                Response.Write("<script>alert('Payment saved successfully with ID: " + paymentId + "');</script>");

                // Get Cart Items
                cmd = new SqlCommand("Cart_Crud", con, trans);
                cmd.Parameters.AddWithValue("@Action", "SELECT");
                cmd.Parameters.AddWithValue("@UserId", Session["userId"]);
                cmd.CommandType = CommandType.StoredProcedure;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    productId = Convert.ToInt32(dr["ProductId"].ToString());
                    quantity = Convert.ToInt32(dr["Quantity"].ToString());

                    UpdateQuantity(productId, quantity, trans, con);
                    DeleteCartItem(productId, trans, con);

                    dt.Rows.Add(Utils.GetUniqueId(), productId, quantity,
                        Convert.ToInt32(Session["userId"]), "Pending", paymentId, Convert.ToDateTime(DateTime.Now));
                }
                dr.Close();

                // Debugging statement
                Response.Write("<script>alert('Cart items fetched successfully');</script>");

                // Save Orders
                if (dt.Rows.Count > 0)
                {
                    cmd = new SqlCommand("Save_Orders", con, trans);
                    cmd.Parameters.AddWithValue("@tblOrders", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    // Debugging statement
                    Response.Write("<script>alert('Orders saved successfully');</script>");
                }

                trans.Commit();
                lblMsg.Visible = true;
                lblMsg.Text = "Order Placed Successfully";
                lblMsg.CssClass = "alert alert-success";
                Response.AddHeader("REFRESH", "1;URL=Invoice.aspx?id=" + paymentId);
            }
            catch (Exception e)
            {
                try
                {
                    trans.Rollback();
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Transaction rollback failed: " + ex.Message + "');</script>");
                }
                Response.Write("<script>alert('Error: " + e.Message + "');</script>");
            }
            #endregion Sql Transaction
            finally
            {
                con.Close();
            }
        }

        // Overloaded method for COD
        void OrderPayment(string address, string paymentMode)
        {
            int paymentId; int productId; int quantity;
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[7]
            {
        new DataColumn("OrderNo", typeof(string)),
        new DataColumn("ProductId", typeof(int)),
        new DataColumn("Quantity", typeof(int)),
        new DataColumn("UserId", typeof(int)),
        new DataColumn("Status", typeof (string)),
        new DataColumn("PaymentId", typeof(int)),
        new DataColumn("OrderDate", typeof(DateTime))
            });
            con = new SqlConnection(Connection.GetConnectionString());
            con.Open();
            #region Sql Transaction
            trans = con.BeginTransaction();
            try
            {
                // Save Payment
                cmd = new SqlCommand("Save_Payment", con, trans);
                cmd.Parameters.AddWithValue("@Name", DBNull.Value);
                cmd.Parameters.AddWithValue("@CardNo", DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@Cvv", DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);
                cmd.Parameters.Add("@InsertedId", SqlDbType.Int);
                cmd.Parameters["@InsertedId"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                paymentId = Convert.ToInt32(cmd.Parameters["@InsertedId"].Value);

                // Debugging statement
                Response.Write("<script>alert('Payment saved successfully with ID: " + paymentId + "');</script>");

                // Get Cart Items
                cmd = new SqlCommand("Cart_Crud", con, trans);
                cmd.Parameters.AddWithValue("@Action", "SELECT");
                cmd.Parameters.AddWithValue("@UserId", Session["userId"]);
                cmd.CommandType = CommandType.StoredProcedure;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    productId = Convert.ToInt32(dr["ProductId"].ToString());
                    quantity = Convert.ToInt32(dr["Quantity"].ToString());

                    UpdateQuantity(productId, quantity, trans, con);
                    DeleteCartItem(productId, trans, con);

                    dt.Rows.Add(Utils.GetUniqueId(), productId, quantity,
                        Convert.ToInt32(Session["userId"]), "Pending", paymentId, Convert.ToDateTime(DateTime.Now));
                }
                dr.Close();

                // Debugging statement
                Response.Write("<script>alert('Cart items fetched successfully');</script>");

                // Save Orders
                if (dt.Rows.Count > 0)
                {
                    cmd = new SqlCommand("Save_Orders", con, trans);
                    cmd.Parameters.AddWithValue("@tblOrders", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    // Debugging statement
                    Response.Write("<script>alert('Orders saved successfully');</script>");
                }

                trans.Commit();
                lblMsg.Visible = true;
                lblMsg.Text = "Order Placed Successfully";
                lblMsg.CssClass = "alert alert-success";
                Response.AddHeader("REFRESH", "1;URL=Invoice.aspx?id=" + paymentId);
            }
            catch (Exception e)
            {
                try
                {
                    trans.Rollback();
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Transaction rollback failed: " + ex.Message + "');</script>");
                }
                Response.Write("<script>alert('Error: " + e.Message + "');</script>");
            }
            #endregion Sql Transaction
            finally
            {
                con.Close();
            }
        }


        void UpdateQuantity(int _product, int _quantity, SqlTransaction sqlTrans, SqlConnection sqlCon)
        {
            int dbQuantity;
            cmd = new SqlCommand("Product_Crud", sqlCon, sqlTrans);
            cmd.Parameters.AddWithValue("@Action", "GETBYID");
            cmd.Parameters.AddWithValue("@ProductId", _product);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                drl = cmd.ExecuteReader();
                while(drl.Read())
                {
                    dbQuantity = Convert.ToInt32(drl["Quantity"].ToString());
                    if(dbQuantity>_quantity && dbQuantity>2)
                    {
                        dbQuantity = dbQuantity - _quantity;
                        cmd = new SqlCommand("Product_Crud", sqlCon);
                        cmd.Parameters.AddWithValue("@Action", "QTYUPDATE");
                        cmd.Parameters.AddWithValue("@Quantity", dbQuantity);
                        cmd.Parameters.AddWithValue("@ProductId", _product);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
                drl.Close();
            }
            catch(Exception e)
            {
                Response.Write("<script>alert('" + e.Message + "')</script>");
            }
            finally
            {

            }
        }

        void DeleteCartItem(int _product, SqlTransaction sqlTrans, SqlConnection sqlCon)
        {
            cmd = new SqlCommand("Cart_Crud", sqlCon);
            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@ProductId", _product);
            cmd.Parameters.AddWithValue("@UserId", Session["userId"]);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Response.Write("<script>alert('" + e.Message + "')</script>");
            }
            finally
            {

            }
        }
    }
}