<%@ Page Title="Student Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentProfile.aspx.cs" Inherits="WAPP.StudentProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            background-color: #f9fafb;
            font-family: 'Segoe UI', sans-serif;
        }

        .profile-container {
            background: white;
            border-radius: 16px;
            padding: 50px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            width: 95%;
            max-width: 1000px;
            margin: 60px auto;
        }

        .section-card {
            background: white;
            border-radius: 16px;
            padding: 30px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 30px;
        }

        .section-card h2 {
            font-size: 20px;
            color: #111827;
            font-weight: 600;
        }

        .section-card p.subtitle {
            font-size: 14px;
            color: #6b7280;
            margin-bottom: 20px;
        }

        .profile-header {
            margin-bottom: 40px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .profile-info {
            display: flex;
            align-items: center;
        }

        .profile-pic-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            margin-right: 20px;
        }

        .profile-header h1 {
            font-size: 28px;
            color: #111827;
            margin: 0;
            font-weight: 700;
        }

        .profile-info-text .role {
            background: #e5e7eb;
            color: #4b5563;
            padding: 2px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
            margin-right: 8px;
        }

        .profile-info-text .student-id {
            font-size: 14px;
            color: #6b7280;
        }

        .btn-edit {
            background-color: #4f46e5;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 8px;
            cursor: pointer;
            transition: 0.3s;
            font-weight: 600;
        }

        .btn-edit:hover {
            background-color: #6366f1;
        }

        .form-column {
            max-width: 700px;
            margin: 0px auto 30px auto;
        }

        .form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px 30px;
            margin-bottom: 30px;
        }

        .form-col-item {
            display: flex;
            flex-direction: column;
            gap: 5px;
        }

        .form-col-item label {
            font-weight: 600;
            color: #111827;
            margin-bottom: 5px;
        }

        .form-col-item input,
        .form-col-item select {
            width: 100%;
            padding: 12px;
            border: 1px solid #d1d5db;
            border-radius: 8px;
            background: #f9fafb;
            font-size: 15px;
        }

        .save-button-container {
            max-width: 700px;
            margin: 0 auto;
            text-align: left;
        }

        .btn-save {
            background-color: #001eff;
            color: white;
            border: none;
            padding: 12px 20px;
            border-radius: 8px;
            cursor: pointer;
            transition: 0.3s;
            display: inline-block;
            margin-top: 10px;
        }

        .btn-save:hover {
            background-color: #3740ff;
        }

        .stats-section {
            display: flex;
            justify-content: center;
            gap: 40px;
            margin-top: 0;
            flex-wrap: wrap;
            max-width: 700px;
            margin: 0px auto;
        }

        .stat-box {
            flex: 1;
            min-width: 250px;
            text-align: center;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
        }

        .stat-coins {
            background-color: #fff9db;
        }

        .stat-badges {
            background-color: #dcfce7;
        }

        .stat-box h2 {
            font-size: 32px;
            color: #001eff;
            margin-bottom: 5px;
        }

        .stat-box p {
            font-weight: 600;
            color: #374151;
            font-size: 16px;
        }

        .badges-section-content {
            max-width: 700px;
            margin: 0px auto;
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

        .auto-style1 {
            border-radius: 50%;
            border: 4px solid #f3f4f6;
            object-fit: cover;
            display: block;
        }
    </style>

    <div class="profile-container">
        <div class="profile-header">
            <div class="profile-info">
                <div class="profile-pic-container">
                    <asp:Image ID="imgProfile" runat="server"
                        CssClass="auto-style1" Height="120px" Width="120px" />
                    <asp:FileUpload ID="fileUploadProfile" runat="server" style="margin-top: 5px; font-size: 12px;" />
                </div>
                <div class="profile-info-text">
                    <h1><asp:Label ID="lblUserName" runat="server" Text="Demo Student"></asp:Label></h1>
                    <p>
                        <span class="role">Student</span>
                        <span class="student-id">ID: <asp:Label ID="lblStudentId" runat="server"></asp:Label></span>
                    </p>
                </div>
            </div>

            <!-- 🔹 Smooth scroll button -->
            <button type="button" class="btn-edit" onclick="scrollToEdit()">Edit Profile</button>
        </div>

        <!-- 🔹 Anchor target for smooth scroll -->
        <a id="editSection"></a>

        <div class="section-card">
            <h2>Personal Information</h2>
            <p class="subtitle">Your account information</p>

            <div class="form-column">
                <div class="form-grid">
                    <div class="form-col-item">
                        <label for="txtFullName">Full Name</label>
                        <asp:TextBox ID="txtFullName" runat="server"></asp:TextBox>
                    </div>
                    <div class="form-col-item">
                        <label for="txtEmail">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="form-col-item">
                        <label for="txtAge">Age</label>
                        <asp:TextBox ID="txtAge" runat="server"></asp:TextBox>
                    </div>
                    <div class="form-col-item">
                        <label for="ddlGender">Gender</label>
                        <asp:DropDownList ID="ddlGender" runat="server">
                            <asp:ListItem Value="">Select Gender</asp:ListItem>
                            <asp:ListItem Value="Male">Male</asp:ListItem>
                            <asp:ListItem Value="Female">Female</asp:ListItem>
                            <asp:ListItem Value="Other">Other</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-col-item">
                        <label for="txtSchool">School</label>
                        <asp:TextBox ID="txtSchool" runat="server"></asp:TextBox>
                    </div>
                    <div class="form-col-item">
                        <label for="txtInterestSubject">Interest Subject</label>
                        <asp:TextBox ID="txtInterestSubject" runat="server"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="save-button-container">
                <asp:Button ID="btnSaveProfile" runat="server" Text="Save Changes" CssClass="btn-save" OnClick="btnSaveProfile_Click" />
            </div>
            <asp:Label ID="lblMessage" runat="server" CssClass="lblMessage" ForeColor="Green"></asp:Label>
        </div>

        <div class="section-card">
            <h2>Learning Statistics</h2>
            <p class="subtitle">Your achievements and progress</p>

            <div class="stats-section">
                <div class="stat-box stat-coins">
                    <h2><asp:Label ID="lblCoins" runat="server"></asp:Label></h2>
                    <p>Coins Earned</p>
                </div>
                <div class="stat-box stat-badges">
                    <h2><asp:Label ID="lblBadges" runat="server"></asp:Label></h2>
                    <p>Badges Earned</p>
                </div>
            </div>
        </div>

        <div class="section-card">
            <h2>Your Badges</h2>
            <p class="subtitle">List of badges you have earned</p>

            <div class="badges-section-content">
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
        </div>
    </div>

    <!-- 🔹 Smooth scroll script -->
    <script>
        function scrollToEdit() {
            document.getElementById("editSection").scrollIntoView({ behavior: 'smooth' });
        }
    </script>
</asp:Content>
