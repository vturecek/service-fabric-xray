import {bootstrap}    from 'angular2/platform/browser'
import 'rxjs/Rx';
import {AppComponent} from './components/app.component'
import {enableProdMode} from 'angular2/core';

enableProdMode();
bootstrap(AppComponent);