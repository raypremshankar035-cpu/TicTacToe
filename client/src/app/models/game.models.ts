export type Player = 'X' | 'O';
export type GameMode = 'TwoPlayer' | 'Computer';
export type GameStatus = 'InProgress' | 'Won' | 'Draw';
 
export interface Move {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
}
 
export interface WinningCell {
  row: number;
  column: number;
}
 
export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}
 
/** Matches TicTacToe.Core.DTOs.GameStateResponse exactly. */
export interface GameState {
  gameId: string;
  board: (Player | null)[][];
  currentPlayer: Player;
  gameMode: GameMode;
  status: GameStatus;
  winner: Player | null;
  winningCells: WinningCell[];
  moveHistory: Move[];
  scoreboard: Scoreboard;
  canUndo: boolean;
}
 
/** Matches TicTacToe.Core.DTOs.CreateGameRequest. */
export interface CreateGameRequest {
  gameMode: GameMode;
}
 
/** Matches TicTacToe.Core.DTOs.MoveRequest. */
export interface MoveRequest {
  player: Player;
  row: number;
  column: number;
}