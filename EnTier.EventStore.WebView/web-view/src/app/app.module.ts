import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AggregatesComponent } from './pages/aggregates/aggregates.component';
import { AggregateViewComponent } from './pages/aggregates/aggregate-view/aggregate-view.component';
import {HttpClientModule} from "@angular/common/http";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { EventsComponent } from './pages/events/events.component';

@NgModule({
  declarations: [
    AppComponent,
    AggregatesComponent,
    AggregateViewComponent,
    EventsComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    NgbModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
