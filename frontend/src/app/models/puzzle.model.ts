export interface PuzzleResponse {
  id: number;
  board: number[][];
}

export interface SolveRequest {
  board: number[][];
}

export interface SolveStep {
  type: 'Attempt' | 'Backtrack';
  row: number;
  col: number;
  value: number;
  attemptNumber: number;
  backtrackNumber: number;
  board: number[][];
}

export interface SolveResponse {
  solved: boolean;
  solution: number[][] | null;
  errorMessage?: string;
  attempts: number;
  backtracks: number;
  durationMs: number;
  steps: SolveStep[];
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
