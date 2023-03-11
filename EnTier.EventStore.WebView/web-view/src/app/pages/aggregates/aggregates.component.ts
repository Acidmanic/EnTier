import { Component, OnInit } from '@angular/core';
import {AggregateModel} from "../../../models/aggregate.model";
import {EventStoreService} from "../../../services/event-store-service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'aggregates',
  templateUrl: './aggregates.component.html',
  styleUrls: ['./aggregates.component.css']
})
export class AggregatesComponent implements OnInit {


  aggregates:AggregateModel[]=[];


  constructor(private svcEventStore:EventStoreService) { }

  ngOnInit(): void {

    this.svcEventStore.getAllAggregates().subscribe({
      next: aggs=>this.aggregates=aggs
    });
  }

  aggregateClicked(aggregate:AggregateModel){

  }

}
