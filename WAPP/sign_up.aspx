<%@ Page Title="Sign Up" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="sign_up.aspx.cs" Inherits="WAPP.sign_up" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #eaf0ff, #ffffff);
        }

        html::-webkit-scrollbar {
            display: none;
        }

        .btnSignIn:hover::after, .btnSignUp:hover::after {
            transform: translate(-50%, -50%) scale(6); 
            opacity: 1;
        }

        .signup-container {
            padding: 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            width: 400px;
            margin: 80px auto;
            text-align: center;
            background: rgba(255, 255, 255, 0.25); 
            border-radius: 12px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px); 
            border: 1px solid rgba(255, 255, 255, 0.3);
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
            text-align:left
        }

        input[type="text"], input[type="password"], select {
            width: 100%;
            padding: 10px;
            border-radius: 6px;
            border: 1px solid #d1d5db;
            background-color: #f9fafb;
            box-sizing: border-box;
            max-width: none;   
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
          width: 300px;
        }

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

        .studentSection, .educatorSection {
            transition: all 0.5s ease;
            opacity: 0;
            transform: scaleY(0);
            transform-origin: top;
            height: 0;
            overflow: hidden;
        }

        .studentSection.visible, .educatorSection.visible {
            opacity: 1;
            transform: scaleY(1);
            height: auto;
            overflow: visible;
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

        <asp:Panel ID="studentSection" runat="server" Visible="false" CssClass="studentSection">
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

        <asp:Panel ID="educatorSection" runat="server" Visible="false" CssClass="educatorSection">
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

        <script type="text/javascript">
            function scrollToSection(sectionId) {
                document.getElementById(sectionId).scrollIntoView({ behavior: 'smooth', block: 'start' });
            }

            window.onload = function () {
                var roleSelected = document.querySelector('input[name$="rblRole"]:checked');
                if (roleSelected) {
                    if (roleSelected.value === "student") {
                        scrollToSection('<%= studentSection.ClientID %>');
                    } else if (roleSelected.value === "educator") {
                        scrollToSection('<%= educatorSection.ClientID %>');
                    }
                }
            };
        </script>

        <asp:Button ID="btnCreate" runat="server" Text="Create Account" CssClass="btn" OnClick="btnCreate_Click" />

        <div class="signin-link">
            Already have an account? <a href="sign_in.aspx">Sign In</a>
        </div>

    </div>
</asp:Content>
