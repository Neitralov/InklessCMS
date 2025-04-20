import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { useAuthStore } from "@/shared"
import { useNavigate } from "react-router"
import { LoginUserRequest } from "@/shared"

export const useLoginForm = () => {
  const login = useAuthStore(state => state.login)
  const isLoginDenied = useAuthStore(state => state.isLoginDenied)
  const navigate = useNavigate()

  const form = useForm<z.infer<typeof LoginUserRequest>>({
    resolver: zodResolver(LoginUserRequest),
    defaultValues: {
      email: "",
      password: ""
    },
  })

  const onSubmit = (loginRequest: z.infer<typeof LoginUserRequest>) => {
    login(loginRequest, navigate)
  }

  return { form, onSubmit, isLoginDenied }
}
