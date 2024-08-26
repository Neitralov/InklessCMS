import axios from "axios"
import AuthService from "./authService.ts"
import { ApiUrl } from "../config/config.ts"

export const httpClient = axios.create({
  baseURL: ApiUrl,
})

httpClient.interceptors.request.use((config) => {
  config.headers.Authorization = `Bearer ${ localStorage.getItem('accessToken') }`
  return config
})

httpClient.interceptors.response.use(response => {
  return response
}, async error => {
  const originalRequest = error.config

  if (error.response.status === 401) {
    const accessToken = localStorage.getItem('accessToken')
    const refreshToken = localStorage.getItem('refreshToken')

    const request = { expiredAccessToken: accessToken!, refreshToken: refreshToken!}

    try {
      const response = await AuthService.refreshTokens(request)
      localStorage.setItem('accessToken', response.data.accessToken)
      localStorage.setItem('refreshToken', response.data.refreshToken)
      return httpClient.request(originalRequest)
    } catch (error) {
      window.location.href = '/login'
      return Promise.reject(error);
    }
  } else if (error.status === 403) {
    console.error('У пользователя недостаточно прав на выполнение данного запроса')

  } else {
    throw error
  }
})
