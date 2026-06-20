import { Component, OnInit } from '@angular/core';
import { SudokuApiService } from './sudoku-api.service';
import { SudokuBoardComponent } from './sudoku-board/sudoku-board.component';
import { SolveButtonComponent } from './solve-button/solve-button.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [SudokuBoardComponent, SolveButtonComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  currentBoard: number[][] | null = null;
  solutionBoard: number[][] | null = null;
  loading: boolean = false;
  error: string | null = null;
  attempts: number = 0;
  durationMs: number = 0;

  constructor(private sudokuApiService: SudokuApiService) {}

  ngOnInit(): void {
    this.loadRandomPuzzle();
  }

  loadRandomPuzzle(): void {
    this.loading = true;
    this.error = null;
    this.solutionBoard = null;
    this.attempts = 0;
    this.durationMs = 0;

    this.sudokuApiService.getRandomPuzzle().subscribe({
      next: (response) => {
        this.currentBoard = response.board;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load puzzle. Please try again.';
        this.loading = false;
        console.error('Error loading puzzle:', err);
      }
    });
  }

  solvePuzzle(): void {
    if (!this.currentBoard) return;

    this.loading = true;
    this.error = null;

    this.sudokuApiService.solvePuzzle(this.currentBoard).subscribe({
      next: (response) => {
        if (response.solved && response.solution) {
          this.solutionBoard = response.solution;
          this.currentBoard = response.solution;
          this.attempts = response.attempts;
          this.durationMs = response.durationMs;
        } else {
          this.error = response.errorMessage || 'Failed to solve puzzle';
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to solve puzzle. Please try again.';
        this.loading = false;
        console.error('Error solving puzzle:', err);
      }
    });
  }

  get canSolve(): boolean {
    return this.currentBoard !== null && this.solutionBoard === null;
  }
}
