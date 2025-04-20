import { LoginUserRequest } from "../model/LoginUserRequest.ts"
import { AxiosResponse } from "axios"
import { LoginUserResponse } from "../model/LoginUserResponse.ts"
import { httpClient } from "./httpClient.ts"
import { RefreshUserTokenRequest } from "../model/RefreshUserTokenRequest.ts"
import { z } from "zod"

export default class AuthService {
  static login = async (request: z.infer<typeof LoginUserRequest> ) : Promise<AxiosResponse<LoginUserResponse>> =>
    await httpClient.post<LoginUserResponse>('api/users/login', request)

  static refreshTokens = async (request: RefreshUserTokenRequest ) : Promise<AxiosResponse<LoginUserResponse>> =>
    await httpClient.post<LoginUserResponse>('api/users/refresh-tokens', request)
}
