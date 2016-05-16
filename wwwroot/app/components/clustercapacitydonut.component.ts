import {Component, Input, AfterViewInit, OnChanges, SimpleChange, ViewChild, ElementRef} from 'angular2/core';
import {ClusterCapacity} from './../models/clustercapacity';

declare var Chart: any;

@Component({
    selector: 'cluster-capacity-donut',
    templateUrl: 'app/components/clustercapacitydonut.component.html',
    styleUrls: ['app/components/clustercapacitydonut.component.css']
})
export class ClusterCapacityDonut implements AfterViewInit, OnChanges {

    @Input()
    private capacityViewModel: ClusterCapacity;

    @ViewChild("chartCanvas")
    private chartCanvasElement: ElementRef;

    private chart: any;
    
    public ngAfterViewInit(): void {
        this.chart = new Chart(this.chartCanvasElement.nativeElement, {
            type: 'doughnut',
            data: {
                labels: ['Load', 'Remaining capacity'],
                datasets: [
                    {
                        data: [0, 0],
                        hoverBackgroundColor: ["#FFFFFF", "#CCCCCC"],
                        borderWidth: 1,
                        borderColor: "#000000"
                    }
                ]
            },
            options: {
                animation: {
                    duration: 0,
                    animateRotate: false
                },
                responsive: false,
                cutoutPercentage: 90,
                legend: {
                    display: false
                }
            }
        });

        if (this.capacityViewModel) {
            this.update();
        }
    }

    public ngOnChanges(changes: { [propertyName: string]: SimpleChange }) {
        
        if (this.chart) {
            this.update();
        }
    }

    private update(): void {
        
        let dataset = this.chart.config.data.datasets[0];
        
        dataset.data[0] = this.capacityViewModel.load
        dataset.data[1] = this.capacityViewModel.remainingCapacity;
        dataset.backgroundColor = [this.getLoadColor(), "#666666"];
        
        this.chart.update();
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
}