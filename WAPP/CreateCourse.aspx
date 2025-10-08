<%@ Page Title="Create New Course" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs" Inherits="WAPP.CreateCourse" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p style="font-size: x-large" class="text-start">
        <strong>Create New Course</strong></p>
    <div class="sub">Build an engaging learning experience for your students<br />
        <br />
        Course Information<br />
        Basic details about your course<br />
        <br />
        Course Title<br />
        <asp:TextBox ID="TextBox1" runat="server" Width="1400px">Enter course title</asp:TextBox>
        <br />
        <br />
        Course Type<br />
        <asp:RadioButtonList ID="RadioButtonList1" runat="server" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged">
        </asp:RadioButtonList>
        <asp:RadioButtonList ID="RadioButtonList2" runat="server">
        </asp:RadioButtonList>
        <br />
    </div>
</asp:Content>
