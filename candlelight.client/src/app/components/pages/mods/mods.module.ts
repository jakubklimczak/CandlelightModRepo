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
import { UploadNewModTabComponent } from './upload-mod-page/upload-new-mod-tab/upload-new-mod-tab.component';
import { UploadNewVersionTabComponent } from './upload-mod-page/upload-new-version-tab/upload-new-version-tab.component';
import { MatTabsModule } from "@angular/material/tabs";
import { MatProgressSpinner } from "@angular/material/progress-spinner";

@NgModule({
  declarations: [
    ModsPageComponent,
    ModsListComponent,
    ModsListItemComponent,
    ModDetailsPageComponent,
    UploadModPageComponent,
    UploadNewModTabComponent,
    UploadNewVersionTabComponent
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
    MatTabsModule,
    MatProgressSpinner,
  ],
})
export class ModsModule {}
