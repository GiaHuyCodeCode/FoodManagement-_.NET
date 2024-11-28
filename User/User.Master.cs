using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FoodShop.User
{
    public partial class User : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.Url.AbsoluteUri.ToString().Contains("Default.aspx"))
            {
                form1.Attributes.Add("class", "sub_page");
            }
            else
            {
                // Load the control
                Control sliderUserControl = (Control)Page.LoadControl("SliderUserControl.ascx");

                // Add the control to the panel
                pnlSliderUC.Controls.Add(sliderUserControl);
            }

            if (Session["userId"] != null)
            {
                lbLoginOrLogout.Text = "Logout";
            }
            else lbLoginOrLogout.Text = "Login";
		}

        protected System.Void lbLoginOrLogout_Click()
        {
            if (Session["userId"]==null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                Session.Abandon();
                Response.Redirect("Login.aspx");
            }
        }
    }
}