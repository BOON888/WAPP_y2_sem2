<%@ Page Title="My Posts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="my_post.aspx.cs" Inherits="WAPP.my_post" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .reply-container {
            max-height: 200px;          
            overflow-y: auto;           
            background-color: #f8f9fa;  
            padding: 10px;
            border-radius: 8px;
            border: 1px solid #dee2e6;
        }

        .reply-item {
            background-color: #fff;
            border-radius: 6px;
            padding: 8px;
            box-shadow: 0 1px 2px rgba(0,0,0,0.05);
        }

    </style>
    <div class="container mt-4">
        <h2 class="mb-4 text-center">My Posts</h2>

        <!-- User Post List -->
        <asp:Repeater ID="rptMyPosts" runat="server" OnItemCommand="rptMyPosts_ItemCommand">
            <ItemTemplate>
                <div class="card mb-4 shadow-sm border-0">
                    <div class="card-body">
                        <div class="d-flex justify-content-between">
                            <div>
                                <strong><%# Eval("FullName") %></strong>
                                <span class="badge bg-secondary ms-2"><%# Eval("Role") %></span>
                                <p class="text-muted small mb-1"><%# Eval("PostDateTime", "{0:g}") %></p>
                            </div>
                            <asp:LinkButton ID="btnDeletePost" runat="server"
                                CommandName="DeletePost"
                                CommandArgument='<%# Eval("Id") %>'
                                CssClass="btn btn-sm btn-outline-danger">Delete</asp:LinkButton>
                        </div>
                        <p class="mt-2"><%# Eval("PostContent") %></p>

                        <!-- Replies Section -->
                        <div class="mt-3 ps-3 border-start">
                            <h6 class="text-muted">Replies</h6>
                            <div class="reply-container">
                                <asp:Repeater ID="rptReplies" runat="server" DataSource='<%# Eval("Replies") %>'>
                                    <ItemTemplate>
                                        <div class="reply-item mb-2">
                                            <strong><%# Eval("FullName") %></strong>
                                            <p class="mb-1"><%# Eval("ReplyContent") %></p>
                                            <p class="text-muted small"><%# Eval("ReplyDateTime", "{0:g}") %></p>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- If no posts -->
        <asp:Label ID="lblNoPosts" runat="server" CssClass="text-muted d-block text-center mt-4" Visible="false">
            You haven’t created any posts yet.
        </asp:Label>
    </div>
</asp:Content>
