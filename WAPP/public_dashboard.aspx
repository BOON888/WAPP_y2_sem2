<%@ Page Title="Sea Learner" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="public_dashboard.aspx.cs" Inherits="WAPP.sing_in" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, #eaf0ff, #ffffff);
        }

        html {
          scrollbar-width: none; /* Firefox */
        }

        html::-webkit-scrollbar {
          display: none; /* Chrome, Safari, Edge */
        }

        .btnSignIn, .btnSignUp {
            position: relative;
            color: white;
            text-decoration: none;
            font-size: 16px;
            padding: 10px;
            overflow: hidden; /* hides the wave box */
            transition: color 0.3s ease;
        }

        .btnSignIn::after, .btnSignUp::after {
            content: "";
            position: absolute;
            left: 50%;
            top: 50%;
            width: 15px;
            height: 8px;
            background: rgba(255, 255, 255, 0.3);
            transform: translate(-50%, -50%) scale(0);
            transition: transform 0.6s ease, opacity 0.6s ease;
            opacity: 0;
            border-radius: 2px; /* makes corners slightly soft, optional */
        }

        .btnSignIn:hover, .btnSignUp:hover {
            color: black;
        }

        .btnSignIn:hover::after, .btnSignUp:hover::after {
            transform: translate(-50%, -50%) scale(6); /* expands outward like a wave */
            opacity: 1;
        }

        .text-center {
            text-align: center;
        }

        .btn {
          background-color: #001eff;
          color: white;
          border: none;
          border-radius: 6px;
          padding: 10px 20px;
          font-size: 16px;
          cursor: pointer;
          margin: 5px;
          transition: background-color 0.3s ease, color 0.3s ease, transform 0.25s ease, box-shadow 0.25s ease;
        }

        .btn-outline {
          background-color: white;
          border: 1px solid #001eff;
          color: #001eff;
        }

        .btn:hover {
          transform: translateY(-2px);
          box-shadow: 0 6px 15px rgba(0, 30, 255, 0.25);
          opacity: 0.95;
        }

        /* Optional: Make active click feel responsive */
        .btn:active {
          transform: translateY(0);
          box-shadow: 0 3px 8px rgba(0, 30, 255, 0.2);
          opacity: 1;
        }

        .section {
          max-width: 900px;
          margin: 40px auto;
          padding: 30px;
          background: rgba(255, 255, 255, 0.25); /* half-transparent white */
          border-radius: 12px;
          box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* soft blue shadow */
          backdrop-filter: blur(10px); /* frosted glass effect */
          -webkit-backdrop-filter: blur(10px); /* Safari support */
          border: 1px solid rgba(255, 255, 255, 0.3); /* subtle border for glass look */
        }

        /* ===== Carousel ===== */
        .carousel {
            position: relative;
            overflow: hidden;
            width: 100%;
            max-width: 700px;
            height: 300px;
            margin: 0 auto;
            border-radius: 12px;
        }

        .carousel-item {
            opacity: 0;
            visibility: hidden;
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-size: cover;
            background-position: center;
            border-radius: 12px;
            transition: opacity 1s ease-in-out, visibility 1s ease-in-out;
        }

        .carousel-item.active {
            opacity: 1;
            visibility: visible;
            z-index: 2;
        }

        .carousel-fade-overlay {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: #374151;
            opacity: 0;
            transition: opacity 0.8s ease-in-out;
            z-index: 3;
            pointer-events: none;
            border-radius: 12px;
        }

        .overlay {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(0, 0, 0, 0.4);
            border-radius: 12px;
            z-index: 1;
        }

        .carousel-text {
            position: absolute;
            bottom: 20px;
            left: 20px;
            color: #fff;
            z-index: 2;
            max-width: 80%;
            opacity: 0;
            transform: translateY(15px);
            transition: opacity 1s ease, transform 1s ease;
        }

        .carousel-item.active .carousel-text {
            opacity: 1;
            transform: translateY(0);
        }

        .carousel-text h3 {
            font-size: 1.5rem;
            line-height: 1.4;
            color: #fff;
            text-shadow: 0 2px 6px rgba(0, 0, 0, 0.5);
        }

        .stats-box {
            background: rgba(255, 255, 255, 0.25); /* half-transparent white */
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 4px 25px rgba(0, 30, 255, 0.25); /* soft blue shadow */
            backdrop-filter: blur(10px); /* frosted glass effect */
            -webkit-backdrop-filter: blur(10px); /* Safari support */
            border: 1px solid rgba(255, 255, 255, 0.3); /* subtle border for glass look */
        }

        .stats-box p {
            font-size: 16px;
            margin: 5px 0;
        }

        .stats-box span {
            font-weight: bold;
            color: #001eff;
        }

        ul {
            margin-top: 10px;
            padding-left: 20px;
        }

        h1, h2, h3 {
            color: #111827;
        }
    </style>

    <!-- Welcome Section -->
    <div class="text-center" style="margin-top: 40px;">
        <h1 style="text-align:center"><strong>Welcome to Sea Learner</strong></h1>
        <h2 style="font-size: medium; color: #374151;">
            Dive into a world of knowledge with our comprehensive learning platform. Whether you're a student seeking to expand your horizons or an educator ready to share your expertise, Sea Learner provides the tools and community to make learning engaging and effective.
        </h2>
        <div style="margin-top: 20px; text-align:center;">
            <ul style="list-style-type:none; padding:0; margin:0; display:flex; justify-content:center; gap:15px;">
                <li>
                    <a class="btn" href="sign_in">Sign-in to Continue</a>
                </li>
                <li>
                    <a class="btn btn-outline" href="sign_up">Join Sea Learner</a>
                </li>
            </ul>
        </div>
    </div>

    <!-- Latest Updates Section -->
    <div class="section">
        <h2 class="text-center"><strong>Latest Updates & Opportunities</strong></h2>
        <div id="adCarousel" class="carousel">
            <div class="carousel-fade-overlay"></div> 
            <asp:Repeater ID="rptAds" runat="server">
                <ItemTemplate>
                    <div class="carousel-item" 
                         style="background-image: url('<%# ResolveUrl("~/image/" + Eval("AdImage")) %>');">
                        <div class="overlay"></div>
                        <div class="carousel-text">
                            <h3><%# Eval("AdContent") %></h3>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Carousel script -->
    <script>
        (function () {
            let index = 0;
            const items = document.querySelectorAll('.carousel-item');
            const fadeOverlay = document.querySelector('.carousel-fade-overlay');
            let intervalId = null;

            function showItem(newIndex) {
                items.forEach((item, i) => {
                    item.classList.toggle('active', i === newIndex);
                });
            }

            function nextItem() {
                // Fade to black
                fadeOverlay.style.opacity = '1';

                // After fade-out (dark), change image
                setTimeout(() => {
                    index = (index + 1) % items.length;
                    showItem(index);

                    // Fade back in
                    fadeOverlay.style.opacity = '0';
                }, 800); // fade-out duration (same as CSS)
            }

            function startCarousel() {
                if (intervalId) return;
                intervalId = setInterval(nextItem, 4000);
            }

            function stopCarousel() {
                clearInterval(intervalId);
                intervalId = null;
            }

            if (items.length > 0) {
                showItem(index);
                startCarousel();

                const carousel = document.getElementById('adCarousel');
                carousel.addEventListener('mouseenter', stopCarousel);
                carousel.addEventListener('mouseleave', startCarousel);
            }
        })();
    </script>

    <!-- About Section -->
    <div class="section">
        <h2 class="text-center"><strong>About Sea Learner</strong></h2>
        <h3><strong>Our Mission</strong></h3>
        <p>
            Sea Learner is dedicated to creating an inclusive, interactive learning environment where knowledge flows freely between educators and students. We believe that education should be accessible, engaging, and rewarding for everyone.
        </p>

        <h3><strong>What We Offer</strong></h3>
        <ul>
            <li>Interactive courses with quizzes and progress tracking</li>
            <li>Community-driven learning with Q&A forums</li>
            <li>Gamified experience with coins and badges</li>
            <li>Professional development for educators</li>
            <li>Comprehensive leaderboards and achievements</li>
        </ul>

        <div class="stats-box" style="margin-top: 20px;">
            <h3 style="text-align:center">Join Our Community</h3>
            <p>Active Students: <span><asp:Label ID="lblStudentCount" runat="server" Text="0"></asp:Label>+</span></p>
            <p>Expert Educators: <span><asp:Label ID="lblEducatorCount" runat="server" Text="0"></asp:Label>+</span></p>
            <p>Courses Available: <span><asp:Label ID="lblCourseCount" runat="server" Text="0"></asp:Label>+</span></p>
        </div>
    </div>
</asp:Content>
