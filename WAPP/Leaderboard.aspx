<%@ Page Title="Leaderboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Leaderboard.aspx.cs"
    Inherits="WAPP.Leaderboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        body { font-family: 'Segoe UI', sans-serif; background-color: #f9f9fb; }
        .leaderboard-container {
            background-color: white;
            border-radius: 12px;
            padding: 40px;
            background: rgba(255, 255, 255, 0.25); /* half-transparent white */
            width: 99%;
            max-width: 1900px;
            margin: 60px auto;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* soft blue shadow */ 

            backdrop-filter: blur(10px); /* frosted glass effect */ 

            -webkit-backdrop-filter: blur(10px); /* Safari support */ 

            border: 1px solid rgba(255, 255, 255, 0.3); /* subtle border for glass look */ 
        }
        .header {
            text-align: center;
            margin-bottom: 30px;
        }
        .header h1 { color: #111827; font-size: 38px; font-weight: bold; }
        .header p { color: #6b7280; }

        .stats-grid {
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
            justify-content: center;
            margin-bottom: 40px;
        }
        .stat-card {
            background: rgba(255, 255, 255, 0.25);
            border-radius: 10px;
            
            
            text-align: center;
            flex: 1;
            min-width: 230px;
            padding: 20px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* soft blue shadow */ 

            backdrop-filter: blur(10px); /* frosted glass effect */ 

            -webkit-backdrop-filter: blur(10px); /* Safari support */ 

            border: 1px solid rgba(255, 255, 255, 0.3); /* subtle border for glass look */
        }
        .stat-card h2 { color: #001eff; font-size: 28px; margin: 0; }
        .stat-card p { color: #6b7280; margin: 5px 0 0; }

        .leaderboard-section h2 {
            color: #111827;
            margin-bottom: 10px;
        }
        .leaderboard-section p { color: #6b7280; margin-bottom: 20px; }

        .leaderboard-item {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 15px 20px;
            margin-bottom: 10px;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            background-color: #fff;
            transition: 0.3s;
        }
        .leaderboard-item:hover { box-shadow: 0 3px 8px rgba(0,0,0,0.08); }
        .highlighted { border-color: #001eff; background-color: #eef3ff; }

        .rank {
            font-size: 20px;
            font-weight: bold;
            color: #001eff;
            width: 40px;
            text-align: center;
        }
        .gold { color: #facc15; }
        .silver { color: #9ca3af; }
        .bronze { color: #d97706; }

        .avatar {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            color: white;
            font-weight: bold;
            font-size: 18px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 15px;
        }

        .student-info {
            flex: 1;
        }
        .student-name { font-weight: 600; color: #111827; }
        .badge-label {
            margin-left: 8px;
            font-size: 13px;
            padding: 3px 8px;
            border-radius: 6px;
            font-weight: 500;
        }
        .champion { background-color: #fef08a; color: #78350f; }
        .runner { background-color: #e5e7eb; color: #1f2937; }
        .third { background-color: #fde68a; color: #92400e; }

        .student-stats span {
            display: inline-block;
            margin-left: 20px;
            font-size: 14px;
            color: #374151;
        }
        .auto-style1 {
            font-size: xx-large;
        }
        /* Highlight current user row */
        .highlighted {
            background-color: #e0e7ff !important;  /* light blue */
            border: 2px solid #001eff !important;
            box-shadow: 0 0 10px rgba(0, 30, 255, 0.3);
            transition: all 0.3s ease;
        }
    </style>

    <div class="leaderboard-container">
        <div class="header">
            <h1>Leaderboard</h1>
            <p>See how you rank among other learners</p>
        </div>

        <!-- Top Stats -->
        <div class="stats-grid">
            <div class="stat-card">
                <h2><asp:Label ID="lblTotalStudents" runat="server" Text="0"></asp:Label></h2>
                <p>Total Students</p>
            </div>
            <div class="stat-card">
                <h2>#<asp:Label ID="lblRank" runat="server" Text="0"></asp:Label></h2>
                <p>Your Rank</p>
            </div>
            <div class="stat-card">
                <h2><asp:Label ID="lblBadges" runat="server" Text="0"></asp:Label></h2>
                <p>Badges Earned</p>
            </div>
            <div class="stat-card">
                <h2><asp:Label ID="lblCourses" runat="server" Text="0"></asp:Label></h2>
                <p>Courses Completed</p>
            </div>
        </div>

        <!-- Leaderboard List -->
        <div class="leaderboard-section">
            <h2>Student Rankings</h2>
            <p>Rankings are based on badges earned and courses completed</p>

            <asp:Repeater ID="rptLeaderboard" runat="server" OnItemDataBound="rptLeaderboard_ItemDataBound">
                <ItemTemplate>
                    <asp:Panel ID="pnlRow" runat="server" CssClass="leaderboard-item">
                        <div class="rank"><%# Container.ItemIndex + 1 %></div>

                        <div class="student-info">
                            <div class="student-name">
                                <%# Eval("FullName") %><asp:Literal ID="litYou" runat="server"></asp:Literal>
                                <asp:Literal ID="litBadgeLabel" runat="server"></asp:Literal>
                            </div>
                            <div class="student-meta">
                                <asp:Literal ID="litSchool" runat="server"></asp:Literal> |
                                <asp:Literal ID="litSubject" runat="server"></asp:Literal>
                            </div>
                        </div>

                        <div class="student-stats">
                            <span><%# Eval("BadgesEarned") %> Badges</span>
                            <span><%# Eval("CoursesCompleted") %> Courses</span>
                        </div>
                    </asp:Panel>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

</asp:Content>
