import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CharCreateComponent } from './character/char-create/char-create.component';
import { CharManualComponent } from './character/char-manual/char-manual.component';
import { ToastrModule } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, ToastrModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  private modalService = inject(NgbModal);

  constructor(private router: Router) { }

  public openCreateModal(): void {
    const modal = this.modalService.open(CharCreateComponent);
  }

  public openManualModal(): void {
    this.modalService.open(CharManualComponent);
  }
}
