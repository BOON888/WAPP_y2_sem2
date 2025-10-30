<%@ Page Title="Public Courses" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="PublicCourse.aspx.cs"
    Inherits="WAPP.PublicCourse" %>

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
            margin-bottom: 30px;
            align-items: center;
            justify-content: flex-start;
        }

        .searchBox {
            padding: 10px 12px;
            border-radius: 6px;
            min-width: 300px;
            font-size: 14px;
            background-color: #f9fafb;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
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
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            align-self: center;
            margin-top: 10px;

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

        

        html { scrollbar-width: none; } 
        html::-webkit-scrollbar { display: none; }
        .auto-style1 {
            --bs-btn-padding-x: 0.75rem;
            --bs-btn-padding-y: 0.375rem;
            --bs-btn-font-family;
            --bs-btn-font-size: 1rem;
            --bs-btn-font-weight: 400;
            --bs-btn-line-height: 1.5;
            --bs-btn-color: #212529;
            --bs-btn-bg: transparent;
            --bs-btn-border-width: 1px;
            --bs-btn-border-color: transparent;
            --bs-btn-border-radius: 0.375rem;
            --bs-btn-hover-border-color: transparent;
            --bs-btn-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.15), 0 1px 1px rgba(0, 0, 0, 0.075);
            --bs-btn-disabled-opacity: 0.65;
            --bs-btn-focus-box-shadow: 0 0 0 0.25rem rgba(var(--bs-btn-focus-shadow-rgb), .5);
            display: inline-block;
            font-family: var(--bs-btn-font-family);
            font-size: 15px;
            font-weight: var(--bs-btn-font-weight);
            line-height: var(--bs-btn-line-height);
            color: white;
            text-align: center;
            text-decoration: none;
            vertical-align: middle;
            cursor: pointer;
            -webkit-user-select: none;
            -moz-user-select: none;
            user-select: none;
            border-radius: 6px;
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            align-self: center;
            border: 1px solid #001eff;
            margin-top: 0px;
            padding: 10px 20px;
            background-color: #001eff;
        }
    </style>

    <div class="page-container">
        <h1>Public Courses</h1>

        <!--Search Bar -->
        <div class="search-bar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="searchBox" Placeholder="Search by course title or educator..." />
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="auto-style1" OnClick="btnSearch_Click" />
        </div>

        <!--Course Listing -->
        <div class="course-grid">
            <asp:Repeater ID="rptPublicCourses" runat="server" OnItemCommand="rptPublicCourses_ItemCommand">
                <ItemTemplate>
                    <div class="course-card">
                        <div>
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
                        </div>
                        <asp:Button ID="btnStart" runat="server" Text="Start Course"
                            CssClass="btn"
                            CommandName="StartCourse"
                            CommandArgument='<%# Eval("Id") %>' />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
