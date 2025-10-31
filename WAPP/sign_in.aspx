<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="sign_in.aspx.cs" Inherits="WAPP.sign_in" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #eaf0ff, #ffffff);
        }

        html {
          scrollbar-width: none; /* Firefox */
        }

        html::-webkit-scrollbar {
          display: none; /* Chrome, Safari, Edge */
        }

        .login-container {
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 400px;
            margin: 80px auto;
            margin-bottom: 30px;
            text-align: center;
            background: rgba(255, 255, 255, 0.25); /* half-transparent white */
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* soft blue shadow */
            backdrop-filter: blur(10px); /* frosted glass effect */
            -webkit-backdrop-filter: blur(10px); /* Safari support */
            border: 1px solid rgba(255, 255, 255, 0.3); /* subtle border for glass look */
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
            margin-bottom: 15px;
        }

        label {
            display: block;
            color: #374151;
            font-weight: 600;
            margin-bottom: 5px;
            text-align:left;
        }

        input[type="text"], input[type="password"] {
            width: 100%;
            padding: 10px;
            border-radius: 6px;
            border: 1px solid #d1d5db;
            background-color: #f9fafb;
            max-width: none;    /* remove inherited max-width if any */
        }

        .forgot {
            display: block;
            text-align: right;
            font-size: 14px;
            margin-top: 5px;
            color: #001eff;
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

        .error {
            color: red;
            margin-bottom: 10px;
            font-size: 14px;
        }

        .btnSign_In {
          background-color: #001eff;
          color: white;
          border: none;
          border-radius: 6px;
          padding: 10px 20px;
          font-size: 16px;
          cursor: pointer;
          margin: 5px;
          transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
          width: 300px;
        }

        .btn {
          background-color: #001eff;
          border: 1px solid #001eff;
          color: white;
          border-radius: 6px;
          padding: 10px 20px;
          font-size: 16px;
          cursor: pointer;
          margin: 5px;
          transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
          text-decoration: none;
          width: 300px;
        }

        /* Shared hover and active effects */
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

        .signup-link {
            display: block;
            text-align: center;
            margin-top: 15px;
            color: #6b7280;
        }

        .signup-link a {
            color: #001eff;
            text-decoration: none;
            font-weight: 600;
        }

        .signup-link a:hover {
            text-decoration: underline;
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

        <a href="forgot_password" class="forgot">Forgot Password?</a>
        <hr />
        <div class="signup-link">
            Don't have an account? <a href="sign_up.aspx">Sign Up</a>
        </div>
    </div>
</asp:Content>
