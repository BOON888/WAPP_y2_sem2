<%@ Page Title="My Posts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="my_post.aspx.cs" Inherits="WAPP.my_post" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>


        html::-webkit-scrollbar {
            display: none; 
        }
        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #e9efff, #ffffff);
            margin: 0;
            padding: 0;
        }

        h2 {
            color: #111827;
            margin-bottom: 10px;
            text-align: center;
        }

        p.text-muted {
            color: #666;
        }

        p.text {
            color: #666;
            text-align: left;
        }

        .my-posts-container {
            padding: 40px;
            max-width: 1200px;
            margin: auto;
            text-align: left;
            background: rgba(255, 255, 255, 0.25);
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .btn {
            background-color: #001eff;
            color: white;
            border: none;
            border-radius: 6px;
            padding: 8px 16px;
            font-size: 14px;
            cursor: pointer;
            margin: 5px;
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
            opacity: 0.95;
        }

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
            color: white;
            transform: translateY(-2px);
        }

        .card {
            background-color: rgba(255, 255, 255, 0.85);
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            padding: 20px;
            margin-top: 25px;
        }

        .card-body strong {
            color: #111827;
        }

        .badge {
            background-color: #001eff;
            color: white;
            padding: 4px 8px;
            border-radius: 6px;
            font-size: 12px;
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

        .reply-item {
            background-color: #fff;
            border-radius: 6px;
            padding: 10px;
            margin-bottom: 8px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.05);
        }

        .reply-item strong {
            color: #001eff;
        }

        .reply-item p {
            margin-bottom: 5px;
            color: #333;
        }

        .text-muted.small {
            color: #777;
            font-size: 12px;
        }

        .text-muted.d-block.text-center {
            color: #777;
        }
    </style>

<div class="my-posts-container mt-4">
    <h2 class="mb-4">My Posts</h2>

    <asp:Repeater ID="rptMyPosts" runat="server" OnItemCommand="rptMyPosts_ItemCommand">
        <ItemTemplate>
            <div class="card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <strong><%# Eval("FullName") %></strong>
                            <span class="badge ms-2"><%# Eval("Role") %></span>
                            <p class="text-muted small mb-1"><%# Eval("PostDateTime", "{0:g}") %></p>
                        </div>
                        <asp:LinkButton ID="btnDelete" runat="server"
                            CommandName="DeletePost"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn-delete delete-post"
                            OnPreRender="btnDelete_PreRender">
                            Delete
                        </asp:LinkButton>
                    </div>
                    <p class="mt-2"><%# Eval("PostContent") %></p>

                    <div class="mt-3 ps-3 border-start">
                        <h6 class="text-muted">Replies</h6>
                        <div class="reply-container">
                            <asp:Repeater ID="rptReplies" runat="server" DataSource='<%# Eval("Replies") %>'>
                                <ItemTemplate>
                                    <div class="reply-item">
                                        <strong><%# Eval("FullName") %></strong>
                                        <p><%# Eval("ReplyContent") %></p>
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
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            document.querySelectorAll('.delete-post').forEach(function (btn) {
                btn.addEventListener('click', function (event) {
                    event.preventDefault(); 

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
                                __doPostBack(uniqueID, ''); // ✅ triggers ASP.NET DeletePost
                            });
                            document.getElementById('cancelDelete').addEventListener('click', function () {
                                Swal.close();
                            });
                        }, 100);
                    });
                });
            });
        </script>

    <asp:Label ID="lblNoPosts" runat="server" CssClass="text-muted d-block text-center mt-4" Visible="false">
        You haven’t created any posts yet.
    </asp:Label>
</div>

</asp:Content>
