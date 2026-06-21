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
  backtracks: number;
  durationMs: number;
}

export interface SolveStreamEvent {
  eventType: 'attempt' | 'backtrack' | 'complete' | 'error';
  row?: number;
  col?: number;
  value?: number;
  attemptNumber: number;
  backtrackNumber: number;
  board?: number[][];
  errorMessage?: string;
  durationMs?: number;
}
