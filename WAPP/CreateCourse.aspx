<%@ Page Title="Create New Course" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs"
    Inherits="WAPP.CreateCourse" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: #f8f9fc;
        }

        .top-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin: 20px;
        }

        /* Left panel */
        .left-panel {
            background-color: #312EFA; /* Blue color */
            color: white;
            padding: 8px 16px;
            border-radius: 8px;
            font-weight: 600;
        }

        /* Right panel */
        .right-panel {
            background-color: #312EFA; /* Blue color */
            color: white;
            padding: 8px 16px;
            border-radius: 8px;
            font-weight: 600;
        }

        /* Link style */
        .back-link {
            color: white;
            text-decoration: none;
        }

            .back-link:hover {
                opacity: 0.8;
            }

        .top-left {
            display: flex;
            gap: 12px;
            align-items: center;
        }

        .logo {
            background: #fff;
            color: #3733d1;
            width: 34px;
            height: 34px;
            border-radius: 8px;
            text-align: center;
            line-height: 34px;
            font-weight: 700;
        }

        .brand {
            font-weight: 600;
            color: #fff;
        }

        .page-wrap {
            max-width: 1100px;
            margin: 0 auto 60px;
        }

        .card {
            background: #fff;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.06);
        }

        .title {
            font-size: 26px;
            font-weight: 700;
            margin: 0 0 6px;
        }

        .subtitle {
            color: #6b7280;
            margin: 0 0 20px;
        }

        .section {
            background: #f9fafb;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            padding: 16px;
            margin-bottom: 16px;
        }

            .section h3 {
                margin: 0 0 6px;
                font-size: 16px;
                font-weight: 700;
            }

        .muted {
            color: #6b7280;
            margin-bottom: 12px;
        }

        .input {
            width: 100%;
            max-width: 100%;
            padding: 10px 12px;
            border: 1px solid #d1d5db;
            border-radius: 8px;
            margin-bottom: 10px;
            box-sizing: border-box;
            font-size: 15px;
            resize: vertical; /* allow resizing if multiline */
        }

        .btn-primary {
            background: #001eff;
            color: #fff;
            border: none;
            padding: 8px 14px;
            border-radius: 8px;
            cursor: pointer;
        }

        .btn-outline {
            background: #fff;
            color: #001eff;
            border: 1px solid #001eff;
            padding: 8px 12px;
            border-radius: 8px;
            cursor: pointer;
        }

        .lesson-item {
            background: #fff;
            border: 1px solid #e6e6e6;
            border-radius: 8px;
            padding: 12px;
            margin-bottom: 8px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .lesson-meta {
            color: #6b7280;
            font-size: 13px;
        }

        .small-btn {
            background: transparent;
            border: 1px solid #d1d5db;
            padding: 6px 10px;
            border-radius: 6px;
            cursor: pointer;
        }

            .small-btn.danger {
                border-color: #ff6666;
                color: #ff3333;
            }

        .question-item {
            background: #fff;
            border: 1px dashed #e6e6e6;
            padding: 8px;
            border-radius: 6px;
            margin-bottom: 6px;
        }

        .right {
            text-align: right;
        }

        .msg {
            color: green;
            font-weight: 700;
            margin-top: 8px;
        }
    </style>

    <!-- Top bar -->
    <div class="top-header">
        <div class="left-panel">
            <a href="Educator_dashboard.aspx" class="back-link">Back to Dashboard</a>
        </div>
        <div class="right-panel">
            <asp:Label ID="lblEducatorName" runat="server" Text="Educator" />
        </div>
    </div>

    <div class="page-wrap">
        <div class="card">
            <div class="title">Create New Course</div>
            <div class="subtitle">Build an engaging learning experience for your students</div>

            <!-- Course Info -->
            <div class="section">
                <h3>Course Information</h3>
                <div class="muted">Basic details about your course</div>

                <h3>Course Title</h3>
                <asp:TextBox ID="txtCourseTitle" runat="server" CssClass="input" placeholder="Enter course title"></asp:TextBox>

                <asp:RadioButtonList ID="rblCourseType" runat="server" CssClass="muted" RepeatDirection="Vertical">
                    <asp:ListItem Value="public" Selected="True">Public (Free for all students)</asp:ListItem>
                    <asp:ListItem Value="private">Private (Requires coins to access)</asp:ListItem>
                </asp:RadioButtonList>

                <asp:Button ID="btnSaveCourseInfo" runat="server" Text="Save Course Info" CssClass="btn-outline" OnClick="btnSaveCourseInfo_Click" />
                <asp:Label ID="lblCourseInfoMsg" runat="server" />
            </div>

            <!-- Add / Edit Lesson -->
            <div class="section">
                <h3>Add New Lesson</h3>
                <div class="muted">Create engaging lesson content for your students</div>

                <asp:HiddenField ID="hfEditingLessonId" runat="server" />

                <label>Lesson Title</label>
                <asp:TextBox ID="txtNewLessonTitle" runat="server" CssClass="input" placeholder="Enter lesson title" Width="100%" />

                <label>Lesson Content (or upload file)</label>
                <asp:TextBox ID="txtNewLessonContent" runat="server" CssClass="input" TextMode="MultiLine" Rows="4" placeholder="Enter lesson content..." Width="100%"></asp:TextBox>
                <asp:FileUpload ID="fuLessonFile" runat="server" />

                <div style="margin-top: 10px;">
                    <asp:Button ID="btnOpenQuiz" runat="server" Text="Add Quiz" CssClass="btn-outline" OnClick="btnOpenQuiz_Click" />
                    <asp:Button ID="btnAddLesson" runat="server" Text="Add Lesson" CssClass="btn-primary" OnClick="btnAddLesson_Click" />
                </div>

                <asp:Label ID="lblAddLessonMsg" runat="server" />

                <!-- Quiz panel -->
                <asp:Panel ID="pnlQuiz" runat="server" Visible="false" Style="margin-top: 12px; border-top: 1px solid #e6e6e6; padding-top: 12px;">
                    <div style="font-weight: 700; margin-bottom: 6px;">Quiz Setup</div>
                    <div class="muted">Create quiz questions for this lesson</div>

                    <label>Coin Reward</label>
                    <asp:TextBox ID="txtQuizCoins" runat="server" CssClass="input" Text="10" />

                    <div style="margin-top: 8px;">
                        <label>Question</label>
                        <asp:TextBox ID="txtQuestionText" runat="server" CssClass="input" placeholder="Enter question text" />
                        <asp:TextBox ID="txtOptA" runat="server" CssClass="input" placeholder="Option A" />
                        <asp:TextBox ID="txtOptB" runat="server" CssClass="input" placeholder="Option B" />
                        <asp:TextBox ID="txtOptC" runat="server" CssClass="input" placeholder="Option C" />
                        <asp:TextBox ID="txtOptD" runat="server" CssClass="input" placeholder="Option D" />

                        <label style="margin-top: 6px;">Correct Answer</label>
                        <asp:RadioButtonList ID="rblCorrect" runat="server">
                            <asp:ListItem Value="A">A</asp:ListItem>
                            <asp:ListItem Value="B">B</asp:ListItem>
                            <asp:ListItem Value="C">C</asp:ListItem>
                            <asp:ListItem Value="D">D</asp:ListItem>
                        </asp:RadioButtonList>

                        <div style="margin-top: 8px;">
                            <asp:Button ID="btnAddQuestion" runat="server" Text="Add Question" CssClass="btn-outline" OnClick="btnAddQuestion_Click" />
                            &nbsp;
                            <asp:Button ID="btnDoneQuiz" runat="server" Text="Done Quiz" CssClass="btn-primary" OnClick="btnDoneQuiz_Click" />
                        </div>

                        <div style="margin-top: 10px;">
                            <asp:Repeater ID="rptTempQuestions" runat="server">
                                <ItemTemplate>
                                    <div class="question-item">
                                        <div><strong>Q:</strong> <%# Eval("QuestionText") %></div>
                                        <div><small>A: <%# Eval("OptionA") %> | B: <%# Eval("OptionB") %> | C: <%# Eval("OptionC") %> | D: <%# Eval("OptionD") %></small></div>
                                        <div><small>Answer: <%# Eval("CorrectAnswer") %></small></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </asp:Panel>
            </div>

            <!-- Course Lessons -->
            <div class="section">
                <h3>Course Lessons</h3>
                <div class="muted">Add lessons to your course</div>

                <asp:ListView ID="lvLessons" runat="server">
                    <ItemTemplate>
                        <div class="lesson-item">
                            <div>
                                <div style="font-weight: 700">Lesson <%# Eval("LessonNumber") %>: <%# Eval("LessonTitle") %></div>
                                <div class="lesson-meta"><%# Convert.ToBoolean(Eval("HasQuiz")) ? "Has quiz" : "No quiz" %></div>
                            </div>
                            <div>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditLesson" CommandArgument='<%# Eval("Id") %>' CssClass="small-btn">Edit</asp:LinkButton>
                                <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteLesson" CommandArgument='<%# Eval("Id") %>' CssClass="small-btn danger">Delete</asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div style="color: #6b7280;">No lessons added yet. Create your first lesson below.</div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>

            <!-- Publish -->
            <div class="section" style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <div style="font-weight: 700;">Ready to publish?</div>
                    <div class="muted">Make sure you've added all lessons and quizzes before creating the course</div>
                </div>
                <div class="right">
                    <asp:Button ID="btnCreateCourse" runat="server" Text="Create Course" CssClass="btn-primary" OnClick="btnCreateCourse_Click" />
                </div>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="msg" />
        </div>
    </div>
</asp:Content>
