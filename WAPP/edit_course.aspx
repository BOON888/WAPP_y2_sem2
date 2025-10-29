<%@ Page Title="Edit Course" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="edit_course.aspx.cs"
    Inherits="WAPP.edit_course" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        
        <asp:UpdatePanel ID="updAll" runat="server">
            <ContentTemplate>

                <h2 class="text-center mb-4">Edit Course</h2>

                <div class="card shadow-sm p-3 mb-4">
                    <h4>
                        Selected Course:
                        <asp:Label ID="lblCourseTitle" runat="server" CssClass="text-primary fw-bold"></asp:Label>
                    </h4>
                </div>

                <div class="card p-4 shadow-sm mb-4">
                    <%-- This is the top repeater, it is unchanged --%>
                    <h5 class="mb-3">Lessons in this Course</h5>
                    <asp:Repeater ID="rptLessons" runat="server" OnItemCommand="rptLessons_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-bordered align-middle">
                                <thead class="table-light">
                                    <tr>
                                        <th>Lesson ID</th>
                                        <th>Lesson Number</th>
                                        <th>Lesson Title</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Id") %></td>
                                <td><%# Eval("LessonNumber") %></td>
                                <td><%# Eval("LessonTitle") %></td>
                                <td>
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit"
                                        CommandName="Edit" CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-warning btn-sm me-2" />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete"
                                        CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-danger btn-sm"
                                        OnClientClick="return confirm('Are you sure you want to delete this lesson? This will also delete its quiz and all associated questions.');" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <%-- This is the main edit panel --%>
                <asp:Panel ID="pnlLessonEdit" runat="server" CssClass="card p-4 shadow-sm" Visible="false">
                    <h5 class="mb-3">Edit Lesson Details</h5>

                    <asp:HiddenField ID="hfLessonId" runat="server" />
                    <%-- REQUIRED CHANGES: Add two hidden fields to track the existing file --%>
                    <asp:HiddenField ID="hfExistingFilePath" runat="server" />
                    <asp:HiddenField ID="hfExistingContentType" runat="server" />

                    <div class="mb-3">
                        <label class="form-label">Lesson Title</label>
                        <asp:TextBox ID="txtLessonTitle" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>

                    <%-- This textbox will now show the ORIGINAL filename or the TEXT content --%>
                    <div class="mb-3">
                        <label class="form-label">Lesson Content (Text or Original Filename)</label>
                        <asp:TextBox ID="txtLessonContentPath" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        <small class="form-text text-muted">Enter text OR upload a new file below. Uploading a new file will replace the old content.</small>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Upload New Lesson File (Optional)</label>
                        <asp:FileUpload ID="fuLessonFile" runat="server" CssClass="form-control" />
                    </div>

                    <asp:Button ID="btnUpdateLesson" runat="server" Text="Update Lesson"
                        CssClass="btn btn-success mt-3" OnClick="btnUpdateLesson_Click" />

                    <hr />

                    <%-- The entire quiz section (Repeater, Add New, etc.) is unchanged --%>
                    <h5 class="mb-3">Lesson Quiz (Editable)</h5>
                    <asp:Repeater ID="rptQuizQuestions" runat="server" 
                                    OnItemDataBound="rptQuizQuestions_ItemDataBound"
                                    OnItemCommand="rptQuizQuestions_ItemCommand">
                        <ItemTemplate>
                            <div class="card p-3 mb-3 border">
                                <asp:HiddenField ID="hfQuestionId" runat="server" Value='<%# Eval("Id") %>' />
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Question <%# Container.ItemIndex + 1 %>:</label>
                                    <asp:TextBox ID="txtQuestionText" runat="server" Text='<%# Eval("QuestionText") %>' CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                </div>
                                <div class="row g-2 mb-3">
                                    <div class="col-md-6">
                                        <div class="input-group">
                                            <span class="input-group-text">A</span>
                                            <asp:TextBox ID="txtOptionA" runat="server" Text='<%# Eval("OptionA") %>' CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-group">
                                            <span class="input-group-text">B</span>
                                            <asp:TextBox ID="txtOptionB" runat="server" Text='<%# Eval("OptionB") %>' CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-group">
                                            <span class="input-group-text">C</span>
                                            <asp:TextBox ID="txtOptionC" runat="server" Text='<%# Eval("OptionC") %>' CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-group">
                                            <span class="input-group-text">D</span>
                                            <asp:TextBox ID="txtOptionD" runat="server" Text='<%# Eval("OptionD") %>' CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="d-flex justify-content-between align-items-center">
                                    <div class="mb-2">
                                        <label class="form-label">Correct Answer:</label>
                                        <asp:DropDownList ID="ddlCorrectAnswer" runat="server" CssClass="form-select" style="max-width: 150px;">
                                            <asp:ListItem Value="A">Option A</asp:ListItem>
                                            <asp:ListItem Value="B">Option B</asp:ListItem>
                                            <asp:ListItem Value="C">Option C</asp:ListItem>
                                            <asp:ListItem Value="D">Option D</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <asp:Button ID="btnDeleteQuestion" runat="server" Text="Delete" 
                                        CommandName="DeleteQuestion" CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="btn btn-danger btn-sm" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Button ID="btnUpdateQuiz" runat="server" Text="Update All Questions" 
                        CssClass="btn btn-primary mt-3" OnClick="btnUpdateQuiz_Click" />
                    
                    <div class="card p-3 mt-4 border-primary">
                        <h5 class="mb-3">Add New Question</h5>
                        <div class="mb-3">
                            <label class="form-label fw-bold">New Question Text:</label>
                            <asp:TextBox ID="txtNewQuestionText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                        </div>
                        <div class="row g-2 mb-3">
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-text">A</span>
                                    <asp:TextBox ID="txtNewOptionA" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-text">B</span>
                                    <asp:TextBox ID="txtNewOptionB" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-text">C</span>
                                    <asp:TextBox ID="txtNewOptionC" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-text">D</span>
                                    <asp:TextBox ID="txtNewOptionD" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Correct Answer:</label>
                            <asp:DropDownList ID="ddlNewCorrectAnswer" runat="server" CssClass="form-select" style="max-width: 150px;">
                                <asp:ListItem Value="A">Option A</asp:ListItem>
                                <asp:ListItem Value="B">Option B</asp:ListItem>
                                <asp:ListItem Value="C">Option C</asp:ListItem>
                                <asp:ListItem Value="D">Option D</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <asp:Button ID="btnAddQuestion" runat="server" Text="Add This Question" 
                            CssClass="btn btn-info mt-2" OnClick="btnAddQuestion_Click" />
                    </div>
                </asp:Panel>
                
            </ContentTemplate>
            
            <Triggers>
                <asp:PostBackTrigger ControlID="btnUpdateLesson" />
            </Triggers>

        </asp:UpdatePanel> 
    </div>
</asp:Content>