<%@ Page Title="Create Course" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="CreateCourse.aspx.cs" Inherits="WAPP.createcourse" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        html {
            scrollbar-width: none;
        }

            html::-webkit-scrollbar {
                display: none;
            }

        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #eaf0ff, #ffffff);
        }

        h1, h2, h3 {
            color: #111827;
        }

        .btn {
            background-color: #001eff;
            color: white;
            border: 1px solid #001eff;
            border-radius: 6px;
            padding: 10px 20px;
            font-size: 16px;
            cursor: pointer;
            margin: 5px;
            font-weight: bold;
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
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

        .main-box {
            max-width: 100%;
            margin: auto;
            padding: 30px;
            background: rgba(255, 255, 255, 0.25);
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .form-section {
            margin-bottom: 30px;
        }

            .form-section h3 {
                margin-bottom: 15px;
                border-bottom: 2px solid #001eff;
                display: inline-block;
                padding-bottom: 5px;
            }

            .form-section h4 {
                color: #333;
                margin-top: 20px;
                margin-bottom: 10px;
            }

        .form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
        }

        .form-grid-full {
            display: grid;
            grid-template-columns: 1fr;
            gap: 15px;
        }

        .form-grid-quiz {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 15px;
        }

            .form-grid label, .form-grid-full label, .form-grid-quiz label {
                font-weight: bold;
                display: block;
                margin-bottom: 5px;
                color: #111827;
            }

            .form-grid input[type=text], .form-grid input[type=number], .form-grid select, textarea,
            .form-grid-full input[type=text], .form-grid-full input[type=number], .form-grid-full select,
            .form-grid-quiz input[type=text], .form-grid-quiz input[type=number], .form-grid-quiz select {
                width: 100%;
                padding: 10px;
                border-radius: 6px;
                border: 1px solid rgba(0, 30, 255, 0.3);
                box-sizing: border-box;
                background: rgba(255, 255, 255, 0.5);
                color: #111827;
                font-family: 'Segoe UI', sans-serif;
            }

        input[type="file"] {
            color: transparent;
        }

            input[type="file"]::file-selector-button {
                background-color: white;
                color: #001eff;
                border: 1px solid #001eff;
                border-radius: 6px;
                padding: 10px 20px;
                font-size: 16px;
                cursor: pointer;
                margin: 5px;
                font-weight: bold;
                transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            }

            input[type="file"]::-webkit-file-upload-button {
                background-color: white;
                color: #001eff;
                border: 1px solid #001eff;
                border-radius: 6px;
                padding: 10px 20px;
                font-size: 16px;
                cursor: pointer;
                margin: 5px;
                font-weight: bold;
                transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            }

                input[type="file"]::file-selector-button:hover,
                input[type="file"]::-webkit-file-upload-button:hover {
                    transform: translateY(-2px);
                    background-color: #001eff;
                    color: white;
                }

        ::placeholder {
            color: #555;
            opacity: 1;
        }

        :-ms-input-placeholder {
            color: #555;
        }

        ::-ms-input-placeholder {
            color: #555;
        }

        .btn-primary, .btn-secondary {
            background-color: #001eff;
            color: white;
            border-color: #001eff;
        }

            .btn-primary:hover, .btn-secondary:hover {
                background-color: white;
                color: #001eff;
                border-color: #001eff;
            }

        .btn-edit {
            background-color: white;
            color: #bdce27;
            border-color: #bdce27;
        }

            .btn-edit:hover {
                background-color: #bdce27;
                color: white;
                border-color: #bdce27;
            }

        .btn-delete {
            background-color: white;
            color: #ff0000;
            border-color: #ff0000;
        }

            .btn-delete:hover {
                background-color: #ff0000;
                color: white;
                border-color: #ff0000;
            }

        .btn-cancel {
            background-color: white;
            color: #001eff;
            border-color: #001eff;
        }

            .btn-cancel:hover {
                background-color: #001eff;
                color: white;
                border-color: #001eff;
            }

        .lesson-list, .quiz-list {
            margin-top: 30px;
            overflow-x: auto;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
            background: transparent;
        }

            .gridview th, .gridview td {
                border: 1px solid rgba(0, 30, 255, 0.2);
                padding: 12px;
                text-align: left;
                vertical-align: top;
            }

            .gridview th {
                background: rgba(0, 30, 255, 0.05);
                font-weight: bold;
            }

            .gridview tr:nth-child(even) {
                background: transparent;
            }

            .gridview tr:hover {
                background: rgba(0, 30, 255, 0.03);
            }

        .status-label {
            font-weight: bold;
            margin-top: 15px;
            display: block;
            padding: 10px;
            border-radius: 6px;
            background: rgba(255, 255, 255, 0.2);
        }

        .action-buttons {
            display: flex;
            gap: 5px;
            flex-wrap: wrap;
        }

        .file-upload-note {
            font-size: 12px;
            color: #333;
            margin-top: 5px;
        }

        .required-field::after {
            content: " *";
            color: red;
        }
    </style>



    <div class="main-box">
        <!-- ================== COURSE INFORMATION ================== -->
        <div class="form-section">
            <h3>Course Information</h3>
            <div class="form-grid">
                <div>
                    <label class="required-field">Course Title</label>
                    <asp:TextBox ID="txtCourseTitle" runat="server" placeholder="Enter course title"></asp:TextBox>
                </div>
                <div>
                    <label class="required-field">Course Type</label>
                    <asp:DropDownList ID="ddlCourseType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCourseType_SelectedIndexChanged">
                        <asp:ListItem Text="Select Type" Value=""></asp:ListItem>
                        <asp:ListItem Text="Public" Value="Public"></asp:ListItem>
                        <asp:ListItem Text="Private" Value="Private"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div id="coinDiv" runat="server" visible="false">
                    <label class="required-field">Course Coin</label>
                    <asp:TextBox ID="txtCourseCoin" runat="server" placeholder="Enter coin (only for private)" TextMode="Number" min="0"></asp:TextBox>
                </div>
            </div>
        </div>

        <hr />

        <!-- ================== LESSON INFORMATION ================== -->
        <div class="form-section">
            <h3>Add Lesson</h3>
            <div class="form-grid">
                <div>
                    <label class="required-field">Lesson Title</label>
                    <asp:TextBox ID="txtLessonTitle" runat="server" placeholder="Lesson title"></asp:TextBox>
                </div>

                <div>
                    <label>File Upload</label>
                    <asp:FileUpload ID="fuContentFile" runat="server" />
                    <div class="file-upload-note">Maximum file size: 50MB.</div>
                </div>

                <div style="grid-column: span 2;">
                    <label>Text Content (Optional)</label>
                    <asp:TextBox ID="txtTextContent" runat="server" TextMode="MultiLine" Rows="4"
                        placeholder="Enter text content here"></asp:TextBox>
                </div>
            </div>
            <br />
            <%-- *** CssClass ADDED *** --%>
            <asp:Button ID="btnAddLesson" runat="server" Text="Add Lesson" CssClass="btn btn-primary" OnClick="btnAddLesson_Click" />
        </div>

        <!-- ================== LESSON LIST ================== -->
        <div class="lesson-list">
            <h3>Lesson List (<asp:Label ID="lblLessonCount" runat="server" Text="0"></asp:Label>
                lessons)</h3>
            <asp:GridView ID="gvLessons" runat="server" CssClass="gridview" AutoGenerateColumns="False"
                OnRowEditing="gvLessons_RowEditing" OnRowUpdating="gvLessons_RowUpdating"
                OnRowCancelingEdit="gvLessons_RowCancelingEdit" OnRowDeleting="gvLessons_RowDeleting"
                OnRowDataBound="gvLessons_RowDataBound" OnRowCommand="gvLessons_RowCommand"
                GridLines="None">
                <Columns>
                    <asp:BoundField DataField="LessonNumber" HeaderText="No." ReadOnly="true" ItemStyle-Width="50px" />

                    <asp:TemplateField HeaderText="Lesson Title">
                        <ItemTemplate>
                            <asp:Label ID="lblLessonTitle" runat="server" Text='<%# Eval("LessonTitle") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditLessonTitle" runat="server" Text='<%# Bind("LessonTitle") %>' Width="95%" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Content">
                        <ItemTemplate>
                            <asp:Label ID="lblContentFile" runat="server" Text='<%# Eval("ContentFile") != null && ((string)Eval("ContentFile")).Length > 50 ? ((string)Eval("ContentFile")).Substring(0, 50) + "..." : Eval("ContentFile") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditContentFile" runat="server" Text='<%# Bind("ContentFile") %>' TextMode="MultiLine" Rows="3" Width="95%" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="File Path">
                        <ItemTemplate>
                            <asp:Label ID="lblContentFilePath" runat="server" Text='<%# Eval("ContentFilePath") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:FileUpload ID="fuEditContentFile" runat="server" />
                            <div class="file-upload-note">
                                Current:
                                <asp:Label ID="lblCurrentFile" runat="server" Text='<%# Eval("ContentFilePath") %>'></asp:Label>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Quiz Status" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="lblQuizStatus" runat="server" Text="No"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Lesson Actions" ItemStyle-Width="200px">
                        <ItemTemplate>
                            <div class="action-buttons">
                                <%-- *** CssClass ADDED *** --%>
                                <asp:Button ID="btnEdit" runat="server" CommandName="Edit" Text="Edit Lesson" CssClass="btn btn-edit" />
                                <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="Delete Lesson" CssClass="btn btn-delete"
                                    OnClientClick="return confirm('Are you sure you want to delete this lesson and any associated quiz?');" />
                            </div>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <div class="action-buttons">
                                <%-- *** CssClass ADDED *** --%>
                                <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CssClass="btn btn-secondary" />
                                <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" CssClass="btn btn-cancel" />
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Quiz Actions" ItemStyle-Width="200px">
                        <ItemTemplate>
                            <div class="action-buttons">
                                <%-- *** CssClass ADDED *** --%>
                                <asp:Button ID="btnEditQuiz" runat="server" CommandName="EditQuiz" Text='<%# (Eval("LessonQuiz") != null) ? "Edit Quiz" : "Add Quiz" %>'
                                    CommandArgument='<%# Eval("LessonNumber") %>' CssClass="btn btn-primary" />
                                <asp:Button ID="btnDeleteQuiz" runat="server" CommandName="DeleteQuiz" Text="Delete Quiz"
                                    CommandArgument='<%# Eval("LessonNumber") %>' CssClass="btn btn-delete"
                                    Visible='<%# Eval("LessonQuiz") != null %>'
                                    OnClientClick="return confirm('Are you sure you want to delete the quiz for this lesson?');" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
                <EmptyDataTemplate>
                    <div style="text-align: center; padding: 20px; color: #666;">
                        No lessons added yet. Please add lessons using the form above.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>

        <hr />

        <!-- ================== NEW QUIZ SECTION ================== -->
        <div class="form-section">
            <h3>Create / Edit Quiz</h3>
            <div class="form-grid">
                <div>
                    <label class="required-field">Select Lesson</label>
                    <asp:DropDownList ID="ddlQuizLesson" runat="server"></asp:DropDownList>
                </div>
                <div>
                    <label>Quiz Reward Coins (Optional)</label>
                    <asp:TextBox ID="txtQuizRewardCoins" runat="server" TextMode="Number" min="0" placeholder="e.g. 50"></asp:TextBox>
                </div>
            </div>

            <h4>Add Questions</h4>
            <div class="form-grid-full">
                <div>
                    <label class="required-field">Question Text</label>
                    <asp:TextBox ID="txtQuestionText" runat="server" TextMode="MultiLine" Rows="3" placeholder="Enter the question"></asp:TextBox>
                </div>
            </div>
            <br />
            <div class="form-grid-quiz">
                <div>
                    <label class="required-field">Option A</label>
                    <asp:TextBox ID="txtOptionA" runat="server" placeholder="Option A"></asp:TextBox>
                </div>
                <div>
                    <label class="required-field">Option B</label>
                    <asp:TextBox ID="txtOptionB" runat="server" placeholder="Option B"></asp:TextBox>
                </div>
                <div>
                    <label>Option C (Optional)</label>
                    <asp:TextBox ID="txtOptionC" runat="server" placeholder="Option C"></asp:TextBox>
                </div>
                <div>
                    <label>Option D (Optional)</label>
                    <asp:TextBox ID="txtOptionD" runat="server" placeholder="Option D"></asp:TextBox>
                </div>
            </div>
            <br />
            <div class="form-grid-quiz">
                <label class="required-field">Correct Answer</label>
                <asp:DropDownList ID="ddlCorrectAnswer" runat="server">
                    <asp:ListItem Text="Select Answer" Value=""></asp:ListItem>
                    <asp:ListItem Text="Option A" Value="A"></asp:ListItem>
                    <asp:ListItem Text="Option B" Value="B"></asp:ListItem>
                    <asp:ListItem Text="Option C" Value="C"></asp:ListItem>
                    <asp:ListItem Text="Option D" Value="D"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <br />
            <%-- *** CssClass ADDED *** --%>
            <asp:Button ID="btnAddQuestion" runat="server" Text="Add Question" CssClass="btn btn-primary" OnClick="btnAddQuestion_Click" />

            <!-- Temporary Question List -->
            <div class="quiz-list">
                <h4>Temporary Question List</h4>
                <asp:Label ID="lblQuizStatus" runat="server" CssClass="status-label" EnableViewState="false"></asp:Label>
                <asp:GridView ID="gvTempQuestions" runat="server" CssClass="gridview" AutoGenerateColumns="False"
                    OnRowEditing="gvTempQuestions_RowEditing" OnRowUpdating="gvTempQuestions_RowUpdating"
                    OnRowCancelingEdit="gvTempQuestions_RowCancelingEdit" OnRowDeleting="gvTempQuestions_RowDeleting"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="QuestionNumber" HeaderText="No." ReadOnly="true" ItemStyle-Width="50px" />

                        <asp:TemplateField HeaderText="Question">
                            <ItemTemplate>
                                <asp:Label ID="lblQText" runat="server" Text='<%# Eval("QuestionText") %>'></asp:Label></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditQuestionText" runat="server" Text='<%# Bind("QuestionText") %>' TextMode="MultiLine" Rows="3" Width="95%"></asp:TextBox></EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="A">
                            <ItemTemplate>
                                <asp:Label ID="lblOptA" runat="server" Text='<%# Eval("OptionA") %>'></asp:Label></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditOptionA" runat="server" Text='<%# Bind("OptionA") %>' Width="90%"></asp:TextBox></EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="B">
                            <ItemTemplate>
                                <asp:Label ID="lblOptB" runat="server" Text='<%# Eval("OptionB") %>'></asp:Label></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditOptionB" runat="server" Text='<%# Bind("OptionB") %>' Width="90%"></asp:TextBox></EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="C">
                            <ItemTemplate>
                                <asp:Label ID="lblOptC" runat="server" Text='<%# Eval("OptionC") %>'></asp:Label></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditOptionC" runat="server" Text='<%# Bind("OptionC") %>' Width="90%"></asp:TextBox></EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="D">
                            <ItemTemplate>
                                <asp:Label ID="lblOptD" runat="server" Text='<%# Eval("OptionD") %>'></asp:Label></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditOptionD" runat="server" Text='<%# Bind("OptionD") %>' Width="90%"></asp:TextBox></EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Answer" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:Label ID="lblAns" runat="server" Text='<%# Eval("CorrectAnswer") %>'></asp:Label></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlEditCorrectAnswer" runat="server" SelectedValue='<%# Bind("CorrectAnswer") %>'>
                                    <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                    <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                    <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                    <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <%-- *** CssClass ADDED *** --%>
                                    <asp:Button ID="btnEdit" runat="server" CommandName="Edit" Text="Edit" CssClass="btn btn-edit" />
                                    <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="Delete" CssClass="btn btn-delete"
                                        OnClientClick="return confirm('Delete this question from the temporary list?');" />
                                </div>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <div class="action-buttons">
                                    <%-- *** CssClass ADDED *** --%>
                                    <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CssClass="btn btn-secondary" />
                                    <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" CssClass="btn btn-cancel" />
                                </div>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="text-align: center; padding: 20px; color: #666;">
                            No questions added to this quiz yet.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

            <br />
            <div>
                <%-- *** CssClass ADDED *** --%>
                <asp:Button ID="btnSaveQuiz" runat="server" Text="Save Quiz to Lesson" CssClass="btn btn-secondary" OnClick="btnSaveQuiz_Click" />
                <asp:Button ID="btnCancelQuizEdit" runat="server" Text="Cancel" CssClass="btn btn-cancel" OnClick="btnCancelQuizEdit_Click" CausesValidation="false" />
            </div>
        </div>

        <hr />

        <!-- ================== FINAL CREATE BUTTON ================== -->
        <br />
        <div style="text-align: center;">
            <%-- *** CssClass ADDED *** --%>
            <asp:Button ID="btnCreateCourse" runat="server" Text="Create Course" CssClass="btn btn-secondary"
                OnClick="btnCreateCourse_Click" OnClientClick="return validateForm();" />
        </div>

        <%-- This label will now show validation errors --%>
        <asp:Label ID="lblStatus" runat="server" CssClass="status-label" EnableViewState="false"></asp:Label>
    </div>

    <%-- *** UPDATED JAVASCRIPT (No more alerts) *** --%>
    <script type="text/javascript">
        function validateForm() {
            var courseTitle = document.getElementById('<%= txtCourseTitle.ClientID %>').value;
            var courseType = document.getElementById('<%= ddlCourseType.ClientID %>').value;
            var lessonCount = parseInt(document.getElementById('<%= lblLessonCount.ClientID %>').innerText);
            var statusLabel = document.getElementById('<%= lblStatus.ClientID %>');

            statusLabel.style.color = 'red'; // Set error color

            if (courseTitle.trim() === '') {
                statusLabel.innerText = 'Please enter a course title.';
                return false;
            }

            if (courseType === '') {
                statusLabel.innerText = 'Please select a course type.';
                return false;
            }

            if (lessonCount === 0) {
                statusLabel.innerText = 'Please add at least one lesson to the course.';
                return false;
            }

            // Validate coin for private courses
            if (courseType === 'Private') {
                var coin = document.getElementById('<%= txtCourseCoin.ClientID %>').value;
                if (coin.trim() === '' || parseInt(coin) < 0) {
                    statusLabel.innerText = 'Please enter a valid coin amount for private courses.';
                    return false;
                }
            }

            statusLabel.innerText = ''; // Clear errors
            statusLabel.style.color = 'green'; // Set success color
            // Confirmation before creating
            return confirm('Are you sure you want to create this course with all its lessons and quizzes?');
        }
    </script>

</asp:Content>

