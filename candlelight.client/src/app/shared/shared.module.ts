import { NgModule } from '@angular/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { PaginatorComponent } from './components/paginator/paginator.component';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderDirective } from './directives/skeleton-loader.directive';
import { ImageSkeletonLoaderDirective } from './directives/image-skeleton-loader.directive';
import { FiltersAndOptionsBarComponent } from './components/filters-and-options-bar/filters-and-options-bar.component';
import { EnumSelectComponent } from './components/enum-select/enum-select.component';
import { MatSelectModule } from '@angular/material/select';
import { CamelCaseSpacerPipe } from './pipes/camel-case-spacer.pipe';
import { TruncatePipe } from './pipes/truncate.pipe';

@NgModule({
  declarations: [
    PaginatorComponent, 
    SkeletonLoaderDirective, 
    ImageSkeletonLoaderDirective, 
    FiltersAndOptionsBarComponent, 
    EnumSelectComponent,
    CamelCaseSpacerPipe,
    TruncatePipe,
  ],
  imports: [
    CommonModule, 
    MatPaginatorModule,
    MatSelectModule,
  ],
  exports: [
    PaginatorComponent, 
    SkeletonLoaderDirective, 
    ImageSkeletonLoaderDirective,
    FiltersAndOptionsBarComponent,
    EnumSelectComponent,
    CamelCaseSpacerPipe,
    TruncatePipe,
  ],
})
export class SharedModule {}
