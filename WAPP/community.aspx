<%@ Page Title="Community Forum" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="community.aspx.cs" Inherits="WAPP.community"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>

        /* ===== Remove Scrollbar ===== */
        html {
            scrollbar-width: none; /* Firefox */
        }
        html::-webkit-scrollbar {
            display: none; /* Chrome, Safari, Edge */
        }

        /* ===== General Page Style ===== */
        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #e9efff, #ffffff);
            margin: 0;
            padding: 0;
        }

        h2 {
            color: #111827;
            margin-bottom: 10px;
        }

        p.text-muted {
            color: #666;
        }

        p.text {
            color: #666;
            text-align: left;
        }

        /* ===== Container (Main Panel) ===== */
        .signup-container {
            padding: 40px;
            max-width: 1900px;
            margin: auto;
            text-align: left;
            background: rgba(255, 255, 255, 0.25);
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        /* ===== Buttons ===== */
        .btn {
            background-color: #001eff;
            color: white;
            border: none;
            border-radius: 6px;
            padding: 10px 20px;
            font-size: 16px;
            cursor: pointer;
            margin: 5px;
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            width: 200px;
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
            opacity: 0.95;
        }

        .btn-post {
            background-color: white;
            color: #001eff;
            border: 2px solid #00000019;
        }

        .btn:active {
            transform: translateY(0);
            box-shadow: 0 3px 8px rgba(0, 30, 255, 0.2);
            opacity: 1;
        }

        /* ===== Delete Button (Red Theme) ===== */
        .btn-delete {
            background-color: white;
            color: red;
            border-radius: 6px;
            padding: 6px 14px;
            cursor: pointer;
            transition: background-color 0.3s ease, transform 0.25s ease;
            font-size: 14px;
            text-decoration: none;
            border: 1px solid rgba(255, 0, 0, 0.2);

        }

        .btn-delete:hover {
            background-color: red;
            color:white;
            transform: translateY(-2px);
        }

        /* ===== Card / Post Styling ===== */
        .card {
            background-color: rgba(255, 255, 255, 0.85);
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            padding: 20px;
            margin-top: 25px;
        }

        textarea, input[type="text"] {
            width: 100%;
            max-width: none;
            border: 1px solid #ccc;
            border-radius: 6px;
            padding: 10px;
            font-size: 14px;
        }


        .reply-container {
            background-color: #f9f9f9;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 10px;
            max-height: 200px;
            overflow-y: auto;
            margin-top: 10px;
        }

        .reply-box {
            display: flex;
            gap: 10px;
            margin-top: 10px;
        }

        .text-danger {
            color: red;
        }
    </style>

    <div class="signup-container">

        <h2>Community Forum</h2>
        <p class="text">Ask questions, share knowledge, and connect with other learners</p>

        <!-- My Posts Button -->
        <asp:Button ID="btnMyPosts" runat="server" CssClass="btn btn-post" Text="My Posts" OnClick="btnMyPosts_Click" />

        <!-- Ask a Question Panel -->
        <div class="card">
            <h5>Ask a Question</h5>
            <p class="text">Get help from the community or share your insights</p>
            <asp:TextBox ID="txtQuestion" runat="server" TextMode="MultiLine" Rows="3" placeholder="What's your question or what would you like to share?"></asp:TextBox>
            <br /><br />
            <asp:Button ID="btnAsk" runat="server" CssClass="btn" Text="Ask Question" OnClick="btnAsk_Click" />
        </div>

        <!-- Posts Display -->
        <asp:Repeater ID="rptPosts" runat="server" OnItemCommand="rptPosts_ItemCommand" OnItemDataBound="rptPosts_ItemDataBound">
            <ItemTemplate>
                <div class="card post-card">
                    <div style="display: flex; justify-content: space-between; align-items: start;">
                        <div>
                            <strong><%# Eval("FullName") %></strong>
                            <span style="background-color: #ddd; border-radius: 3px; padding: 2px 6px; font-size: 12px; margin-left: 5px;">
                                <%# Eval("Role") %>
                            </span>
                            <p class="text-muted" style="margin: 0; font-size: 12px;">
                                <%# Eval("PostDateTime", "{0:g}") %>
                            </p>
                        </div>

                        <!-- Delete Button -->
                        <asp:LinkButton ID="btnDelete" runat="server"
                            CommandName="DeletePost"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn-delete delete-post"
                            Visible="false"
                            OnPreRender="btnDelete_PreRender">
                            Delete
                        </asp:LinkButton>

                    </div>

                    <hr />
                    <p><%# Eval("PostContent") %></p>

                    <!-- Replies -->
                    <div class="reply-container">
                        <asp:Repeater ID="rptReplies" runat="server" DataSource='<%# Eval("Replies") %>'>
                            <ItemTemplate>
                                <div style="margin-bottom: 8px;">
                                    <strong><%# Eval("FullName") %>:</strong> <%# Eval("ReplyContent") %>
                                    <p class="text-muted" style="margin: 0; font-size: 12px;">
                                        <%# Eval("ReplyDateTime", "{0:g}") %>
                                    </p>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <!-- Reply Input -->
                    <div class="reply-box">
                        <asp:TextBox ID="txtReply" runat="server" placeholder="Write a reply..."></asp:TextBox>
                        <asp:Button ID="btnReply" runat="server"
                            CommandName="AddReply"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn"
                            Text="Reply" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

        <script>
            document.addEventListener('DOMContentLoaded', function () {
                document.querySelectorAll('.delete-post').forEach(function (btn) {
                    btn.addEventListener('click', function (event) {
                        event.preventDefault();

                        const postId = btn.getAttribute('commandargument');
                        const uniqueID = btn.getAttribute('data-uniqueid');

                        Swal.fire({
                            html: `
                            <div class="logout-container">
                                <h4 style="margin-bottom:15px; color:black;">Confirm Delete</h4>
                                <p style="margin-bottom:25px; color:#333;">
                                    Are you sure you want to delete this post?<br>
                                </p>
                                <div style="display:flex; justify-content:center; gap:20px;">
                                    <button id="confirmDelete" class="btn-delete">Delete</button>
                                    <button id="cancelDelete" class="btn-cancel">Cancel</button>
                                </div>
                            </div>
                        `,
                            showConfirmButton: false,
                            showCancelButton: false,
                            background: 'transparent',
                            allowOutsideClick: false,
                            allowEscapeKey: false,
                        });

                        setTimeout(() => {
                            document.getElementById('confirmDelete').addEventListener('click', function () {
                                // ✅ Correctly trigger postback with the server's unique ID
                                __doPostBack(uniqueID, '');
                            });

                            document.getElementById('cancelDelete').addEventListener('click', function () {
                                Swal.close();
                            });
                        }, 100);
                    });
                });
            });
        </script>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>

    </div>
</asp:Content>
