<%@ Page Title="Course Content" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentCourseContent.aspx.cs" Inherits="WAPP.StudentCourseContent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body { background-color:#f9fafb; font-family:'Segoe UI',sans-serif; }

        .course-container {
            width:99%; max-width:1900px; margin:20px auto;
            background:white; border-radius:10px;
            box-shadow:0 3px 8px rgba(0,0,0,0.1); padding:40px;
        }

        .back-btn {
            background:#001eff; color:white; padding:10px 18px;
            border-radius:6px; border:none; cursor:pointer;
            font-weight:600; margin-bottom:25px; transition:0.3s;
        }
        .back-btn:hover { background:#3246ff; }

        .course-header {
            display:flex; justify-content:space-between; align-items:center;
            border-bottom:2px solid #e5e7eb; padding-bottom:15px; margin-bottom:25px;
        }

        .course-title { font-size:28px; font-weight:700; color:#111827; }
        .course-info { color:#4b5563; font-size:15px; }

        .progress-section { margin:20px 0; }
        .progress-title { font-weight:600; margin-bottom:5px; }
        .progress-bar {
            height:25px; background:#e5e7eb; border-radius:10px; overflow:hidden;
            position:relative;
        }
        .progress-fill {
            background:#001eff; height:100%; color:white; text-align:center;
            font-weight:600; line-height:25px;
        }

        .lesson-list,.quiz-list { margin-top:30px; }
        .section-title { font-size:20px; font-weight:600; color:#001eff; margin-bottom:15px; }

        .lesson-item,.quiz-item {
            border:1px solid #e5e7eb; border-radius:8px; padding:15px;
            margin-bottom:12px; display:flex; justify-content:space-between;
            align-items:center; background:#f9fafb;
        }

        .lesson-item:hover { background:#eef2ff; }
        .lesson-title { font-weight:600; color:#111827; }

        .btn-view {
            background:#001eff; color:white; border:none;
            padding:8px 14px; border-radius:6px; cursor:pointer;
            transition:0.3s;
        }
        .btn-view:hover { background:#3b4bff; }

        .btn-disabled {
            background:#d1d5db; color:#6b7280; cursor:not-allowed;
        }

        .error-message {
            background:#fee2e2; border:1px solid #fca5a5;
            color:#b91c1c; padding:10px; border-radius:8px;
            margin-top:15px; font-weight:500;
        }
    </style>

    <div class="course-container">
        <asp:Button ID="btnBackDashboard" runat="server" CssClass="back-btn"
            Text="← Back to Dashboard" OnClick="btnBackDashboard_Click" />

        <div class="course-header">
            <div>
                <h1 class="course-title"><asp:Label ID="lblCourseTitle" runat="server" Text="Course Title" /></h1>
                <p class="course-info">
                    By <asp:Label ID="lblEducator" runat="server" /> |
                    Type: <asp:Label ID="lblCourseType" runat="server" /> |
                    Status: <asp:Label ID="lblStatus" runat="server" /> |
                    Coins Needed: <asp:Label ID="lblCoin" runat="server" />
                </p>
            </div>
        </div>

        <!-- Progress Bar -->
        <div class="progress-section">
            <p class="progress-title">Course Progress: <asp:Label ID="lblProgressPercent" runat="server" Text="0%" /></p>
            <div class="progress-bar">
                <div id="progressFill" runat="server" class="progress-fill" style="width:0%;">0%</div>
            </div>
        </div>

        <div class="lesson-list">
            <h2 class="section-title">📘 Lessons</h2>
            <asp:Repeater ID="rptLessons" runat="server">
                <ItemTemplate>
                    <div class="lesson-item">
                        <div>
                            <span class="lesson-title">
                                Lesson <%# Eval("LessonNumber") %>: <%# Eval("LessonTitle") %>
                            </span><br />
                            <small>Type: <%# Eval("ContentType") %></small>
                        </div>
                        <a class="btn-view" href='<%# "StudentLesson.aspx?lessonId=" + Eval("Id") %>'>View</a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

       

        <!-- Quizzes -->
        <div class="quiz-list">
            <h2 class="section-title">🧩 Quizzes</h2>
            <asp:Repeater ID="rptQuizzes" runat="server">
                <ItemTemplate>
                    <div class="quiz-item">
                        <div>
                            <span class="lesson-title">Quiz <%# Container.ItemIndex + 1 %>: <%# "Quiz for " + Eval("LessonTitle") %></span><br />
                            <small>Reward: <%# Eval("QuizRewardCoins") %> coins</small>
                        </div>
                        <asp:Button ID="btnQuiz" runat="server" Text='<%# Eval("ButtonText") %>'
                            CssClass='<%# Eval("ButtonClass") %>'
                            CommandName="OpenQuiz" CommandArgument='<%# Eval("Id") %>'
                            Enabled='<%# Convert.ToBoolean(Eval("ButtonEnabled")) %>' />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <asp:Label ID="lblError" runat="server" Visible="false" CssClass="error-message" />
    </div>
</asp:Content>
