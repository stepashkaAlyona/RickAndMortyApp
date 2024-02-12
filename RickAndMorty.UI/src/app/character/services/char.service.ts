import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { CharacterCreate } from '../models/character-create';

@Injectable({
  providedIn: 'root'
})
export class CharService {

  private apiUrl = 'https://localhost:7281';

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':'application/json'
    })
  }
  
  constructor(private httpClient: HttpClient) {   }

  getAllChars():Observable<any>{
    return this.httpClient.get(this.apiUrl + '/api/char')
      .pipe(catchError(this.handleError));
  }

  createChar(char:CharacterCreate):Observable<any>{
    return this.httpClient.post(this.apiUrl + '/api/char', JSON.stringify(char), this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  getCharById(id:number):Observable<any>{
    return this.httpClient.get(this.apiUrl + '/api/char/' + id)
      .pipe(catchError(this.handleError));
  }
  
  handleError(error:any) {
    let errorMessage = '';
    if(error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(errorMessage);
 }
}