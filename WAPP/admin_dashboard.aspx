<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="admin_dashboard.aspx.cs" Inherits="WAPP.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ===== Navigation Bar ===== */
        .navbar {
            background-color: #ffffff;
            padding: 12px 20px;
            display: flex;
            justify-content: flex-start;
            align-items: center;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.15);
        }
        .navbar a,
        .navbar asp\:LinkButton {
            color: black;
            text-decoration: none;
            margin-right: 30px;
            font-weight: 500;
            font-size: 16px;
            transition: color 0.3s ease, transform 0.3s ease;
        }
        .navbar a:hover,
        .navbar asp\:LinkButton:hover {
            color: #38bdf8;
            transform: translateY(-2px);
        }
        .navbar .active {
            color: #38bdf8;
            border-bottom: 2px solid #38bdf8;
            padding-bottom: 4px;
        }

        /* ===== Card Layout ===== */
        .card-container {
            display: grid !important;
            grid-template-columns: repeat(4, 1fr) !important; /* four equal columns */
            gap: 25px !important;
            margin: 40px 0 !important;
        }

        .card {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
            padding: 25px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }
        .card:hover {
            transform: translateY(-6px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.15);
        }

        .card-content h4 {
            margin: 0;
            font-size: 1.2rem;
            color: #333;
        }
        .card-content p {
            font-size: 1.6rem;
            font-weight: bold;
            color: #007bff;
            margin-top: 8px;
        }
        .card-icon {
            font-size: 2.5rem;
            color: #007bff;
        }
        h3 {
            margin-bottom: 10px;
        }

        .course-container {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
        }

        .course-card {
            background-color: #fff;
            border: 1px solid #e1e1e1;
            border-radius: 12px;
            padding: 20px;
            flex: 1 1 calc(25% - 20px); /* 4 per row */
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }

        .course-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }

        .course-title {
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 5px;
        }

        .course-info {
            font-size: 14px;
            color: #666;
            margin-bottom: 10px;
        }

        .course-actions {
            display: flex;
            justify-content: space-between;
            margin-top: 10px;
        }

        .btn {
            padding: 6px 12px;
            border: none;
            border-radius: 6px;
            font-size: 14px;
            cursor: pointer;
        }

        .btn-view {
            background-color: #007bff;
            color: white;
        }

        .btn-delete {
            background-color: transparent;
            color: red;
            border: 1px solid red;
        }

        .course-image {
            width: 100%;
            height: 150px;
            object-fit: cover;
            border-radius: 10px;
            margin-bottom: 10px;
        }
    </style>

    <!-- ===== NAVIGATION BAR ===== -->
    <div class="navbar">
        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="active" PostBackUrl="~/admin_dashboard.aspx">Dashboard</asp:LinkButton>
        <asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/community.aspx">Community</asp:LinkButton>
        <asp:LinkButton ID="LinkButton3" runat="server" PostBackUrl="~/user_management.aspx">User Management</asp:LinkButton>
        <asp:LinkButton ID="LinkButton4" runat="server" PostBackUrl="~/feedback.aspx">Feedback</asp:LinkButton>
        <asp:LinkButton ID="LinkButton5" runat="server" PostBackUrl="~/manage_ads.aspx">Manage Ads</asp:LinkButton>
    </div>

    <br />
    <h1>Admin Dashboard</h1>
    <p>Manage the Sea Learner Platform</p>

    <!-- ===== DASHBOARD CARDS ===== -->
    <div class="card-container">
        <asp:Panel ID="Panel1" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Total Students</h4>
                <p><asp:Label ID="lblTotalStudents" runat="server" Text="120"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgStudents" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel2" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Total Educators</h4>
                <p><asp:Label ID="lblTotalTeachers" runat="server" Text="25"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgteacher" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel3" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Total Courses</h4>
                <p><asp:Label ID="lblTotalCourses" runat="server" Text="15"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgcourse" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel4" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Pending Feedback</h4>
                <p><asp:Label ID="lblTotalEvents" runat="server" Text="8"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgfeedback" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>
    </div>
    <!-- ===== Course Management ===== -->
    <div>
        <h3>Course Management</h3>
        <p>View and manage all courses on the platform</p>
        <div>
        <h3>Course Management</h3>
        <p>View and manage all courses on the platform</p>
        <div class="course-container">
            <asp:Repeater ID="RepeaterCourses" runat="server">
                <ItemTemplate>
                    <div class="course-card" onclick="window.location.href='CourseDetails.aspx?id=<%# Eval("Id") %>'">
                        <img src='<%# Eval("CoursePicture") %>' alt="Course Image" />

                        <p><b>Created by:</b> <%# Eval("EducatorId") %></p>
                        <p><b>Status:</b> <%# Eval("Status") %></p>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
