<%@ Page Title="Educator Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Educator_dashboard.aspx.cs"
    Inherits="WAPP.Educator_dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        html {
            scrollbar-width: none; /* Firefox */
        }

            html::-webkit-scrollbar {
                display: none; /* Chrome, Safari, Edge */
            }

        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
        }

        .dashboard-container {
            background-color: white;
            border-radius: 12px;
            padding: 36px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
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
            border: none;
            padding: 18px 20px;
            flex: 1;
            min-width: 200px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
        }


            .card h3 {
                /* 🚀 Increased font size for title */
                color: #001eff;
                margin: 0 0 4px 0; /* Add bottom margin */
                font-size: 18px; /* BEFORE: 14px */
                font-weight: 700;
                text-align: center; /* Ensure title is centered */
                width: 100%; /* Important for centering */
            }

        .stat-value {
            /* 🚀 Increased font size for number */
            font-size: 32px; /* BEFORE: 20px */
            font-weight: 700;
            color: #111827;
            text-align: center; /* Ensure number is centered */
            width: 100%; /* Important for centering */
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
            border: 1px solid #001eff; /* 🚀 ADD THIS BORDER */
            border-radius: 8px;
            padding: 10px 20px;
            text-decoration: none;
            font-weight: 600;
            display: inline-block;
            transition: all 0.25s ease; /* 🚀 CHANGED TO 'all' for smooth color transition */
        }

            .create-btn:hover {
                transform: translateY(-2px);
                box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
                /* 🚀 ADDED FIX: Change colors on hover */
                background: white;
                color: #001eff;
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
            border: none; /* Changed from 1px solid #001eff */
            border-radius: 6px;
            padding: 10px 20px; /* Increased padding */
            font-size: 16px; /* Increased font size */
            cursor: pointer;
            margin: 5px; /* Added margin */
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            width: 100%; /* Changed to 100% to fill flex container */
            max-width: 200px; /* Added max-width for better control */
            flex-grow: 1; /* Allow buttons to grow in Repeater */
        }

            .btn:hover {
                transform: translateY(-2px);
                box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
                opacity: 0.95;
            }

        .btn-secondary { /* Renaming .btn-back to .btn-secondary for consistency */
            background-color: white;
            color: #001eff;
            border: 1px solid #001eff; /* Added border to secondary button */
        }

        .btn:active {
            transform: translateY(0);
            box-shadow: 0 3px 8px rgba(0, 30, 255, 0.2);
            opacity: 1;
        }

        /* Quick actions as full clickable cards */
        .quick-actions {
            margin-top: 18px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 16px;
        }

        .action-card {
            display: block;
            background: #f9fafb; /* Current background color */
            border: 1px solid #e5e7eb; /* Current light border */
            border-radius: 10px;
            padding: 20px;
            text-align: center;
            text-decoration: none;
            /* 🚀 NEW: Ensure text color can be inherited or explicitly set for links */
            color: #111827; /* Default text color for the card content */
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            min-height: 110px;
            /* 🚀 UPDATED: Use 'all' for smooth color transitions */
            transition: all 0.25s ease;

        }

            .action-card:hover {
                transform: translateY(-4px);
                box-shadow: 0 6px 18px rgba(0,0,0,0.06);
                /* 🚀 ADDED FIX: Apply the Blue-to-White effect */
                background: #001eff; /* Change background to blue */
                color: white; /* Change text color to white for the whole card */
                border-color: #001eff; /* Make the border blue */
            }

        .action-title {
            /* 🚀 FONT FIX: Ensure consistency and better size */
            font-family: 'Segoe UI', sans-serif;
            font-size: 18px; /* Slightly larger title */
            font-weight: 700;
            color: #111827;
            margin-bottom: 6px;
        }

        .action-desc {
            /* 🚀 FONT FIX: Ensure consistency */
            font-family: 'Segoe UI', sans-serif;
            color: #6b7280;
            font-size: 14px; /* Consistent body size */
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

        .action-card:hover .action-title,
        .action-card:hover .action-desc {
            color: white;
        }
    </style>

    <div class="dashboard-container">
        <p class="auto-style1">
            Welcome back,
            <asp:Label ID="lblEducatorName" runat="server" />!
        </p>
        <div class="sub">Manage your courses and inspire students</div>

        <!-- stats -->
        <div class="stats">
            <div class="card">
                <div class="card-left">
                    <div>
                        <h3>Total Students</h3>
                        <div class="stat-value">
                            <asp:Label ID="lblTotalStudents" runat="server" Text="0" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-left">
                    <div>
                        <h3>Courses Created</h3>
                        <div class="stat-value">
                            <asp:Label ID="lblCoursesCreated" runat="server" Text="0" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-left">
                    <div>
                        <h3>Course Completions</h3>
                        <div class="stat-value">
                            <asp:Label ID="lblCourseCompletions" runat="server" Text="0" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Your Courses -->
        <div class="section">
            <div class="heading-row">
                <h2>Your Courses</h2>
                <asp:HyperLink ID="hlCreate" runat="server" NavigateUrl="~/CreateCourse.aspx"
                    CssClass="create-btn">Create New Course</asp:HyperLink>
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
                                <asp:Button ID="btnView" runat="server" Text="View"
                                    CssClass="btn"
                                    CommandName="View" CommandArgument='<%# Eval("Id") %>' />

                                <asp:Button ID="btnEdit" runat="server" Text="Edit"
                                    CssClass="btn btn-secondary"
                                    CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
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
