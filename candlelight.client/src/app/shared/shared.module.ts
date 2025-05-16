import { NgModule } from '@angular/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { PaginatorComponent } from './components/paginator/paginator.component';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderDirective } from './directives/skeleton-loader.directive';
import { ImageSkeletonLoaderDirective } from './directives/image-skeleton-loader.directive';

@NgModule({
  declarations: [
    PaginatorComponent, 
    SkeletonLoaderDirective, 
    ImageSkeletonLoaderDirective
  ],
  imports: [
    CommonModule, 
    MatPaginatorModule
  ],
  exports: [
    PaginatorComponent, 
    SkeletonLoaderDirective, 
    ImageSkeletonLoaderDirective
  ],
})
export class SharedModule {}
