import {Component, Input, OnChanges, SimpleChange} from 'angular2/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass} from 'angular2/common';
import {CHART_DIRECTIVES} from 'ng2-charts/ng2-charts';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';

declare var Chart: any;

@Component({
    selector: 'cluster-capacity-donut',
    templateUrl: 'app/components/clustercapacitydonut.component.html',
    styleUrls: ['app/components/clustercapacitydonut.component.css'],
    directives: [CHART_DIRECTIVES, NgClass, CORE_DIRECTIVES, FORM_DIRECTIVES]
})
export class ClusterCapacityDonut implements OnChanges {

    @Input()
    private capacityViewModel: ClusterCapacityViewModel;


    private chartColors: Array<any>;
    private chartData: Array<number>;
    private chartLabels = ['Load', 'Remaining capacity'];
    private chartType = 'Doughnut';
    private chartOptions =
    {
        animation: true,
        responsive: true,
        maintainAspectRatio: false,
        tooltipCornerRadius: 0,
        tooltipFontFamily: "'Segoe UI', 'Segoe', Arial, sans-serif",
        tooltipFontSize: 12,
        segmentShowStroke: true,
        segmentStrokeColor: "#000000",
        segmentStrokeWidth: 1,
        percentageInnerCutout: 90,
        animationEasing: "easeOutQuint",
    }

    public ngOnChanges(changes: { [propertyName: string]: SimpleChange }) {

        this.chartData = [this.capacityViewModel.load, this.capacityViewModel.remainingCapacity];

        this.chartColors = [
            {
                color: this.getLoadColor(),
                highlight: "#FFFFFF"
            },
            {
                color: "#666666",
                highlight: "#CCCCCC"
            }
        ];
    }

    private getLoadColor(): string {
        if (this.capacityViewModel.isClusterCapacityViolation) {
            return "#E81123";
        }

        if (this.capacityViewModel.load / this.capacityViewModel.capacity > 0.9) {
            return "#FCD116";
        }

        return "#00ABEC";
    }

    private chartClicked(e: any) {
        console.log(e);
    }

    private chartHovered(e: any) {
        console.log(e);
    }
}