import {Component, Input, OnInit} from '@angular/core';
import {AggregateModel} from "../../../../models/aggregate.model";

@Component({
  selector: 'aggregate-view',
  templateUrl: './aggregate-view.component.html',
  styleUrls: ['./aggregate-view.component.css']
})
export class AggregateViewComponent implements OnInit {

  @Input('aggregate') aggregate:AggregateModel=new AggregateModel();


  constructor() { }

  ngOnInit(): void {

  }

}
