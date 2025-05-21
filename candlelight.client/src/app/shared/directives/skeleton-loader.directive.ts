import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appSkeletonLoader]'
})
export class SkeletonLoaderDirective {
    private hasView = false;

    constructor(
        private templateRef: TemplateRef<unknown>,
        private viewContainer: ViewContainerRef
    ) {}

    @Input() set appSkeletonLoader(isLoading: boolean) {
        this.viewContainer.clear();
        if (isLoading) {
            const skeleton = document.createElement('div');
            skeleton.classList.add('skeleton-loader');
            this.viewContainer.element.nativeElement.appendChild(skeleton);
            this.hasView = false;
        } else {
            this.viewContainer.createEmbeddedView(this.templateRef);
            this.hasView = true;
        }
    }
}
