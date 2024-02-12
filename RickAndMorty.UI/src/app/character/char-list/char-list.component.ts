import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CharService } from '../services/char.service';
import { CharDetailComponent } from '../char-detail/char-detail.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CharacterList } from '../models/character-list';
import { NgToastModule, NgToastService } from 'ng-angular-popup';
import { CharacterResponse } from '../models/character-data-response';

@Component({
  selector: 'app-char-list',
  standalone: true,
  imports: [CommonModule, RouterModule, NgToastModule],
  templateUrl: './char-list.component.html',
  styleUrl: './char-list.component.css'
})
export class CharListComponent {

  modal: any;

  isToastrEnabled = false;

  private modalService = inject(NgbModal);

  characters: CharacterList[] = [];

  constructor(private charService: CharService, private toast: NgToastService) { }

  ngOnInit(): void {
    this.charService.getAllChars().subscribe((data: CharacterResponse<CharacterList[]>) => {
      if (data.isSuccess) {
        this.characters = data.data;
      }
      else {
        this.isToastrEnabled = true;
        this.toast.error({ detail: "Something went wrong ...", summary: data.errorMessage, duration: 5000, position: 'topRight' });
      }
    })
  }

  public onSelect(id: number): void {
    this.modal = this.modalService.open(CharDetailComponent);
    this.modal.componentInstance.Id = id;
  }
}
