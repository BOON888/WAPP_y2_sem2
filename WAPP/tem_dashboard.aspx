<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="tem_dashboard.aspx.cs" Inherits="WAPP.tem_dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 class="fw-bold mb-3">Team Dashboard</h2>

        <!-- Button to navigate to Community Page -->
        <asp:Button 
            ID="btnCommunity" 
            runat="server" 
            Text="Go to Community Page" 
            CssClass="btn btn-primary" 
            OnClick="btnCommunity_Click" />
    </div>
</asp:Content>
