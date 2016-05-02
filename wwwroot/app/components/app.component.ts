import {Component} from 'angular2/core';
import {RouteConfig, ROUTER_DIRECTIVES, ROUTER_PROVIDERS} from 'angular2/router';
import {ServiceComponent} from './service.component';
import {ServiceDetailComponent} from './service-detail.component';
import {DataService} from './../services/data.service';
import {DashboardComponent} from './dashboard.component';
import { HTTP_PROVIDERS }    from 'angular2/http';

@RouteConfig([
    {
        path: '/services',
        name: 'Services',
        component: ServiceComponent
    },
    {
        path: '/dashboard',
        name: 'Dashboard',
        component: DashboardComponent,
        useAsDefault: true
    },
    {
        path: '/service/:id',
        name: 'ServiceDetail',
        component: ServiceDetailComponent
    }
])

@Component({
    selector: 'my-app',
    templateUrl: 'app/components/app.component.html',
    styleUrls: ['app/components/app.component.css'],
    directives: [ROUTER_DIRECTIVES],
    providers: [HTTP_PROVIDERS, DataService, ROUTER_PROVIDERS]
})
    
export class AppComponent {
    title: String 
}