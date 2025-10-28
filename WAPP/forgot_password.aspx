<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="forgot_password.aspx.cs" Inherits="WAPP.forgot_password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #eaf0ff, #ffffff);
        }

        .btnSignIn, .btnSignUp {
            position: relative;
            color: white;
            text-decoration: none;
            font-size: 16px;
            padding: 10px;
            overflow: hidden; /* hides the wave box */
            transition: color 0.3s ease;
        }

        .btnSignIn::after, .btnSignUp::after {
            content: "";
            position: absolute;
            left: 50%;
            top: 50%;
            width: 15px;
            height: 8px;
            background: rgba(255, 255, 255, 0.3);
            transform: translate(-50%, -50%) scale(0);
            transition: transform 0.6s ease, opacity 0.6s ease;
            opacity: 0;
            border-radius: 2px; /* makes corners slightly soft, optional */
        }

        .btnSignIn:hover, .btnSignUp:hover {
            color: black;
        }

        .btnSignIn:hover::after, .btnSignUp:hover::after {
            transform: translate(-50%, -50%) scale(6); /* expands outward like a wave */
            opacity: 1;
        }

        .signup-container {
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 400px;
            margin: 80px auto;
            text-align: center;
            background: rgba(255, 255, 255, 0.25); /* half-transparent white */
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* soft blue shadow */
            backdrop-filter: blur(10px); /* frosted glass effect */
            -webkit-backdrop-filter: blur(10px); /* Safari support */
            border: 1px solid rgba(255, 255, 255, 0.3); /* subtle border for glass look */
        }

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
        }

        .btn:active {
          transform: translateY(0);
          box-shadow: 0 3px 8px rgba(0, 30, 255, 0.2);
          opacity: 1;
        }

        .form-group {
            margin-bottom: 18px;
            margin-top: 18px;
        }

        label {
            display: block;
            color: #374151;
            font-weight: 600;
            margin-bottom: 5px;
            text-align:left
        }

        input[type="text"], input[type="password"], select {
            width: 100%;
            padding: 10px;
            border-radius: 6px;
            border: 1px solid #d1d5db;
            background-color: #f9fafb;
            box-sizing: border-box;
            max-width: none;    /* remove inherited max-width if any */
        }

        .hidden {
            display: none;
        }

        h1, h2, h3 {
            color: #111827;
        }

    </style>

    <div class="signup-container">
        <h2>Forgot Password</h2>

        <!-- Step 1: Email -->
        <div id="step1">
            <div class="form-group">
                <label for="txtEmail">Registered Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input" placeholder="Enter your registered email"></asp:TextBox>
            </div>
            <button type="button" class="btn" onclick="nextStep(2)">Next</button>
        </div>

        <!-- Step 2: Father & Mother -->
        <div id="step2" class="hidden">
            <div class="form-group">
                <label for="txtFather">Father Name</label>
                <asp:TextBox ID="txtFather" runat="server" CssClass="input" placeholder="Enter your father name"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="txtMother">Mother Name</label>
                <asp:TextBox ID="txtMother" runat="server" CssClass="input" placeholder="Enter your mother name"></asp:TextBox>
            </div>
            <button type="button" class="btn" onclick="nextStep(3)">Next</button>
            <button type="button" class="btn btn-back" onclick="prevStep(1)">Back</button>
        </div>

        <!-- Step 3: New Password -->
        <div id="step3" class="hidden">
            <div class="form-group">
                <label for="txtNewPassword">New Password</label>
                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="input" TextMode="Password" placeholder="Enter new password"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="txtConfirmPassword">Confirm Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="input" TextMode="Password" placeholder="Confirm your new password"></asp:TextBox>
            </div>
            <asp:Button ID="btnDone" runat="server" Text="Done" CssClass="btn" OnClick="btnDone_Click" />
            <button type="button" class="btn btn-back" onclick="prevStep(2)">Back</button>
        </div>

        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
    </div>

    <script>
        // Handle showing/hiding steps
        function nextStep(step) {
            document.querySelectorAll('[id^="step"]').forEach(div => div.classList.add('hidden'));
            document.getElementById('step' + step).classList.remove('hidden');
        }

        function prevStep(step) {
            document.querySelectorAll('[id^="step"]').forEach(div => div.classList.add('hidden'));
            document.getElementById('step' + step).classList.remove('hidden');
        }
    </script>
</asp:Content>
