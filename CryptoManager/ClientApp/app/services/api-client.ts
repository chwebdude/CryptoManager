﻿/* tslint:disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v11.12.13.0 (NJsonSchema v9.10.14.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

import 'rxjs/add/observable/fromPromise';
import 'rxjs/add/observable/of';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/catch';

import { Observable } from 'rxjs/Observable';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpResponse, HttpResponseBase, HttpErrorResponse } from '@angular/common/http';

export const ApiBaseUrl = new InjectionToken<string>('ApiBaseUrl');

@Injectable()
export class CryptoApiClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(ApiBaseUrl) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    /**
     * @return Success
     */
    apiExchangesAvailableExchangesGet(): Observable<ExchangeMeta[]> {
        let url_ = this.baseUrl + "/api/Exchanges/availableExchanges";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Content-Type": "application/json", 
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).flatMap((response_ : any) => {
            return this.processApiExchangesAvailableExchangesGet(response_);
        }).catch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processApiExchangesAvailableExchangesGet(<any>response_);
                } catch (e) {
                    return <Observable<ExchangeMeta[]>><any>Observable.throw(e);
                }
            } else
                return <Observable<ExchangeMeta[]>><any>Observable.throw(response_);
        });
    }

    protected processApiExchangesAvailableExchangesGet(response: HttpResponseBase): Observable<ExchangeMeta[]> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            response instanceof HttpErrorResponse ? response.error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).flatMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (resultData200 && resultData200.constructor === Array) {
                result200 = [];
                for (let item of resultData200)
                    result200.push(ExchangeMeta.fromJS(item));
            }
            return Observable.of(result200);
            });
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).flatMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Observable.of<ExchangeMeta[]>(<any>null);
    }

    /**
     * @return Success
     */
    apiExchangesGet(): Observable<Exchange[]> {
        let url_ = this.baseUrl + "/api/Exchanges";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Content-Type": "application/json", 
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).flatMap((response_ : any) => {
            return this.processApiExchangesGet(response_);
        }).catch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processApiExchangesGet(<any>response_);
                } catch (e) {
                    return <Observable<Exchange[]>><any>Observable.throw(e);
                }
            } else
                return <Observable<Exchange[]>><any>Observable.throw(response_);
        });
    }

    protected processApiExchangesGet(response: HttpResponseBase): Observable<Exchange[]> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            response instanceof HttpErrorResponse ? response.error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).flatMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (resultData200 && resultData200.constructor === Array) {
                result200 = [];
                for (let item of resultData200)
                    result200.push(Exchange.fromJS(item));
            }
            return Observable.of(result200);
            });
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).flatMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Observable.of<Exchange[]>(<any>null);
    }

    /**
     * @value (optional) 
     * @return Success
     */
    apiExchangesPost(value: Exchange | null | undefined): Observable<void> {
        let url_ = this.baseUrl + "/api/Exchanges";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(value);

        let options_ : any = {
            body: content_,
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Content-Type": "application/json", 
            })
        };

        return this.http.request("post", url_, options_).flatMap((response_ : any) => {
            return this.processApiExchangesPost(response_);
        }).catch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processApiExchangesPost(<any>response_);
                } catch (e) {
                    return <Observable<void>><any>Observable.throw(e);
                }
            } else
                return <Observable<void>><any>Observable.throw(response_);
        });
    }

    protected processApiExchangesPost(response: HttpResponseBase): Observable<void> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            response instanceof HttpErrorResponse ? response.error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).flatMap(_responseText => {
            return Observable.of<void>(<any>null);
            });
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).flatMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Observable.of<void>(<any>null);
    }
}

export class ExchangeMeta implements IExchangeMeta {
    exchangeId?: ExchangeMetaExchangeId | undefined;
    name?: string | undefined;
    readonly supportsPublicKey?: boolean | undefined;
    labelPublicKey?: string | undefined;
    readonly supportsPrivateKey?: boolean | undefined;
    labelPrivateKey?: string | undefined;

    constructor(data?: IExchangeMeta) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.exchangeId = data["exchangeId"];
            this.name = data["name"];
            (<any>this).supportsPublicKey = data["supportsPublicKey"];
            this.labelPublicKey = data["labelPublicKey"];
            (<any>this).supportsPrivateKey = data["supportsPrivateKey"];
            this.labelPrivateKey = data["labelPrivateKey"];
        }
    }

    static fromJS(data: any): ExchangeMeta {
        let result = new ExchangeMeta();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["exchangeId"] = this.exchangeId;
        data["name"] = this.name;
        data["supportsPublicKey"] = this.supportsPublicKey;
        data["labelPublicKey"] = this.labelPublicKey;
        data["supportsPrivateKey"] = this.supportsPrivateKey;
        data["labelPrivateKey"] = this.labelPrivateKey;
        return data; 
    }
}

export interface IExchangeMeta {
    exchangeId?: ExchangeMetaExchangeId | undefined;
    name?: string | undefined;
    supportsPublicKey?: boolean | undefined;
    labelPublicKey?: string | undefined;
    supportsPrivateKey?: boolean | undefined;
    labelPrivateKey?: string | undefined;
}

export class Exchange implements IExchange {
    id?: string | undefined;
    comment?: string | undefined;
    publicKey?: string | undefined;
    privateKey?: string | undefined;
    exchangeId?: ExchangeId | undefined;

    constructor(data?: IExchange) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["id"];
            this.comment = data["comment"];
            this.publicKey = data["publicKey"];
            this.privateKey = data["privateKey"];
            this.exchangeId = data["exchangeId"];
        }
    }

    static fromJS(data: any): Exchange {
        let result = new Exchange();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["comment"] = this.comment;
        data["publicKey"] = this.publicKey;
        data["privateKey"] = this.privateKey;
        data["exchangeId"] = this.exchangeId;
        return data; 
    }
}

export interface IExchange {
    id?: string | undefined;
    comment?: string | undefined;
    publicKey?: string | undefined;
    privateKey?: string | undefined;
    exchangeId?: ExchangeId | undefined;
}

export enum ExchangeMetaExchangeId {
    _1 = 1, 
}

export enum ExchangeId {
    _1 = 1, 
}

export class SwaggerException extends Error {
    message: string;
    status: number; 
    response: string; 
    headers: { [key: string]: any; };
    result: any; 

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isSwaggerException = true;

    static isSwaggerException(obj: any): obj is SwaggerException {
        return obj.isSwaggerException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): Observable<any> {
    if(result !== null && result !== undefined)
        return Observable.throw(result);
    else
        return Observable.throw(new SwaggerException(message, status, response, headers, null));
}

function blobToText(blob: any): Observable<string> {
    return new Observable<string>((observer: any) => {
        if (!blob) {
            observer.next("");
            observer.complete();
        } else {
            let reader = new FileReader(); 
            reader.onload = function() { 
                observer.next(this.result);
                observer.complete();
            }
            reader.readAsText(blob); 
        }
    });
}