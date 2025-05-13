import { NgModule } from '@angular/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { PaginatorComponent } from './components/paginator/paginator.component';
import { CommonModule } from '@angular/common';

@NgModule({
    declarations: [
        PaginatorComponent,
    ],
    imports: [
        CommonModule,
        MatPaginatorModule
    ],
    exports: [
        PaginatorComponent,
    ]
})
export class SharedModule {}
