import { Directive, ElementRef, HostListener, Input } from 'angular2/core';

@Directive({
    selector: '[nodeCapacityInfo]'
})
export class NodeCapacityInfoDirective {
    private el: HTMLElement;
    private height: string;
     
    @Input('nodeCapacityInfo')
    private name: string;

    public constructor(el: ElementRef)
    {
        this.el = el.nativeElement;
    }
    
    @HostListener('mouseenter') onMouseEnter() {
        this.height = this.el.style.height;
        this.el.style.height = '10px';
    }

    @HostListener('mouseleave') onMouseLeave() {
        this.el.style.height = this.height
        
    }
}