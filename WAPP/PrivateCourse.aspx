<%@ Page Title="Private Courses" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="PrivateCourse.aspx.cs"
    Inherits="WAPP.PrivateCourse" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
        }

        .page-container {
            background-color: white;
            border-radius: 12px;
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 90%;
            max-width: 1100px;
            margin: 60px auto;
        }

        h1 {
            color: #111827;
            font-size: 26px;
            font-weight: 700;
            margin-bottom: 20px;
        }

        /* 🔍 Search and Filter Row */
        .search-bar {
            display: flex;
            gap: 10px;
            align-items: center;
            justify-content: flex-start;
            margin-bottom: 30px;
            flex-wrap: wrap;
        }

        .searchBox {
            padding: 10px 12px;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            min-width: 260px;
            font-size: 14px;
            background-color: #f9fafb;
        }

        .filterBox {
            padding: 10px 12px;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            background-color: #f9fafb;
            font-size: 14px;
            min-width: 160px;
        }

        .btn {
            background-color: #001eff;
            color: white;
            border: 1px solid #001eff;
            border-radius: 6px;
            padding: 10px 20px;
            font-size: 15px;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .btn:hover {
            background-color: white;
            color: #001eff;
        }

        /* Course grid */
        .course-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 20px;
        }

        .course-card {
            background-color: #ffffff;
            border: 1px solid #d1d5db;
            border-radius: 10px;
            box-shadow: 0 1px 5px rgba(0,0,0,0.05);
            padding: 20px;
            min-height: 250px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .course-card h4 {
            color: #111827;
            font-size: 18px;
            margin-bottom: 8px;
        }

        .course-card p {
            color: #6b7280;
            font-size: 14px;
            margin-bottom: 10px;
        }
    </style>

    <div class="page-container">
        <h1>Private Courses</h1>

        <!-- 🔍 Search and Coin Filter -->
        <div class="search-bar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="searchBox" Placeholder="Search by title or educator..." />

            <asp:DropDownList ID="ddlCoinFilter" runat="server" CssClass="filterBox">
                <asp:ListItem Text="All Coins" Value="" />
                <asp:ListItem Text="≤ 50 Coins" Value="50" />
                <asp:ListItem Text="≤ 100 Coins" Value="100" />
                <asp:ListItem Text="≤ 200 Coins" Value="200" />
            </asp:DropDownList>

            <asp:Button ID="btnSearch" runat="server" Text="Filter" CssClass="btn" OnClick="btnSearch_Click" />
        </div>

        <!-- Course Listing -->
        <div class="course-grid">
            <asp:Repeater ID="rptPrivateCourses" runat="server">
                <ItemTemplate>
                    <div class="course-card">
                        <div>
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
                            <p><strong><%# Eval("Coin") %> Coins Required</strong></p>
                        </div>
                        <button class="btn" type="button"
                            onclick="window.location='CourseDetails.aspx?id=<%# Eval("Id") %>'">
                            View Details
                        </button>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
