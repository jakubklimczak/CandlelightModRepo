import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ModsService } from '../../services/mods.service';
import { GameDetailsDto } from '../../../games/models/game-details-dto';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ModsListItemDto } from '../../models/mods-list-item-dto.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-upload-new-mod-tab',
  templateUrl: './upload-new-mod-tab.component.html',
  styleUrls: ['./upload-new-mod-tab.component.scss']
})
export class UploadNewModTabComponent implements OnInit {
  newModForm: FormGroup;
  selectedFile: File | null = null;
  gameSearchControl = new FormControl('');
  gameSuggestions: GameDetailsDto[] = [];
  selectedGame: GameDetailsDto | null = null;
  dependencySearchControl = new FormControl('');
  dependencySuggestions: ModsListItemDto[] = [];
  selectedDependencies: ModsListItemDto[] = [];
  previewImages: { name: string; preview: string }[] = [];
  selectedThumbnail: string | null = null;
  selectedImageFiles: File[] = [];

  constructor(
    private fb: FormBuilder,
    private modsService: ModsService,
    private snackBar: MatSnackBar,
    private router: Router,
  ) {
    this.newModForm = this.fb.group({
      name: ['', Validators.required],
      descriptionSnippet: [''],
      description: [''],
      version: ['', Validators.required],
      changelog: [''],
      gameId: ['', Validators.required],
      thumbnailUrl: [''],
      supportedVersions: [[]],
      dependencies: [[]]
    });
  }


  ngOnInit(): void {
    this.gameSearchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      if (term && term.length > 1) {
        this.modsService.searchGames(term).subscribe(res => {
          this.gameSuggestions = res.items.slice(0, 5);
        });
      } else {
        this.gameSuggestions = [];
      }
    });
    this.dependencySearchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      if (term && term?.length > 1) {
        this.modsService.searchModsByGame(this.selectedGame!.id, term).subscribe(res => {
          this.dependencySuggestions = res.items;
        });
      } else {
        this.dependencySuggestions = [];
      }
    });
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }

  public onSubmitNewMod(): void {
    if (!this.selectedFile || this.newModForm.invalid) return;

    const formData = new FormData();
    const formValue = this.newModForm.value;

    for (const [key, value] of Object.entries(formValue)) {
      if (value == null) continue;

      if (Array.isArray(value)) {
        for (const v of value) {
          formData.append(key, v); 
        }
      } else {
        formData.append(key, value.toString());
      }
    }

    if (this.selectedImageFiles.length > 0) {
      for (const img of this.selectedImageFiles) {
        formData.append('images', img);
      }
    }
    if (this.selectedThumbnail) {
      formData.append('selectedThumbnail', this.selectedThumbnail);
    }

    formData.append('file', this.selectedFile!);

    this.modsService.uploadNewMod(formData).subscribe({
      next: (res) => {
        this.snackBar.open('Mod uploaded successfully!', 'Close', { duration: 3000 });
        this.newModForm.reset();
        this.selectedFile = null;
        this.selectedImageFiles = [];
        this.selectedDependencies = [];
        this.selectedThumbnail = null;
        this.previewImages = [];
        
        this.router.navigate(['/mods', res.modId]);
      },
      error: () => {
        this.snackBar.open('Failed to upload mod.', 'Close', { duration: 3000 });
      }
    });
  }

  public selectGame(game: GameDetailsDto): void {
    this.newModForm.patchValue({ gameId: game.id });
    this.selectedGame = game;
    this.gameSuggestions = [];
    this.selectedDependencies = [];
    this.newModForm.patchValue({ dependencies: [] });
    this.dependencySuggestions = [];
    this.dependencySearchControl.setValue('');
  }

  public fetchGameById(): void {
    const id = this.newModForm.value.gameId;
    if (!id) return;

    this.modsService.getGameById(id).subscribe({
      next: game => (this.selectedGame = game),
      error: () => (this.selectedGame = null)
    });
  }


  public onSupportedVersionsBlur(event: FocusEvent): void {
    const input = (event.target as HTMLInputElement).value;
    const parsed = input.split(',').map(v => v.trim()).filter(v => v);
    this.newModForm.patchValue({ supportedVersions: parsed });
  }


  public addDependency(mod: { id: string; name: string }): void {
    if (!this.selectedDependencies.some(d => d.id === mod.id)) {
      this.selectedDependencies.push({
        id: mod.id, 
        name: mod.name,
        descriptionSnippet: '',
        thumbnailUrl: '',
        authorId: '',
        author: '',
        lastUpdatedDate: new Date(),
        totalDownloads: 0,
        averageRating: 0,
        totalFavourited: 0,
        totalReviews: 0
      });
      this.updateDependencyFormValue();
    }
    this.dependencySuggestions = [];
    this.dependencySearchControl.setValue('');
  }

  public removeDependency(modId: string): void {
    this.selectedDependencies = this.selectedDependencies.filter(d => d.id !== modId);
    this.updateDependencyFormValue();
  }

  public updateDependencyFormValue(): void {
    this.newModForm.patchValue({
      dependencies: this.selectedDependencies.map(d => d.id)
    });
  }

  public onImagesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;

    const files = Array.from(input.files).slice(0, 10);
    this.previewImages = [];
    this.selectedImageFiles = files;

    files.forEach(file => {
      const reader = new FileReader();
      reader.onload = e => {
        this.previewImages.push({ name: file.name, preview: e.target?.result as string });
      };
      reader.readAsDataURL(file);
    });
  }

  public selectThumbnail(fileName: string): void {
    this.selectedThumbnail = fileName;
  }

  public get selectedImageFileNames(): string {
    return this.selectedImageFiles.map(f => f.name).join(', ');
  }

}
