import { z } from "zod"

export const LoginUserRequest = z.object({
  email: z.string().min(3, { message: `Строка должна содержать минимум 3 символа` }).max(50),
  password: z.string().min(3, { message: `Строка должна содержать минимум 3 символа` }).max(50)
})
