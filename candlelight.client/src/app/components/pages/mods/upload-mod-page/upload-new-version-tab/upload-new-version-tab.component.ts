import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ModsListItemDto } from '../../models/mods-list-item-dto.model';
import { ModsService } from '../../services/mods.service';

@Component({
  selector: 'app-upload-new-version-tab',
  templateUrl: './upload-new-version-tab.component.html',
  styleUrls: ['./upload-new-version-tab.component.scss']
})
export class UploadNewVersionTabComponent implements OnInit {
  createdMods: ModsListItemDto[] | null = null;
  filteredMods: ModsListItemDto[] = [];
  selectedMod: ModsListItemDto | null = null;
  searchControl = new FormControl('');
  newVersionForm: FormGroup;
  versionFile: File | null = null;

  constructor(
    private fb: FormBuilder,
    private modsService: ModsService,
    private snackBar: MatSnackBar
  ) {
    this.newVersionForm = this.fb.group({
      version: ['', Validators.required],
      changelog: ['']
    });
  }

  ngOnInit(): void {
    this.modsService.getCurrentUserCreatedMods().subscribe(mods => {
      this.createdMods = mods;
      this.filteredMods = mods;
    });

    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      if (term) {
        this.filteredMods = this.createdMods?.filter(mod =>
          mod.name.toLowerCase().includes(term.toLowerCase())
        ) || [];
      }
    });
  }

  selectMod(mod: ModsListItemDto): void {
    this.selectedMod = mod;
    this.newVersionForm.reset();
    this.versionFile = null;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.versionFile = input.files[0];
    }
  }

  onSubmitNewVersion(): void {
    if (!this.versionFile || !this.selectedMod || this.newVersionForm.invalid) return;

    const formData = new FormData();
    formData.append('modId', this.selectedMod.id);
    formData.append('version', this.newVersionForm.value.version);
    formData.append('changelog', this.newVersionForm.value.changelog);
    formData.append('file', this.versionFile);

    this.modsService.uploadNewModVersion(formData).subscribe({
      next: () => {
        this.snackBar.open('Mod version uploaded successfully!', 'Close', { duration: 3000 });
        this.newVersionForm.reset();
        this.versionFile = null;
      },
      error: () => {
        this.snackBar.open('Failed to upload mod version.', 'Close', { duration: 3000 });
      }
    });
  }
}