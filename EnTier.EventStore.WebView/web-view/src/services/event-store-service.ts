import {HttpClient} from "@angular/common/http";
import {Observable, Subject} from "rxjs";
import {AggregateModel} from "../models/aggregate.model";
import {environment} from "../environments/environment";
import {Injectable} from "@angular/core";


@Injectable({ providedIn: 'root' })
export class EventStoreService {


  private baseUrl:string=environment.baseUrl;

  constructor(private http: HttpClient) {
  }


  public getAllAggregates():Observable<AggregateModel[]>{

    let handler = new Subject<AggregateModel[]>();

    let url = this.baseUrl + 'event-store/streams';

    this.http.get<AggregateModel[]>(url).subscribe({
      next: aggs => handler.next(aggs),
      error: err => {},
      complete: ()=>{}
    });

    return handler;
  }

}
