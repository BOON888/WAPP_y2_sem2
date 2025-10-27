<%@ Page Title="Student Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentDashboard.aspx.cs"
    Inherits="WAPP.StudentDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
        }

        .dashboard-container {
            background-color: white;
            border-radius: 12px;
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 99%;
            max-width: 1900px;
            margin: 60px auto;
        }

        .welcome {
            color: #111827;
            font-size: 26px;
            font-weight: 700;
            margin-bottom: 20px;
        }

        .stats {
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
            margin-bottom: 40px;
        }

        .card {
            background-color: #f9fafb;
            border-radius: 10px;
            border: 1px solid #e5e7eb;
            box-shadow: 0 1px 4px rgba(0,0,0,0.05);
            padding: 25px;
            flex: 1;
            text-align: center;
            min-width: 250px;
        }

        .card h3 {
            color: #001eff;
            margin-bottom: 10px;
        }

        .section {
            margin-top: 40px;
        }

        .section h2 {
            color: #111827;
            margin-bottom: 20px;
        }

        .course-list {
            display: flex;
            gap: 20px;
            overflow-x: auto;
            padding-bottom: 15px;
            scroll-snap-type: x mandatory;
            -webkit-overflow-scrolling: touch;
        }

        .course-card {
            background-color: #ffffff;
            border: 1px solid #d1d5db;
            border-radius: 10px;
            box-shadow: 0 1px 5px rgba(0,0,0,0.05);
            padding: 20px;
            width: 300px;
            min-height: 250px;
            flex-shrink: 0;
            scroll-snap-align: start;
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

        .btn {
            background-color: #001eff;
            color: white;
            border: 1px solid #001eff;
            border-radius: 6px;
            padding: 10px;
            width: 100%;
            font-size: 15px;
            cursor: pointer;
            margin-top: auto;
            transition: all 0.3s ease;
        }

        .btn:hover {
            background-color: white;
            color: #001eff;
        }

        .searchBox {
            padding: 8px 10px;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            min-width: 220px;
            font-size: 14px;
            background-color: #f9fafb;
        }

        .course-list::-webkit-scrollbar {
            height: 8px;
        }
        .course-list::-webkit-scrollbar-thumb {
            background-color: #001eff;
            border-radius: 4px;
        }
        .course-list::-webkit-scrollbar-track {
            background: #f0f0f0;
        }
    </style>

    <div class="dashboard-container">
        <p class="welcome">Welcome back, <asp:Label ID="lblStudentName" runat="server" /> 👋</p>

        <!-- Stats -->
        <div class="stats">
            <div class="card">
                <h3>Coins</h3>
                <asp:Label ID="lblCoins" runat="server" Text="0"></asp:Label>
            </div>
            <div class="card">
                <h3>Badges</h3>
                <asp:Label ID="lblBadges" runat="server" Text="0"></asp:Label>
            </div>
            <div class="card">
                <h3>Completed Courses</h3>
                <asp:Label ID="lblCoursesCompleted" runat="server" Text="0"></asp:Label>
            </div>
        </div>

        <!-- Incomplete Courses -->
        <div class="section">
            <h2>Incomplete Courses</h2>
            <div class="course-list">
                <asp:Repeater ID="rptIncompleteCourses" runat="server">
                    <ItemTemplate>
                        <div class="course-card" style="background:#bde9fa; border-color:darkblue;">
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
                            <p><strong>⌛ On Going</strong></p>
                            <button class="btn" type="button"
                                onclick="window.location='StudentCourseContent.aspx?courseId=<%# Eval("Id") %>'">Continue Learning</button>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Public Courses -->
        <div class="section">
            <div style="display:flex; justify-content:space-between; align-items:center;">
                <h2>Public Courses</h2>
                <div style="display:flex; gap:10px;">
                    <asp:TextBox ID="txtSearchPublic" runat="server" CssClass="searchBox" Placeholder="Search public courses..." />
                    <asp:Button ID="btnSearchPublic" runat="server" Text="Search" CssClass="btn" OnClick="btnSearchPublic_Click" />
                </div>
            </div>

            <div class="course-list">
                <asp:Repeater ID="rptPublicCourses" runat="server">
                    <ItemTemplate>
                        <div class="course-card">
                            <div>
                                <h4><%# Eval("Title") %></h4>
                                <p>By <%# Eval("EducatorName") %></p>
                            </div>
                            <button class="btn" type="button"
                                onclick="window.location='StudentCourseContent.aspx?courseId=<%# Eval("Id") %>'">Start Learning</button>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Private Courses -->
        <div class="section">
            <div style="display:flex; justify-content:space-between; align-items:center;">
                <h2>Private Courses</h2>
                <div style="display:flex; gap:10px;">
                    <asp:TextBox ID="txtSearchPrivate" runat="server" CssClass="searchBox" Placeholder="Search private courses..." />
                    <asp:Button ID="btnSearchPrivate" runat="server" Text="Search" CssClass="btn" OnClick="btnSearchPrivate_Click" />
                </div>
            </div>

            <div class="course-list">
                <asp:Repeater ID="rptPrivateCourses" runat="server" OnItemCommand="rptPrivateCourses_ItemCommand">
                    <ItemTemplate>
                        <div class="course-card">
                            <div>
                                <h4><%# Eval("Title") %></h4>
                                <p>By <%# Eval("EducatorName") %></p>
                                <p><strong><%# Eval("CoinReward") %> Coins Required</strong></p>
                            </div>
                            <asp:Button ID="btnSubscribe" runat="server" Text="Subscribe with Coins" CssClass="btn"
                                CommandName="SubscribeCourse" CommandArgument='<%# Eval("Id") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Completed Courses -->
        <div class="section">
            <h2>Completed Courses</h2>
            <div class="course-list">
                <asp:Repeater ID="rptCompletedCourses" runat="server">
                    <ItemTemplate>
                        <div class="course-card" style="background:#eafbea; border-color:green;">
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
                            <p><strong>✅ Completed</strong></p>
                            <button class="btn" type="button"
                                onclick="window.location='StudentCourseContent.aspx?courseId=<%# Eval("Id") %>'">Review Course</button>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
