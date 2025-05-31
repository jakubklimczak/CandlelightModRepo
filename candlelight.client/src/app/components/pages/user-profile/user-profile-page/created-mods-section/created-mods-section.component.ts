import { Component, Input, OnInit } from '@angular/core';
import { ModsService } from '../../../mods/services/mods.service';
import { ModsListItemDto } from '../../../mods/models/mods-list-item-dto.model';

@Component({
  selector: 'app-created-mods-section',
  templateUrl: './created-mods-section.component.html',
  styleUrl: './created-mods-section.component.scss'
})
export class CreatedModsSectionComponent implements OnInit {
  constructor(private readonly modsService: ModsService) {}
  @Input() userId!: string;
  createdMods: ModsListItemDto[] = [];
  visibleRows = 1;
  readonly modsPerRow = 5;


  ngOnInit(): void {
    this.modsService.getUserCreatedMods(this.userId).subscribe(mods => {
      this.createdMods = mods;
    });
  }

  public get visibleMods(): ModsListItemDto[] {
    return this.createdMods.slice(0, this.visibleRows * this.modsPerRow);
  }

  public showMore(): void {
    this.visibleRows++;
  }

  public collapse(): void {
    this.visibleRows = 1;
  }

  public get canShowMore(): boolean {
    return this.visibleMods.length < this.createdMods.length;
  }

  public get isExpanded(): boolean {
    return this.visibleRows > 1;
  }
}
