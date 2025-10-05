<%@ Page Title="Student Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentDashboard.aspx.cs"
    Inherits="WAPP.StudentDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .container {
            max-width: 1100px;
            margin: auto;
            padding: 30px;
            font-family: Arial;
        }

        .welcome {
            color: #3732d0;
            font-size: 24px;
            font-weight: bold;
        }

        .stats {
            display: flex;
            gap: 20px;
            margin: 20px 0;
            flex-wrap: wrap;
        }

        .card {
            background: white;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            padding: 20px;
            flex: 1;
            text-align: center;
            min-width: 250px;
        }

        .card h3 {
            color: #3732d0;
        }

        .section {
            margin-top: 40px;
        }

        /* 🟣 Horizontal Scroll Container */
        .course-list {
            display: flex;
            gap: 20px;
            overflow-x: auto;
            overflow-y: hidden;
            padding-bottom: 15px;
            scroll-snap-type: x mandatory;
            -webkit-overflow-scrolling: touch;
        }

        .course-card {
            background: white;
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 20px;
            width: 300px;
            min-height: 250px;
            box-sizing: border-box;
            flex-shrink: 0;
            scroll-snap-align: start;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
        }

        .course-card h4 {
            margin: 0 0 8px;
            color: #000;
            font-size: 18px;
        }

        .course-card p {
            margin: 0 0 10px;
            color: #666;
            font-size: 14px;
        }

        .btn {
            background-color: #3732d0;
            color: white;
            border: none;
            padding: 8px 12px;
            border-radius: 6px;
            cursor: pointer;
            width: 100%;
            margin-top: auto; /* ensures alignment at bottom */
        }

        .btn:hover {
            background-color: #2926a6;
        }

        /* ✅ Scrollbar style for modern look */
        .course-list::-webkit-scrollbar {
            height: 8px;
        }

        .course-list::-webkit-scrollbar-thumb {
            background-color: #3732d0;
            border-radius: 4px;
        }

        .course-list::-webkit-scrollbar-track {
            background: #f0f0f0;
        }
        .searchBox {
            padding: 8px 10px;
            border: 1px solid #ccc;
            border-radius: 6px;
            min-width: 220px;
            font-size: 14px;
        }

    </style>

    <div class="container">
        <p class="welcome">Welcome back, <asp:Label ID="lblStudentName" runat="server" /> 👋</p>

        <!-- Stats -->
        <div class="stats">
            <div class="card">
                <h3>Coins</h3>
                <asp:Label ID="lblCoins" runat="server" Text="0" Font-Size="Large"></asp:Label>
            </div>
            <div class="card">
                <h3>Badges</h3>
                <asp:Label ID="lblBadges" runat="server" Text="0" Font-Size="Large"></asp:Label>
            </div>
            <div class="card">
                <h3>Completed Courses</h3>
                <asp:Label ID="lblCoursesCompleted" runat="server" Text="0" Font-Size="Large"></asp:Label>
            </div>
        </div>

        <!-- 🔹 Incomplete Courses -->
        <div class="section">
            <h2>Incomplete Courses</h2>
            <div class="course-list">
                <asp:Repeater ID="rptIncompleteCourses" runat="server">
                    <ItemTemplate>
                        <div class="course-card" style="border-color: darkblue; background:#bde9fa;">
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
                            <p><strong>⌛ On Going</strong></p>
                            <button class="btn" type="button"
                                onclick="window.location='CourseDetails.aspx?id=<%# Eval("Id") %>'">Continue Learning</button>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- 🟢 Public Courses -->
        <div class="section">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <h2>Public Courses</h2>

                <!-- 🔍 Search Bar -->
                <div style="display: flex; gap: 10px;">
                    <asp:TextBox ID="txtSearchPublic" runat="server" CssClass="searchBox" 
                        Placeholder="Search public courses..." />
                    <asp:Button ID="btnSearchPublic" runat="server" Text="Search" CssClass="btn"
                        OnClick="btnSearchPublic_Click" />
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
                                onclick="window.location='CourseDetails.aspx?id=<%# Eval("Id") %>'">Start Learning</button>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- 🔵 Private Courses -->
        <div class="section">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <h2>Private Courses</h2>

                <!-- 🔍 Search Bar -->
                <div style="display: flex; gap: 10px;">
                    <asp:TextBox ID="txtSearchPrivate" runat="server" CssClass="searchBox"
                        Placeholder="Search private courses..." />
                    <asp:Button ID="btnSearchPrivate" runat="server" Text="Search" CssClass="btn"
                        OnClick="btnSearchPrivate_Click" />
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

        <!-- 🟩 Completed Courses -->
        <div class="section">
            <h2>Completed Courses</h2>
            <div class="course-list">
                <asp:Repeater ID="rptCompletedCourses" runat="server">
                    <ItemTemplate>
                        <div class="course-card" style="border-color: green; background:#eafbea;">
                            <h4><%# Eval("Title") %></h4>
                            <p>By <%# Eval("EducatorName") %></p>
                            <p><strong>✅ Completed</strong></p>
                            <button class="btn" type="button"
                                onclick="window.location='CourseDetails.aspx?id=<%# Eval("Id") %>'">Review Course</button>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
</asp:Content>
