import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {EventWrapModel} from "../../../models/event-wrap.model";
import {AggregateModel} from "../../../models/aggregate.model";
import {EventStoreService} from "../../../services/event-store-service";
import {ActivatedRoute} from "@angular/router";
import {Observable, Observer, Subscription} from "rxjs";
import {FormControl} from "@angular/forms";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";


@Component({
  selector: 'events',
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.css']
})
export class EventsComponent implements OnInit, OnDestroy {


  aggregate: AggregateModel = new AggregateModel();
  events: EventWrapModel[] = [];

  selectedEvent: EventWrapModel = new EventWrapModel();


  page: number = 1;
  pageSize: number = 50;
  filterStreamId: string | null = null;

  filter = new FormControl('', {nonNullable: true});

  private subscription: Subscription = new Subscription();

  constructor(private svcEventStore: EventStoreService,
              private route: ActivatedRoute,
              private modalService: NgbModal) {
  }


  ngOnInit(): void {

    this.subscription = this.route.paramMap.subscribe({
      next: p => {

        let aggregateName = p.get('aggregateName');

        if (aggregateName) {
          this.svcEventStore.getAggregateByName(aggregateName).subscribe({
            next: value => {

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

    this.filter.valueChanges.subscribe({
      next: streamId => {
        console.log('streamId',streamId);
        this.filterStreamId = streamId;
        this.fetchItemsOfCurrentPage();
      },
      error: err => {
      },
      complete: () =>{}
    });

  }


  fetchItemsOfCurrentPage() {

    let observer: Observer<{ events: EventWrapModel[] }> = {
      next: evs => {
        console.log('events:', evs);
        this.events = evs.events;
        console.log('events:', this.events);
      },
      error: err => {
      },
      complete: () => {
      }
    };

    if (this.filterStreamId) {
      this.svcEventStore.getEventsByStreamIdPaginated(this.aggregate.streamName,
        this.filterStreamId, this.page, this.pageSize).subscribe(observer);
    } else {
      this.svcEventStore.getEventsPaginated(this.aggregate.streamName, this.page, this.pageSize).subscribe(observer);
    }
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
      return JSON.stringify(this.selectedEvent.event, undefined, '    ');
    }
    return "No Event has been selected";
  }


  eventRowClicked(content:any, event:EventWrapModel){

    this.selectedEvent = event;

    this.modalService.open(content, {
      backdropClass: 'json-modal-backdrop',
      modalDialogClass: 'json-modal-dark',
      windowClass: 'dark-modal',
      centered: true
    });

  }

  preview(event: any) {

    var longestString = this.aggregate.streamName;
    var longestKey = '';
    var longestLength = 0;

    for (let key in event) {

      let value = event[key];

      if (typeof value == 'string') {

        let length = value.length;

        if (length > longestLength) {
          longestLength = length;
          longestString = value;
          longestKey = key + ': ';
        }
      }

    }

    return longestKey + longestString;
  }


}
