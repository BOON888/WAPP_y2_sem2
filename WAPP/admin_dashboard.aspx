<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="admin_dashboard.aspx.cs" Inherits="WAPP.WebForm1" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ===== General Page Style ===== */
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
            margin: 0;
            padding: 0;
        }

        h1 {
            color: #111827;
            text-align: center;
            margin-bottom: 10px;
        }

        p {
            color: #666;
            text-align: center;
            margin-bottom: 30px;
        }

        /* ===== Main Container ===== */
        .dashboard-container {
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

        /* ===== Dashboard Cards ===== */
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
            font-size: 1.1rem;
            color: #333;
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
            transition: all 0.25s ease;
        }

        .btn-filter:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 30, 255, 0.25);
        }

        .btn-view {
            background-color: #001eff;
            color: white;
        }

        .btn-delete {
            background-color: white;
            color: red;
            border: 1px solid rgba(255, 0, 0, 0.3);
        }

        .btn-delete:hover {
            background-color: red;
            color: white;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(255, 0, 0, 0.2);
        }

        .btn-pagination {
            padding: 8px 16px;
            border: 1px solid #001eff;
            background-color: white;
            color: #001eff;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.25s ease;
        }

        .btn-pagination:hover {
            background-color: #001eff;
            color: white;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 30, 255, 0.25);
        }

        .btn-pagination:disabled {
            border-color: #ccc;
            color: #ccc;
            cursor: not-allowed;
            transform: none;
            box-shadow: none;
        }

        .btn-pagination:disabled:hover {
            background-color: white;
            color: #ccc;
        }

        /* ===== Course Management Section ===== */
        .course-management-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin: 50px 0 20px 0;
            padding: 20px;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 10px;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .course-management-header h3 {
            margin: 0;
            color: #111827;
        }

        .course-management-header p {
            margin: 5px 0 0 0;
            color: #666;
            text-align: left;
        }

        /* ===== Filter Controls ===== */
        .filter-controls {
            display: flex;
            gap: 15px;
            margin-bottom: 20px;
            flex-wrap: wrap;
            align-items: center;
            padding: 20px;
            background: rgba(255, 255, 255, 0.2);
            border-radius: 10px;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .filter-group {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .filter-label {
            font-weight: bold;
            color: #333;
        }

        .filter-control {
            padding: 8px 12px;
            border: 1px solid rgba(0, 0, 0, 0.1);
            border-radius: 6px;
            font-size: 14px;
            background: rgba(255, 255, 255, 0.8);
        }

        /* ===== Course GridView ===== */
        .course-gridview {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 10px;
            overflow: hidden;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .course-gridview th {
            background-color: rgba(248, 249, 250, 0.7);
            padding: 15px;
            text-align: left;
            font-weight: bold;
            border-bottom: 2px solid rgba(233, 236, 239, 0.5);
            color: #333;
        }

        .course-gridview td {
            padding: 15px;
            border-bottom: 1px solid rgba(225, 225, 225, 0.5);
            background-color: rgba(255, 255, 255, 0.5);
        }

        .course-gridview tr:hover td {
            background-color: rgba(248, 249, 250, 0.8);
        }

        .course-id {
            font-weight: 600;
            color: #333;
        }

        .course-title {
            font-weight: 500;
            color: #333;
        }

        .course-type {
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: bold;
            text-align: center;
            display: inline-block;
        }

        .free {
            background-color: rgba(232, 245, 232, 0.8);
            color: #28a745;
        }

        .premium {
            background-color: rgba(255, 240, 230, 0.8);
            color: #ff6b00;
        }

        .course-status {
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 12px;
            text-align: center;
            display: inline-block;
        }

        .active {
            background-color: rgba(232, 245, 232, 0.8);
            color: #28a745;
        }

        .inactive {
            background-color: rgba(255, 230, 230, 0.8);
            color: #dc3545;
        }

        .course-actions {
            display: flex;
            gap: 8px;
        }

        /* ===== Pagination ===== */
        .pagination {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 15px;
            margin-top: 20px;
            padding: 20px;
            background: rgba(255, 255, 255, 0.2);
            border-radius: 10px;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .page-info {
            font-weight: bold;
            color: #333;
        }

        /* ===== Message and No Courses ===== */
        .no-courses {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 40px;
            width: 100%;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 10px;
        }

        .message-container {
            margin: 15px 0;
            padding: 15px;
            border-radius: 8px;
            font-weight: bold;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
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
        @media (max-width: 1200px) {
            .card-container {
                grid-template-columns: repeat(2, 1fr) !important;
            }
        }

        @media (max-width: 768px) {
            .card-container {
                grid-template-columns: 1fr !important;
            }
            
            .filter-controls {
                flex-direction: column;
                align-items: stretch;
            }
            
            .filter-group {
                justify-content: space-between;
            }
            
            .course-management-header {
                flex-direction: column;
                gap: 15px;
                text-align: center;
            }
            
            .course-management-header p {
                text-align: center;
            }
        }
    </style>

    <div class="dashboard-container">
        <h1>Admin Dashboard</h1>
        <p>Manage the Sea Learner Platform</p>

        <!-- Message Display Area -->
        <asp:Panel ID="pnlMessage" runat="server" CssClass="message-container" Visible="false">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- ===== DASHBOARD CARDS ===== -->
        <div class="card-container">
            <asp:Panel ID="Panel1" runat="server" CssClass="card">
                <div class="card-content">
                    <h4>Total Students</h4>
                    <p><asp:Label ID="lblTotalStudents" runat="server" Text="0"></asp:Label></p>
                </div>
                <div class="card-icon">
                    <asp:Image ID="imgStudents" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
                </div>
            </asp:Panel>

            <asp:Panel ID="Panel2" runat="server" CssClass="card">
                <div class="card-content">
                    <h4>Total Educators</h4>
                    <p><asp:Label ID="lblTotalTeachers" runat="server" Text="0"></asp:Label></p>
                </div>
                <div class="card-icon">
                    <asp:Image ID="imgteacher" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
                </div>
            </asp:Panel>

            <asp:Panel ID="Panel3" runat="server" CssClass="card">
                <div class="card-content">
                    <h4>Total Courses</h4>
                    <p><asp:Label ID="lblTotalCourses" runat="server" Text="0"></asp:Label></p>
                </div>
                <div class="card-icon">
                    <asp:Image ID="imgcourse" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
                </div>
            </asp:Panel>

            <asp:Panel ID="Panel4" runat="server" CssClass="card">
                <div class="card-content">
                    <h4>Pending Feedback</h4>
                    <p><asp:Label ID="lblPendingFeedback" runat="server" Text="0"></asp:Label></p>
                </div>
                <div class="card-icon">
                    <asp:Image ID="imgfeedback" runat="server" ImageUrl="~/Image/ben.jpg" Width="40px" />
                </div>
            </asp:Panel>
        </div>

        <!-- ===== Course Management ===== -->
        <div class="course-management-header">
            <div>
                <h3 style="margin: 0;">Course Management</h3>
                <p style="margin: 5px 0 0 0;">View and manage all courses on the platform</p>
            </div>
        </div>

        <!-- Filter Controls -->
        <div class="filter-controls">
            <div class="filter-group">
                <span class="filter-label">Filter by:</span>
                <asp:DropDownList ID="ddlFilterType" runat="server" CssClass="filter-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilterType_SelectedIndexChanged">
                    <asp:ListItem Value="All" Text="All Courses"></asp:ListItem>
                    <asp:ListItem Value="CourseID" Text="Course ID"></asp:ListItem>
                    <asp:ListItem Value="EducatorID" Text="Educator ID"></asp:ListItem>
                    <asp:ListItem Value="CourseType" Text="Course Type"></asp:ListItem>
                    <asp:ListItem Value="Status" Text="Status"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="filter-group">
                <asp:TextBox ID="txtSearchValue" runat="server" CssClass="filter-control" placeholder="Enter search value"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-filter" OnClick="btnSearch_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-filter" OnClick="btnClear_Click" />
            </div>
        </div>

        <!-- Course GridView -->
        <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" 
            CssClass="course-gridview" OnRowCommand="gvCourses_RowCommand"
            OnRowDataBound="gvCourses_RowDataBound" ShowHeader="True"
            EmptyDataText="No courses found." EmptyDataRowStyle-CssClass="no-courses">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="course-id" />
                <asp:BoundField DataField="Title" HeaderText="Course Title" ItemStyle-CssClass="course-title" />
                <asp:BoundField DataField="EducatorId" HeaderText="Educator ID" />
                <asp:TemplateField HeaderText="Type">
                    <ItemTemplate>
                        <span class='course-type <%# GetCourseTypeClass(Eval("CourseType").ToString()) %>'>
                            <%# Eval("CourseType") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='course-status <%# GetStatusClass(Eval("Status").ToString()) %>'>
                            <%# Eval("Status") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="course-actions">
                    <ItemTemplate>
                        <asp:Button ID="btnView" runat="server" Text="View Content" CssClass="btn btn-view" 
                            CommandName="ViewCourseContent" CommandArgument='<%# Eval("Id") %>' />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-delete" 
                            CommandName="DeleteCourse" CommandArgument='<%# Eval("Id") + "|" + Eval("Title") %>'
                            OnClientClick='<%# "return confirm(\"Are you sure you want to delete the course \\\"" + Eval("Title") + "\\\" (ID: " + Eval("Id") + ")? This action cannot be undone.\");" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Pagination -->
        <div class="pagination">
            <asp:Button ID="btnPrev" runat="server" Text="Previous" CssClass="btn-pagination" OnClick="btnPrev_Click" />
            <span class="page-info">
                Page <asp:Label ID="lblCurrentPage" runat="server" Text="1"></asp:Label> of 
                <asp:Label ID="lblTotalPages" runat="server" Text="1"></asp:Label>
            </span>
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn-pagination" OnClick="btnNext_Click" />
        </div>
    </div>
</asp:Content>