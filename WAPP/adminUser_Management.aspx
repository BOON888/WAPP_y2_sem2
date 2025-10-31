<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="user_management.aspx.cs" Inherits="WAPP.user_management" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ===== General Page Style ===== */
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
            margin: 0;
            padding: 0;
        }

        /* ===== Main Container ===== */
        .management-container {
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 90%;
            max-width: 1400px;
            margin: 40px auto;
            text-align: center;
            background: rgba(255, 255, 255, 0.25);
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        /* ===== Navigation Bar ===== */
        .navbar {
            background: rgba(255, 255, 255, 0.4);
            padding: 12px 20px;
            display: flex;
            justify-content: flex-start;
            align-items: center;
            border-radius: 8px;
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.15);
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            margin-bottom: 30px;
        }
        .navbar a,
        .navbar asp\:LinkButton {
            color: #111827;
            text-decoration: none;
            margin-right: 30px;
            font-weight: 500;
            font-size: 16px;
            transition: color 0.3s ease, transform 0.3s ease;
            padding: 8px 16px;
            border-radius: 6px;
        }
        .navbar a:hover,
        .navbar asp\:LinkButton:hover {
            color: #001eff;
            transform: translateY(-2px);
            background: rgba(0, 30, 255, 0.1);
        }
        .navbar .active {
            color: #001eff;
            background: rgba(0, 30, 255, 0.15);
            border-bottom: 2px solid #001eff;
            padding-bottom: 6px;
        }

        /* ===== Card Layout ===== */
        .card-container {
            display: grid !important;
            grid-template-columns: repeat(4, 1fr) !important;
            gap: 25px !important;
            margin: 40px 0 !important;
        }

        .card {
            background: rgba(255, 255, 255, 0.4);
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.15);
            padding: 25px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }
        .card:hover {
            transform: translateY(-6px);
            box-shadow: 0 8px 25px rgba(0, 30, 255, 0.25);
        }

        .card-content h4 {
            margin: 0;
            font-size: 1.2rem;
            color: #111827;
            text-align: left;
        }
        .card-content p {
            font-size: 1.6rem;
            font-weight: bold;
            color: #001eff;
            margin-top: 8px;
            text-align: left;
        }
        .card-icon {
            font-size: 2.5rem;
            color: #001eff;
        }

        /* ===== User Management ===== */
        .user-management-header {
            margin: 0 0 30px 0;
            text-align: center;
        }

        .user-management-header h1 {
            color: #111827;
            margin-bottom: 10px;
        }

        .user-management-header p {
            color: #666;
            font-size: 1.1rem;
        }

        .search-section {
            background: rgba(255, 255, 255, 0.3);
            padding: 25px;
            border-radius: 12px;
            margin-bottom: 25px;
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.1);
        }

        .search-box {
            display: flex;
            gap: 10px;
            align-items: center;
            margin-bottom: 15px;
        }

        .search-input {
            flex: 1;
            padding: 12px 15px;
            border: 1px solid rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            font-size: 14px;
            background: rgba(255, 255, 255, 0.8);
            transition: border-color 0.3s ease, box-shadow 0.3s ease;
        }

        .search-input:focus {
            outline: none;
            border-color: #001eff;
            box-shadow: 0 0 0 2px rgba(0, 30, 255, 0.25);
        }

        /* ===== Buttons ===== */
        .btn {
            background-color: #001eff;
            color: white;
            border: none;
            border-radius: 6px;
            padding: 10px 20px;
            font-size: 14px;
            cursor: pointer;
            margin: 2px;
            transition: background-color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
            opacity: 0.95;
        }

        .btn:active {
            transform: translateY(0);
            box-shadow: 0 3px 8px rgba(0, 30, 255, 0.2);
            opacity: 1;
        }

        .btn-search {
            background-color: #001eff;
            color: white;
        }

        .clear-search {
            background-color: white;
            color: #001eff;
            border: 1px solid #001eff;
        }

        .clear-search:hover {
            background-color: #f8f9fa;
        }

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
            background-color: white;
            color: red;
            border: 1px solid rgba(255, 0, 0, 0.3);
        }

        .disable-btn:hover {
            background-color: red;
            color: white;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(255, 0, 0, 0.2);
        }

        .enable-btn {
            background-color: #28a745;
            color: white;
        }

        .enable-btn:hover {
            background-color: #218838;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
        }

        .user-tabs {
            display: flex;
            gap: 5px;
            background: rgba(255, 255, 255, 0.3);
            padding: 5px;
            border-radius: 8px;
            border: 1px solid rgba(255, 255, 255, 0.3);
            backdrop-filter: blur(5px);
        }

        .tab {
            padding: 8px 16px;
            border: none;
            background: transparent;
            cursor: pointer;
            transition: all 0.3s ease;
            border-radius: 6px;
            font-size: 14px;
            color: #111827;
            text-decoration: none;
            display: inline-block;
        }

        .tab.active {
            background: #001eff;
            color: white;
        }

        .tab:hover {
            background: rgba(0, 30, 255, 0.1);
        }

        .tab.active:hover {
            background: #001eff;
        }

        /* User Cards */
        .user-cards-container {
            display: flex;
            flex-direction: column;
            gap: 20px;
            margin-top: 20px;
        }

        .user-card {
            background: rgba(255, 255, 255, 0.4);
            border: 1px solid rgba(255, 255, 255, 0.3);
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
        }

        .user-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 6px 20px rgba(0, 30, 255, 0.2);
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
            background: linear-gradient(135deg, #001eff, #0014a3);
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 18px;
            flex-shrink: 0;
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.3);
        }

        .user-info {
            flex: 1;
        }

        .user-name {
            margin: 0 0 5px 0;
            font-size: 20px;
            color: #111827;
            font-weight: 600;
        }

        .user-role {
            background: rgba(233, 236, 239, 0.8);
            color: #495057;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            display: inline-block;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .student-role {
            background: rgba(212, 237, 218, 0.8);
            color: #155724;
            border: 1px solid rgba(40, 167, 69, 0.2);
        }

        .educator-role {
            background: rgba(209, 236, 241, 0.8);
            color: #0c5460;
            border: 1px solid rgba(23, 162, 184, 0.2);
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
            color: #001eff;
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
            box-shadow: inset 0 2px 4px rgba(0,0,0,0.1);
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
            box-shadow: 0 2px 4px rgba(0,0,0,0.2);
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
            padding: 60px 40px;
            border: 2px dashed rgba(221, 221, 221, 0.5);
            border-radius: 12px;
            margin: 20px 0;
            background: rgba(250, 250, 250, 0.5);
            backdrop-filter: blur(5px);
        }

        /* Success/Error Messages */
        .success-message {
            background-color: rgba(212, 237, 218, 0.8);
            border: 1px solid rgba(195, 230, 203, 0.5);
            color: #155724;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            backdrop-filter: blur(5px);
        }

        .error-message {
            background-color: rgba(248, 215, 218, 0.8);
            border: 1px solid rgba(245, 198, 203, 0.5);
            color: #721c24;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            backdrop-filter: blur(5px);
        }

        .user-actions {
            display: flex;
            gap: 10px;
            margin-top: 10px;
        }

        /* Section Headers */
        .section-header {
            margin: 30px 0 20px 0;
            text-align: center;
        }

        .section-header h3 {
            color: #111827;
            margin-bottom: 5px;
        }

        .section-header p {
            color: #666;
            margin: 0;
        }

        /* Responsive Design */
        @media (max-width: 1200px) {
            .card-container {
                grid-template-columns: repeat(2, 1fr) !important;
            }
        }

        @media (max-width: 768px) {
            .management-container {
                padding: 20px;
                width: 95%;
            }
            
            .card-container {
                grid-template-columns: 1fr !important;
            }
            
            .navbar {
                flex-wrap: wrap;
                gap: 10px;
            }
            
            .navbar a,
            .navbar asp\:LinkButton {
                margin-right: 15px;
                font-size: 14px;
            }
            
            .search-box {
                flex-direction: column;
            }
            
            .user-card {
                flex-direction: column;
                gap: 20px;
            }
            
            .user-status-section {
                align-items: flex-start;
                width: 100%;
            }
            
            .user-tabs {
                flex-wrap: wrap;
            }
            
            .user-stats {
                flex-wrap: wrap;
                gap: 15px;
            }
        }
    </style>

    <!-- ===== MAIN CONTENT ===== -->
    <div class="management-container">
        
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
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-search" OnClick="btnSearch_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn clear-search" OnClick="btnClear_Click" />
            </div>
            
            <div class="user-tabs">
                <asp:LinkButton ID="btnAllUsers" runat="server" CssClass="tab active" CommandArgument="All" OnClick="btnTab_Click">All Users</asp:LinkButton>
                <asp:LinkButton ID="btnStudents" runat="server" CssClass="tab" CommandArgument="student" OnClick="btnTab_Click">Students</asp:LinkButton>
                <asp:LinkButton ID="btnEducators" runat="server" CssClass="tab" CommandArgument="educator" OnClick="btnTab_Click">Educators</asp:LinkButton>
            </div>
        </div>

        <!-- ===== User Accounts Section ===== -->
        <div class="section-header">
            <h3>User Accounts</h3>
            <p>Manage user status and permissions (<asp:Label ID="lblUsersShown" runat="server" Text="0"></asp:Label> users shown)</p>
        </div>
        
        <div class="user-cards-container">
            <asp:Panel ID="pnlUserContainer" runat="server">
                <!-- User cards will be dynamically added here -->
            </asp:Panel>
            
            <asp:Label ID="lblNoUsers" runat="server" Text="No users found." CssClass="no-users" Visible="false"></asp:Label>
        </div>
    </div>

    <script type="text/javascript">
        function confirmDisable(userId, userName) {
            if (confirm('Are you sure you want to disable user: ' + userName + '?')) {
                __doPostBack('DisableUser', userId);
            }
            return false;
        }

        function confirmEnable(userId, userName) {
            if (confirm('Are you sure you want to enable user: ' + userName + '?')) {
                __doPostBack('EnableUser', userId);
            }
            return false;
        }
    </script>
</asp:Content>