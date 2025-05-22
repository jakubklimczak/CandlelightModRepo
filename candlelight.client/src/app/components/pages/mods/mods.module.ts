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
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { UploadModPageComponent } from './upload-mod-page/upload-mod-page.component';

@NgModule({
  declarations: [
    ModsPageComponent,
    ModsListComponent,
    ModsListItemComponent,
    ModDetailsPageComponent,
    UploadModPageComponent
  ],
  imports: [
    MatCardModule,
    MatIconModule, 
    CommonModule, 
    SharedModule, 
    RouterModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
  ],
})
export class ModsModule {}
