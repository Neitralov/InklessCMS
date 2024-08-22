import {LoginUserRequest} from "../model/LoginUserRequest.ts";
import {AxiosResponse} from "axios";
import {LoginUserResponse} from "../model/LoginUserResponse.ts";
import {httpClient} from "./httpClient.ts";
import {RefreshUserTokenRequest} from "../model/RefreshUserTokenRequest.ts";

export default class AuthService {
  static async login(request: LoginUserRequest ) : Promise<AxiosResponse<LoginUserResponse>> {
    return await httpClient.post<LoginUserResponse>('users/login', request)
  }

  static async refreshTokens(request: RefreshUserTokenRequest ) : Promise<AxiosResponse<LoginUserResponse>> {
    return await httpClient.post<LoginUserResponse>('users/refresh-tokens', request)
  }
}





