export interface ResponseBase {
  errorCode: string;
  message: string;
  code: string;
}

export interface Response<T> {
  data?: T;
}

export interface ErrorResponse {
  error: ResponseBase;
}
