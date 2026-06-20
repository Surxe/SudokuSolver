export interface PuzzleResponse {
  id: number;
  board: number[][];
}

export interface SolveRequest {
  board: number[][];
}

export interface SolveResponse {
  solved: boolean;
  solution: number[][] | null;
  errorMessage?: string;
  attempts: number;
  durationMs: number;
}
