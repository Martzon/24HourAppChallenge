import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Response } from 'src/app/_shared/models/response.model';
import { UserPaginate } from '../models/user-paginate.model';
import { User } from '../models/user.model';

const baseUrl = 'https://localhost:7255/api/User';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private http: HttpClient) { }

  getUsersWithPagination(skip: number, take: number, sort: string, filter: string): Observable<Response<UserPaginate[]>> {
    return this.http.post<Response<UserPaginate[]>>(`${baseUrl}/getuserswithpagination`,
      {
        skip: skip.toString(),
        take: take.toString(),
        sort,
        filter
      }
    );
  }

  updateUser(updatedUser: User) {
    return this.http.put<any>(`${baseUrl}`, updatedUser);
  }

  AddUser(user: User) {
    return this.http.post<any>(`${baseUrl}/createclient`, user);
  }

  deleteUser(userId: string) {
    return this.http.delete<any>(`${baseUrl}?Id=${userId}`);
  }
}
