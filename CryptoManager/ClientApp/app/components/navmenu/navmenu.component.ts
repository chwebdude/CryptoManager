import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from "rxjs";
import { IntervalObservable } from "rxjs/observable/IntervalObservable";
import { CryptoApiClient } from '../../services/api-client';


@Component({
  selector: 'nav-menu',
  templateUrl: './navmenu.component.html',
  styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
  httpParams: string;
  backgroundTasks: number;
  updateRelease: LatestRelease;


  constructor(private http: HttpClient, private client: CryptoApiClient) { }


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

    // Check every 10 Minutes
    IntervalObservable.create(600000)
      .subscribe(() => {
        this.getLatestRelease();
      });

    var lastCheck = localStorage.getItem("lastUpdateCheck");
    if (lastCheck == null || parseInt(lastCheck) < Date.now() - 600000) {
      this.getLatestRelease();
    }
  }

  getHangfireStats(): Observable<HangfireStats> {
    return this.http.post<HangfireStats>("/hangfire/stats",
      this.httpParams,
      {
        headers: new HttpHeaders()
          .set('Content-Type', 'application/x-www-form-urlencoded')
      });
  }

  recalculate(): void {
    this.client.apiTransactionsRecalculatePost().subscribe();
  }

  getLatestRelease(): void {
    this.http.get<LatestRelease>("https://api.github.com/repos/chwebdude/CryptoManager/releases/latest")
      .subscribe((res) => {
        var currentVersion = "0.1.8";
        if (res.name != currentVersion) {
          console.info("Update available");
          this.updateRelease = res;
        } else {
          localStorage.setItem("lastUpdateCheck", Date.now().toString());
        }
      });
  }

  openGithubRelease() {
    window.open(this.updateRelease.html_url, '_blank');

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

class LatestRelease {
  html_url: string;
  id: number;
  name: string;
  body: string;
  draft: boolean;
  prerelease: boolean;
  published_at: Date;
}