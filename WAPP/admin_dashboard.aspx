<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="admin_dashboard.aspx.cs" Inherits="WAPP.WebForm1" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
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

        .course-management-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin: 30px 0 20px 0;
        }

        .filter-controls {
            display: flex;
            gap: 15px;
            margin-bottom: 20px;
            flex-wrap: wrap;
            align-items: center;
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
            border: 1px solid #ddd;
            border-radius: 6px;
            font-size: 14px;
        }

        .btn-filter {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            cursor: pointer;
        }

        .btn-filter:hover {
            background-color: #0056b3;
        }

        .course-gridview {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

        .course-gridview th {
            background-color: #f8f9fa;
            padding: 15px;
            text-align: left;
            font-weight: bold;
            border: 2px solid #e9ecef;
        }

        .course-gridview td {
            padding: 15px;
            border-bottom: 1px solid #e1e1e1;
            background-color: #fff;
        }

        .course-gridview tr:hover td {
            background-color: #f8f9fa;
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
            background-color: #e8f5e8;
            color: #28a745;
        }

        .premium {
            background-color: #fff0e6;
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
            background-color: #e8f5e8;
            color: #28a745;
        }

        .inactive {
            background-color: #ffe6e6;
            color: #dc3545;
        }

        .course-actions {
            display: flex;
            gap: 8px;
        }

        .btn {
            padding: 6px 12px;
            border: none;
            border-radius: 6px;
            font-size: 12px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            text-align: center;
        }

        .btn-view {
            background-color: #007bff;
            color: white;
        }

        .btn-view:hover {
            background-color: #0056b3;
        }

        .btn-delete {
            background-color: #dc3545;
            color: white;
        }

        .btn-delete:hover {
            background-color: #c82333;
        }

        .pagination {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 15px;
            margin-top: 20px;
            padding: 15px;
        }

        .btn-pagination {
            padding: 8px 16px;
            border: 1px solid #007bff;
            background-color: white;
            color: #007bff;
            border-radius: 6px;
            cursor: pointer;
        }

        .btn-pagination:hover {
            background-color: #007bff;
            color: white;
        }

        .btn-pagination:disabled {
            border-color: #ccc;
            color: #ccc;
            cursor: not-allowed;
        }

        .btn-pagination:disabled:hover {
            background-color: white;
            color: #ccc;
        }

        .page-info {
            font-weight: bold;
            color: #333;
        }

        .no-courses {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 40px;
            width: 100%;
        }

        .message-container {
            margin: 15px 0;
            padding: 12px;
            border-radius: 6px;
            font-weight: bold;
        }

        .message-success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .message-error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
    </style>

    <br />
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
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-filter" OnClick="btnSearch_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn-filter" OnClick="btnClear_Click" />
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
                    <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-view" 
                        CommandName="ViewCourse" CommandArgument='<%# Eval("Id") %>' />
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
</asp:Content>