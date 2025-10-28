<%@ Page Title="Educator Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="educatorProfile.aspx.cs"
    Inherits="WAPP.educatorProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: #f8f9fc;
        }

        /* ===== HEADER ===== */
        .top-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin: 20px;
        }

        .left-panel, .right-panel {
            background-color: #312EFA;
            color: white;
            padding: 8px 16px;
            border-radius: 8px;
            font-weight: 600;
        }

        .back-link {
            color: white;
            text-decoration: none;
        }

            .back-link:hover {
                opacity: 0.8;
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

        /* ===== PROFILE CARD ===== */
        .profile-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            padding: 24px;
            text-align: center;
            margin-bottom: 20px;
        }

        .profile-image {
            width: 100px;
            height: 100px;
            background: linear-gradient(135deg, #667eea, #764ba2);
            color: white;
            font-size: 36px;
            border-radius: 50%;
            display: flex;
            justify-content: center;
            align-items: center;
            margin: 0 auto 12px;
        }

        .badge {
            display: inline-block;
            background: #312EFA;
            color: white;
            font-size: 13px;
            font-weight: 600;
            padding: 3px 10px;
            border-radius: 6px;
            margin-bottom: 6px;
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
        }

        .muted {
            color: #6b7280;
            margin-bottom: 12px;
            text-align: center;
        }

        /* ===== INFO GRID CENTERED ===== */
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

        /* ===== BUTTONS ===== */
        .btn-primary {
            background: #312EFA;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 600;
        }

        .btn-secondary {
            background: #fff;
            color: #312EFA;
            border: 1px solid #312EFA;
            padding: 8px 16px;
            border-radius: 8px;
            cursor: pointer;
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

    <!-- HEADER -->
    <div class="top-header">
        <div class="left-panel">
            <a href="Educator_dashboard.aspx" class="back-link">Back to Dashboard</a>
        </div>
        <div class="right-panel">Educator</div>
    </div>

    <!-- PAGE BODY -->
    <div class="page-wrap">
        <div class="title">Profile</div>
        <div class="subtitle">Manage your account information</div>

        <!-- PROFILE CARD -->
        <div class="profile-card">
            <div class="profile-image" id="profileInitial">D</div>
            <div class="badge">Educator</div>
            <div style="font-size:18px;font-weight:700;margin-top:5px;">
                <asp:Label ID="lblFullName" runat="server" Text="Demo Educator" />
            </div>
            <div style="color:#6b7280;">
                <asp:Label ID="lblEmail" runat="server" Text="educator@demo.com" />
            </div>
            <br />
            <asp:Button ID="btnEdit" runat="server" Text="Edit Profile" CssClass="btn-primary" OnClick="btnEdit_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" Visible="false" OnClick="btnCancel_Click" />
        </div>

        <!-- PERSONAL INFO -->
        <div class="section">
            <h3 style="text-align:center;">Personal Information</h3>
            <div class="muted">Update your personal details</div>

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
            <div style="text-align:center;">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-primary" Visible="false" OnClick="btnSave_Click" />
            </div>
            <br />
            <asp:Label ID="lblMsg" runat="server" ForeColor="Green" Style="display:block;text-align:center;" />
        </div>

        <!-- STATS -->
        <div class="section">
            <h3 style="text-align:center;">Teaching Statistics</h3>
            <div class="muted">Your impact as an educator</div>
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number"><asp:Label ID="lblTotalStudents" runat="server" Text="4" /></div>
                    <div class="stat-label">Total Students</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number"><asp:Label ID="lblCoursesCreated" runat="server" Text="19" /></div>
                    <div class="stat-label">Courses Created</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number"><asp:Label ID="lblCompletions" runat="server" Text="4" /></div>
                    <div class="stat-label">Course Completions</div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
