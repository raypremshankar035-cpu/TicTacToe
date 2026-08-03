# Tic-Tac-Toe

## Overview

Browser-based Tic Tac Toe application with an Angular frontend and a .NET Core API backend. You can play Two Player Mode locally against another person, or switch to Computer Mode and play against a rule-based AI. 

The backend owns all game state — the board, whose turn it is, move history, win/draw detection, and the scoreboard — so the frontend is really just a thin rendering layer over whatever the API returns.

---

## Tech Stack

* **Frontend:** Angular 21, TypeScript, standalone components, signals for state, SCSS
* **Backend:** ASP.NET Core 8 Web API (C#), split into `TicTacToe.Api` (controllers/DI/middleware) and `TicTacToe.Core` (domain logic)
* **API:** REST, JSON, Swagger/OpenAPI for interactive docs
* **Storage:** In-memory (thread-safe `ConcurrentDictionary`), scoped to the life of the backend process
* **Testing:** xUnit + FluentAssertions on the backend

---

## Features Implemented

Everything in the brief, plus a couple of small usability additions:

* **3×3 board:** Click to play, cells lock once filled
* **Modes:** Two Player Mode and Play Against Computer Mode (computer always plays O, moves right after your turn)
* **Status:** Turn indicator and status banner (whose turn / winner / draw)
* **Win detection:** Across all 8 lines (3 rows, 3 columns, 2 diagonals) with the winning cells highlighted on the board
* **Draw detection:** When the board fills with no winner
* **Move history table:** Move number, player, and row/column for every move
* **Undo Last Move:** Removes 1 move in Two Player Mode; removes the computer's move and the human move before it in Computer Mode, so it's always the human's turn again after an undo
* **Session scoreboard:** X wins / O wins / draws that update exactly once per completed game
* **Reset Game:** Board and history only (scoreboard is untouched) and a separate Reset Scoreboard action
* **Mode Switch Sync:** Switching the mode dropdown starts a fresh game in that mode immediately, so the dropdown and the active game never drift apart

---

## How to Run the Backend Locally

Requires the **.NET 8 SDK**.

1. Open `TicTacToe.sln` in Visual Studio 2022.
2. Set `TicTacToe.Api` as the startup project and press **F5**.
3. Access Swagger at `/swagger` once it's running — that's the fastest way to test the API directly without the frontend.

> **Note:** The API runs on `http://localhost:5274` by default (see `src/TicTacToe.Api/Properties/launchSettings.json` — the http profile). If you change the port, update `apiBaseUrl` in the frontend's environment file to match.

---

## How to Run the Frontend Locally

Requires **Node.js** and **npm**.

```bash
cd client
npm install
ng serve
