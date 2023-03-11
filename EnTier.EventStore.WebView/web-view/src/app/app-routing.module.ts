import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AggregatesComponent} from "./pages/aggregates/aggregates.component";
import {EventsComponent} from "./pages/events/events.component";
import {AggregateModel} from "../models/aggregate.model";

const routes: Routes = [
  {path: "events/:aggregateName", component: EventsComponent,data:AggregateModel},
  {path: "", component: AggregatesComponent},
  {path: "#", component: AggregatesComponent},
  {path: '404', component: AggregatesComponent},
  {path: '**', component: AggregatesComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
