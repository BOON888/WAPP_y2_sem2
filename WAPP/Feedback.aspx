<%@ Page Title="Feedback" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="WAPP.Feedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        body { background-color: #f9f9fb; font-family: 'Segoe UI', sans-serif; }
        .feedback-container {
            background: white;
            border-radius: 12px;
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 90%;
            max-width: 900px;
            margin: 60px auto;
        }
        .feedback-container h1 {
            font-size: 38px;
            font-weight: bold;
            color: #111827;
        }
        .feedback-container p {
            color: #6b7280;
            margin-bottom: 30px;
        }
        .feedback-form {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }
        .feedback-form label {
            font-weight: 600;
            color: #111827;
        }
        .feedback-form input[type="text"],
        .feedback-form textarea,
        .feedback-form select {
            width: 100%;
            padding: 10px;
            border: 1px solid #d1d5db;
            border-radius: 8px;
            background-color: #f9fafb;
            font-size: 14px;
        }
        .feedback-form textarea { resize: vertical; height: 120px; }
        .btn-submit {
            background-color: #001eff;
            color: white;
            border: none;
            padding: 10px;
            border-radius: 8px;
            cursor: pointer;
            transition: 0.3s;
        }
        .btn-submit:hover { background-color: #3740ff; }
        .feedback-history {
            margin-top: 50px;
        }
        .feedback-item {
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 15px;
            background: #fafafa;
        }
        .feedback-item h3 { color: #111827; margin-bottom: 10px; }
        .feedback-item p { margin: 5px 0; color: #555; }
        .priority-high { color: #dc2626; font-weight: bold; }
        .priority-medium { color: #2563eb; font-weight: bold; }
        .priority-low { color: #059669; font-weight: bold; }
        .status-pending { background-color: #fef3c7; color: #92400e; padding: 3px 10px; border-radius: 6px; }
        .status-completed { background-color: #d1fae5; color: #065f46; padding: 3px 10px; border-radius: 6px; }
        .auto-style1 {
            text-align: center;
            font-size: xx-large;
        }
    </style>

    <div class="feedback-container">
        <h1 class="text-center">Feedback</h1>
        <p>Help us improve your learning experience</p>

        <!-- Feedback Form -->
        <div class="feedback-form">
            <label>Category</label>
            <asp:DropDownList ID="ddlCategory" runat="server">
                <asp:ListItem Value="">Select Category</asp:ListItem>
                <asp:ListItem Value="Course Content">Course Content</asp:ListItem>
                <asp:ListItem Value="Community Post">Community Post</asp:ListItem>
            </asp:DropDownList>

            <label>Priority</label>
            <asp:DropDownList ID="ddlPriority" runat="server">
                <asp:ListItem Value="">Select Priority</asp:ListItem>
                <asp:ListItem Value="High">High</asp:ListItem>
                <asp:ListItem Value="Medium">Medium</asp:ListItem>
                <asp:ListItem Value="Low">Low</asp:ListItem>
            </asp:DropDownList>

            <label>Subject</label>
            <asp:TextBox ID="txtSubject" runat="server" placeholder="Brief summary of your feedback"></asp:TextBox>

            <label>Description</label>
            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"
                placeholder="Provide detailed feedback or suggestions..." Rows="5"></asp:TextBox>

            <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>

            <asp:Button ID="btnSubmit" runat="server" Text="Send Feedback" CssClass="btn-submit" OnClick="btnSubmit_Click" />
        </div>

        <!-- Feedback History -->
        <div class="feedback-history">
            <h2 class="auto-style1">Your Feedback History</h2>
            <asp:Repeater ID="rptFeedbackHistory" runat="server">
                <ItemTemplate>
                    <div class="feedback-item">
                        <h3><%# Eval("Subject") %></h3>
                        <p><strong>Category:</strong> <%# Eval("Category") %></p>
                        <p><strong>Priority:</strong>
                            <span class='priority-<%# Eval("Priority").ToString().ToLower() %>'>
                                <%# Eval("Priority") %>
                            </span>
                        </p>
                        <p><strong>Status:</strong>
                            <span class='status-<%# Eval("Status").ToString().ToLower() %>'>
                                <%# Eval("Status") %>
                            </span>
                        </p>
                        <p><strong>Date:</strong> <%# Eval("CreatedAt", "{0:yyyy-MM-dd HH:mm}") %></p>
                        <p><strong>Description:</strong> <%# Eval("Description") %></p>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

</asp:Content>
