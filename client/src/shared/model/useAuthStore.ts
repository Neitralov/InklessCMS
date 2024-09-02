import { create } from 'zustand'
import AuthService from "../api/authService.ts";
import {NavigateFunction} from "react-router-dom";
import {LoginUserRequest} from "./LoginUserRequest.ts";

interface UseAuthStore {
  isAuth: boolean
  isLoading: boolean
  isLoginDenied: boolean
  login: (loginData: LoginUserRequest, navigate: NavigateFunction) => void
  logout: () => void
  checkIsAuth: () => void
}

export const useAuthStore = create<UseAuthStore>((set) => ({
  isAuth: false,
  isLoginDenied: false,
  isLoading: true,
  login: async (loginData, navigate) => {
    try {
      const response = await AuthService.login(loginData)
      localStorage.setItem('accessToken', response.data.accessToken)
      localStorage.setItem('refreshToken', response.data.refreshToken)
      set({isAuth: true})
      navigate('/inkless')
    } catch (error) {
      set({isLoginDenied: true})
      console.error(error)
    }
  },
  logout: () => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    set({ isAuth: false })
    window.location.href = '/inkless/login'
  },
  checkIsAuth: async () => {
    const accessToken = localStorage.getItem('accessToken')
    const refreshToken = localStorage.getItem('refreshToken')

    if (accessToken == null || refreshToken == null) {
      set({ isAuth: false, isLoading: false })
      return
    }

    const request = { expiredAccessToken: accessToken, refreshToken: refreshToken}

    try {
      const response = await AuthService.refreshTokens(request)
      localStorage.setItem('accessToken', response.data.accessToken)
      localStorage.setItem('refreshToken', response.data.refreshToken)
      set({ isAuth: true, isLoading: false })
    } catch {
      window.location.href = '/inkless/login'
    }
  }
}))
