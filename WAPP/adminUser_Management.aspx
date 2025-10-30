<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="user_management.aspx.cs" Inherits="WAPP.user_management" %>

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
            grid-template-columns: repeat(4, 1fr) !important;
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

        /* ===== User Management ===== */
        .user-management-header {
            margin: 30px 0 20px 0;
        }

        .search-section {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 12px;
            margin-bottom: 25px;
        }

        .search-box {
            display: flex;
            gap: 10px;
            align-items: center;
            margin-bottom: 15px;
        }

        .search-input {
            flex: 1;
            padding: 10px 15px;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-size: 14px;
        }

        .btn-search {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-search:hover {
            background-color: #0056b3;
        }

        .user-tabs {
            display: flex;
            gap: 5px;
            background: white;
            padding: 5px;
            border-radius: 8px;
            border: 1px solid #e1e1e1;
        }

        .tab {
            padding: 8px 16px;
            border: none;
            background: transparent;
            cursor: pointer;
            transition: all 0.3s ease;
            border-radius: 6px;
            font-size: 14px;
        }

        .tab.active {
            background: #007bff;
            color: white;
        }

        .tab:hover {
            background: #e9ecef;
        }

        .tab.active:hover {
            background: #0056b3;
        }

        /* User Cards */
        .user-cards-container {
            display: flex;
            flex-direction: column;
            gap: 20px;
            margin-top: 20px;
        }

        .user-card {
            background: white;
            border: 1px solid #e1e1e1;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            transition: transform 0.2s ease, box-shadow 0.2s ease;
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
        }

        .user-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }

        .user-main-content {
            flex: 1;
            display: flex;
            flex-direction: column;
            gap: 15px;
        }

        .user-header {
            display: flex;
            align-items: flex-start;
            gap: 15px;
            margin-bottom: 10px;
        }

        .user-avatar {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            background: linear-gradient(135deg, #007bff, #0056b3);
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 18px;
            flex-shrink: 0;
        }

        .user-info {
            flex: 1;
        }

        .user-name {
            margin: 0 0 5px 0;
            font-size: 20px;
            color: #333;
            font-weight: 600;
        }

        .user-role {
            background: #e9ecef;
            color: #495057;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            display: inline-block;
        }

        .student-role {
            background: #d4edda;
            color: #155724;
        }

        .educator-role {
            background: #d1ecf1;
            color: #0c5460;
        }

        .user-details {
            margin-bottom: 10px;
        }

        .user-detail {
            display: flex;
            align-items: center;
            margin-bottom: 8px;
            font-size: 14px;
            color: #666;
        }

        .user-detail i {
            margin-right: 8px;
            width: 16px;
            text-align: center;
        }

        .user-stats {
            display: flex;
            gap: 30px;
            margin-top: 10px;
        }

        .stat-item {
            text-align: center;
            min-width: 80px;
        }

        .stat-value {
            font-size: 20px;
            font-weight: bold;
            color: #007bff;
            display: block;
            margin-bottom: 2px;
        }

        .stat-label {
            font-size: 12px;
            color: #666;
            display: block;
        }

        .user-status-section {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            gap: 10px;
            min-width: 120px;
        }

        .status-toggle {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .switch {
            position: relative;
            display: inline-block;
            width: 44px;
            height: 24px;
        }

        .switch input {
            opacity: 0;
            width: 0;
            height: 0;
        }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            transition: .4s;
            border-radius: 24px;
        }

        .slider:before {
            position: absolute;
            content: "";
            height: 18px;
            width: 18px;
            left: 3px;
            bottom: 3px;
            background-color: white;
            transition: .4s;
            border-radius: 50%;
        }

        input:checked + .slider {
            background-color: #28a745;
        }

        input:checked + .slider:before {
            transform: translateX(20px);
        }

        .status-text {
            font-size: 14px;
            font-weight: 500;
            color: #28a745;
            min-width: 70px;
            text-align: center;
        }

        .status-text.disabled {
            color: #dc3545;
        }

        .no-users {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 40px;
            border: 1px dashed #ddd;
            border-radius: 8px;
            margin: 20px 0;
            grid-column: 1 / -1;
        }

        .clear-search {
            background: #6c757d;
            color: white;
            border: none;
            padding: 10px 15px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            margin-left: 10px;
        }

        .clear-search:hover {
            background: #545b62;
        }

        /* Success message */
        .success-message {
            background-color: #d4edda;
            border: 1px solid #c3e6cb;
            color: #155724;
            padding: 12px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
        }

        .error-message {
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
            padding: 12px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
        }
        /* Button Styles */
        .disable-btn, .enable-btn {
            padding: 10px 20px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.3s ease;
            min-width: 120px;
            margin: 5px;
        }

        .disable-btn {
            background-color: #dc3545;
            color: white;
        }

        .disable-btn:hover {
            background-color: #c82333;
        }

        .enable-btn {
            background-color: #28a745;
            color: white;
        }

        .enable-btn:hover {
            background-color: #218838;
        }

        /* Success/Error Messages */
        .success-message {
            background-color: #d4edda;
            border: 1px solid #c3e6cb;
            color: #155724;
            padding: 12px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
        }

        .error-message {
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
            padding: 12px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
        }
    </style>

    <!-- ===== NAVIGATION BAR ===== -->
    <div class="navbar">
        <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/admin_dashboard.aspx">Dashboard</asp:LinkButton>
        <asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/admin_communitymanagement.aspx">Community</asp:LinkButton>
        <asp:LinkButton ID="LinkButton3" runat="server" PostBackUrl="~/adminUser_Management.aspx">User Management</asp:LinkButton>
        <asp:LinkButton ID="LinkButton4" runat="server" PostBackUrl="~/admin_feedbackmanagement.aspx">Feedback</asp:LinkButton>
        <asp:LinkButton ID="LinkButton5" runat="server" CssClass="active" PostBackUrl="~/adminManage_Ads.aspx">Manage Ads</asp:LinkButton>
    </div>

    <br />
    
    <!-- Success/Error Messages -->
    <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-message" Visible="false">
        <asp:Label ID="lblSuccess" runat="server" Text=""></asp:Label>
    </asp:Panel>
    
    <asp:Panel ID="pnlError" runat="server" CssClass="error-message" Visible="false">
        <asp:Label ID="lblError" runat="server" Text=""></asp:Label>

    </asp:Panel>

    <div class="user-management-header">
        <h1>User Management</h1>
        <p>Manage user accounts and permissions</p>
    </div>

    <!-- ===== DASHBOARD CARDS ===== -->
    <div class="card-container">
        <asp:Panel ID="Panel1" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Total Users</h4>
                <p><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgUsers" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel2" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Active Users</h4>
                <p><asp:Label ID="lblActiveUsers" runat="server" Text="0"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgActive" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel3" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Students</h4>
                <p><asp:Label ID="lblTotalStudents" runat="server" Text="0"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgStudents" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel4" runat="server" CssClass="card">
            <div class="card-content">
                <h4>Educators</h4>
                <p><asp:Label ID="lblTotalEducators" runat="server" Text="0"></asp:Label></p>
            </div>
            <div class="card-icon">
                <asp:Image ID="imgEducators" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
            </div>
        </asp:Panel>
    </div>

    <!-- ===== Search Section ===== -->
    <div class="search-section">
        <div class="search-box">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search users by name or email..."></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-search" OnClick="btnSearch_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="clear-search" OnClick="btnClear_Click" />
        </div>
        
        <div class="user-tabs">
            <asp:LinkButton ID="btnAllUsers" runat="server" CssClass="tab active" CommandArgument="All" OnClick="btnTab_Click">All Users</asp:LinkButton>
            <asp:LinkButton ID="btnStudents" runat="server" CssClass="tab" CommandArgument="student" OnClick="btnTab_Click">Students</asp:LinkButton>
            <asp:LinkButton ID="btnEducators" runat="server" CssClass="tab" CommandArgument="educator" OnClick="btnTab_Click">Educators</asp:LinkButton>
        </div>
    </div>

    <!-- ===== User Accounts Section ===== -->
    <div>
        <h3>User Accounts</h3>
        <p>Manage user status and permissions (<asp:Label ID="lblUsersShown" runat="server" Text="0"></asp:Label> users shown)</p>
        
        <div class="user-cards-container">
            <asp:Panel ID="pnlUserContainer" runat="server">
                <!-- User cards will be dynamically added here -->
            </asp:Panel>
            
            <asp:Label ID="lblNoUsers" runat="server" Text="No users found." CssClass="no-users" Visible="false"></asp:Label>
        </div>
    </div>
</asp:Content>