<%@ Page Title="Course Students" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="EducatorCourseStudents.aspx.cs"
    Inherits="WAPP.EducatorCourseStudents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* 1. LAYOUT ADJUSTMENTS */
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f8f9fc;
        }

        .page-wrap {
            max-width: 1100px;
            /* 🚀 FIX: Reduce top margin to pull content closer to the header */
            margin: 20px auto 40px; 
            padding: 20px;
        }

        .title {
            font-size: 28px;
            font-weight: 700;
            margin-bottom: 10px;
            color: #111827; /* Ensure clear title color */
        }

        /* 2. TAB BUTTONS (Implementing Blue Hover Effect) */
        .tab-buttons {
            display: flex;
            justify-content: center;
            gap: 15px; /* Slightly reduced gap */
            margin: 20px 0;
        }

        .tab-button {
            background-color: #001eff;
            color: white;
            border: 1px solid #001eff; /* Add border for consistency */
            padding: 10px 24px;
            border-radius: 8px; /* Slightly larger radius */
            cursor: pointer;
            font-weight: 600;
            transition: all 0.25s ease; /* Smooth transitions */
        }

        .tab-button:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 10px rgba(0, 30, 255, 0.2);
            /* 🚀 FIX: Apply white-on-blue hover for consistency (Blue to Blue is acceptable, but this adds lift) */
            opacity: 0.95; 
        }

        .tab-button.active {
            /* 🚀 FIX: Remove black background, use a stronger blue or inverse style */
            background-color: white; /* Inverse style for active tab */
            color: #001eff;
            border-color: #001eff;
            box-shadow: 0 4px 10px rgba(0, 30, 255, 0.2);
        }
        
        .tab-button.active:hover {
            transform: none; /* Prevent active tab from moving */
            box-shadow: 0 4px 10px rgba(0, 30, 255, 0.2);
        }

        .info-message {
            text-align: center;
            color: #6b7280;
            font-size: 14px;
            margin-bottom: 10px;
            font-style: italic;
        }

        /* 3. STUDENT LIST STYLING (Applying Soft Shadow) */
        .students-list {
            display: flex;
            flex-direction: column;
            gap: 12px; /* Slightly increased gap for visual separation */
            margin-top: 15px;
        }

        .student-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            background: #fff;
            /* 🚀 FIX: Apply the soft glow shadow instead of just a border */
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.04); 
            border: none;
            border-radius: 10px;
            padding: 12px 16px;
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }
        
        .student-row:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 15px rgba(0, 0, 0, 0.08);
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

        /* 4. STATUS LABELS */
        .status-label {
            padding: 5px 10px;
            border-radius: 12px; /* Pill shape */
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .status-complete {
            background-color: #dcfce7;
            color: #047857; /* Darker green for readability */
        }

        .status-incomplete {
            background-color: #fee2e2;
            color: #b91c1c; /* Darker red for readability */
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
