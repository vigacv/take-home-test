import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { AuthService } from '../auth.service';
import { Loan } from '../loan.model';
import { LoanService } from '../loan.service';

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './loans.component.html',
  styleUrls: ['./loans.component.scss'],
})
export class LoansComponent implements OnInit {
  private readonly loanService = inject(LoanService);
  private readonly authService = inject(AuthService);

  displayedColumns: string[] = ['amount', 'currentBalance', 'applicantName', 'status'];
  loans = signal<Loan[]>([]);

  ngOnInit(): void {
    this.loanService.getAll().subscribe((loans) => this.loans.set(loans));
  }

  logout(): void {
    this.authService.logout();
  }
}
