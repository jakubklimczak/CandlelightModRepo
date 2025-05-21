import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ModDetailsDto } from '../models/mod-details-dto.model';
import { ModsService } from '../services/mods.service';

@Component({
  selector: 'app-mod-details-page',
  templateUrl: './mod-details-page.component.html',
  styleUrl: './mod-details-page.component.scss'
})
export class ModDetailsPageComponent implements OnInit {
  modId!: string;
  details!: ModDetailsDto;
  isLoading = true;

  constructor(private route: ActivatedRoute, private modsService: ModsService) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.modId = params.get('id')!;
      this.getDetails(this.modId);
    });
  }

  public getDetails(id: string): void {
    this.modsService.getModDetails(id).subscribe({
      next: (data) => {
        this.details = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load mod details', err);
        this.isLoading = false;
      }
    });
  }

}
