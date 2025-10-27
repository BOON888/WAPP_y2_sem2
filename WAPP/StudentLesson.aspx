<%@ Page Title="Lesson Content" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentLesson.aspx.cs" Inherits="WAPP.StudentLesson" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .lesson-view-container {
            width: 99%;
            max-width: 1900px;
            margin: 40px auto;
            background: white;
            border-radius: 10px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            padding: 30px;
        }

        .lesson-header {
            border-bottom: 2px solid #e0e0e0;
            padding-bottom: 20px;
            margin-bottom: 30px;
        }

        .lesson-title {
            font-size: 32px;
            font-weight: 700;
            color: #1a237e;
        }

        .lesson-info {
            color: #757575;
            font-size: 16px;
        }

        .lesson-content {
            min-height: 400px; 
            padding: 20px 0;
            font-size: 16px;
            line-height: 1.6;
        }
        
        /* Style for text content panel */
        .text-content-panel {
            padding: 20px;
            background-color: #f0f8ff; 
            border: 1px solid #cceeff;
            border-radius: 8px;
            margin-top: 20px;
        }

        /* Styles for embedded content (e.g., video, PDF iframe) */
        .content-embed iframe,
        .content-embed object {
            width: 100%;
            height: 500px; 
            border: 1px solid #ccc;
            border-radius: 8px;
        }
        
        .action-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-top: 1px solid #e0e0e0;
            padding-top: 20px;
            margin-top: 30px;
        }

        .back-btn {
            background:#001eff; color:white; padding:10px 18px;
            border-radius:6px; border:none; cursor:pointer;
            font-weight:600; margin-bottom:25px; transition:0.3s;
        }
        .back-btn:hover { background:#3246ff; }

        .next-btn {
            padding: 10px 18px;
            border-radius:6px; border:none; cursor:pointer;
            font-weight:600; transition:0.3s;
        }
        

        .next-btn { background:#00c497; color:white; }
        .next-btn:hover { background:#00a07a; }
        
        .error-message {
            background:#fee2e2; border:1px solid #fca5a5;
            color:#b91c1c; padding:10px; border-radius:8px;
            margin-top:15px; font-weight:500;
        }
    </style>

    <div class="lesson-view-container">
        
        <%-- Back Button remains at the top --%>
        <asp:Button ID="btnBackToCourse" runat="server" CssClass="back-btn" 
            Text="← Back to Course Content" OnClick="btnBackToCourse_Click" />

        <div class="lesson-header">
            <h1 class="lesson-title">
                Lesson <asp:Literal ID="litLessonNumber" runat="server" />: 
                <asp:Literal ID="litLessonTitle" runat="server" />
            </h1>
            <p class="lesson-info">Content Type: <asp:Literal ID="litContentType" runat="server" /></p>
        </div>

        <div class="lesson-content">
            <%-- Div for embedded content (Video/PDF iframe) --%>
            <div id="contentEmbed" runat="server" class="content-embed">
            </div>
            
            <%-- Text Content from ContentFile (Visible if ContentFile has text) --%>
            <asp:Panel ID="pnlTextContent" runat="server" CssClass="text-content-panel" Visible="false">
                <asp:Literal ID="litContentFileText" runat="server" />
            </asp:Panel>
            
            <asp:Label ID="lblError" runat="server" Visible="false" CssClass="error-message" />
        </div>

        <div class="action-footer">
            <%-- Left side is empty here, but the Back button is already at the top --%>
            <div></div> 

            <%-- Next Lesson Button remains at the bottom right --%>
            <asp:Button ID="btnNextLesson" runat="server" CssClass="next-btn" 
                Text="Next Lesson →" OnClick="btnNextLesson_Click" Visible="false" />
        </div>
    </div>
</asp:Content>