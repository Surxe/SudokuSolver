import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { SudokuApiService } from './sudoku-api.service';
import { SudokuBoardComponent } from './sudoku-board/sudoku-board.component';
import { SolveButtonComponent } from './solve-button/solve-button.component';
import { SolveStep } from './models/puzzle.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [SudokuBoardComponent, SolveButtonComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  currentBoard: number[][] | null = null;
  solutionBoard: number[][] | null = null;
  loading: boolean = false;
  error: string | null = null;
  attempts: number = 0;
  backtracks: number = 0;
  durationMs: number = 0;
  highlightCell?: { row: number; col: number; type: 'attempt' | 'backtrack' };
  // Force re-render counter
  private renderCounter: number = 0;

  get renderKey(): number {
    return this.renderCounter;
  }
  
  // Animation control
  private animationTimer: any = null;
  private readonly ANIMATION_DELAY_MS = 300; // 300ms per cell fill for better visibility

  constructor(private sudokuApiService: SudokuApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadRandomPuzzle();
  }

  ngOnDestroy(): void {
    if (this.animationTimer) {
      clearTimeout(this.animationTimer);
    }
  }

  loadRandomPuzzle(): void {
    this.loading = true;
    this.error = null;
    this.solutionBoard = null;
    this.attempts = 0;
    this.backtracks = 0;
    this.durationMs = 0;

    if (this.animationTimer) {
      clearTimeout(this.animationTimer);
      this.animationTimer = null;
    }

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

    if (this.animationTimer) {
      clearTimeout(this.animationTimer);
      this.animationTimer = null;
    }

    this.sudokuApiService.solvePuzzle(this.currentBoard).subscribe({
      next: (response) => {
        if (response.solved && response.solution) {
          this.solutionBoard = response.solution;
          this.attempts = response.attempts;
          this.backtracks = response.backtracks;
          this.durationMs = response.durationMs;
          
          // Animate the solution using the steps
          this.animateSolution(response.steps);
        } else {
          this.error = response.errorMessage || 'Failed to solve puzzle';
          this.loading = false;
        }
      },
      error: (err) => {
        this.error = 'Failed to solve puzzle. Please try again.';
        this.loading = false;
        console.error('Error solving puzzle:', err);
      }
    });
  }

  private animateSolution(steps: SolveStep[]): void {
    // Set loading to false so the board is visible during animation
    this.loading = false;
    
    let stepIndex = 0;
    
    const playNextStep = () => {
      if (stepIndex >= steps.length) {
        this.loading = false;
        this.currentBoard = this.solutionBoard;
        return;
      }

      const step = steps[stepIndex];
      
      // Update current board with the step's board state
      this.currentBoard = JSON.parse(JSON.stringify(step.board));
      
      // Highlight the cell based on step type
      const highlightType = step.type === 'Attempt' ? 'attempt' : 'backtrack';
      this.highlightCell = { row: step.row, col: step.col, type: highlightType };
      this.renderCounter++;
      console.log(`Step ${stepIndex + 1}/${steps.length}: ${step.type} at [${step.row}, ${step.col}] value=${step.value}`);
      
      // Direct DOM manipulation to highlight the cell
      const cells = document.querySelectorAll('.sudoku-cell');
      const cellIndexInDOM = step.row * 9 + step.col;
      if (cells[cellIndexInDOM]) {
        const cell = cells[cellIndexInDOM] as HTMLElement;
        if (step.type === 'Attempt') {
          cell.style.backgroundColor = '#00ff00';
          cell.style.color = '#000';
          cell.style.fontWeight = '900';
        } else {
          cell.style.backgroundColor = '#ff0000';
          cell.style.color = '#fff';
          cell.style.fontWeight = '900';
        }
        console.log(`Directly highlighted cell [${step.row}, ${step.col}] as ${step.type}`);
        
        // Remove highlight after delay
        setTimeout(() => {
          cell.style.backgroundColor = '';
          cell.style.color = '';
          cell.style.fontWeight = '';
        }, this.ANIMATION_DELAY_MS);
      } else {
        console.log(`Cell at index ${cellIndexInDOM} not found in DOM`);
      }

      stepIndex++;
      this.animationTimer = setTimeout(playNextStep, this.ANIMATION_DELAY_MS);
    };

    playNextStep();
  }

  get canSolve(): boolean {
    return this.currentBoard !== null && this.solutionBoard === null;
  }
}
