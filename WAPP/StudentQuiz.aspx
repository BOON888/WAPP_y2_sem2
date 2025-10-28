<%@ Page Title="Quiz" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentQuiz.aspx.cs"
    Inherits="WAPP.StudentQuiz" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body { background-color: #f9fafb; font-family: 'Segoe UI', sans-serif; }

        .quiz-container {
            width: 99%; max-width: 1900px; margin: 40px auto;
            border-radius: 10px;
            padding: 40px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);

            backdrop-filter: blur(10px);

            -webkit-backdrop-filter: blur(10px);

            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }

        .back-btn {
            background: #001eff; color: white; padding: 10px 18px;
            border-radius: 6px; border: none; cursor: pointer;
            font-weight: 600; margin-bottom: 25px; 
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }
        .back-btn:hover { background-color: white; 

            color: #001eff;  }

        .quiz-title { font-size: 28px; font-weight: 700; color: #111827; margin-bottom: 20px; }
        .question-block { 
            margin-bottom: 25px; 
            padding: 20px; 
            border-radius: 8px; 
            
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);

            backdrop-filter: blur(10px);

            -webkit-backdrop-filter: blur(10px);

            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }
        .question-text { 
            font-weight: 600; 
            margin-bottom: 15px; 
            color: #111827; 
            font-size: 1.1em;
        }

        .options-list { 
            list-style: none;
            padding: 0;
            margin: 0; 
        }
        .options-list label {
            display: block;
            padding: 10px 15px;
            margin-bottom: 5px;
            border-radius: 5px;
            cursor: pointer;
            transition: background-color 0.2s;
            border: 1px solid transparent;
        }
        .options-list label:hover {
             background-color: #f3f4f6;
             border: 1px solid #d1d5db;
        }

        .options-list input[type="radio"] { 
            margin-right: 10px; 
        }

        .btn-submit {
            background: #001eff; color: white; padding: 10px 16px;
            border: none; border-radius: 6px; cursor: pointer;
            font-weight: 600; 
            margin-top: 20px;
            transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }
        .btn-submit:hover { background-color: white; 

            color: #001eff; }

        .success-message {
            background: #e0ffe6; color: #0f5132; border: 1px solid #a3e4b0;
            padding: 10px; border-radius: 8px; margin-top: 15px; font-weight: 500;
        }

        .error-message {
            background: #fee2e2; border: 1px solid #fca5a5;
            color: #b91c1c; padding: 10px; border-radius: 8px;
            margin-top: 15px; font-weight: 500;
        }
    </style>

    <div class="quiz-container">
        <asp:Button ID="btnBack" runat="server" CssClass="back-btn" Text="← Back to Course" OnClick="btnBack_Click" />
        <h1 class="quiz-title">
            <asp:Label ID="lblQuizTitle" runat="server" Text="Loading Quiz..."></asp:Label>
        </h1>

        <asp:Repeater ID="rptQuestions" runat="server">
            <ItemTemplate>
                <div class="question-block">
                    <div class="question-text">
                        Q<%# Container.ItemIndex + 1 %>: <%# Eval("QuestionText") %>
                    </div>
                    <asp:RadioButtonList ID="rblOptions" runat="server" CssClass="options-list" />
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Button ID="btnSubmit" runat="server" CssClass="btn-submit" Text="Submit Quiz" OnClick="btnSubmit_Click" />
        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="success-message"></asp:Label>
        <asp:Label ID="lblError" runat="server" Visible="false" CssClass="error-message"></asp:Label>
    </div>
</asp:Content>