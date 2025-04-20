import { useAuthStore } from "@/shared/index";
import { Navigate, Outlet } from "react-router";
import { useEffect } from "react";

export const PrivateRoutes = () => {
  const isAuth = useAuthStore(state => state.isAuth)
  const isLoading = useAuthStore(state => state.isLoading)
  const checkAuth = useAuthStore(state => state.checkIsAuth)

  useEffect(() => {
    checkAuth()
  }, [checkAuth])

  if (!isLoading) {
    if (isAuth) {
      return(<Outlet />)
    } else {
      return <Navigate to={"/inkless/login"} />
    }
  }
}
