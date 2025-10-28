<%@ Page Title="Community Forum" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="community.aspx.cs" Inherits="WAPP.community" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5">

        <h2 class="mb-4">Community Forum</h2>
        <p class="text-muted">Ask questions, share knowledge, and connect with other learners</p>

        <!-- My Posts Button -->
        <asp:Button ID="btnMyPosts" runat="server" CssClass="btn btn-outline-primary mb-3" Text="My Posts" OnClick="btnMyPosts_Click" />

        <!-- Ask a Question Panel -->
        <div class="card mb-4 p-3 shadow-sm">
            <h5>Ask a Question</h5>
            <p class="text-muted small mb-2">Get help from the community or share your insights</p>
            <asp:TextBox ID="txtQuestion" runat="server" CssClass="form-control mb-3" TextMode="MultiLine" Rows="3" placeholder="What's your question or what would you like to share?"></asp:TextBox>
            <asp:Button ID="btnAsk" runat="server" CssClass="btn btn-primary" Text="Ask Question" OnClick="btnAsk_Click" />
        </div>

        <!-- Posts Display -->
        <asp:Repeater ID="rptPosts" runat="server" OnItemCommand="rptPosts_ItemCommand" OnItemDataBound="rptPosts_ItemDataBound">
            <ItemTemplate>
                <div class="card p-3 mb-4 shadow-sm post-card">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <strong><%# Eval("FullName") %></strong>
                            <span class="badge bg-secondary ms-2"><%# Eval("Role") %></span>
                            <p class="text-muted small mb-0"><%# Eval("PostDateTime", "{0:g}") %></p>
                        </div>

                        <!-- Delete Button (visible only if owner) -->
                        <asp:LinkButton ID="btnDelete" runat="server" 
                            CommandName="DeletePost"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="text-danger"
                            Visible="false">
                            Delete
                        </asp:LinkButton>
                    </div>

                    <hr />
                    <p><%# Eval("PostContent") %></p>

                    <!-- Replies -->
                    <div class="reply-container border rounded p-2 mt-3" style="max-height: 200px; overflow-y: auto;">
                        <asp:Repeater ID="rptReplies" runat="server" DataSource='<%# Eval("Replies") %>'>
                            <ItemTemplate>
                                <div class="mb-2">
                                    <strong><%# Eval("FullName") %>:</strong> <%# Eval("ReplyContent") %>
                                    <p class="text-muted small mb-0"><%# Eval("ReplyDateTime", "{0:g}") %></p>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <!-- Reply Input -->
                    <div class="mt-2 d-flex">
                        <asp:TextBox ID="txtReply" runat="server" CssClass="form-control me-2" placeholder="Write a reply..."></asp:TextBox>
                        <asp:Button ID="btnReply" runat="server" CommandName="AddReply" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-outline-primary" Text="Reply" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3 d-block"></asp:Label>

    </div>
</asp:Content>
