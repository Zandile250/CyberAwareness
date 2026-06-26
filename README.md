CyberAwareness 
This is a cyber security awareness project I built using C#.
It includes a chatbot, a mini‑game, a quiz, and a SQL export + activity log system.
The goal is to teach users about safe online behaviour in a fun and interactive way.

** What the Chatbot Can Do
Greets you with your name

Shows a list of cyber security topics

Lets you type your own questions

Types out responses like a real person

Checks your input so the program doesn’t crash

Uses colours and simple graphics to make learning fun

** Mini‑Game
A simple game you can play inside the console

Keeps track of your score

Saves your score to the database

Logs the activity so you can see what happened later

** Cyber Security Quiz
Multiple‑choice questions

Calculates your score

Saves the results to SQL

Logs that you completed the quiz

🗄 SQL Export Checks
Before exporting results, the system checks:

If quiz results exist

If mini‑game results exist

If both are ready for export

This prevents exporting empty or missing data.

** Activity Log
The system records everything important, such as:

When you finish the quiz

When you finish the mini‑game

When you try to export

Errors or invalid inputs

Chatbot interactions

All logs are saved in the ActivityLog table.

Source Code
All the source code for the CyberAwareness project is included in this repository.

Files in the Project
ActivityLog.cs – Records user actions like quiz completion, mini‑game results, and exports.

Chatbot.cs – Handles chatbot responses and user interaction.

DatabaseHelper.cs – Connects to the SQL database and manages saving/exporting data.

Form1.cs – The main graphical interface (GUI) for the application.

Program.cs – Starts the application and controls the main flow.

QuizEngine.cs – Manages quiz questions, scoring, and feedback.

Sound_wav.wav – Adds sound effects for a more interactive experience.

** Continuous Integration (CI)
A GitHub Actions workflow automatically builds the project every time I push changes.
This helps me make sure the project compiles and meets the assignment requirements.

A screenshot of a successful workflow run is included in the repo.

** How to Use
Run the program

Type your name

Choose:

Chatbot

Mini‑game

Quiz

Export results

Learn about cyber security in a fun way

 Author
Zandile G
