import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable } from "rxjs";
import { interval } from "rxjs";
import { CryptoApiClient } from '../services/api-client.service';
import { environment } from '../../environments/environment';
import { faHome, faList, faMoneyBillAlt, faChartBar, faExchangeAlt, faRandom, faSync } from '@fortawesome/free-solid-svg-icons';



@Component({
  selector: 'nav-menu',
  templateUrl: './navmenu.component.html',
  styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
  httpParams: string;
  backgroundTasks: number;
  backgroundTasksRetries: number;
  updateRelease: LatestRelease;

  faHome = faHome;
  faList = faList;
  faMoneyBillAlt = faMoneyBillAlt
  faChartBar = faChartBar;
  faExchangeAlt = faExchangeAlt;
  faRandom = faRandom;
  faSync = faSync;

  constructor(private http: HttpClient, private client: CryptoApiClient) { }


  ngOnInit() {
    const metrics = ['enqueued:count-or-null', 'processing:count', 'succeeded:count'];
    let httpParams = new HttpParams();
    metrics.forEach(id => {
      httpParams = httpParams.append('metrics[]', id);
    });
    this.httpParams = httpParams.toString();


    interval(environment.backgroundProcessCheckInterval).subscribe(() => {
      this.getHangfireStats()
        .subscribe(data => {
          this.backgroundTasks = data["processing:count"].value + data["enqueued:count-or-null"].value;
          this.backgroundTasksRetries = data["retries:count"].value;
        });
    });

    // Check every 10 Minutes
    interval(600000)
      .subscribe(() => {
        this.getLatestRelease();
      });

    var lastCheck = localStorage.getItem("lastUpdateCheck");
    if (lastCheck == null || parseInt(lastCheck) < Date.now() - 600000) {
      this.getLatestRelease();
    }
  }

  getHangfireStats(): Observable<HangfireStats> {
    var metrics = new FormData();
    metrics.append("metrics[]", "retries:count");
    metrics.append("metrics[]", "enqueued:count-or-null");
    metrics.append("metrics[]", "processing:count");

    return this.http.post<HangfireStats>(environment.apiBaseUrl + "/hangfire/stats", metrics);
  }

  recalculate(): void {
    this.client.recalculate().subscribe();
  }

  getLatestRelease(): void {
    this.http.get<LatestRelease>("https://api.github.com/repos/chwebdude/CryptoManager/releases/latest")
      .subscribe((res) => {
        var currentVersion = environment.version;
        if (res.name != currentVersion) {
          console.info("Update available. Current version " + currentVersion + ". Available: " + res.name);
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
  'retries:count': HangfireMetric;
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