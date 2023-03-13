import {Injectable} from "@angular/core";
import {Observable, Subject} from "rxjs";


@Injectable({providedIn:'root'})
export class ModalBlur{

  private static css:string ='';


  public blurChange:Observable<string> = new Subject<string>();


  public blur(){
    ModalBlur.css="blur-as-f";

    (this.blurChange as Subject<string>).next(ModalBlur.css);
  }

  public clear(){
    ModalBlur.css="";

    (this.blurChange as Subject<string>).next(ModalBlur.css);
  }

  public getCss(){
    return ModalBlur.css;
  }


}
