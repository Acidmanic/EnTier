import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {AggregatesComponent} from "./pages/aggregates/aggregates.component";

const routes: Routes = [
  {path:"", component:AggregatesComponent},
  {path:"#", component:AggregatesComponent},
  { path: '404', component: AggregatesComponent },
  { path: '**', component: AggregatesComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
