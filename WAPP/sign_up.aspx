<%@ Page Title="Sign Up" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="sign_up.aspx.cs" Inherits="WAPP.sign_up" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9fb;
        }

        .signup-container {
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

        input[type="text"], input[type="password"], select {
            width: 100%;
            padding: 10px;
            border-radius: 6px;
            border: 1px solid #d1d5db;
            background-color: #f9fafb;
            box-sizing: border-box;
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

        .role-group {
            text-align: left;
            margin-bottom: 15px;
        }

        .role-options {
            margin-top: 5px;
        }

        .role-options label {
            display: inline-block;
            margin-right: 15px;
            font-weight: normal;
            cursor: pointer;
        }

        .role-options input[type="radio"] {
            width: auto;
            margin-right: 5px;
        }

        .section-title {
            color: #111827;
            margin: 20px 0 15px 0;
            text-align: left;
            font-size: 18px;
        }

        .row {
            display: flex;
            gap: 15px;
            margin-bottom: 15px;
        }

        .col {
            flex: 1;
        }

        hr {
            margin: 25px 0;
            border: 0;
            border-top: 1px solid #e5e7eb;
        }

        .signin-link {
            display: block;
            text-align: center;
            margin-top: 15px;
            color: #6b7280;
        }

        .signin-link a {
            color: #001eff;
            text-decoration: none;
            font-weight: 600;
        }

        .signin-link a:hover {
            text-decoration: underline;
        }

        .error {
            color: red;
            margin-bottom: 10px;
            font-size: 14px;
            text-align: left;
        }

        .info-section {
            margin-top: 20px;
            text-align: left;
        }
    </style>

    <div class="signup-container">
        <h2>Join Sea Learner</h2>
        <p>Create your account to start learning</p>

        <asp:Label ID="lblMessage" runat="server" CssClass="error"></asp:Label>

        <div class="form-group">
            <label for="txtFullName">Full Name</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="input" placeholder="Enter your full name"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="txtEmail">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input" placeholder="Enter your email"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="txtPassword">Password</label>
            <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" CssClass="input" placeholder="Enter your password"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="txtConfirmPassword">Confirm Password</label>
            <asp:TextBox ID="txtConfirmPassword" TextMode="Password" runat="server" CssClass="input" placeholder="Confirm your password"></asp:TextBox>
        </div>
        
        <div>
            <label for="txtSecurityReminder" style="text-align:left">Password Recovery Questions</label>
        </div>
            
        <div class="form-group">
            <label for="txtFatherName">Father Name</label>
            <asp:TextBox ID="txtFatherName" runat="server" CssClass="input" placeholder="Enter your father name"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="txtMontherName">Mother Name</label>
            <asp:TextBox ID="txtMotherName" runat="server" CssClass="input" placeholder="Enter your mother name"></asp:TextBox>
        </div>

        <div>
            <label for="txtIdentity" style="text-align:left">Role Selection and Information</label>
        </div>

        <div class="role-group">
            <label>I am a:</label>
            <div class="role-options">
                <asp:RadioButtonList ID="rblRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblRole_SelectedIndexChanged" RepeatDirection="Horizontal">
                    <asp:ListItem Value="student" Text="Student"></asp:ListItem>
                    <asp:ListItem Value="educator" Text="Educator" style="margin-left: 15px;"></asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>

        <!-- Student Info -->
        <asp:Panel ID="studentSection" runat="server" Visible="false">
            <hr />
            <h5 class="section-title">Student Information</h5>

            <div class="form-group">
                <label for="txtSchool">School</label>
                <asp:TextBox ID="txtSchool" runat="server" CssClass="input" placeholder="Enter your school name"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="ddlSubject">Interest Subject</label>
                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="input">
                    <asp:ListItem>Computer Science</asp:ListItem>
                    <asp:ListItem>Mathematics</asp:ListItem>
                    <asp:ListItem>Physics</asp:ListItem>
                    <asp:ListItem>English</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="row">
                <div class="col">
                    <div class="form-group">
                        <label for="txtAge">Age</label>
                        <asp:TextBox ID="txtAge" runat="server" CssClass="input" placeholder="Age"></asp:TextBox>
                    </div>
                </div>
                <div class="col">
                    <div class="form-group">
                        <label for="ddlGender">Gender</label>
                        <asp:DropDownList ID="ddlGender" runat="server" CssClass="input">
                            <asp:ListItem Text="Select" Value=""></asp:ListItem>
                            <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                            <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Educator Info -->
        <asp:Panel ID="educatorSection" runat="server" Visible="false">
            <hr />
            <h5 class="section-title">Educator Information</h5>

            <div class="form-group">
                <label for="ddlQualification">Education Qualification</label>
                <asp:DropDownList ID="ddlQualification" runat="server" CssClass="input">
                    <asp:ListItem>Degree</asp:ListItem>
                    <asp:ListItem>Master</asp:ListItem>
                    <asp:ListItem>PhD</asp:ListItem>
                    <asp:ListItem>Professor</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label for="txtUniversity">Graduated University</label>
                <asp:TextBox ID="txtUniversity" runat="server" CssClass="input" placeholder="Enter your university name"></asp:TextBox>
            </div>

            <div class="row">
                <div class="col">
                    <div class="form-group">
                        <label for="txtAgeEdu">Age</label>
                        <asp:TextBox ID="txtAgeEdu" runat="server" CssClass="input" placeholder="Age"></asp:TextBox>
                    </div>
                </div>
                <div class="col">
                    <div class="form-group">
                        <label for="ddlGenderEdu">Gender</label>
                        <asp:DropDownList ID="ddlGenderEdu" runat="server" CssClass="input">
                            <asp:ListItem Text="Select" Value=""></asp:ListItem>
                            <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                            <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Button ID="btnCreate" runat="server" Text="Create Account" CssClass="btn" OnClick="btnCreate_Click" />

        <div class="signin-link">
            Already have an account? <a href="sign_in.aspx">Sign In</a>
        </div>
    </div>
</asp:Content>
