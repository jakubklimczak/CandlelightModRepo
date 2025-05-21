import {
  Directive,
  ElementRef,
  Renderer2,
  AfterViewInit,
  HostListener
} from '@angular/core';

@Directive({
  selector: '[appImageSkeletonLoader]'
})
export class ImageSkeletonLoaderDirective implements AfterViewInit {
  private skeleton: HTMLElement;

  constructor(private el: ElementRef<HTMLImageElement>, private renderer: Renderer2) {
    this.skeleton = this.renderer.createElement('div');
    this.renderer.addClass(this.skeleton, 'skeleton-img');
    this.renderer.setStyle(this.skeleton, 'position', 'absolute');
    this.renderer.setStyle(this.skeleton, 'inset', '0');
    this.renderer.setStyle(this.skeleton, 'background', '#e0e0e0');
    this.renderer.setStyle(this.skeleton, 'animation', 'pulse 1.5s infinite');
    this.renderer.setStyle(this.skeleton, 'z-index', '1');
  }

  ngAfterViewInit(): void {
    const parent = this.renderer.parentNode(this.el.nativeElement);

    const computedStyle = window.getComputedStyle(parent);
    if (computedStyle.position === 'static') {
      this.renderer.setStyle(parent, 'position', 'relative');
    }

    this.renderer.setStyle(this.el.nativeElement, 'opacity', '0');
    this.renderer.setStyle(this.el.nativeElement, 'transition', 'opacity 0.3s ease');

    this.renderer.appendChild(parent, this.skeleton);
  }

  @HostListener('load')
  onLoad(): void {
    this.renderer.setStyle(this.el.nativeElement, 'opacity', '1');
    this.renderer.removeChild(this.renderer.parentNode(this.el.nativeElement), this.skeleton);
  }
}
