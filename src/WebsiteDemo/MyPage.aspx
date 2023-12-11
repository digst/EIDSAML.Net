<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyPage.aspx.cs" Inherits="WebsiteDemo.WebForm1" MasterPageFile="~/sp.Master" %>

<%@ Import Namespace="eid.saml20.identity" %>
<%@ Import Namespace="eid.saml20.config" %>
<%@ Import Namespace="eid.saml20.Schema.Core" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
    <style type="text/css">
        table {
            border-width: 1px;
            border-spacing: 2px;
            border-style: solid;
            border-color: black;
            border-collapse: collapse;
            background-color: white;
        }

            table th {
                border-width: 1px;
                padding: 3px;
                border-style: dotted;
                border-color: gray;
            }

            table td {
                border-width: 1px;
                padding: 3px;
                border-style: dotted;
                border-color: gray;
                background-color: white;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

    <% if (Saml20Identity.IsInitialized())
        { %>
    <div>
        Welcome, <%= Saml20Identity.Current.Name %><br />
        <h1>SAML attributes</h1>
        <table id="userAttributes" style="border: solid 1px;">
            <thead>
                <tr>
                    <th>Attribute name
                    </th>
                    <th>Attribute value
                    </th>
                </tr>
            </thead>
            <% foreach (SamlAttribute att in Saml20Identity.Current)
                {
                    foreach (string attVal in att.AttributeValue)
                    {
            %>
            <tr>
                <td style="vertical-align: top">
                    <%= att.Name %>
                </td>
                <td style="word-break: break-word;">
                    <%= att.AttributeValue.Length > 0 ? Server.HtmlEncode(attVal) : string.Empty %>
                </td>
            </tr>
            <% }
                } %>
        </table>
    </div>
    <% } %>

    <div>
        <asp:Button Style="margin-top: 10px;" ID="btnLogoff" runat="server" Enabled="true" Text="Logoff" OnClick="Btn_Logoff_Click" />
    </div>
    <br />
    <div>
        Relogin with IdP: 
    <asp:Button ID="Btn_Relogin" runat="server" Enabled="true" Text="Re-Login" OnClick="Btn_Relogin_Click" />

    </div>
</asp:Content>
