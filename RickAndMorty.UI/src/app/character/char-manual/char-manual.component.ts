import { Component, inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-char-manual',
  standalone: true,
  imports: [],
  templateUrl: './char-manual.component.html',
  styleUrl: './char-manual.component.css'
})
export class CharManualComponent {
	activeModal = inject(NgbActiveModal);
}
