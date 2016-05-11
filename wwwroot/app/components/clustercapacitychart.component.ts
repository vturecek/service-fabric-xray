import {Component, Input} from 'angular2/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass} from 'angular2/common';
import {CHART_DIRECTIVES} from 'ng2-charts/ng2-charts';

import {DataService} from './../services/data.service';

declare var Chart: any;

class CapacityHistoryDataPoint {

    public constructor(
        public capacity: number,
        public timestamp: Date) {
    }
}

class CapacityHistory {

    public constructor(
        public capacityName: string,
        public capacityData: CapacityHistoryDataPoint[]) {
    }
}

@Component({
    selector: 'cluster-capacity-chart',
    templateUrl: 'app/components/clustercapacitychart.component.html',
    styleUrls: ['app/components/clustercapacitychart.component.css'],
    directives: [CHART_DIRECTIVES, NgClass, CORE_DIRECTIVES, FORM_DIRECTIVES]
})
export class ClusterCapacityChart {

    @Input()
    private loadMetricHistory: CapacityHistory[];

    
    private lineChartData: Array<any>;
    private lineChartLabels: Array<any>;
    private lineChartSeries: Array<any>;
    
    private lineChartOptions: any = {
        animation: true,
        responsive: true,
        maintainAspectRatio: false,
        fontFamily: "'Segoe UI', 'Segoe', Arial, sans-serif",
        scaleFontFamily: "'Segoe UI', 'Segoe', Arial, sans-serif",
        scaleFontSize: 11,
        showTooltips: true,
        pointLabelFontFamily: "'Segoe UI', 'Segoe', Arial, sans-serif",
        tooltipCornerRadius: 0,
        tooltipFontFamily: "'Segoe UI', 'Segoe', Arial, sans-serif",
        tooltipFontSize: 12,
        tooltipXPadding: 10,
        tooltipYPadding: 10,
        pointDotRadius: 2,
        bezierCurveTension: 0.2,
        datasetStrokeWidth: 1,
        scaleGridLineColor: "#333333",
        multiTooltipTemplate: '<%if (datasetLabel){%><%=datasetLabel %>: <%}%><%= value %>'
    };

    private lineChartColours: Array<any> = [
        { // grey
            fillColor: 'rgba(148,159,177,0.2)',
            strokeColor: 'rgba(148,159,177,1)',
            pointColor: 'rgba(148,159,177,1)',
            pointStrokeColor: '#CCC',
            pointHighlightFill: '#fff',
            pointHighlightStroke: 'rgba(148,159,177,0.8)'
        },
        { // dark grey
            fillColor: 'rgba(77,83,96,0.2)',
            strokeColor: 'rgba(77,83,96,1)',
            pointColor: 'rgba(77,83,96,1)',
            pointStrokeColor: '#CCC',
            pointHighlightFill: '#fff',
            pointHighlightStroke: 'rgba(77,83,96,1)'
        }
    ];

    private lineChartLegend: boolean = true;
    private lineChartType: string = 'Line';

    public constructor(private dataService: DataService)
    {
        
        Chart.defaults.global.defaultFontFamily = '"Segoe UI","Segoe",Arial,sans-serif';
        this.loadMetricHistory = [];
        
        var points1: CapacityHistoryDataPoint[] = [];
        for (let i = 0; i < 20; ++i) {
            points1.push(new CapacityHistoryDataPoint(Math.random() * 100, new Date(2016, 5, 9, 1, i)));
        }

        var points2: CapacityHistoryDataPoint[] = [];
        for (let i = 0; i < 20; ++i) {
            points2.push(new CapacityHistoryDataPoint(Math.random() * 200, new Date(2016, 5, 9, 1, i)));
        }

        this.loadMetricHistory.push(new CapacityHistory("MemoryKB", points1));
        this.loadMetricHistory.push(new CapacityHistory("DiskKB", points2));

        
        //data
        this.lineChartData = this.loadMetricHistory.map(x => x.capacityData.map(y => y.capacity));

        // series
        this.lineChartSeries = this.loadMetricHistory.map(x => x.capacityName);

        // labels
        this.lineChartLabels = this.loadMetricHistory[0].capacityData.map(x => x.timestamp.toLocaleTimeString());
    }

    // events
    chartClicked(e: any) {
        //console.log(e);
    }

    chartHovered(e: any) {
       // console.log(e);
    }

}