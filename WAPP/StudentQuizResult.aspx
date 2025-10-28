<%@ Page Title="Quiz Result" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentQuizResult.aspx.cs"
    Inherits="WAPP.StudentQuizResult" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body { 
            background-color: #f0f2f5; 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            color: #333;
            margin: 0;
            padding: 0;

        }

        .result-wrapper {
            max-width: 800px;
            margin: 50px auto;
            padding: 30px;
            background-color: #fff;
            border-radius: 12px;
            
            text-align: center;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);

            backdrop-filter: blur(10px);

            -webkit-backdrop-filter: blur(10px);

            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }

        .quiz-title-header {
            font-size: 2.2em;
            color: #2c3e50;
            margin-bottom: 25px;
            font-weight: 700;
            border-bottom: 2px solid #e0e0e0;
            padding-bottom: 15px;
        }

        /* Status Section */
        .result-status-section {
            padding: 25px 0;
            margin-bottom: 30px;
            border-radius: 8px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            font-size: 1.6em;
            font-weight: 600;
            transition: all 0.3s ease;
        }
        .result-status-section.passed {
            background-color: #e6ffed; /* Light green */
            color: #1a6e34; /* Darker green */
            border: 1px solid #c8e6c9;
        }
        .result-status-section.failed {
            background-color: #ffe6e6; /* Light red */
            color: #b71c1c; /* Darker red */
            border: 1px solid #fbcaca;
        }
        .status-icon {
            font-size: 2.5em; /* Larger icon */
            margin-bottom: 15px;
        }

        /* Score Display */
        .score-display {
            display: flex;
            justify-content: space-around;
            gap: 20px;
            margin-bottom: 30px;
        }
        .score-card {
            flex: 1;
            background-color: #f8f9fa;
            
            border-radius: 10px;
            padding: 20px;
            
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);

            backdrop-filter: blur(10px);

            -webkit-backdrop-filter: blur(10px);

            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }
        .score-card-label {
            font-size: 1em;
            color: #7f8c8d;
            margin-bottom: 10px;
            font-weight: 500;
        }
        .score-card-value {
            font-size: 2.5em;
            font-weight: 700;
            color: #34495e;
        }

        /* Reward Section */
        .reward-panel {
            background-color: #f9fafb;
            border: 1px solid #b3e0ff;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 30px;
            
            color: #2196f3;
            font-size: 1.1em;
            font-weight: 600;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);

            backdrop-filter: blur(10px);

            -webkit-backdrop-filter: blur(10px);

            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }
        .reward-icon {
            font-size: 1.8em;
            vertical-align: middle;
            margin-right: 10px;
        }

        /* Action Buttons */
        .action-buttons {
            display: flex;
            justify-content: center;
            gap: 20px;
            margin-top: 30px;
        }
        .btn-action {
            padding: 12px 25px;
            font-size: 1.1em;
            font-weight: 600;
            border-radius: 8px;
            cursor: pointer;
            transition: background-color 0.3s ease, transform 0.2s ease;
            text-decoration: none; /* For asp:LinkButton if used, or just good practice */
            border: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
        }
        .btn-action:hover {
            transform: translateY(-2px);
        }

        .btn-back {
            background-color: #001eff;
            color: white;
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }
        .btn-back:hover {
            background-color: white; 

            color: #001eff; 
        }

        .btn-retry {
            background-color: #e67e22; /* Orange */
            color: white;
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }
        .btn-retry:hover {
            background-color: white; 

            color: #001eff; 
        }

        .error-message {
            background: #fee2e2; border: 1px solid #fca5a5;
            color: #b91c1c; padding: 10px; border-radius: 8px;
            margin-top: 15px; font-weight: 500;
            text-align: left;
        }
    </style>

    <div class="result-wrapper" id="pnlResult" runat="server"> 
        <h1 class="quiz-title-header">
            <asp:Label ID="lblQuizTitle" runat="server" Text="Quiz Results" />
        </h1>

        <%-- Dynamic Status Section --%>
        <div class="result-status-section">
            <span class="status-icon" id="statusIcon" runat="server"></span>
            <asp:Label ID="lblStatus" runat="server" Text="Status Loading..." />
        </div>

        <div class="score-display">
            <div class="score-card">
                <div class="score-card-label">Correct Answers</div>
                <div class="score-card-value">
                    <asp:Literal ID="litScore" runat="server" />
                </div>
            </div>
            <div class="score-card">
                <div class="score-card-label">Percentage Score</div>
                <div class="score-card-value">
                    <asp:Literal ID="litPercentage" runat="server" />
                </div>
            </div>
        </div>

        <%-- Reward/Message Panel --%>
        <asp:Panel ID="pnlReward" runat="server" CssClass="reward-panel" Visible="false">
            <span class="reward-icon"></span>
            <asp:Literal ID="litRewardMessage" runat="server" />
        </asp:Panel>

        <%-- Action Buttons --%>
        <div class="action-buttons">
            <asp:Button ID="btnBackToCourse" runat="server" Text="← Back to Course Content" 
                CssClass="btn-action btn-back" OnClick="btnBackToCourse_Click" />
            
            <asp:Button ID="btnRetry" runat="server" Text="Retry Quiz" 
                CssClass="btn-action btn-retry" OnClick="btnRetry_Click" Visible="false" />
        </div>
    </div>
    
    <asp:Label ID="lblError" runat="server" Visible="false" CssClass="error-message" />
</asp:Content>