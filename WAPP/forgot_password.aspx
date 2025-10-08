<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="forgot_password.aspx.cs" Inherits="WAPP.forgot_password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .signup-container {
            background-color: white;
            border-radius: 12px;
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 400px;
            margin: 80px auto;
            text-align: center;
        }

        .btn {
            background-color: #001eff;
            color: white;
            border: solid 1px #001eff;
            border-radius: 6px;
            padding: 10px;
            width: 100%;
            font-size: 16px;
            cursor: pointer;
            margin-top: 10px;
        }

        .btn:hover {
            background-color: white;
            border: solid 1px #001eff;
            color: #001eff;
        }

        .btn-back {
            background-color: white;
            color: #001eff;
            border: solid 1px #001eff;
            border-radius: 6px;
            padding: 10px;
            width: 100%;
            font-size: 16px;
            cursor: pointer;
            margin-top: 10px;
        }

        .btn-back:hover {
            background-color: #001eff;
            color: white;
        }

        .form-group {
            margin-bottom: 20px;
            text-align: left;
        }

        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
        }

        .form-group input {
            width: 100%;
            padding: 8px;
            border-radius: 6px;
            border: 1px solid #ccc;
        }

        .hidden {
            display: none;
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
            <button type="button" class="btn-back" onclick="prevStep(1)">Back</button>
            <button type="button" class="btn" onclick="nextStep(3)">Next</button>
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
            <button type="button" class="btn-back" onclick="prevStep(2)">Back</button>
            <asp:Button ID="btnDone" runat="server" Text="Done" CssClass="btn" OnClick="btnDone_Click" />
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
