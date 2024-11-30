using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace FoodShop.User
{
	public partial class Profile : System.Web.UI.Page
	{

		SqlConnection con;
		SqlCommand cmd;
		SqlDataAdapter sda;
		DataTable dt;

		protected void Page_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
			{
				if (Session["userId"]==null)
				{
					Response.Redirect("Default.aspx");
				}
				else
				{
					getUserDetails();
				}
			}
		}


		void getUserDetails() {
			con = new SqlConnection(Connection.GetConnectionString());
			cmd = new SqlCommand("User_Crud", con);
			cmd.Parameters.AddWithValue("@Action", "SELECT4PROFILE");
			cmd.Parameters.AddWithValue("@UserId", Session["userId"]);
			cmd.CommandType=CommandType.StoredProcedure;
			sda = new SqlDataAdapter(cmd);
			dt=new DataTable();
			sda.Fill(dt);
			rUserProfile.DataSource = dt;
			rUserProfile.DataBind();
			if(dt.Rows.Count == 1)
			{
				Session["name"]=dt.Rows[0]["name"].ToString();
				Session["email"] = dt.Rows[0]["Email"].ToString();
				Session["imageUrl"] = dt.Rows[0]["ImageUrl"].ToString();
				Session["createdDate"] = dt.Rows[0]["CreatedDate"].ToString();
			}
		}
	}
}