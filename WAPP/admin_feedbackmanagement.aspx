<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="admin_feedbackmanagement.aspx.cs" Inherits="WAPP.admin_feedbackmanagement" %>

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
            max-width: 1200px;
            margin: 40px auto;
            text-align: center;
            background: rgba(255, 255, 255, 0.25);
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .management-header {
            margin: 0 0 30px 0;
            text-align: center;
        }

        .management-header h1 {
            font-size: 2rem;
            color: #111827;
            margin-bottom: 10px;
        }

        .management-header p {
            color: #666;
            font-size: 1.1rem;
        }

        /* ===== Stats Cards ===== */
        .stats-container {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin: 30px 0;
        }

        .stat-card {
            background: rgba(255, 255, 255, 0.4);
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.15);
            border-left: 4px solid #001eff;
            text-align: center;
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .stat-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 6px 20px rgba(0, 30, 255, 0.25);
        }

        .stat-number {
            font-size: 2.5rem;
            font-weight: bold;
            color: #001eff;
            margin-bottom: 10px;
        }

        .stat-label {
            color: #666;
            font-size: 1rem;
        }

        .stat-card.pending {
            border-left-color: #ffc107;
        }

        .stat-card.pending .stat-number {
            color: #ffc107;
        }

        .stat-card.completed {
            border-left-color: #28a745;
        }

        .stat-card.completed .stat-number {
            color: #28a745;
        }

        /* ===== Section Headers ===== */
        .section-header {
            margin: 40px 0 20px 0;
            padding-bottom: 15px;
            border-bottom: 2px solid rgba(233, 236, 239, 0.5);
            text-align: center;
        }

        .section-header h2 {
            margin: 0;
            color: #111827;
            font-size: 1.5rem;
        }

        .section-header p {
            color: #666;
            margin: 5px 0 0 0;
        }

        /* ===== Filter Section ===== */
        .filter-section {
            background: rgba(255, 255, 255, 0.3);
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.1);
            margin-bottom: 20px;
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .filter-row {
            display: flex;
            gap: 15px;
            align-items: end;
            flex-wrap: wrap;
        }

        .filter-group {
            flex: 1;
            min-width: 200px;
        }

        .filter-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 500;
            color: #333;
            font-size: 14px;
            text-align: left;
        }

        .filter-control {
            width: 100%;
            padding: 8px 12px;
            border: 1px solid rgba(0, 0, 0, 0.1);
            border-radius: 6px;
            font-size: 14px;
            background: rgba(255, 255, 255, 0.8);
        }

        /* ===== Buttons ===== */
        .btn {
            background-color: #001eff;
            color: white;
            border: none;
            border-radius: 6px;
            padding: 8px 16px;
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

        .btn-filter {
            background-color: #001eff;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
            width: 100%;
        }

        .btn-sm {
            padding: 6px 12px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 12px;
            border: none;
            transition: all 0.25s ease;
        }

        .btn-success {
            background-color: #28a745;
            color: white;
        }

        .btn-success:hover {
            background-color: #218838;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
        }

        .btn-danger {
            background-color: white;
            color: red;
            border: 1px solid rgba(255, 0, 0, 0.3);
        }

        .btn-danger:hover {
            background-color: red;
            color: white;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(255, 0, 0, 0.2);
        }

        /* ===== Feedback Cards ===== */
        .feedback-container {
            display: flex;
            flex-direction: column;
            gap: 20px;
            margin-top: 20px;
        }

        .feedback-card {
            background: rgba(255, 255, 255, 0.4);
            border: 1px solid rgba(255, 255, 255, 0.3);
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.1);
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .feedback-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 6px 20px rgba(0, 30, 255, 0.2);
        }

        .feedback-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
            flex-wrap: wrap;
            gap: 10px;
        }

        .feedback-title {
            font-size: 1.3rem;
            font-weight: 600;
            color: #111827;
            margin: 0;
            flex: 1;
        }

        .feedback-meta {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .feedback-priority, .feedback-category, .feedback-status {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 500;
            text-transform: capitalize;
            backdrop-filter: blur(5px);
        }

        .priority-high {
            background: rgba(248, 215, 218, 0.8);
            color: #721c24;
            border: 1px solid rgba(220, 53, 69, 0.2);
        }

        .priority-medium {
            background: rgba(255, 243, 205, 0.8);
            color: #856404;
            border: 1px solid rgba(255, 193, 7, 0.2);
        }

        .priority-low {
            background: rgba(209, 236, 241, 0.8);
            color: #0c5460;
            border: 1px solid rgba(23, 162, 184, 0.2);
        }

        .category-course-content {
            background: rgba(209, 236, 241, 0.8);
            color: #0c5460;
            border: 1px solid rgba(23, 162, 184, 0.2);
        }

        .category-community-post {
            background: rgba(212, 237, 218, 0.8);
            color: #155724;
            border: 1px solid rgba(40, 167, 69, 0.2);
        }

        .category-account {
            background: rgba(226, 227, 229, 0.8);
            color: #383d41;
            border: 1px solid rgba(108, 117, 125, 0.2);
        }

        .status-pending {
            background: rgba(255, 243, 205, 0.8);
            color: #856404;
            border: 1px solid rgba(255, 193, 7, 0.2);
        }

        .status-completed {
            background: rgba(212, 237, 218, 0.8);
            color: #155724;
            border: 1px solid rgba(40, 167, 69, 0.2);
        }

        .status-inprogress {
            background: rgba(204, 231, 255, 0.8);
            color: #004085;
            border: 1px solid rgba(0, 123, 255, 0.2);
        }

        .feedback-content {
            color: #666;
            line-height: 1.6;
            margin-bottom: 20px;
            font-size: 14px;
            white-space: pre-line;
            text-align: left;
        }

        .feedback-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-top: 15px;
            border-top: 1px solid rgba(233, 236, 239, 0.5);
            font-size: 13px;
            color: #999;
            flex-wrap: wrap;
            gap: 10px;
        }

        .feedback-user {
            font-weight: 500;
            color: #666;
        }

        .feedback-date {
            color: #888;
        }

        .feedback-actions {
            display: flex;
            gap: 10px;
        }

        /* ===== Empty States ===== */
        .no-feedback {
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

        .error-message {
            text-align: center;
            color: #dc3545;
            font-style: italic;
            padding: 60px 40px;
            border: 2px dashed rgba(220, 53, 69, 0.3);
            border-radius: 12px;
            margin: 20px 0;
            background: rgba(248, 215, 218, 0.5);
            backdrop-filter: blur(5px);
        }

        /* ===== Message Container ===== */
        .message-container {
            margin: 15px 0;
            padding: 15px;
            border-radius: 8px;
            font-weight: bold;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            text-align: center;
        }

        .message-success {
            background-color: rgba(212, 237, 218, 0.8);
            color: #155724;
            border: 1px solid rgba(195, 230, 203, 0.5);
        }

        .message-error {
            background-color: rgba(248, 215, 218, 0.8);
            color: #721c24;
            border: 1px solid rgba(245, 198, 203, 0.5);
        }

        /* ===== Responsive Design ===== */
        @media (max-width: 768px) {
            .management-container {
                padding: 20px;
                width: 95%;
            }
            
            .stats-container {
                grid-template-columns: 1fr;
            }
            
            .feedback-header {
                flex-direction: column;
                align-items: flex-start;
            }
            
            .feedback-meta {
                width: 100%;
                justify-content: flex-start;
            }
            
            .feedback-footer {
                flex-direction: column;
                align-items: flex-start;
            }
            
            .feedback-actions {
                width: 100%;
                justify-content: flex-start;
            }
            
            .filter-row {
                flex-direction: column;
            }
            
            .filter-group {
                min-width: 100%;
            }
        }
    </style>

    <!-- MAIN CONTENT -->
    <div class="management-container">
        <div class="management-header">
            <h1>Feedback Management</h1>
            <p>Review and manage user feedback</p>
        </div>

        <!-- STATS CARDS -->
        <div class="stats-container">
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblTotalFeedback" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Total Feedback</div>
            </div>
            <div class="stat-card pending">
                <div class="stat-number"><asp:Label ID="lblPendingFeedback" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Pending</div>
            </div>
            <div class="stat-card completed">
                <div class="stat-number"><asp:Label ID="lblCompletedFeedback" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Completed</div>
            </div>
        </div>

        <!-- FILTER SECTION -->
        <div class="filter-section">
            <div class="filter-row">
                <div class="filter-group">
                    <label for="ddlStatus">Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-control" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                        <asp:ListItem Value="">All Status</asp:ListItem>
                        <asp:ListItem Value="pending">Pending</asp:ListItem>
                        <asp:ListItem Value="inprogress">In Progress</asp:ListItem>
                        <asp:ListItem Value="completed">Completed</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label for="ddlPriority">Priority</label>
                    <asp:DropDownList ID="ddlPriority" runat="server" CssClass="filter-control" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                        <asp:ListItem Value="">All Priority</asp:ListItem>
                        <asp:ListItem Value="high">High</asp:ListItem>
                        <asp:ListItem Value="medium">Medium</asp:ListItem>
                        <asp:ListItem Value="low">Low</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label for="ddlCategory">Category</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="filter-control" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                        <asp:ListItem Value="">All Categories</asp:ListItem>
                        <asp:ListItem Value="course-content">Course Content</asp:ListItem>
                        <asp:ListItem Value="community-post">Community Post</asp:ListItem>
                        <asp:ListItem Value="account">Account</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" CssClass="btn btn-filter" OnClick="btnClearFilters_Click" />
                </div>
            </div>
        </div>

        <!-- FEEDBACK SECTION -->
        <div class="section-header">
            <h2>All Feedback</h2>
            <p>Review and manage user feedback submissions</p>
        </div>

        <asp:UpdatePanel ID="upFeedback" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="feedback-container">
                    <asp:Panel ID="pnlFeedback" runat="server">
                        <!-- Feedback cards will be dynamically added here -->
                    </asp:Panel>
                    
                    <asp:Label ID="lblNoFeedback" runat="server" Text="No feedback received yet. User feedback will appear here once submitted." CssClass="no-feedback" Visible="false"></asp:Label>
                    
                    <asp:Label ID="lblError" runat="server" Text="Error loading feedback. Please try again." CssClass="error-message" Visible="false"></asp:Label>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <script type="text/javascript">
        function confirmDelete(feedbackId, title) {
            if (confirm('Are you sure you want to delete the feedback: "' + title + '"?')) {
                __doPostBack('DeleteFeedback', feedbackId);
            }
            return false;
        }

        function completeFeedback(feedbackId) {
            if (confirm('Mark this feedback as completed?')) {
                __doPostBack('CompleteFeedback', feedbackId);
            }
            return false;
        }
    </script>
</asp:Content>