<%@ Page Title="Educator Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="educatorProfile.aspx.cs"
    Inherits="WAPP.educatorProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: #f8f9fc;
        }

        /* ===== MAIN PAGE ===== */
        .page-wrap {
            max-width: 1000px;
            margin: 0 auto;
            padding-bottom: 40px;
        }

        .title {
            font-size: 28px;
            font-weight: 700;
            margin-top: 10px;
        }

        .subtitle {
            color: #6b7280;
            margin-bottom: 20px;
        }

        /* ===== PROFILE HEADER ===== */
        .profile-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin: 40px auto 20px;
            max-width: 1000px;
        }

        .profile-info {
            display: flex;
            align-items: center;
            gap: 18px;
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
            border: 3px solid #f3f4f6;
            object-fit: cover;
        }

        .file-upload {
            font-size: 13px;
            width: 140px;
            padding: 4px;
            text-align: center;
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

        .header-actions {
            text-align: right;
        }

        .btn-primary {
            background: #312EFA;
            color: white;
            border: none;
            padding: 10px 18px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 600;
        }

        .btn-secondary {
            background: #fff;
            color: #312EFA;
            border: 1px solid #312EFA;
            padding: 10px 18px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 600;
        }

        /* ===== SECTION ===== */
        .section {
            background: #fff;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-top: 20px;
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

        /* ===== INFO GRID ===== */
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
            padding: 8px 10px;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            background: #f9fafb;
            color: #111827;
        }

        input[readonly], select:disabled {
            cursor: not-allowed;
            background: #f3f4f6;
        }

        /* ===== STATS ===== */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 16px;
        }

        .stat-card {
            background: #f9fafb;
            border-radius: 12px;
            padding: 20px;
            text-align: center;
        }

        .stat-number {
            font-size: 28px;
            font-weight: 700;
        }

        .stat-label {
            color: #6b7280;
        }
    </style>

    <!-- ===== PROFILE HEADER (Single clean version) ===== -->
    <div class="profile-header">
        <div class="profile-info">
            <div class="profile-pic-container">
                <asp:Image ID="imgProfile" runat="server" CssClass="img-profile" />
                <asp:FileUpload ID="fileUploadProfile" runat="server" CssClass="file-upload" />
            </div>
            <div class="profile-info-text">
                <h1><asp:Label ID="lblFullName" runat="server" Text="Demo Educator"></asp:Label></h1>
                <p>
                    <span class="role-badge">Educator</span>
                    ID: <asp:Label ID="lblEducatorId" runat="server" Text="E1001"></asp:Label>
                </p>
            </div>
        </div>

        <div class="header-actions">
            <asp:Button ID="btnEdit" runat="server" Text="Edit Profile" CssClass="btn-primary" OnClick="btnEdit_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" Visible="false" OnClick="btnCancel_Click" />
        </div>
    </div>

    <!-- ===== PAGE BODY ===== -->
    <div class="page-wrap">
        <div class="title">Profile</div>
        <div class="subtitle">Manage your account information</div>

        <!-- PERSONAL INFO -->
        <div class="section">
            <h3>Personal Information</h3>
            <div class="muted">Update your personal details</div>

            <div class="info-grid">
                <div>
                    <label>Full Name</label>
                    <asp:TextBox ID="txtFullName" runat="server" ReadOnly="true" />
                </div>
                <div>
                    <label>Age</label>
                    <asp:TextBox ID="txtAge" runat="server" ReadOnly="true" />
                </div>
                <div>
                    <label>Gender</label>
                    <asp:DropDownList ID="ddlGender" runat="server" Enabled="false">
                        <asp:ListItem Text="Male" Value="Male" />
                        <asp:ListItem Text="Female" Value="Female" />
                        <asp:ListItem Text="Other" Value="Other" />
                    </asp:DropDownList>
                </div>
                <div>
                    <label>Education Qualification</label>
                    <asp:DropDownList ID="ddlQualification" runat="server" Enabled="false">
                        <asp:ListItem Text="Bachelor's Degree" Value="Bachelor's Degree" />
                        <asp:ListItem Text="Master's Degree" Value="Master's Degree" />
                        <asp:ListItem Text="PhD in Computer Science" Value="PhD in Computer Science" />
                        <asp:ListItem Text="Diploma" Value="Diploma" />
                    </asp:DropDownList>
                </div>
                <div>
                    <label>Graduated University</label>
                    <asp:TextBox ID="txtUniversity" runat="server" ReadOnly="true" />
                </div>
            </div>
            <br />
            <div style="text-align: center;">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-primary" Visible="false" OnClick="btnSave_Click" />
            </div>
            <br />
            <asp:Label ID="lblMsg" runat="server" ForeColor="Green" Style="display: block; text-align: center;" />
        </div>

        <!-- STATS -->
        <div class="section">
            <h3>Teaching Statistics</h3>
            <div class="muted">Your impact as an educator</div>
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblTotalStudents" runat="server" Text="4" /></div>
                    <div class="stat-label">Total Students</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblCoursesCreated" runat="server" Text="19" /></div>
                    <div class="stat-label">Courses Created</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblCompletions" runat="server" Text="4" /></div>
                    <div class="stat-label">Course Completions</div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
