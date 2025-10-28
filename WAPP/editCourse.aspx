<%@ Page Title="Edit Course" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="EditCourse.aspx.cs"
    Inherits="WAPP.EditCourse" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: #f8f9fc;
        }

        .page-wrap {
            max-width: 1100px;
            margin: 40px auto 60px;
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

        .input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #d1d5db;
            border-radius: 8px;
            margin-bottom: 10px;
            font-size: 15px;
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
    </style>

    <div class="page-wrap">
        <div class="card">
            <div class="title">Edit Course</div>
            <div class="subtitle">Update your existing course information</div>

            <!-- Course Info -->
            <div class="section">
                <h3>Course Information</h3>
                <label>Course Title</label>
                <asp:TextBox ID="txtCourseTitle" runat="server" CssClass="input" />

                <asp:RadioButtonList ID="rblCourseType" runat="server" RepeatDirection="Vertical">
                    <asp:ListItem Value="public">Public (Free for all students)</asp:ListItem>
                    <asp:ListItem Value="private">Private (Requires coins to access)</asp:ListItem>
                </asp:RadioButtonList>

                <div id="priceContainer" runat="server" style="margin-top:10px;">
                    <label>Course Price (10 - 250 coins)</label>
                    <asp:TextBox ID="txtCoursePrice" runat="server" CssClass="input" />
                </div>

                <asp:Label ID="lblCourseInfoMsg" runat="server" ForeColor="Green" />
            </div>

            <!-- Lesson Editor -->
            <div class="section">
                <h3>Edit / Add Lesson</h3>
                <asp:HiddenField ID="hfLessonId" runat="server" />
                <label>Lesson Title</label>
                <asp:TextBox ID="txtLessonTitle" runat="server" CssClass="input" />
                <label>Lesson Content</label>
                <asp:TextBox ID="txtLessonContent" runat="server" CssClass="input" TextMode="MultiLine" Rows="4" />
                <asp:FileUpload ID="fuLessonFile" runat="server" />
                <br />
                <asp:Button ID="btnAddLesson" runat="server" Text="Add Lesson" CssClass="btn-primary" OnClick="btnAddLesson_Click" />
                <asp:Button ID="btnUpdateLesson" runat="server" Text="Update Lesson" CssClass="btn-outline" Visible="false" OnClick="btnUpdateLesson_Click" />
                <asp:Label ID="lblLessonMsg" runat="server" ForeColor="Green" />
            </div>

            <!-- Lesson List -->
            <div class="section">
                <h3>All Lessons</h3>
                <asp:ListView ID="lvLessons" runat="server" OnItemCommand="lvLessons_ItemCommand">
                    <ItemTemplate>
                        <div class="lesson-item">
                            <div>
                                <strong>Lesson <%# Eval("LessonNumber") %>:</strong> <%# Eval("LessonTitle") %>
                            </div>
                            <div>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditLesson" CommandArgument='<%# Eval("Id") %>' CssClass="small-btn">Edit</asp:LinkButton>
                                <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteLesson" CommandArgument='<%# Eval("Id") %>' CssClass="small-btn danger" OnClientClick="return confirm('Delete this lesson?');">Delete</asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div style="color:gray;">No lessons added yet.</div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>

            <!-- Save Course -->
            <div class="section" style="display:flex; justify-content:space-between; align-items:center;">
                <div>
                    <strong>Ready to save your changes?</strong><br />
                    <span style="color:#6b7280;">All updates will be reflected for students.</span>
                </div>
                <div>
                    <asp:Button ID="btnSaveCourse" runat="server" Text="Save Course" CssClass="btn-primary" OnClick="btnSaveCourse_Click" />
                </div>
            </div>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green" />
        </div>
    </div>
</asp:Content>
