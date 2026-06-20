import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sudoku-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sudoku-board.component.html',
  styleUrl: './sudoku-board.component.scss'
})
export class SudokuBoardComponent {
  @Input() board: number[][] | null = null;
  @Input() loading: boolean = false;

  getCellValue(value: number): string {
    return value === 0 ? '' : value.toString();
  }

  isGivenCell(value: number): boolean {
    return value !== 0;
  }
}
