import {Component, Input, OnInit} from 'angular2/core';
import {Service} from './../models/service';
import {DataService} from './../services/data.service';
import {RouteParams} from 'angular2/router';

@Component({
    selector: 'service-detail',
    templateUrl: 'app/components/service-detail.component.html'
})

export class ServiceDetailComponent implements OnInit {

    @Input()
    service: Service

    constructor(
        private dataService: DataService,
        private routeParams: RouteParams)
    { }

    ngOnInit() {
        let id = +this.routeParams.get('id');
        //this.dataService.getService('name')
        //    .then(result => this.service = result);
    }

    goBack() {
        window.history.back();
    }
}