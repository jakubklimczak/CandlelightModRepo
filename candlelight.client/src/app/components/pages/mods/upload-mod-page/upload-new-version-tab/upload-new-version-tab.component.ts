import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ModsListItemDto } from '../../models/mods-list-item-dto.model';
import { ModsService } from '../../services/mods.service';
import { ModVersion } from '../../models/mod-version.model';

@Component({
  selector: 'app-upload-new-version-tab',
  templateUrl: './upload-new-version-tab.component.html',
  styleUrls: ['./upload-new-version-tab.component.scss']
})
export class UploadNewVersionTabComponent implements OnInit {
  createdMods: ModsListItemDto[] | null = null;
  filteredMods: ModsListItemDto[] = [];
  selectedMod: ModsListItemDto | null = null;
  selectedModGameId = '';
  searchControl = new FormControl('');
  newVersionForm: FormGroup;
  versionFile: File | null = null;
  dependencySearchControl = new FormControl('');
  dependencySuggestions: ModsListItemDto[] = [];
  selectedDependencies: ModVersion[] = [];
  modVersionOptions: ModVersion[] = [];
  showVersionSelectForMod: string | null = null;


  constructor(
    private fb: FormBuilder,
    private modsService: ModsService,
    private snackBar: MatSnackBar
  ) {
    this.newVersionForm = this.fb.group({
      version: ['', Validators.required],
      changelog: ['', Validators.required],
      supportedVersions: [[]],
      dependencies: [[]]
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
      if (term && term != '') {
        this.filteredMods = this.createdMods?.filter(mod =>
          mod.name.toLowerCase().includes(term.toLowerCase())
        ) || [];
      }
    });
    this.dependencySearchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      if (term && term?.length > 1) {
        this.modsService.searchModsByGame(this.selectedModGameId, term).subscribe(res => {
          this.dependencySuggestions = res.items;
        });
      } else {
        this.dependencySuggestions = [];
      }
    });
  }

  public selectMod(mod: ModsListItemDto): void {
    this.selectedMod = mod;
    this.modsService.getGameIdByModId(mod.id).subscribe(resp => this.selectedModGameId = resp)
    this.newVersionForm.reset();
    this.versionFile = null;
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.versionFile = input.files[0];
    }
  }

  public onSubmitNewVersion(): void {
    if (!this.versionFile || !this.selectedMod || this.newVersionForm.invalid) return;

    const formData = new FormData();
    formData.append('modId', this.selectedMod.id);
    const formValue = this.newVersionForm.value;

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

    formData.append('file', this.versionFile);

    this.modsService.uploadNewModVersion(formData).subscribe({
      next: () => {
        this.snackBar.open('Mod version uploaded successfully!', 'Close', { duration: 3000 });
        this.newVersionForm.reset();
        this.selectedMod = null;
        this.searchControl.setValue('');
        this.versionFile = null;
      },
      error: () => {
        this.snackBar.open('Failed to upload mod version.', 'Close', { duration: 3000 });
      }
    });
  }

   public onSupportedVersionsBlur(event: FocusEvent): void {
    const input = (event.target as HTMLInputElement).value;
    const parsed = input.split(',').map(v => v.trim()).filter(v => v);
    this.newVersionForm.patchValue({ supportedVersions: parsed });
  }


  public addDependency(mod: ModsListItemDto): void {
    this.modVersionOptions = [];
    this.showVersionSelectForMod = mod.id;
    this.modsService.getModVersions(mod.id).subscribe({
      next: versions => {
        this.modVersionOptions = versions;
      },
      error: () => {
        this.snackBar.open('Failed to load versions.', 'Close', { duration: 3000 });
        this.showVersionSelectForMod = null;
      }
    });
  }


  public selectDependencyVersion(version: ModVersion): void {
    if (!this.selectedDependencies.some(d => d.id === version.id)) {
      this.selectedDependencies.push(version);
      this.updateDependencyFormValue();
    }
    this.showVersionSelectForMod = null;
    this.modVersionOptions = [];
    this.dependencySearchControl.setValue('');
  }

  public removeDependency(modId: string): void {
    this.selectedDependencies = this.selectedDependencies.filter(d => d.id !== modId);
    this.updateDependencyFormValue();
  }

  public updateDependencyFormValue(): void {
    this.newVersionForm.patchValue({
      dependencies: this.selectedDependencies.map(d => d.id)
    });
  }

}