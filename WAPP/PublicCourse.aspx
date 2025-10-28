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

        /* 🔙 Back Button */
        .back-btn {
            background-color: #38bdf8;
            color: white;
            border: none;
            border-radius: 6px;
            padding: 10px 20px;
            font-size: 15px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 25px;
            text-decoration: none;
        }

        .back-btn:hover {
            background-color: #0ea5e9;
            transform: translateY(-2px);
        }

        .back-btn i {
            font-style: normal;
            font-weight: bold;
        }

        /* 🔍 Search bar aligned to left */
        .search-bar {
            display: flex;
            gap: 10px;
            margin-bottom: 30px;
            align-items: center;
            justify-content: flex-start;
        }

        .searchBox {
            padding: 10px 12px;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            min-width: 300px;
            font-size: 14px;
            background-color: #f9fafb;
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
        <!-- 🔙 Back Button -->
        <a href="StudentDashboard.aspx" class="back-btn">
            <i>←</i> Back to Dashboard
        </a>

        <h1>Public Courses</h1>

        <!-- 🔍 Search Bar (aligned left) -->
        <div class="search-bar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="searchBox" Placeholder="Search by course title or educator..." />
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn" OnClick="btnSearch_Click" />
        </div>

        <!-- Course Listing -->
        <div class="course-grid">
            <asp:Repeater ID="rptPublicCourses" runat="server">
                <ItemTemplate>
                    <div class="course-card">
                        <div>
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
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
