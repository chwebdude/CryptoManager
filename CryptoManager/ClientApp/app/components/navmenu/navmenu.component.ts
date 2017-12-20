import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from "rxjs";
import { IntervalObservable } from "rxjs/observable/IntervalObservable";


@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
  httpParams: string;
  backgroundTasks :number;

  constructor(private http: HttpClient) { }


  ngOnInit() {
    const metrics = ['enqueued:count-or-null', 'processing:count', 'succeeded:count'];
    let httpParams = new HttpParams();
    metrics.forEach(id => {
      httpParams = httpParams.append('metrics[]', id);
    });
    this.httpParams = httpParams.toString();

    IntervalObservable.create(1000)
      .subscribe(() => {
        this.getHangfireStats()
          .subscribe(data => {
            this.backgroundTasks = data["processing:count"].value;
          });
      });
  }

  getHangfireStats(): Observable<HangfireStats> {
    return this.http.post<HangfireStats>("/hangfire/stats",
      this.httpParams,
      {
        headers: new HttpHeaders()
          .set('Content-Type', 'application/x-www-form-urlencoded')
      });
  }
}


class HangfireMetric {
  value: number;
}

class HangfireStats {
  'enqueued:count-or-null': HangfireMetric;
  'processing:count': HangfireMetric;
  'succeeded:count': HangfireMetric;
}