import { Component, OnInit } from '@angular/core';
import { GameListItem } from '../models/game-list-item.model';
import { GameService } from '../services/game.service';
import { PaginatedResponse } from '../../../../shared/models/paginated-result.model';
import { PaginatedQuery } from '../../../../shared/models/paginated-query.model';

@Component({
  selector: 'app-games-page',
  templateUrl: './games-page.component.html',
  styleUrl: './games-page.component.scss'
})
export class GamesPageComponent implements OnInit {
  response?: PaginatedResponse<GameListItem>;
  query: PaginatedQuery = { page: 1, pageSize: 10 };
  
  constructor(private gameService: GameService) {}

  ngOnInit(): void {
    this.loadGames();
  }

  private loadGames(): void {
    this.gameService.getGames(this.query).subscribe(games => this.response = games);
  }

  onPageChanged(event: { page: number; pageSize: number }) {
    this.query.page = event.page;
    this.query.pageSize = event.pageSize;
    this.loadGames();
  }
}
