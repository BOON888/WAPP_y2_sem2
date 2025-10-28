<%@ Page Title="Educator Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Educator_dashboard.aspx.cs"
    Inherits="WAPP.Educator_dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
        }

        .dashboard-container {
            background-color: white;
            border-radius: 12px;
            padding: 36px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            width: 92%;
            max-width: 1200px;
            margin: 40px auto;
        }

        .welcome {
            color: #111827;
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .sub {
            color: #6b7280;
            margin-bottom: 22px;
        }

        .stats {
            display: flex;
            gap: 18px;
            flex-wrap: wrap;
            margin-bottom: 30px;
        }

        .card {
            background-color: #f9fafb;
            border-radius: 10px;
            border: 1px solid #e5e7eb;
            padding: 18px 20px;
            flex: 1;
            min-width: 200px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .card-left {
            display: flex;
            gap: 12px;
            align-items: center;
        }

        .card h3 {
            color: #001eff;
            margin: 0;
            font-size: 14px;
            font-weight: 700;
        }

        .stat-value {
            font-size: 20px;
            font-weight: 700;
            color: #111827;
        }

        /* Courses */
        .section {
            margin-top: 28px;
        }

            .section .heading-row {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-bottom: 12px;
            }

        .create-btn {
            background: #001eff;
            color: white;
            border-radius: 8px;
            padding: 8px 14px;
            text-decoration: none;
            font-weight: 600;
            display: inline-block;
        }

        .course-list {
            display: flex;
            gap: 18px;
            overflow-x: auto;
            padding-bottom: 12px;
            -webkit-overflow-scrolling: touch;
        }

        .course-card {
            background: #fff;
            border: 1px solid #e6e6e6;
            border-radius: 10px;
            width: 360px;
            padding: 16px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.03);
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .course-top {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 12px;
        }

        .course-title {
            font-size: 16px;
            font-weight: 700;
            color: #111827;
            margin-bottom: 8px;
        }

        .badge-free {
            background: #eef6ff;
            color: #0b57ff;
            border-radius: 12px;
            padding: 4px 8px;
            font-size: 12px;
            font-weight: 600;
        }

        .course-meta {
            color: #6b7280;
            font-size: 13px;
            margin-bottom: 8px;
        }

        .course-actions {
            display: flex;
            gap: 10px;
            margin-top: 10px;
        }

        .btn {
            background-color: #001eff;
            color: white;
            border: 1px solid #001eff;
            border-radius: 6px;
            padding: 8px 12px;
            font-size: 14px;
            cursor: pointer;
        }

            .btn.secondary {
                background-color: white;
                color: #001eff;
                border: 1px solid #d1d5db;
            }

        /* Quick actions as full clickable cards */
        .quick-actions {
            margin-top: 18px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 16px;
        }

        .action-card {
            display: block; /* hyperlinks will fill entire card */
            background: #f9fafb;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            padding: 20px;
            text-align: center;
            text-decoration: none;
            color: inherit;
            min-height: 110px;
            transition: transform .08s ease, box-shadow .08s ease;
        }

            .action-card:hover {
                transform: translateY(-4px);
                box-shadow: 0 6px 18px rgba(0,0,0,0.06);
            }

        .action-title {
            font-size: 15px;
            font-weight: 700;
            color: #111827;
            margin-bottom: 6px;
        }

        .action-desc {
            color: #6b7280;
            font-size: 13px;
        }

        /* scrollbar small style */
        .course-list::-webkit-scrollbar {
            height: 8px;
        }

        .course-list::-webkit-scrollbar-thumb {
            background: #cfd8ff;
            border-radius: 6px;
        }

        .auto-style1 {
            color: #111827;
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 8px;
            text-align: left;
        }
    </style>

    <div class="dashboard-container">
        <p class="auto-style1">Welcome back,
            <asp:Label ID="lblEducatorName" runat="server" />!</p>
        <div class="sub">Manage your courses and inspire students</div>

        <!-- stats -->
        <div class="stats">
            <div class="card">
                <div class="card-left">
                    <div>
                        <h3>Total Students</h3>
                        <div class="stat-value">
                            <asp:Label ID="lblTotalStudents" runat="server" Text="0" /></div>
                    </div>
                </div>
                <div>
                    <!-- small icon placeholder -->
                    <img src="~/assets/icons/users.png" alt="students" style="width: 36px; opacity: 0.9" />
                </div>
            </div>

            <div class="card">
                <div class="card-left">
                    <div>
                        <h3>Courses Created</h3>
                        <div class="stat-value">
                            <asp:Label ID="lblCoursesCreated" runat="server" Text="0" /></div>
                    </div>
                </div>
                <div>
                    <img src="~/assets/icons/book-stack.png" alt="courses" style="width: 36px; opacity: 0.9" />
                </div>
            </div>

            <div class="card">
                <div class="card-left">
                    <div>
                        <h3>Course Completions</h3>
                        <div class="stat-value">
                            <asp:Label ID="lblCourseCompletions" runat="server" Text="0" /></div>
                    </div>
                </div>
                <div>
                    <img src="~/assets/icons/trophy.png" alt="completions" style="width: 36px; opacity: 0.9" />
                </div>
            </div>
        </div>

        <!-- Your Courses -->
        <div class="section">
            <div class="heading-row">
                <h2>Your Courses</h2>
                <asp:HyperLink ID="hlCreate" runat="server" NavigateUrl="~/CreateCourse.aspx" CssClass="create-btn">Create New Course</asp:HyperLink>
            </div>

            <div class="course-list" id="courseScroll">
                <asp:Repeater ID="rptCourses" runat="server" OnItemCommand="rptCourses_ItemCommand">
                    <ItemTemplate>
                        <div class="course-card">
                            <div class="course-top">
                                <div style="flex: 1">
                                    <div class="course-title"><%# Eval("Title") %></div>
                                    <div class="course-meta"><%# Eval("LessonCount") %> lessons</div>
                                    <div class="course-meta">Students Enrolled: <%# Eval("StudentCount") %></div>
                                    <div class="course-meta">Course Type: <%# Eval("CourseType") %></div>
                                </div>
                                <div style="margin-left: 8px; white-space: nowrap;">
                                    <%# (Eval("CourseType") != DBNull.Value && Eval("CourseType").ToString().ToLower()=="free") ?
                                          "<span class='badge-free'>Free</span>" : "" %>
                                </div>
                            </div>

                            <div class="course-actions">
                                <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn" CommandName="View" CommandArgument='<%# Eval("Id") %>' />
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn secondary" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Quick Actions -->
        <div class="section">
            <h2>Quick Actions</h2>
            <div class="quick-actions">
                <asp:HyperLink ID="hlQuickCreate" runat="server" NavigateUrl="~/CreateCourse.aspx" CssClass="action-card">
            <div class="action-title">Create Course</div>
            <div class="action-desc">Start a new learning experience</div>
                </asp:HyperLink>

                <asp:HyperLink ID="hlCommunity" runat="server" NavigateUrl="~/Community.aspx" CssClass="action-card">
            <div class="action-title">Community</div>
            <div class="action-desc">Engage with students</div>
                </asp:HyperLink>

                <asp:HyperLink ID="hlProfile" runat="server" NavigateUrl="~/educatorProfile.aspx" CssClass="action-card">
            <div class="action-title">Profile</div>
            <div class="action-desc">Update your information</div>
                </asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>
