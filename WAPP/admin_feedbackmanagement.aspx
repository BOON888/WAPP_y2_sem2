<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="admin_feedbackmanagement.aspx.cs" Inherits="WAPP.admin_feedbackmanagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ===== Main Content ===== */
        .management-header {
            margin: 30px 0 20px 0;
        }

        .management-header h1 {
            font-size: 2rem;
            color: #333;
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
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            border-left: 4px solid #007bff;
            text-align: center;
        }

        .stat-number {
            font-size: 2.5rem;
            font-weight: bold;
            color: #007bff;
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

        .stat-card.technical {
            border-left-color: #17a2b8;
        }

        .stat-card.technical .stat-number {
            color: #17a2b8;
        }

        /* ===== Section Header ===== */
        .section-header {
            margin: 40px 0 20px 0;
            padding-bottom: 10px;
            border-bottom: 2px solid #e9ecef;
        }

        .section-header h2 {
            margin: 0;
            color: #333;
            font-size: 1.5rem;
        }

        /* ===== Feedback Grid ===== */
        .feedback-container {
            display: flex;
            flex-direction: column;
            gap: 20px;
            margin-top: 20px;
        }

        .feedback-card {
            background: white;
            border: 1px solid #e1e1e1;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }

        .feedback-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
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
            color: #333;
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
        }

        .priority-high {
            background: #f8d7da;
            color: #721c24;
        }

        .priority-medium {
            background: #fff3cd;
            color: #856404;
        }

        .priority-low {
            background: #d1ecf1;
            color: #0c5460;
        }

        .category-technical {
            background: #d1ecf1;
            color: #0c5460;
        }

        .category-content {
            background: #d4edda;
            color: #155724;
        }

        .category-account {
            background: #e2e3e5;
            color: #383d41;
        }

        .category-general {
            background: #f8d7da;
            color: #721c24;
        }

        .status-pending {
            background: #fff3cd;
            color: #856404;
        }

        .status-completed {
            background: #d4edda;
            color: #155724;
        }

        .status-inprogress {
            background: #cce7ff;
            color: #004085;
        }

        .feedback-content {
            color: #666;
            line-height: 1.6;
            margin-bottom: 20px;
            font-size: 14px;
            white-space: pre-line;
        }

        .feedback-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-top: 15px;
            border-top: 1px solid #e9ecef;
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

        .btn-sm {
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
            border: none;
            transition: background-color 0.3s ease;
        }

        .btn-success {
            background-color: #28a745;
            color: white;
        }

        .btn-success:hover {
            background-color: #218838;
        }

        .btn-warning {
            background-color: #ffc107;
            color: #212529;
        }

        .btn-warning:hover {
            background-color: #e0a800;
        }

        .btn-info {
            background-color: #17a2b8;
            color: white;
        }

        .btn-info:hover {
            background-color: #138496;
        }

        .btn-danger {
            background-color: #dc3545;
            color: white;
        }

        .btn-danger:hover {
            background-color: #c82333;
        }

        .no-feedback {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 60px 40px;
            border: 2px dashed #ddd;
            border-radius: 12px;
            margin: 20px 0;
            background: #fafafa;
        }

        /* ===== Filter Section ===== */
        .filter-section {
            background: white;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 20px;
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
        }

        .filter-control {
            width: 100%;
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 6px;
            font-size: 14px;
        }

        .btn-filter {
            background-color: #6c757d;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-filter:hover {
            background-color: #545b62;
        }

        /* Demo Data Section */
        .demo-section {
            background: #f8f9fa;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 20px;
            border-left: 4px solid #17a2b8;
        }

        .demo-section h3 {
            margin-top: 0;
            color: #333;
        }

        .btn-demo {
            background-color: #17a2b8;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
            margin-right: 10px;
            margin-bottom: 10px;
        }

        .btn-demo:hover {
            background-color: #138496;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
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

    <!-- ===== MAIN CONTENT ===== -->
    <div class="management-header">
        <h1>Feedback Management</h1>
        <p>Review and manage user feedback</p>
    </div>

    <!-- ===== DEMO DATA SECTION ===== -->
    <div class="demo-section">
        <h3>Demo Data</h3>
        <p>Add sample feedback data to test the system:</p>
        <asp:Button ID="btnAddDemoData" runat="server" Text="Add Demo Feedback Data" CssClass="btn-demo" OnClick="btnAddDemoData_Click" />
        <asp:Button ID="btnClearDemoData" runat="server" Text="Clear All Feedback" CssClass="btn-demo" OnClick="btnClearDemoData_Click" OnClientClick="return confirm('Are you sure you want to delete ALL feedback? This action cannot be undone.');" />
    </div>

    <!-- ===== STATS CARDS ===== -->
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
        <div class="stat-card technical">
            <div class="stat-number"><asp:Label ID="lblTechnicalFeedback" runat="server" Text="0"></asp:Label></div>
            <div class="stat-label">Technical Issues</div>
        </div>
    </div>

    <!-- ===== FILTER SECTION ===== -->
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
                    <asp:ListItem Value="technical">Technical</asp:ListItem>
                    <asp:ListItem Value="content">Content</asp:ListItem>
                    <asp:ListItem Value="account">Account</asp:ListItem>
                    <asp:ListItem Value="general">General</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" CssClass="btn-filter" OnClick="btnClearFilters_Click" />
            </div>
        </div>
    </div>

    <!-- ===== FEEDBACK SECTION ===== -->
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
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function confirmDelete(feedbackId, title) {
            if (confirm('Are you sure you want to delete the feedback: "' + title + '"?')) {
                __doPostBack('DeleteFeedback', feedbackId);
            }
        }

        function updateStatus(feedbackId, currentStatus) {
            var newStatus = prompt('Enter new status (pending, inprogress, completed):', currentStatus);
            if (newStatus && ['pending', 'inprogress', 'completed'].includes(newStatus.toLowerCase())) {
                __doPostBack('UpdateStatus', feedbackId + '|' + newStatus.toLowerCase());
            } else if (newStatus) {
                alert('Please enter a valid status: pending, inprogress, or completed');
            }
        }
    </script>
</asp:Content>