import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {EventWrapModel} from "../../../models/event-wrap.model";
import {AggregateModel} from "../../../models/aggregate.model";
import {EventStoreService} from "../../../services/event-store-service";
import {ActivatedRoute} from "@angular/router";
import {Observable, Subscription} from "rxjs";


@Component({
  selector: 'events',
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.css']
})
export class EventsComponent implements OnInit, OnDestroy {


  aggregate: AggregateModel = new AggregateModel();
  events: EventWrapModel[] = [];

  selectedEvent: EventWrapModel | null = null;


  page: number = 1;
  pageSize: number = 50;

  private subscription: Subscription = new Subscription();

  constructor(private svcEventStore: EventStoreService,
              private route: ActivatedRoute) {
  }


  ngOnInit(): void {

    this.subscription = this.route.paramMap.subscribe({
      next: p => {
        console.log('params:', p);
        let aggregateName = p.get('aggregateName');
        console.log('agName', aggregateName);
        if (aggregateName) {
          this.svcEventStore.getAggregateByName(aggregateName).subscribe({
            next: value => {
              console.log('aggr', value);
              this.aggregate = value;
              this.fetchItemsOfCurrentPage();
            },
            error: err => {
              console.log(err);
            },
            complete: () => {
            }
          });
        }

      }
    });


  }


  fetchItemsOfCurrentPage() {

    this.svcEventStore.getEventsPaginated(this.aggregate.streamName, this.page, this.pageSize).subscribe({
      next: evs => {
        console.log('events:', evs);
        this.events = evs.events;
        console.log('events:', this.events);
      },
      error: err => {
      },
      complete: () => {
      }
    });

  }

  tableColumns() {
    if (this.selectedEvent) {
      return "col-8"
    }
    return "col-12"
  }


  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  selectedEventJson() {
    if (this.selectedEvent) {
      return JSON.stringify(this.selectedEvent.event,undefined,'    ');
    }
    return "No Event has been selected";
  }


  preview(event:any){

    var longestString =this.aggregate.streamName;
    var longestKey ='';
    var longestLength=0;

      for (let key in event){

        let value =event[key];

        if(typeof value == 'string'){

          let length = value.length;

          if(length>longestLength){
            longestLength = length;
            longestString = value;
            longestKey = key + ': ';
          }
        }

      }

      return longestKey +  longestString;
  }


}
