import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
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
          
          // Animate the solution
          this.animateSolution(response.solution);
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

  private animateSolution(solution: number[][]): void {
    // Set loading to false so the board is visible during animation
    this.loading = false;
    
    // Create a copy of the original board to track which cells to fill
    const originalBoard = this.currentBoard ? JSON.parse(JSON.stringify(this.currentBoard)) : [];
    let cellIndex = 0;
    const totalCells = 81;
    
    const fillNextCell = () => {
      if (cellIndex >= totalCells) {
        this.loading = false;
        this.currentBoard = solution;
        return;
      }

      const row = Math.floor(cellIndex / 9);
      const col = cellIndex % 9;

      // Only animate cells that were originally empty
      if (originalBoard[row] && originalBoard[row][col] === 0) {
        // Update current board with the solution value
        if (!this.currentBoard) {
          this.currentBoard = JSON.parse(JSON.stringify(originalBoard));
        }
        if (this.currentBoard && this.currentBoard[row]) {
          this.currentBoard[row][col] = solution[row][col];
          console.log(`Filling cell [${row}, ${col}] with value ${solution[row][col]}`);
        }
        
        // Direct DOM manipulation to highlight the cell
        const cells = document.querySelectorAll('.sudoku-cell');
        console.log(`Found ${cells.length} cells in DOM`);
        const cellIndexInDOM = row * 9 + col;
        if (cells[cellIndexInDOM]) {
          const cell = cells[cellIndexInDOM] as HTMLElement;
          cell.style.backgroundColor = '#00ff00';
          cell.style.color = '#000';
          cell.style.fontWeight = '900';
          cell.style.border = '3px solid #00ff00';
          console.log(`Directly highlighted cell [${row}, ${col}]`);
          
          // Remove highlight after delay
          setTimeout(() => {
            cell.style.backgroundColor = '';
            cell.style.color = '';
            cell.style.fontWeight = '';
            cell.style.border = '';
          }, this.ANIMATION_DELAY_MS);
        } else {
          console.log(`Cell at index ${cellIndexInDOM} not found in DOM`);
        }
      }

      cellIndex++;
      this.animationTimer = setTimeout(fillNextCell, this.ANIMATION_DELAY_MS);
    };

    fillNextCell();
  }

  get canSolve(): boolean {
    return this.currentBoard !== null && this.solutionBoard === null;
  }
}
