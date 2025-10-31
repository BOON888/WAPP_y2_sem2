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
            width: 99%;
            max-width: 1900px;
            margin: 0px auto 50px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }

        h1 {
            color: #111827;
            font-size: 26px;
            font-weight: 700;
            margin-bottom: 20px;
        }

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
            border-radius: 6px;
            min-width: 260px;
            font-size: 14px;
            background-color: #f9fafb;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }

        .filterBox {
            padding: 10px 12px;
            border-radius: 6px;
            background-color: #f9fafb;
            font-size: 14px;
            min-width: 160px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }

        

        .btn:hover {
            background-color: white;
            color: #001eff;
        }

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
            align-items: center; 
            text-align: center;
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

        html { scrollbar-width: none; }
        html::-webkit-scrollbar { display: none; }
    </style>

    <div class="page-container">
        <h1>Private Courses</h1>

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

        <div class="course-grid">
            <asp:Repeater ID="rptPrivateCourses" runat="server" OnItemCommand="rptPrivateCourses_ItemCommand">
                <ItemTemplate>
                    <div class="course-card">
                        <div>
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
                            <p><strong><%# Eval("Coin") %> Coins Required</strong></p>
                        </div>
                        <asp:Button runat="server" Text="Subscribe With Coins" CssClass="btn"
                            CommandName="SubscribeCourse" CommandArgument='<%# Eval("Id") %>' />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
