import { Component, Output, EventEmitter, Input } from '@angular/core';

@Component({
  selector: 'app-solve-button',
  standalone: true,
  imports: [],
  templateUrl: './solve-button.component.html',
  styleUrl: './solve-button.component.scss'
})
export class SolveButtonComponent {
  @Input() loading: boolean = false;
  @Input() disabled: boolean = false;
  @Output() solve = new EventEmitter<void>();
  @Output() loadNew = new EventEmitter<void>();

  onSolve() {
    if (!this.disabled && !this.loading) {
      this.solve.emit();
    }
  }

  onLoadNew() {
    if (!this.loading) {
      this.loadNew.emit();
    }
  }
}
