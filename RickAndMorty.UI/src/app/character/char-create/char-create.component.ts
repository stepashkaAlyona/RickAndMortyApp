import { Component, inject } from '@angular/core';
import { CharacterCreate } from '../models/character-create';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CharService } from '../services/char.service';
import { NgToastModule, NgToastService } from 'ng-angular-popup';
import { CharacterResponse } from '../models/character-response';

@Component({
  selector: 'app-char-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgToastModule],
  templateUrl: './char-create.component.html',
  styleUrl: './char-create.component.css'
})
export class CharCreateComponent {

  UrlRegexPattern: string = "^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$";

  activeModal = inject(NgbActiveModal);

  characterForm!: FormGroup;

  isToastrEnabled = false;

  constructor(private formBuilder: FormBuilder, private charService: CharService, private toast: NgToastService) { }

  character: CharacterCreate = {
    name: '',
    status: '',
    species: '',
    type: '',
    gender: '',
    originName: '',
    locationName: '',
    image: '',
    episode: [],
    url: ''
  };

  characterTemplate: CharacterCreate = {
    name: 'Rick Astley',
    status: 'Alive',
    species: 'Alien',
    type: 'Parasite',
    gender: 'Male',
    originName: 'Abadango',
    locationName: 'Earth (Replacement Dimension)',
    image: 'https://cdn.vox-cdn.com/thumbor/WR9hE8wvdM4hfHysXitls9_bCZI=/0x0:1192x795/1400x1400/filters:focal(596x398:597x399)/cdn.vox-cdn.com/uploads/chorus_asset/file/22312759/rickroll_4k.jpg',
    episode: ["https://rickandmortyapi.com/api/episode/10", "https://rickandmortyapi.com/api/episode/11"],
    url: 'https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley'
  };

  ngOnInit() {
    this.characterForm = this.formBuilder.group({
      name: ['', Validators.required],
      status: ['', Validators.required],
      species: ['', Validators.required],
      type: ['', Validators.required],
      gender: ['', Validators.required],
      originName: ['', Validators.required],
      locationName: ['', Validators.required],
      image: ['', [Validators.required, Validators.maxLength(256), Validators.pattern(this.UrlRegexPattern)]],
      episode: ['', Validators.required],
      url: ['', [Validators.required, Validators.maxLength(256), Validators.pattern(this.UrlRegexPattern)]]
    });
  }

  onSubmit() {
    // Обработка отправки формы
    if (this.characterForm.valid) {
      const characterData: CharacterCreate = this.characterForm.value;
      this.isToastrEnabled = true;

      this.charService.createChar(characterData).subscribe((data: CharacterResponse) => {
        if (data.isSuccess) {
          this.toast.success({ detail: "Success!", summary: 'New character was successfully created. You can update the page or create a new one character.', duration: 4000, position: 'topRight' });

          this.characterForm.patchValue(this.character);
          this.characterForm.reset();
        }
        else {
          this.toast.error({ detail: "Something went wrong ...", summary: data.errorMessage, duration: 5000, position: 'topRight' });
        }
      },
        (error) => {
          this.toast.error({ detail: "Something went wrong ...", summary: error, duration: 5000, position: 'topRight' });
        });
    }
  }

  fillForm() {
    this.characterForm.patchValue(this.characterTemplate);
  }
}