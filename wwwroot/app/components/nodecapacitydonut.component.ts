import {Component, Input, AfterViewInit, OnChanges, SimpleChange, ViewChild, ElementRef} from 'angular2/core';
import {ClusterCapacity} from './../models/clustercapacity';

declare var Chart: any;

@Component({
    selector: 'node-capacity-donut',
    templateUrl: 'app/components/nodecapacitydonut.component.html',
    styleUrls: ['app/components/nodecapacitydonut.component.css']
})
export class NodeCapacityDonut implements AfterViewInit, OnChanges {
    @Input()
    private name: string;

    @Input()
    private capacity: number;

    @Input()
    private load: number;

    @Input()
    private remainingCapacity: number;

    @Input()
    private isCapacityViolation: boolean;

    @Input()
    private selectedColors: string;

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
                cutoutPercentage: 94,
                legend: {
                    display: false
                }
            }
        });

        if (this.capacity > 0) {
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
        
        dataset.data[0] = this.load
        dataset.data[1] = this.remainingCapacity;
        dataset.backgroundColor = [this.getLoadColor(), "#666666"];
        
        this.chart.update();
    }

    private getLoadColor(): string {
        if (this.isCapacityViolation) {
            return "#E81123";
        }

        if (this.load / this.capacity > 0.9) {
            return "#FCD116";
        }

        return this.selectedColors == 'status' ? "#00ABEC" : '#7FBA00';
    }
}