<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="sign_in.aspx.cs" Inherits="WAPP.sign_in" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
        }

        .login-container {
            background-color: white;
            border-radius: 12px;
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 400px;
            margin: 80px auto;
            text-align: center;
        }

        h2 {
            color: #111827;
            margin-bottom: 10px;
        }

        p {
            color: #6b7280;
            margin-bottom: 20px;
        }

        .form-group {
            text-align: left;
            margin-bottom: 15px;
        }

        label {
            display: block;
            color: #374151;
            font-weight: 600;
            margin-bottom: 5px;
        }

        input[type="text"], input[type="password"] {
            width: 100%;
            padding: 10px;
            border-radius: 6px;
            border: 1px solid #d1d5db;
            background-color: #f9fafb;
        }

        .btn {
            background-color: #001eff;
            color: white;
            border: solid 1px #001eff ;
            border-radius: 6px;
            padding: 10px;
            width: 100%;
            font-size: 16px;
            cursor: pointer;
            margin-top: 10px;
        }

        .btn:hover {
            background-color: white;
            border: solid 1px #001eff  ;
            color:#001eff;
        }

        .forgot {
            display: block;
            text-align: right;
            font-size: 14px;
            margin-top: 5px;
            color: #4F46E5;
            text-decoration: none;
        }

        .forgot:hover {
            text-decoration: underline;
        }

        hr {
            margin: 25px 0;
            border: 0;
            border-top: 1px solid #e5e7eb;
        }

        .signup-link {
            background-color: #001eff;
            color: white;
            border: solid 1px #001eff ;
            border-radius: 6px;
            padding: 10px;
            width: 100%;
            font-size: 16px;
            cursor: pointer;
            margin-top: 10px;
            text-decoration: none;
        }

        .signup-link:hover {
            background-color: white;
            border: solid 1px #001eff  ;
            color:#001eff;
        }

        .error {
            color: red;
            margin-bottom: 10px;
            font-size: 14px;
        }
    </style>

    <div class="login-container">
        <h2>Welcome Back</h2>
        <p>Sign in to your Sea Learner account</p>

        <asp:Label ID="lblError" runat="server" CssClass="error"></asp:Label>

        <div class="form-group">
            <label for="txtEmail">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input" placeholder="Enter your email"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="txtPassword">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
        </div>

        <asp:Button ID="btnSignIn" runat="server" CssClass="btn" Text="Sign In" OnClick="btnSignIn_Click" />

        <a href="#" class="forgot">Forgot Password?</a>

        <hr />
        <p>Don't have an account?</p>
        <a href="sign_up.aspx" class="signup-link">Sign Up</a>
    </div>
</asp:Content>
