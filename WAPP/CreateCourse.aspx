<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs" Inherits="SeaLearner.CreateCourse" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create New Course - Sea Learner</title>
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f8f9fc;
            margin: 0;
            padding: 0;
        }

        /* Top Navigation Bar */
        .top-bar {
            background-color: #3733d1;
            color: white;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 30px;
        }

        .top-bar-left {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .logo {
            background-color: white;
            color: #3733d1;
            font-weight: bold;
            border-radius: 8px;
            width: 30px;
            height: 30px;
            text-align: center;
            line-height: 30px;
        }

        .brand {
            font-size: 20px;
            font-weight: 600;
        }

        .top-bar-right {
            font-size: 14px;
        }

        /* Back link */
        .back-link {
            color: white;
            text-decoration: none;
            font-size: 14px;
            transition: opacity 0.2s;
        }

        .back-link:hover {
            opacity: 0.8;
        }

        /* Page Content */
        .container {
            max-width: 850px;
            margin: 40px auto;
            padding: 20px;
            background-color: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        h1 {
            font-size: 28px;
            font-weight: 700;
            color: #1a1a1a;
        }

        p.subtitle {
            color: #6c757d;
            margin-top: -10px;
            margin-bottom: 30px;
        }

        .form-section {
            background-color: #f9f9fb;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 25px;
        }

        label {
            display: block;
            font-weight: 600;
            margin-bottom: 5px;
        }

        input[type="text"] {
            width: 100%;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 6px;
            font-size: 15px;
        }

        .radio-group {
            margin-top: 10px;
        }

        .create-btn {
            background-color: #3733d1;
            color: white;
            border: none;
            padding: 10px 18px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 600;
            transition: background-color 0.2s;
        }

        .create-btn:hover {
            background-color: #2d29b4;
        }

        .back-to-dashboard {
            color: #3733d1;
            text-decoration: none;
            display: inline-block;
            margin-bottom: 20px;
        }

        .back-to-dashboard:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- Top Bar -->
        <div class="top-bar">
            <div class="top-bar-left">
                <a href="EducatorDashboard.aspx" class="back-link">Back to Dashboard</a>
                <div class="logo">SL</div>
                <span class="brand">Sea Learner</span>
            </div>
            <div class="top-bar-right">
                👤 Educator
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <h1>Create New Course</h1>
            <p class="subtitle">Build an engaging learning experience for your students</p>

            <div class="form-section">
                <label for="txtCourseTitle">Course Title</label>
                <asp:TextBox ID="txtCourseTitle" runat="server" placeholder="Enter course title"></asp:TextBox>

                <div class="radio-group">
                    <label>Course Type</label>
                    <asp:RadioButton ID="rbPublic" runat="server" GroupName="CourseType" Text="Public (Free for all students)" />
                    <br />
                    <asp:RadioButton ID="rbPrivate" runat="server" GroupName="CourseType" Text="Private (Requires coins to access)" />
                </div>
            </div>

            <asp:Button ID="btnCreateCourse" runat="server" CssClass="create-btn" Text="Create Course" OnClick="btnCreateCourse_Click" />

        </div>
    </form>
</body>
</html>
