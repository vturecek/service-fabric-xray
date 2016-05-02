/// <reference path="../../../typings/jquery/jquery.d.ts" />
import {ElementRef} from 'angular2/core';
import {LoadMetric} from './../models/loadmetric';
import {DeployedEntityViewModel} from './../viewmodels/deployedentityviewmodel';

/**
Layout is computed thusly:
Each component height = % capacity of parent mapped to available element space less vertical margin.

    heightPx = (componentMetric / parentCapacity) * parentContainerHeightPx - verticalMargin

Node is the root component that determines available container height.
A node's capacity is normalized to a screen view size yielding an absolute height in pixels representing capacity.
*/
export abstract class MetricComponent {
    
    protected parentCapacity: number;
    protected parentContainerSize: number;
    protected selectedColors: string = "status";
    protected selectedMetricName: string = "Default Replica Count";
    protected container: ElementRef;

    /**
     * Computes the height in pixels for the given metric.
     * @param metric
     */
    protected getElementHeight(metric: number): number {
        return Math.max(0, ((metric / this.parentCapacity) * this.parentContainerSize) - this.getOuterVerticalSpacing(this.container));
    }

    /**
     * Computes the height in pixels of the inner container for the given metric
     * @param metric
     */
    protected getContainerSize(metric: number): number {
        return Math.max(0, this.getElementHeight(metric) - this.getInnerVerticalSpacing(this.container));
    }

    /**
     * Gets the value of the currently selected metric.
     * @param metrics
     */
    protected getSelectedMetric(metrics: LoadMetric[]): number {
        let metric = metrics.find(x => x.name == this.selectedMetricName);
        
        return metric ? metric.value : 1;
    }

    /**
     * Determines if the selected metric is in the list of given metrics.
     * @param metrics
     */
    protected hasSelectedMetric(metrics: LoadMetric[]): boolean {
        return metrics.find(x => x.name == this.selectedMetricName) != undefined;
    }

    /**
     * Gets the height of the top and bottom margin of the given element.
     * @param el
     */
    protected getOuterVerticalSpacing(el: ElementRef) {
        if (el) {
            return jQuery(el.nativeElement).outerHeight(true) - jQuery(el.nativeElement).outerHeight();
        }
        return 0;
    }

    /**
     * Gets the height of the top and bottom padding and border width of the given element.
     * @param el
     */
    protected getInnerVerticalSpacing(el: ElementRef) {
        if (el) {
            return jQuery(el.nativeElement).outerHeight(false) - jQuery(el.nativeElement).height();
        }
        return 0;
    }


    protected getDeployedEntityClass<T>(entity: DeployedEntityViewModel<T>, classes: string[]): string[] {
        let result: string = "";
        switch (this.selectedColors) {
            case "status":
                result = entity.status;
                break;
            case "health":
                result = entity.healthState;
                break;
        }

        result = result ?
            result.toLowerCase() :
            "unknown";

        return [result].concat(classes);
    }
}