export interface RefreshUserTokenRequest {
  expiredAccessToken: string,
  refreshToken: string
}