import { Component, Inject, Input, inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Character } from '../models/character';
import { CharService } from '../services/char.service';
import { CommonModule } from '@angular/common';
import { CharacterResponse } from '../models/character-data-response';
import { NgToastModule, NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-char-detail',
  standalone: true,
  imports: [CommonModule, NgToastModule],
  templateUrl: './char-detail.component.html',
  styleUrl: './char-detail.component.css'
})
export class CharDetailComponent {

  character!: Character;

  isToastrEnabled = false;

  activeModal = inject(NgbActiveModal);

  @Input() Id: number = 0;

  constructor(private charService: CharService, private toast: NgToastService) { }

  ngOnInit(): void {
    this.charService.getCharById(this.Id).subscribe((data: CharacterResponse<Character>) => {
      if (data.isSuccess) {
        this.character = data.data;
      }
      else {
        this.isToastrEnabled = true;
        
        this.toast.error({ detail: "Something went wrong ...", summary: data.errorMessage, duration: 5000, position: 'topRight' });
      }
    })
  }
}
