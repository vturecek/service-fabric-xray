import {Component, provide} from 'angular2/core';
import {RouteConfig, ROUTER_DIRECTIVES, ROUTER_PROVIDERS} from 'angular2/router';
import {ClusterComponent} from './cluster.component';
import {ServiceDetailComponent} from './service-detail.component';
import {DataService} from './../services/data.service';
import {HttpDataService} from './../services/httpdata.service';
import {MockDataService} from './../services/mocks/mockdata.service';
import {DashboardComponent} from './dashboard.component';
import { HTTP_PROVIDERS }    from 'angular2/http';

@RouteConfig([
    {
        path: '/cluster',
        name: 'Cluster',
        component: ClusterComponent
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
    selector: 'xray',
    templateUrl: 'app/components/app.component.html',
    styleUrls: ['app/components/app.component.css'],
    directives: [ROUTER_DIRECTIVES],
    providers: [HTTP_PROVIDERS, ROUTER_PROVIDERS, provide(DataService, { useClass: HttpDataService })]
})
export class AppComponent {
    title: String
}