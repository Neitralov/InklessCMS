import { LoginUserRequest } from "../model/LoginUserRequest.ts"
import { AxiosResponse } from "axios"
import { LoginUserResponse } from "../model/LoginUserResponse.ts"
import { httpClient } from "./httpClient.ts"
import { RefreshUserTokenRequest } from "../model/RefreshUserTokenRequest.ts"

export default class AuthService {
  static login = async (request: LoginUserRequest ) : Promise<AxiosResponse<LoginUserResponse>> =>
    await httpClient.post<LoginUserResponse>('users/login', request)

  static refreshTokens = async (request: RefreshUserTokenRequest ) : Promise<AxiosResponse<LoginUserResponse>> =>
    await httpClient.post<LoginUserResponse>('users/refresh-tokens', request)
}
