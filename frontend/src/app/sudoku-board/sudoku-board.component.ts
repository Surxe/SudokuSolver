import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sudoku-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sudoku-board.component.html',
  styleUrl: './sudoku-board.component.scss'
})
export class SudokuBoardComponent implements OnChanges {
  @Input() board: number[][] | null = null;
  @Input() loading: boolean = false;
  @Input() highlightCell?: { row: number; col: number; type: 'attempt' | 'backtrack' };
  @Input() renderKey: number = 0;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['highlightCell']) {
      console.log('SudokuBoard highlightCell changed:', this.highlightCell);
    }
    if (changes['renderKey']) {
      console.log('SudokuBoard renderKey changed:', this.renderKey);
    }
  }

  getCellValue(value: number): string {
    return value === 0 ? '' : value.toString();
  }

  isGivenCell(value: number): boolean {
    return value !== 0;
  }

  isHighlighted(row: number, col: number): boolean {
    const result = this.highlightCell?.row === row && this.highlightCell?.col === col;
    return result;
  }

  getHighlightStyle(row: number, col: number): any {
    // Force re-evaluation by using renderKey
    const _ = this.renderKey;
    if (this.highlightCell?.row === row && this.highlightCell?.col === col) {
      if (this.highlightCell.type === 'attempt') {
        return { 'background-color': '#00ff00', 'color': '#000', 'font-weight': '900', 'border': '3px solid #00ff00' };
      } else {
        return { 'background-color': '#ff0000', 'color': '#fff', 'font-weight': '900', 'border': '3px solid #ff0000' };
      }
    }
    return {};
  }
}
