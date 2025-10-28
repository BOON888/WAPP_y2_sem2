<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="manage_ads.aspx.cs" Inherits="WAPP.manage_ads" %>

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
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin: 30px 0;
        }

        .stat-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            border-left: 4px solid #007bff;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .stat-card-content {
            flex: 1;
        }

        .stat-card h3 {
            margin: 0 0 10px 0;
            font-size: 1.2rem;
            color: #333;
        }

        .stat-card p {
            margin: 0 0 15px 0;
            color: #666;
            line-height: 1.5;
        }

        .stat-card-actions {
            margin-top: 15px;
        }

        .btn-primary {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
            transition: background-color 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .btn-primary:hover {
            background-color: #0056b3;
        }

        /* ===== Announcements Section ===== */
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

        /* ===== Announcements Grid ===== */
        .announcements-container {
            display: flex;
            flex-direction: column;
            gap: 20px;
            margin-top: 20px;
        }

        .announcement-card {
            background: white;
            border: 1px solid #e1e1e1;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            transition: transform 0.2s ease, box-shadow 0.2s ease;
            width: 100%;
            box-sizing: border-box;
        }

        .announcement-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }

        .announcement-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
        }

        .announcement-title {
            font-size: 1.3rem;
            font-weight: 600;
            color: #333;
            margin: 0;
            flex: 1;
        }

        .announcement-type {
            background: #e9ecef;
            color: #495057;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 500;
            margin-left: 15px;
            text-transform: capitalize;
        }

        .announcement-content {
            color: #666;
            line-height: 1.6;
            margin-bottom: 20px;
            font-size: 14px;
        }

        .announcement-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-top: 15px;
            border-top: 1px solid #e9ecef;
            font-size: 13px;
            color: #999;
        }

        .announcement-actions {
            display: flex;
            gap: 10px;
        }

        .btn-danger {
            background-color: #dc3545;
            color: white;
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
            transition: background-color 0.3s ease;
        }

        .btn-danger:hover {
            background-color: #c82333;
        }

        .btn-edit {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
            transition: background-color 0.3s ease;
        }

        .btn-edit:hover {
            background-color: #218838;
        }

        /* ===== Modal Styles ===== */
        .modal-backdrop {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            display: none;
            justify-content: center;
            align-items: center;
            z-index: 1000;
        }

        .modal-content {
            background: white;
            border-radius: 12px;
            padding: 30px;
            width: 90%;
            max-width: 600px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.15);
            max-height: 90vh;
            overflow-y: auto;
        }

        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 1px solid #e9ecef;
        }

        .modal-header h3 {
            margin: 0;
            color: #333;
            font-size: 1.3rem;
        }

        .close-modal {
            background: none;
            border: none;
            font-size: 1.5rem;
            cursor: pointer;
            color: #999;
            line-height: 1;
        }

        .close-modal:hover {
            color: #333;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: 500;
            color: #333;
            font-size: 14px;
        }

        .form-control {
            width: 100%;
            padding: 12px;
            border: 1px solid #ddd;
            border-radius: 6px;
            font-size: 14px;
            box-sizing: border-box;
            transition: border-color 0.3s ease;
        }

        .form-control:focus {
            outline: none;
            border-color: #007bff;
            box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
        }

        textarea.form-control {
            resize: vertical;
            min-height: 120px;
            line-height: 1.5;
        }

        .form-actions {
            display: flex;
            justify-content: flex-end;
            gap: 15px;
            margin-top: 25px;
            padding-top: 20px;
            border-top: 1px solid #e9ecef;
        }

        .btn-secondary {
            background-color: #6c757d;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
            transition: background-color 0.3s ease;
        }

        .btn-secondary:hover {
            background-color: #545b62;
        }

        .no-announcements {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 60px 40px;
            border: 2px dashed #ddd;
            border-radius: 12px;
            margin: 20px 0;
            background: #fafafa;
        }

        .announcement-date {
            font-size: 12px;
            color: #888;
            margin-top: 5px;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .stats-container {
                grid-template-columns: 1fr;
            }
            
            .announcement-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 10px;
            }
            
            .announcement-type {
                margin-left: 0;
            }
            
            .announcement-footer {
                flex-direction: column;
                gap: 10px;
                align-items: flex-start;
            }
            
            .modal-content {
                padding: 20px;
                margin: 20px;
            }
        }

        /* Type-specific colors - Updated to match the three types */
        .type-promotion {
            background: #d4edda !important;
            color: #155724 !important;
        }

        .type-lesson {
            background: #d1ecf1 !important;
            color: #0c5460 !important;
        }

        .type-partner {
            background: #fff3cd !important;
            color: #856404 !important;
        }
    </style>

    <!-- ===== NAVIGATION BAR ===== -->
    <div class="navbar">
        <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/admin_dashboard.aspx">Dashboard</asp:LinkButton>
        <asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/community.aspx">Community</asp:LinkButton>
        <asp:LinkButton ID="LinkButton3" runat="server" PostBackUrl="~/user_management.aspx">User Management</asp:LinkButton>
        <asp:LinkButton ID="LinkButton4" runat="server" PostBackUrl="~/feedback.aspx">Feedback</asp:LinkButton>
        <asp:LinkButton ID="LinkButton5" runat="server" CssClass="active" PostBackUrl="~/manage_ads.aspx">Manage Ads</asp:LinkButton>
    </div>

    <!-- ===== MAIN CONTENT ===== -->
    <div class="management-header">
        <h1>Manage Announcements</h1>
        <p>Control the announcements displayed on the public dashboard</p>
    </div>

    <!-- ===== STATS CARDS ===== -->
    <div class="stats-container">
        <div class="stat-card">
            <div class="stat-card-content">
                <h3>Announcements</h3>
                <p>Create and manage announcements for the homepage carousel</p>
            </div>
            <div class="stat-card-actions">
                <asp:Button ID="btnAddNew" runat="server" Text="Add New Announcement" CssClass="btn-primary" OnClientClick="showModal(); return false;" />
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-card-content">
                <h3>Current Announcements</h3>
                <p><asp:Label ID="lblTotalAnnouncements" runat="server" Text="0"></asp:Label> announcements currently displayed</p>
            </div>
        </div>
    </div>

    <!-- ===== ANNOUNCEMENTS SECTION ===== -->
    <div class="section-header">
        <h2>Current Announcements</h2>
    </div>

    <asp:UpdatePanel ID="upAnnouncements" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="announcements-container">
                <asp:Panel ID="pnlAnnouncements" runat="server">
                    <!-- Announcements will be dynamically added here -->
                </asp:Panel>
                
                <asp:Label ID="lblNoAnnouncements" runat="server" Text="No announcements found." CssClass="no-announcements" Visible="false"></asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ===== ADD ANNOUNCEMENT MODAL ===== -->
    <div id="addAnnouncementModal" class="modal-backdrop">
        <div class="modal-content">
            <div class="modal-header">
                <h3>Add New Announcement</h3>
                <button type="button" class="close-modal" onclick="hideModal()">&times;</button>
            </div>
            
            <asp:UpdatePanel ID="upModal" runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <label for="txtTitle">Title</label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Enter announcement title"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtContent">Content</label>
                        <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" TextMode="MultiLine" 
                            placeholder="Enter announcement content" Rows="6"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="ddlType">Type</label>
                        <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="promotion">Promotion</asp:ListItem>
                            <asp:ListItem Value="lesson">Lesson</asp:ListItem>
                            <asp:ListItem Value="partner">Partner</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="form-actions">
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" OnClientClick="hideModal(); return false;" />
                        <asp:Button ID="btnSave" runat="server" Text="Add Announcement" CssClass="btn-primary" OnClick="btnSave_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <script type="text/javascript">
        function showModal() {
            document.getElementById('addAnnouncementModal').style.display = 'flex';
            // Focus on title field when modal opens
            setTimeout(function () {
                document.getElementById('<%= txtTitle.ClientID %>').focus();
            }, 100);
        }

        function hideModal() {
            document.getElementById('addAnnouncementModal').style.display = 'none';
            // Clear form fields
            document.getElementById('<%= txtTitle.ClientID %>').value = '';
            document.getElementById('<%= txtContent.ClientID %>').value = '';
            document.getElementById('<%= ddlType.ClientID %>').selectedIndex = 0;
        }

        function confirmDelete(announcementId, title) {
            if (confirm('Are you sure you want to delete the announcement: "' + title + '"?')) {
                __doPostBack('DeleteAnnouncement', announcementId);
            }
        }

        // Close modal when clicking outside
        window.onclick = function (event) {
            var modal = document.getElementById('addAnnouncementModal');
            if (event.target === modal) {
                hideModal();
            }
        }

        // Close modal with Escape key
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                hideModal();
            }
        });
    </script>
</asp:Content>