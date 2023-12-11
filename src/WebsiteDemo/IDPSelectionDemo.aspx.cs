﻿using System;
using eid.saml20.config;

namespace WebsiteDemo
{
    public partial class IDPSelectionDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litIDPList.Text = "<ul>";
            EidSAML20FederationConfig.GetConfig().Endpoints.IDPEndPoints.ForEach(idp =>
                litIDPList.Text += "<li><a href=\"" + idp.GetIDPLoginUrl(false, false, null, null) + "\">" + (string.IsNullOrEmpty(idp.Name) ? idp.Id : idp.Name) + "</a></li>");
            litIDPList.Text += "</ul>";
        }
    }
}
