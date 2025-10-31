<%@ Page Title="Course Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="admin_view_course.aspx.cs" Inherits="WAPP.admin_view_course" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ===== General Styles ===== */
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
            margin: 0;
            padding: 0;
        }

        .course-details-container {
            padding: 30px;
            max-width: 1200px;
            margin: 20px auto;
            background: rgba(255, 255, 255, 0.25);
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        /* ===== Header Section ===== */
        .course-header {
            display: flex;
            gap: 30px;
            margin-bottom: 30px;
            padding: 25px;
            background: rgba(255, 255, 255, 0.4);
            border-radius: 12px;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .course-image {
            flex-shrink: 0;
            width: 200px;
            height: 150px;
            border-radius: 8px;
            overflow: hidden;
            background: #f0f0f0;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .course-image img {
            max-width: 100%;
            max-height: 100%;
            object-fit: cover;
        }

        .course-info {
            flex-grow: 1;
        }

        .course-title {
            font-size: 2rem;
            font-weight: bold;
            color: #111827;
            margin-bottom: 10px;
        }

        .course-meta {
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
            margin-bottom: 15px;
        }

        .meta-item {
            display: flex;
            align-items: center;
            gap: 5px;
            color: #666;
            font-size: 0.9rem;
        }

        .badge {
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: bold;
        }

        .badge-type-free {
            background-color: rgba(232, 245, 232, 0.8);
            color: #28a745;
        }

        .badge-type-premium {
            background-color: rgba(255, 240, 230, 0.8);
            color: #ff6b00;
        }

        .badge-status-active {
            background-color: rgba(232, 245, 232, 0.8);
            color: #28a745;
        }

        .badge-status-inactive {
            background-color: rgba(255, 230, 230, 0.8);
            color: #dc3545;
        }

        /* ===== Sections ===== */
        .section {
            margin-bottom: 30px;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 10px;
            padding: 20px;
            backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid rgba(0, 30, 255, 0.1);
        }

        .section-title {
            font-size: 1.4rem;
            font-weight: bold;
            color: #111827;
            margin: 0;
        }

        .count-badge {
            background: #001eff;
            color: white;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: bold;
        }

        /* ===== Lesson Cards ===== */
        .lesson-card {
            background: rgba(255, 255, 255, 0.5);
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 15px;
            border-left: 4px solid #001eff;
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }

        .lesson-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 15px rgba(0, 30, 255, 0.15);
        }

        .lesson-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
        }

        .lesson-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: #111827;
            margin: 0;
            flex: 1;
        }

        .lesson-number {
            color: #001eff;
            font-weight: bold;
            font-size: 0.9rem;
            background: rgba(0, 30, 255, 0.1);
            padding: 4px 12px;
            border-radius: 20px;
        }

        .lesson-content {
            display: grid;
            grid-template-columns: auto 1fr;
            gap: 15px;
            align-items: start;
        }

        .content-type-badge {
            background: rgba(0, 30, 255, 0.1);
            color: #001eff;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 0.9rem;
            font-weight: 600;
            text-align: center;
            min-width: 100px;
        }

        .content-details {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .content-material {
            color: #666;
            font-size: 0.95rem;
            line-height: 1.5;
        }

        .material-preview {
            background: rgba(248, 249, 250, 0.8);
            border-radius: 6px;
            padding: 12px;
            margin-top: 8px;
            border-left: 3px solid #6c757d;
        }

        .material-icon {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            background: rgba(40, 167, 69, 0.1);
            color: #28a745;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 0.8rem;
            font-weight: 600;
        }

        /* ===== Quiz Cards ===== */
        .quiz-card {
            background: rgba(255, 255, 255, 0.5);
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 15px;
            border-left: 4px solid #28a745;
        }

        .quiz-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
        }

        .quiz-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: #111827;
            margin: 0;
        }

        .quiz-reward {
            background: rgba(40, 167, 69, 0.1);
            color: #28a745;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: bold;
        }

        .questions-list {
            margin-top: 15px;
        }

        .question-item {
            background: rgba(248, 249, 250, 0.8);
            border-radius: 6px;
            padding: 15px;
            margin-bottom: 10px;
            border-left: 3px solid #6c757d;
        }

        .question-text {
            font-weight: 600;
            margin-bottom: 10px;
            color: #333;
        }

        .options-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 10px;
            margin-bottom: 10px;
        }

        .option {
            padding: 8px;
            border-radius: 4px;
            background: rgba(255, 255, 255, 0.7);
            font-size: 0.9rem;
        }

        .option.correct {
            background: rgba(40, 167, 69, 0.2);
            border: 1px solid #28a745;
            font-weight: bold;
        }

        .correct-answer {
            color: #28a745;
            font-weight: bold;
            font-size: 0.9rem;
            margin-top: 5px;
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
            transition: all 0.25s ease;
            text-decoration: none;
            display: inline-block;
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
            opacity: 0.95;
            color: white;
            text-decoration: none;
        }

        .btn-back {
            background-color: #6c757d;
            margin-bottom: 20px;
        }

        .btn-back:hover {
            background-color: #5a6268;
        }

        /* ===== Message Styles ===== */
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

        /* ===== Empty States ===== */
        .empty-state {
            text-align: center;
            padding: 40px;
            color: #666;
            font-style: italic;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 8px;
        }

        /* ===== Responsive Design ===== */
        @media (max-width: 768px) {
            .course-header {
                flex-direction: column;
                text-align: center;
            }

            .course-image {
                width: 100%;
                height: 200px;
            }

            .course-meta {
                justify-content: center;
            }

            .options-grid {
                grid-template-columns: 1fr;
            }

            .section-header {
                flex-direction: column;
                gap: 10px;
                align-items: flex-start;
            }

            .lesson-content {
                grid-template-columns: 1fr;
                gap: 10px;
            }

            .content-type-badge {
                justify-self: start;
            }
        }
    </style>

    <div class="course-details-container">
        <!-- Back Button -->
        <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-back" NavigateUrl="~/admin_dashboard.aspx">
            &larr; Back to Dashboard
        </asp:HyperLink>

        <!-- Message Display -->
        <asp:Panel ID="pnlMessage" runat="server" CssClass="message-container" Visible="false">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Course Header Section -->
        <div class="course-header">
            <div class="course-image">
                <asp:Image ID="imgCourse" runat="server" AlternateText="Course Image" />
            </div>
            <div class="course-info">
                <h1 class="course-title">
                    <asp:Label ID="lblCourseTitle" runat="server" Text="Course Title"></asp:Label>
                </h1>
                <div class="course-meta">
                    <div class="meta-item">
                        <strong>Course ID:</strong>
                        <asp:Label ID="lblCourseId" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="meta-item">
                        <strong>Educator ID:</strong>
                        <asp:Label ID="lblEducatorId" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="meta-item">
                        <strong>Coins:</strong>
                        <asp:Label ID="lblCoins" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="course-meta">
                    <asp:Label ID="lblCourseType" runat="server" CssClass="badge badge-type-free" Text="Free"></asp:Label>
                    <asp:Label ID="lblStatus" runat="server" CssClass="badge badge-status-active" Text="Active"></asp:Label>
                </div>
            </div>
        </div>

        <!-- Lessons Section -->
        <div class="section">
            <div class="section-header">
                <h3 class="section-title">Lessons</h3>
                <span class="count-badge">
                    <asp:Label ID="lblLessonsCount" runat="server" Text="0"></asp:Label> Lessons
                </span>
            </div>
            
            <asp:Repeater ID="rptLessons" runat="server" OnItemDataBound="rptLessons_ItemDataBound">
                <ItemTemplate>
                    <div class="lesson-card">
                        <div class="lesson-header">
                            <h4 class="lesson-title">
                                <asp:Label ID="lblLessonTitle" runat="server" Text='<%# Eval("LessonTitle") %>'></asp:Label>
                            </h4>
                            <span class="lesson-number">Lesson #<asp:Label ID="lblLessonNumber" runat="server" Text='<%# Eval("LessonNumber") %>'></asp:Label></span>
                        </div>
                        <div class="lesson-content">
                            <div class="content-type-badge">
                                <asp:Label ID="lblContentType" runat="server" Text='<%# Eval("ContentType") %>'></asp:Label>
                            </div>
                            <div class="content-details">
                                <div class="content-material">
                                    <strong>Content: </strong>
                                    <asp:Label ID="lblContentMaterial" runat="server" Text='<%# GetContentMaterial(Eval("ContentType"), Eval("ContentFile")) %>'></asp:Label>
                                </div>
                                <asp:Panel ID="pnlMaterialPreview" runat="server" CssClass="material-preview" Visible='<%# ShowMaterialPreview(Eval("ContentType"), Eval("ContentFile")) %>'>
                                    <span class="material-icon">
                                        <asp:Label ID="lblMaterialIcon" runat="server" Text='<%# GetMaterialIcon(Eval("ContentType")) %>'></asp:Label>
                                        <asp:Label ID="lblMaterialText" runat="server" Text='<%# GetMaterialText(Eval("ContentType"), Eval("ContentFile")) %>'></asp:Label>
                                    </span>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="pnlNoLessons" runat="server" CssClass="empty-state" Visible="false">
                No lessons found for this course.
            </asp:Panel>
        </div>

        <!-- Quizzes Section -->
        <div class="section">
            <div class="section-header">
                <h3 class="section-title">Quizzes & Questions</h3>
                <span class="count-badge">
                    <asp:Label ID="lblQuizzesCount" runat="server" Text="0"></asp:Label> Quizzes
                </span>
            </div>
            
            <asp:Repeater ID="rptQuizzes" runat="server" OnItemDataBound="rptQuizzes_ItemDataBound">
                <ItemTemplate>
                    <div class="quiz-card">
                        <div class="quiz-header">
                            <h4 class="quiz-title">
                                Quiz for Lesson #<asp:Label ID="lblQuizLessonNumber" runat="server" Text='<%# Eval("LessonNumber") %>'></asp:Label>
                            </h4>
                            <span class="quiz-reward">
                                <asp:Label ID="lblQuizReward" runat="server" Text='<%# Eval("QuizRewardCoins") %>'></asp:Label> Coins
                            </span>
                        </div>
                        <div class="quiz-meta">
                            <strong>Quiz ID:</strong> <asp:Label ID="lblQuizId" runat="server" Text='<%# Eval("QuizId") %>'></asp:Label> |
                            <strong>Total Questions:</strong> <asp:Label ID="lblTotalQuestions" runat="server" Text='<%# Eval("TotalQuestions") %>'></asp:Label> |
                            <strong>Lesson:</strong> <asp:Label ID="lblLessonTitle" runat="server" Text='<%# Eval("LessonTitle") %>'></asp:Label>
                        </div>
                        
                        <div class="questions-list">
                            <asp:Repeater ID="rptQuestions" runat="server">
                                <ItemTemplate>
                                    <div class="question-item">
                                        <div class="question-text">
                                            Q: <asp:Label ID="lblQuestionText" runat="server" Text='<%# Eval("QuestionText") %>'></asp:Label>
                                        </div>
                                        <div class="options-grid">
                                            <div class='option <%# Eval("CorrectAnswer").ToString() == "A" ? "correct" : "" %>'>
                                                A. <asp:Label ID="lblOptionA" runat="server" Text='<%# Eval("OptionA") %>'></asp:Label>
                                            </div>
                                            <div class='option <%# Eval("CorrectAnswer").ToString() == "B" ? "correct" : "" %>'>
                                                B. <asp:Label ID="lblOptionB" runat="server" Text='<%# Eval("OptionB") %>'></asp:Label>
                                            </div>
                                            <div class='option <%# Eval("CorrectAnswer").ToString() == "C" ? "correct" : "" %>'>
                                                C. <asp:Label ID="lblOptionC" runat="server" Text='<%# Eval("OptionC") %>'></asp:Label>
                                            </div>
                                            <div class='option <%# Eval("CorrectAnswer").ToString() == "D" ? "correct" : "" %>'>
                                                D. <asp:Label ID="lblOptionD" runat="server" Text='<%# Eval("OptionD") %>'></asp:Label>
                                            </div>
                                        </div>
                                        <div class="correct-answer">
                                            Correct Answer: <asp:Label ID="lblCorrectAnswer" runat="server" Text='<%# Eval("CorrectAnswer") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="pnlNoQuizzes" runat="server" CssClass="empty-state" Visible="false">
                No quizzes found for this course.
            </asp:Panel>
        </div>
    </div>
</asp:Content>