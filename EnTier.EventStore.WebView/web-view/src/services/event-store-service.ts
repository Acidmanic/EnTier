import {HttpClient} from "@angular/common/http";
import {Observable, Subject} from "rxjs";
import {AggregateModel} from "../models/aggregate.model";
import {environment} from "../environments/environment";
import {Injectable} from "@angular/core";
import {EventWrapModel} from "../models/event-wrap.model";
import {EventPageModel} from "../models/event-page.model";


@Injectable({providedIn: 'root'})
export class EventStoreService {


  private baseUrl: string = environment.baseUrl;

  constructor(private http: HttpClient) {
  }


  public getAllAggregates(): Observable<AggregateModel[]> {

    let handler = new Subject<AggregateModel[]>();

    let url = this.baseUrl + 'event-store/streams';

    this.http.get<AggregateModel[]>(url).subscribe({
      next: aggs => handler.next(aggs),
      error: err => {
      },
      complete: () => {
      }
    });

    return handler;
  }


  public getAggregateByName(name: string): Observable<AggregateModel> {

    let handler = new Subject<AggregateModel>();

    let url = this.baseUrl + 'event-store/stream-by-name/' + name;

    this.http.get<AggregateModel>(url).subscribe({
      next: aggs => handler.next(aggs),
      error: err => {
      },
      complete: () => {
      }
    });

    return handler;
  }


  public getEventsPaginated(aggregateName: string, page: number, pageSize: number): Observable<EventPageModel> {

    let handler = new Subject<EventPageModel>();

    let from = (page - 1) * pageSize;

    let url = this.baseUrl + 'event-store/stream/' + aggregateName + '?from=' + from + '&count=' + pageSize;

    this.http.get<EventPageModel>(url).subscribe({
      next: aggs => handler.next(aggs),
      error: err => {
      },
      complete: () => {
      }
    });

    return handler;
  }

  public getEventsByStreamIdPaginated(aggregateName: string, streamId: string, page: number, pageSize: number):
    Observable<EventPageModel> {

    let handler = new Subject<EventPageModel>();

    let from = (page - 1) * pageSize;

    let url = this.baseUrl + 'event-store/stream/' + aggregateName +
      '/' + streamId + '?from=' + from + '&count=' + pageSize;

    this.http.get<EventPageModel>(url).subscribe({
      next: aggs => handler.next(aggs),
      error: err => {
      },
      complete: () => {
      }
    });

    return handler;
  }


}
