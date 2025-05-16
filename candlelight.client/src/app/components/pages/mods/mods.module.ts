import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { MatCardModule } from "@angular/material/card";
import { MatIconModule } from "@angular/material/icon";
import { SharedModule } from "../../../shared/shared.module";
import { ModsPageComponent } from './mods-page/mods-page.component';
import { ModsListComponent } from './mods-page/mods-list/mods-list.component';
import { ModsListItemComponent } from './mods-page/mods-list/mods-list-item/mods-list-item.component';
import { ModDetailsPageComponent } from './mod-details-page/mod-details-page.component';
import { RouterModule } from "@angular/router";

@NgModule({
  declarations: [
    ModsPageComponent,
    ModsListComponent,
    ModsListItemComponent,
    ModDetailsPageComponent
  ],
  imports: [
    MatCardModule,
    MatIconModule, 
    CommonModule, 
    SharedModule, 
    RouterModule
  ],
})
export class ModsModule {}
