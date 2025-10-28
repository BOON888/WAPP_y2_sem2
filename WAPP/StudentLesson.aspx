<%@ Page Title="Lesson Content" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentLesson.aspx.cs" Inherits="WAPP.StudentLesson" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .lesson-view-container {
            width: 99%;
            max-width: 1900px;
            margin: 40px auto;
            
            border-radius: 10px;
            
            padding: 30px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);

            backdrop-filter: blur(10px);

            -webkit-backdrop-filter: blur(10px);

            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
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
        
        
        .text-content-panel {
            padding: 20px;
            background-color: #f0f8ff; 
            
            border-radius: 8px;
            margin-top: 20px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25);

            backdrop-filter: blur(10px);

            -webkit-backdrop-filter: blur(10px);

            border: 1px solid rgba(255, 255, 255, 0.3);
            background: rgba(255, 255, 255, 0.25);
        }

        
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
            font-weight:600; margin-bottom:25px; transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }
        .back-btn:hover { background-color: white; 

            color: #001eff;  }

        .next-btn {
            padding: 10px 18px;
            border-radius:6px; border:none; cursor:pointer;
            font-weight:600; transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }
        

        .next-btn { background:#00c497; color:white; }
        .next-btn:hover { background-color: white; 

            color: #001eff; }
        
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