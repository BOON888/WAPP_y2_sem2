<%@ Page Title="Student Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentProfile.aspx.cs" Inherits="WAPP.StudentProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        html {
            scrollbar-width: none;
        }
        html::-webkit-scrollbar {
            display: none;
        }

        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #eaf0ff, #ffffff);
        }

        .profile-panel {
            background-color: white;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            max-width: 1000px;
            margin: 40px auto 20px;
        }

        .profile-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin: 0;
        }

        .profile-info {
            display: flex;
            align-items: center;
            gap: 20px;
        }

        .profile-pic-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 6px;
            min-width: 120px;
        }

        .img-profile {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            border: 4px solid #f3f4f6;
            object-fit: cover;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
        }

        .file-upload::file-selector-button {
            background: #001eff;
            color: white;
            padding: 1px 8px;
            font-size: 10px;
            border: 1px solid #001eff;
            border-radius: 4px;
            cursor: pointer;
            font-weight: 600;
            box-shadow: 0 2px 6px rgba(0, 30, 255, 0.15);
            transition: all 0.2s ease;
        }

        .file-upload {
            font-size: 10px;
            width: 100px;
            height: 23px;
            padding: 2px;
            overflow: hidden;
        }

        .file-upload:hover::file-selector-button {
            transform: translateY(-1px);
            background: white;
            color: #001eff;
            box-shadow: 0 3px 8px rgba(0, 30, 255, 0.25);
        }

        .file-upload:hover {
            box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
            border-color: #001eff;
        }

        .profile-info-text h1 {
            font-size: 28px;
            color: #111827;
            margin: 0;
            font-weight: 700;
        }

        .profile-info-text p {
            margin: 4px 0;
            color: #6b7280;
            font-size: 14px;
        }

        .role-badge {
            background: #e5e7eb;
            color: #4b5563;
            padding: 2px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
            margin-right: 8px;
        }

        .btn-base {
            padding: 10px 18px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 600;
            transition: transform 0.25s ease, box-shadow 0.25s ease, background-color 0.25s ease, color 0.25s ease;
            border: none;
            margin-left: 10px;
        }

        .btn-primary {
            background: #001eff;
            color: white;
            border: 1px solid #001eff;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
            background: white;
            color: #001eff;
        }

        .btn-secondary {
            background: #fff;
            color: #001eff;
            border: 1px solid #001eff;
        }

        .btn-secondary:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
            background: #001eff;
            color: white;
        }

        .section {
            background: #fff;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            margin: 20px auto;
            max-width: 1000px;
        }

        .section h3 {
            font-size: 18px;
            font-weight: 700;
            margin-bottom: 10px;
            text-align: center;
        }

        .muted {
            color: #6b7280;
            margin-bottom: 12px;
            text-align: center;
        }

        .info-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
            max-width: 700px;
            margin: 0 auto;
        }

        label {
            font-weight: 600;
            margin-bottom: 4px;
            display: block;
            color: #111827;
        }

        input, select {
            width: 100%;
            padding: 10px;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            background: #f9fafb;
            color: #111827;
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

        input:focus, select:focus {
            outline: none;
            border-color: #001eff;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            background: white;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 16px;
            max-width: 700px;
            margin: 0 auto;
        }

        .stat-card {
            background: #fff;
            border-radius: 10px;
            padding: 20px;
            text-align: center;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
        }

        .stat-number {
            font-size: 32px;
            font-weight: 700;
            color: #111827;
        }

        .stat-label {
            color: #001eff;
            font-weight: 600;
            font-size: 14px;
        }

        .badge-item {
            background: linear-gradient(to right, #eef2ff, #f0f9ff);
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 15px;
            display: flex;
            align-items: center;
            gap: 15px;
            max-width: 700px;
            margin: 10px auto;
        }

        .badge-icon {
            font-size: 28px;
        }

        .lblMessage {
            text-align: center;
            display: block;
            margin-top: 10px;
            font-weight: 500;
        }
    </style>

    <div class="profile-panel">
        <div class="profile-header">
            <div class="profile-info">
                <div class="profile-pic-container">
                    <asp:Image ID="imgProfile" runat="server" CssClass="img-profile" />
                    <asp:FileUpload ID="fileUploadProfile" runat="server" CssClass="file-upload" Visible="false" />
                </div>
                <div class="profile-info-text">
                    <h1><asp:Label ID="lblUserName" runat="server" Text="Demo Student"></asp:Label></h1>
                    <p>
                        <span class="role-badge">Student</span>
                        ID: <asp:Label ID="lblStudentId" runat="server"></asp:Label>
                    </p>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnEdit" runat="server" Text="Edit Profile" CssClass="btn-base btn-primary" OnClick="btnEdit_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-base btn-secondary" Visible="false" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>

    <div class="section">
        <h3>Personal Information</h3>
        <div class="muted">Your account information</div>
        <div class="info-grid">
            <div>
                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" ReadOnly="true" />
            </div>
            <div>
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" ReadOnly="true" />
            </div>
            <div>
                <label>Age</label>
                <asp:TextBox ID="txtAge" runat="server" ReadOnly="true" />
            </div>
            <div>
                <label>Gender</label>
                <asp:DropDownList ID="ddlGender" runat="server" Enabled="false">
                    <asp:ListItem Value="">Select Gender</asp:ListItem>
                    <asp:ListItem Value="Male">Male</asp:ListItem>
                    <asp:ListItem Value="Female">Female</asp:ListItem>
                    <asp:ListItem Value="Other">Other</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div>
                <label>School</label>
                <asp:TextBox ID="txtSchool" runat="server" ReadOnly="true" />
            </div>
            <div>
                <label>Interest Subject</label>
                <asp:TextBox ID="txtInterestSubject" runat="server" ReadOnly="true" />
            </div>
        </div>
        <br />
        <div style="text-align:center;">
            <asp:Button ID="btnSaveProfile" runat="server" Text="Save Changes" CssClass="btn-base btn-primary" Visible="false" OnClick="btnSaveProfile_Click" />
        </div>
        <asp:Label ID="lblMessage" runat="server" CssClass="lblMessage" ForeColor="Green"></asp:Label>
    </div>

    <div class="section">
        <h3>Learning Statistics</h3>
        <div class="muted">Your achievements and progress</div>
        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblCoins" runat="server"></asp:Label></div>
                <div class="stat-label">Coins Earned</div>
            </div>
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblBadges" runat="server"></asp:Label></div>
                <div class="stat-label">Badges Earned</div>
            </div>
        </div>
    </div>

    <div class="section">
        <h3>Your Badges</h3>
        <div class="muted">List of badges you have earned</div>
        <asp:Repeater ID="rptBadges" runat="server">
            <ItemTemplate>
                <div class="badge-item">
                    <div class="badge-icon">🏅</div>
                    <div>
                        <strong><%# Eval("BadgeEarned") %></strong><br />
                        <span style="color:#6b7280;"><%# Eval("CourseTitle") %></span>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
