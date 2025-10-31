<%@ Page Title="Admin Community Management" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="admin_communitymanagement.aspx.cs" Inherits="WAPP.admin_communitymanagement"
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
            background-color: #f9f9fb; /* Changed from gradient to solid as per teacher's style */
            margin: 0;
            padding: 0;
        }

        h2 {
            color: #111827;
            margin-bottom: 10px;
            text-align: center; /* Center align as per teacher's style */
        }

        p.text-muted {
            color: #666;
        }

        p.text {
            color: #666;
            text-align: center; /* Center align as per teacher's style */
            margin-bottom: 30px;
        }

        /* ===== Container (Main Panel) ===== */
        .signup-container {
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 90%; /* More responsive width */
            max-width: 1200px; /* Adjusted for better content fit */
            margin: 80px auto; /* Centered with top margin */
            text-align: center; /* Center align as per teacher's style */
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

        .btn-back {
            background-color: white;
            color: #001eff;
            border: 1px solid #001eff; /* Added border for better visibility */
        }

        .btn:active {
            transform: translateY(0);
            box-shadow: 0 3px 8px rgba(0, 30, 255, 0.2);
            opacity: 1;
        }

        /* ===== Delete Button (Red Theme) - Enhanced with teacher's style ===== */
        .btn-delete {
            background-color: white;
            color: red;
            border-radius: 6px;
            padding: 8px 16px; /* Slightly larger for better touch */
            cursor: pointer;
            transition: background-color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            font-size: 14px;
            text-decoration: none;
            border: 1px solid rgba(255, 0, 0, 0.3);
            margin: 2px;
        }

        .btn-delete:hover {
            background-color: red;
            color: white;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(255, 0, 0, 0.2);
        }

        /* ===== Cancel Button - Enhanced with teacher's style ===== */
        .btn-cancel {
            background-color: white;
            color: #333;
            border-radius: 6px;
            padding: 8px 16px;
            cursor: pointer;
            transition: background-color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
            font-size: 14px;
            text-decoration: none;
            border: 1px solid rgba(0, 0, 0, 0.2);
            margin: 2px;
        }

        .btn-cancel:hover {
            background-color: #f5f5f5;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        }

        /* ===== Card / Post Styling - Enhanced with glass effect ===== */
        .card {
            background: rgba(255, 255, 255, 0.4); /* More transparent for glass effect */
            border-radius: 10px;
            box-shadow: 0 2px 15px rgba(0, 30, 255, 0.15); /* Softer blue shadow */
            padding: 20px;
            margin-top: 25px;
            backdrop-filter: blur(5px); /* Added glass effect to cards */
            -webkit-backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        textarea, input[type="text"] {
            width: 100%;
            max-width: none;
            border: 1px solid rgba(0, 0, 0, 0.1);
            border-radius: 6px;
            padding: 10px;
            font-size: 14px;
            background: rgba(255, 255, 255, 0.8);
        }

        .reply-container {
            background-color: rgba(249, 249, 249, 0.7); /* More transparent */
            border: 1px solid rgba(221, 221, 221, 0.5);
            border-radius: 8px;
            padding: 15px;
            max-height: 200px;
            overflow-y: auto;
            margin-top: 15px;
        }

        .reply-box {
            display: flex;
            gap: 10px;
            margin-top: 10px;
        }

        .text-danger {
            color: red;
        }

        .admin-badge {
            background-color: rgba(255, 107, 107, 0.9); /* Slightly transparent */
            color: white;
            border-radius: 4px;
            padding: 3px 8px;
            font-size: 11px;
            margin-left: 5px;
            font-weight: 500;
        }

        .stats-container {
            display: flex;
            gap: 20px;
            margin-bottom: 30px;
            flex-wrap: wrap;
            justify-content: center; /* Center align stats */
        }

        .stat-card {
            background: rgba(255, 255, 255, 0.3); /* Glass effect for stats */
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 15px rgba(0, 30, 255, 0.1);
            min-width: 150px;
            text-align: center;
            backdrop-filter: blur(5px);
            -webkit-backdrop-filter: blur(5px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .stat-number {
            font-size: 28px; /* Slightly larger */
            font-weight: bold;
            color: #001eff;
        }

        .stat-label {
            font-size: 14px;
            color: #666;
            margin-top: 8px;
        }

        /* ===== Post Card Specific Styles ===== */
        .post-card {
            transition: transform 0.25s ease, box-shadow 0.25s ease;
        }

        .post-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0, 30, 255, 0.15);
        }

        /* ===== SweetAlert Customization ===== */
        .logout-container {
            padding: 10px;
        }
    </style>

    <div class="signup-container">

        <h2>Community Forum - Admin Management</h2>
        <p class="text">Monitor and manage all community posts and replies</p>

        <!-- Statistics - Only Total Posts -->
        <div class="stats-container">
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblTotalPosts" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Total Posts</div>
            </div>
        </div>

        <!-- Posts Display -->
        <asp:Repeater ID="rptPosts" runat="server" OnItemCommand="rptPosts_ItemCommand" OnItemDataBound="rptPosts_ItemDataBound">
            <ItemTemplate>
                <div class="card post-card">
                    <div style="display: flex; justify-content: space-between; align-items: start;">
                        <div>
                            <strong><%# Eval("FullName") %></strong>
                            <span style="background-color: rgba(221, 221, 221, 0.7); border-radius: 4px; padding: 3px 8px; font-size: 11px; margin-left: 5px;">
                                <%# Eval("Role") %>
                            </span>
                            <span class="admin-badge">POST ID: <%# Eval("Id") %></span>
                            <p class="text-muted" style="margin: 0; font-size: 12px;">
                                <%# Eval("PostDateTime", "{0:g}") %>
                            </p>
                        </div>

                        <!-- Delete Post Button (Admin can delete any post) -->
                        <asp:LinkButton ID="btnDeletePost" runat="server"
                            CommandName="DeletePost"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn-delete delete-post"
                            OnPreRender="btnDeletePost_PreRender">
                            Delete Post
                        </asp:LinkButton>
                    </div>

                    <hr style="border: 0; border-top: 1px solid rgba(0, 0, 0, 0.1); margin: 15px 0;" />
                    <p style="text-align: left; margin: 15px 0;"><%# Eval("PostContent") %></p>

                    <!-- Replies -->
                    <div class="reply-container">
                        <asp:Repeater ID="rptReplies" runat="server" DataSource='<%# Eval("Replies") %>' OnItemCommand="rptReplies_ItemCommand" OnItemDataBound="rptReplies_ItemDataBound">
                            <ItemTemplate>
                                <div style="margin-bottom: 10px; padding: 8px; border-left: 3px solid rgba(221, 221, 221, 0.7); background: rgba(255, 255, 255, 0.5); border-radius: 0 5px 5px 0;">
                                    <div style="display: flex; justify-content: space-between; align-items: center;">
                                        <div style="flex-grow: 1; text-align: left;">
                                            <strong><%# Eval("FullName") %>:</strong> <%# Eval("ReplyContent") %>
                                            <p class="text-muted" style="margin: 0; font-size: 11px;">
                                                <%# Eval("ReplyDateTime", "{0:g}") %>
                                                <span class="admin-badge" style="margin-left: 10px;">REPLY ID: <%# Eval("ReplyId") %></span>
                                            </p>
                                        </div>
                                        <!-- Delete Reply Button -->
                                        <asp:LinkButton ID="btnDeleteReply" runat="server"
                                            CommandName="DeleteReply"
                                            CommandArgument='<%# Eval("ReplyId") %>'
                                            CssClass="btn-delete delete-reply"
                                            OnPreRender="btnDeleteReply_PreRender">
                                            Delete Reply
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
        <!-- No Posts Message -->
        <asp:Panel ID="pnlNoPosts" runat="server" Visible="false" CssClass="card">
            <h4>No Posts Found</h4>
            <p class="text-muted">There are no community posts to display at the moment.</p>
        </asp:Panel>

        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

        <script>
            document.addEventListener('DOMContentLoaded', function () {
                // Handle post deletion
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
                                    This will also delete all associated replies.
                                </p>
                                <div style="display:flex; justify-content:center; gap:20px;">
                                    <button id="confirmDelete" class="btn-delete" style="padding: 10px 20px;">Delete</button>
                                    <button id="cancelDelete" class="btn-cancel" style="padding: 10px 20px;">Cancel</button>
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
                                __doPostBack(uniqueID, '');
                            });

                            document.getElementById('cancelDelete').addEventListener('click', function () {
                                Swal.close();
                            });
                        }, 100);
                    });
                });

                // Handle reply deletion
                document.querySelectorAll('.delete-reply').forEach(function (btn) {
                    btn.addEventListener('click', function (event) {
                        event.preventDefault();

                        const replyId = btn.getAttribute('commandargument');
                        const uniqueID = btn.getAttribute('data-uniqueid');

                        Swal.fire({
                            html: `
                            <div class="logout-container">
                                <h4 style="margin-bottom:15px; color:black;">Confirm Delete</h4>
                                <p style="margin-bottom:25px; color:#333;">
                                    Are you sure you want to delete this reply?
                                </p>
                                <div style="display:flex; justify-content:center; gap:20px;">
                                    <button id="confirmDeleteReply" class="btn-delete" style="padding: 10px 20px;">Delete</button>
                                    <button id="cancelDeleteReply" class="btn-cancel" style="padding: 10px 20px;">Cancel</button>
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
                            document.getElementById('confirmDeleteReply').addEventListener('click', function () {
                                __doPostBack(uniqueID, '');
                            });

                            document.getElementById('cancelDeleteReply').addEventListener('click', function () {
                                Swal.close();
                            });
                        }, 100);
                    });
                });
            });
        </script>

    </div>
</asp:Content>