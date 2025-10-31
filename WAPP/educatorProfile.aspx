<%@ Page Title="Educator Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="educatorProfile.aspx.cs"
    Inherits="WAPP.educatorProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        html {
            scrollbar-width: none; /* Firefox */
        }

            html::-webkit-scrollbar {
                display: none; /* Chrome, Safari, Edge */
            }

        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #eaf0ff, #ffffff);
        }

        /* 🚀 NEW: Profile Panel Container */
        .profile-panel {
            background-color: white;
            border-radius: 12px;
            padding: 24px;
            /* Consistent soft glow shadow */
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
            max-width: 1000px;
            margin: 40px auto 20px; /* Positions panel below master page header */
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

        /* ===== PROFILE HEADER (Adjusted for panel) ===== */
        .profile-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            /* 🛑 REMOVED EXTERNAL MARGINS as it's now inside .profile-panel */
            margin: 0;
            max-width: 100%;
        }

        .profile-info {
            display: flex;
            align-items: center;
            gap: 20px; /* Slightly increased gap for better spacing */
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
            border: 4px solid #f3f4f6; /* Slightly thicker border for definition */
            object-fit: cover;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);
        }

        .file-upload::file-selector-button {
            background: #001eff;
            color: white;
            padding: 1px 8px; /* 🔹 Much smaller size */
            font-size: 10px; /* 🔹 Tiny text */
            border: 1px solid #001eff;
            border-radius: 4px; /* 🔹 Subtle curve */
            cursor: pointer;
            font-weight: 600;
            box-shadow: 0 2px 6px rgba(0, 30, 255, 0.15);
            transition: all 0.2s ease;
        }

        .file-upload {
            font-size: 10px; /* 🔹 Shrinks BOTH the label and text */
            width: 100px; /* 🔹 Makes the entire input narrower */
            height: 23px; /* 🔹 Tiny height */
            padding: 2px; /* 🔹 Compact spacing */
            overflow: hidden; /* Prevents long text overflow */
        }

            .file-upload:hover::file-selector-button {
                transform: translateY(-1px);
                background: white;
                color: #001eff;
                box-shadow: 0 3px 8px rgba(0, 30, 255, 0.25);
            }

            .file-upload:hover {
                /* Apply the hover effect/shadow to the visible area */
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

        .header-actions {
            text-align: right;
            /* 🚀 FIX: Ensure buttons are vertically centered and laid out correctly */
            display: flex;
            align-items: center;
            gap: 10px;
        }

        /* ===== BUTTONS WITH HOVER EFFECTS (DASHBOARD STYLE) ===== */
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
        /* ===== END BUTTONS ===== */


        /* ===== SECTION (Added soft shadow for consistency) ===== */
        .section {
            background: #fff;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* Soft glow shadow */
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

        /* ===== INPUTS & DROPDOWNS (Added focus style) ===== */
        input, select {
            width: 100%;
            padding: 10px; /* Slightly more padding */
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

            input[readonly], select:disabled {
                cursor: not-allowed;
                background: #f3f4f6;
                border-color: #e5e7eb;
            }
        /* ===== END INPUTS ===== */

        /* ===== STATS (Match Dashboard look) ===== */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 16px;
            max-width: 700px; /* Constrain grid size */
            margin: 0 auto;
        }

        .stat-card {
            background: #fff; /* White background for better contrast */
            border-radius: 10px;
            padding: 20px;
            text-align: center;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* Soft glow shadow */
            border: none;
        }

        .stat-number {
            font-size: 32px; /* Increased size to match dashboard */
            font-weight: 700;
            color: #111827;
        }

        .stat-label {
            color: #001eff; /* Make label blue */
            font-weight: 600;
            font-size: 14px;
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
                    <h1>
                        <asp:Label ID="lblFullName" runat="server" Text="Demo Educator"></asp:Label></h1>
                    <p>
                        <span class="role-badge">Educator</span>
                        ID:
                    <asp:Label ID="lblEducatorId" runat="server" Text="E1001"></asp:Label>
                    </p>
                </div>
            </div>

            <div class="header-actions">
                <asp:Button ID="btnEdit" runat="server" Text="Edit Profile" CssClass="btn-base btn-primary" OnClick="btnEdit_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-base btn-secondary" Visible="false" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>
    <div class="page-wrap">
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


                <!-- 🆕 Added Email field (read-only) -->
                <div>
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" ReadOnly="true" />
                </div>
                <div>
                    <label>Graduated University</label>
                    <asp:TextBox ID="txtUniversity" runat="server" ReadOnly="true" />
                </div>
            </div>
            <br />
            <div style="text-align: center;">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-base btn-primary" Visible="false" OnClick="btnSave_Click" />
            </div>
            <br />
            <asp:Label ID="lblMsg" runat="server" ForeColor="Green" Style="display: block; text-align: center;" />
        </div>

        <div class="section">
            <h3>Teaching Statistics</h3>
            <div class="muted">Your impact as an educator</div>
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblTotalStudents" runat="server" Text="4" />
                    </div>
                    <div class="stat-label">Total Students</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblCoursesCreated" runat="server" Text="19" />
                    </div>
                    <div class="stat-label">Courses Created</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblCompletions" runat="server" Text="4" />
                    </div>
                    <div class="stat-label">Student Course Completed</div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
