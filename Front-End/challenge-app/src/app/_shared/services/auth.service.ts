import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticatedUser } from '../models/authenticated-user.model';
import { Response } from 'src/app/_shared/models/response.model';
import { StorageService } from './storage.service';

const baseUrl = 'https://localhost:7255/api/Authenticate';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  constructor(
    private http: HttpClient,
    private storage: StorageService) { }

  login(params: Record<string, unknown>): Observable<Response<AuthenticatedUser>> {
    return this.http.post<Response<AuthenticatedUser>>(`${baseUrl}/login`, params);
  }

  refreshToken(token: string): Observable<Response<AuthenticatedUser>> {
    return this.http.post<Response<AuthenticatedUser>>(`${baseUrl}/login`,
      { refreshToken: token, email: this.storage.getUser().email}
    );
  }

}
