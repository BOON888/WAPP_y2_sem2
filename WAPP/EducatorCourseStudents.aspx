<%@ Page Title="Course Students" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="EducatorCourseStudents.aspx.cs"
    Inherits="WAPP.EducatorCourseStudents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f8f9fc;
        }

        .page-wrap {
            max-width: 1100px;
            margin: 40px auto;
            padding: 20px;
        }

        .title {
            font-size: 28px;
            font-weight: 700;
            margin-bottom: 10px;
        }

        .tab-buttons {
            display: flex;
            justify-content: center;
            gap: 20px;
            margin: 20px 0;
        }

        .tab-button {
            background-color: #001eff;
            color: white;
            border: none;
            padding: 10px 24px;
            border-radius: 6px;
            cursor: pointer;
            font-weight: 600;
            transition: background 0.3s;
        }

        .tab-button.active {
            background-color: #111827;
        }

        .info-message {
            text-align: center;
            color: #6b7280;
            font-size: 14px;
            margin-bottom: 10px;
            font-style: italic;
        }

        /* Student list horizontal layout */
        .students-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
            margin-top: 15px;
        }

        .student-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            background: #fff;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            padding: 12px 16px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.05);
        }

        .student-info {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .profile-icon {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: #001eff;
            color: white;
            font-weight: 700;
            font-size: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .student-details {
            display: flex;
            flex-direction: column;
        }

        .student-name {
            font-weight: 700;
            font-size: 16px;
            color: #111827;
        }

        .student-email {
            font-size: 13px;
            color: #6b7280;
        }

        .status-label {
            padding: 5px 10px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 600;
        }

        .status-complete {
            background-color: #dcfce7;
            color: #065f46;
        }

        .status-incomplete {
            background-color: #fee2e2;
            color: #991b1b;
        }

        .no-data {
            text-align: center;
            color: #9ca3af;
            font-style: italic;
            margin-top: 15px;
        }
    </style>

    <div class="page-wrap">
        <div class="title">
            <asp:Label ID="lblCourseTitle" runat="server" Text="Course Title" />
        </div>

        <div class="tab-buttons">
            <asp:Button ID="btnIncomplete" runat="server" Text="Incomplete" CssClass="tab-button active" OnClick="btnIncomplete_Click" />
            <asp:Button ID="btnComplete" runat="server" Text="Complete" CssClass="tab-button" OnClick="btnComplete_Click" />
        </div>

        <asp:Label ID="lblStatusMessage" runat="server" CssClass="info-message" Text="Showing students with incomplete progress." />

        <div class="students-list">
            <asp:Repeater ID="rptStudents" runat="server">
                <ItemTemplate>
                    <div class="student-row">
                        <div class="student-info">
                            <div class="profile-icon"><%# Eval("FullName").ToString().Substring(0,1).ToUpper() %></div>
                            <div class="student-details">
                                <div class="student-name"><%# Eval("FullName") %></div>
                                <div class="student-email"><%# Eval("Email") %></div>
                            </div>
                        </div>
                        <span class="status-label <%# Eval("Status").ToString() == "Completed" ? "status-complete" : "status-incomplete" %>">
                            <%# Eval("Status") %>
                        </span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoData" runat="server" CssClass="no-data" Visible="false" />
        </div>
    </div>
</asp:Content>
