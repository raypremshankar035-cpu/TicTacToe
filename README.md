# Tic Tac Toe 

## Overview
Browser-based Tic Tac Toe application with an Angular frontend and a .NET Core API backend. You can play Two Player Mode locally against another person, or switch to Computer Mode and play against a rule-based AI. 

The backend owns all game state — the board, whose turn it is, move history, win/draw detection, and the scoreboard 

## Tech Stack

- **Frontend** — Angular 21, TypeScript, standalone components, signals for state, SCSS
- **Backend** — ASP.NET Core 8 Web API (C#), split into `TicTacToe.Api` (controllers/DI/middleware) and `TicTacToe.Core` (domain logic)
- **API** — REST, JSON, Swagger/OpenAPI for interactive docs
- **Storage** — in-memory (thread-safe `ConcurrentDictionary`), scoped to the life of the backend process
- **Testing** — xUnit + FluentAssertions on the backend(TicTacToe.Tests)
- **IDE** — Visual Studio 2022 & Visual Studio Code

## Features Implemented
- 3×3 board, click to play, cells lock once filled
- Two Player Mode and Play Against Computer Mode (computer always plays O, moves right after your turn)
- Turn indicator and status banner (whose turn / winner / draw)
- Win detection across all 8 lines (3 rows, 3 columns, 2 diagonals) with the winning cells highlighted on the board
- Draw detection when the board fills with no winner
- Move history table — move number, player, and row/column for every move
- Undo Last Move — removes 1 move in Two Player Mode; removes the computer's move and the human move before it in Computer Mode, so it's always the human's turn again after an undo
- Session scoreboard (X wins / O wins / draws) that updates exactly once per completed game
- Reset Game (board and history only — scoreboard is untouched) and a separate Reset Scoreboard action
- Switching the mode dropdown starts a fresh game in that mode immediately, so the dropdown and the game you're actually playing can never drift apart

## How to Run the Backend Locally
- Requires the .NET 8 SDK.
- open `TicTacToe.sln` in Visual Studio 2022, set `TicTacToe.Api` as the startup project, and press F5. Swagger is at `/swagger` once it's running — that's the fastest way to poke at the API directly without the frontend.

- The API runs on `http://localhost:5274` by default (see `src/TicTacToe.Api/Properties/launchSettings.json`, the `http` profile). If you change the port, update `apiBaseUrl` in the frontend's environment file to match.

## How to Run the Frontend Locally

Requires Node.js and npm.

```bash
cd client
npm install
ng serve
```

Then open `http://localhost:4200`. Make sure the backend is already running first — the app calls it as soon as the page loads to create the initial game.

I kept the backend and frontend as separate projects rather than forcing Angular into a .NET SPA template. In practice I ran the API from Visual Studio and the Angular app from a terminal (or VS Code) side by side — both point at each other through plain HTTP + CORS, so there's no build-time coupling between them.

## API Endpoint Summary

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/games` | Create a new game (body: `{ gameMode }`) |
| GET | `/api/games/{id}` | Get current game state |
| POST | `/api/games/{id}/moves` | Submit a move (body: `{ player, row, column }`) |
| POST | `/api/games/{id}/undo` | Undo the last move (disabled once the game is complete) |
| POST | `/api/games/{id}/reset` | Reset the current game — scoreboard is untouched |
| GET | `/api/scoreboard` | Get the scoreboard |
| POST | `/api/scoreboard/reset` | Reset the scoreboard to zero |

Full request/response shapes and example payloads are available in Swagger when the API is running.

### Example `GameStateResponse`

```json
{
  "gameId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "board": [["X", null, null], [null, "O", null], [null, null, null]],
  "currentPlayer": "O",
  "gameMode": "TwoPlayer",
  "status": "InProgress",
  "winner": null,
  "winningCells": [],
  "moveHistory": [
    { "moveNumber": 1, "player": "X", "row": 0, "column": 0 },
    { "moveNumber": 2, "player": "O", "row": 1, "column": 1 }
  ],
  "scoreboard": { "xWins": 0, "oWins": 0, "draws": 0 },
  "canUndo": true
}
```

## How to Run Tests

**Backend (xUnit)**

```bash
cd tests/TicTacToe.Tests
dotnet test
```

Covers move validation, all three win directions, draws, both undo modes, scoreboard updates (including the "only count once" rule), and every step of the computer's move priority (win → block → center → corner → any).


## AI Tools and Prompt Summary

I designed the architecture myself before writing or generating any code: the split between `TicTacToe.Core` (pure game rules, no ASP.NET dependency) and `TicTacToe.Api` (HTTP layer), the API contract and its request/response shapes, the signal-based state pattern on the Angular side, and the decision to keep the frontend as a thin renderer that never predicts game state on its own — it only ever displays what the last API response contained.

Once the shape of the solution was decided, I used Claude to speed up writing the actual code against that design — mostly the repetitive parts: enum/DTO scaffolding, controller CRUD code, the win-detection loop over the 8 board lines, Angular component templates, and a first pass at unit tests for the scenarios I specified (row/column/diagonal win, draw, both undo modes, computer move priority).

I reviewed everything it produced rather than pasting it in blind. A few concrete examples from doing that:

- Changed `WinningCells` from a tuple (as sketched in my own notes) to a small `{ row, column }` class once I noticed tuples serialize as `Item1`/`Item2` in JSON — not usable from the frontend as-is.
- Decided the undo-after-completion policy myself (Option A — undo disabled once a game is won or drawn) and had it implement that specific rule rather than the alternative.

In short: the architecture, the API contract, the state-management approach, and the fixes above are mine. AI tooling saved time on typing out C# code and Angular code that followed directly from decisions I'd already made.

## Design Decisions

- **Undo policy (Option A):** once a game is Won or Draw, Undo is disabled and that game's scoreboard entry is final. Trying to undo a finished game returns `400 Bad Request` rather than silently doing nothing.
- All backend services are registered as singletons — the whole point of in-memory storage here is one shared game/scoreboard state for the life of the process, so per-request scoping would just add complexity for no benefit.
- The game engine (`TicTacToe.Core.Engine.GameEngine`) has no knowledge of HTTP, DTOs, or the repository — it only operates on the `Game` model. That's what let me unit test every rule directly instead of spinning up the whole API for each test.
- Enums serialize as strings (`"Won"`, `"TwoPlayer"`, not `0`/`1`) so the API is self-describing in Swagger and doesn't require the frontend to keep a lookup table of magic numbers in sync with the backend.
- A single `GlobalExceptionMiddleware` maps `GameNotFoundException` → 404 and `InvalidMoveException` → 400, with a consistent JSON error shape either way, instead of scattering try/catch blocks across controllers.
- The computer opponent is a fixed-priority heuristic (win, then block, then center, then corner, then any free cell) rather than minimax — that matches the brief's "Basic Computer Mode" scope and is easy to verify by hand.

## Clarifications and Assumptions

- A game's result is only ever added to the scoreboard once, guarded by a per-game flag, even if the game state is fetched again afterward.
- In Computer Mode, the human is always X and always moves first; O is always the computer and moves automatically, in the same request/response cycle as the human's move — there's no separate "waiting for computer" API call.
- Switching the mode dropdown starts a new game in the newly selected mode immediately rather than only applying on the next explicit "New Game" click — I decided this was less surprising than having the dropdown and the live game potentially disagree.
- CORS is scoped to `http://localhost:4200` rather than left open, since both apps only ever run locally for this exercise.

## Known Limitations

- State is in-memory only — restarting the API clears every game and the scoreboard. Acceptable per the brief, but worth knowing before a demo.
- No authentication — out of scope for this exercise.
- No persistence layer, though SQLite was allowed by the brief — I went with in-memory for faster local setup and fewer moving parts to explain.
- The computer AI doesn't look further than one move ahead, so it can still lose in some board states a full minimax search wouldn't — this was a deliberate scope decision, not an oversight.

## Future Improvements

- Optional SQLite persistence so a game survives an API restart.
- A second, "unbeatable" computer difficulty using minimax, offered alongside the current heuristic.
- Real multiplayer over the network instead of both players sharing one browser tab.
- Integration tests against the live HTTP pipeline with `WebApplicationFactory<Program>`, on top of the current unit tests.
