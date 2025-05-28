export interface ApiProblemDetails {
  title: string;
  status: number;
  type: string;
  errors: ApiError[];
}

export interface ApiError {
  code: string;
  name: string;
}

export enum ApiErrorCode {
  TokenExpired = "Token.TokenExpired",
}
