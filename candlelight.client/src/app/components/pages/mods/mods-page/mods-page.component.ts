import { Component, OnInit } from '@angular/core';
import { PaginatedResponse } from '../../../../shared/models/paginated-result.model';
import { ModsListItemDto } from '../models/mods-list-item-dto.model';
import { PaginatedQuery } from '../../../../shared/models/paginated-query.model';
import { ActivatedRoute } from '@angular/router';
import { ModsService } from '../services/mods.service';

@Component({
  selector: 'app-mods-page',
  templateUrl: './mods-page.component.html',
  styleUrl: './mods-page.component.scss'
})
export class ModsPageComponent implements OnInit {
  response?: PaginatedResponse<ModsListItemDto>;
  query: PaginatedQuery = { page: 1, pageSize: 10 };
  gameId!: string;

  constructor(private modService: ModsService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.gameId = params['gameId'];
      this.query.page = 1;
      this.loadMods();
    });
  }

  private loadMods(): void {
    this.modService.getModsForGame(this.gameId, this.query.page, this.query.pageSize).subscribe(res => {
      this.response = res;
    });
  }

  public onPageChanged(e: { page: number; pageSize: number }): void {
    this.query.page = e.page;
    this.query.pageSize = e.pageSize;
    this.loadMods();
  }
}
