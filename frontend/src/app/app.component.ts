import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { LoanService } from './loan.service';
import { Loan } from './loan.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatTableModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  private readonly loanService = inject(LoanService);

  displayedColumns: string[] = ['amount', 'currentBalance', 'applicantName', 'status'];
  loans = signal<Loan[]>([]);

  ngOnInit(): void {
    this.loanService.getAll().subscribe(loans => this.loans.set(loans));
  }
}
