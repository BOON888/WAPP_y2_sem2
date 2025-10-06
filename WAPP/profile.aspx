<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="WAPP.profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align:center; margin-top:50px;">
        <h2>Welcome to Your Profile</h2>
        <div style="margin-top:30px;">
            <asp:Image ID="imgProfile" runat="server" 
                       Width="150px" Height="150px"
                       Style="border-radius:50%; object-fit:cover; border:3px solid #001eff;" />
            <h3 style="margin-top:20px;">
                <asp:Label ID="lblFullName" runat="server" Text=""></asp:Label>
            </h3>
        </div>
    </div>
</asp:Content>
