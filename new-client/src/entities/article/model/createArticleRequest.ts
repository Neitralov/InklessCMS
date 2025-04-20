import { z } from "zod"

export const createArticleRequest = z.object({
  articleId: z.string()
    .min(3, { message: `Строка должна содержать минимум 3 символа` })
    .max(64, { message: `Строка должна содержать максимум 64 символа` })
    .regex(/^[a-zA-Z0-9-]+$/),
  title: z.string()
    .min(3, { message: `Строка должна содержать минимум 3 символа` })
    .max(64, { message: `Строка должна содержать максимум 64 символа` }),
  description: z.string().max(64, { message: `Строка должна содержать максимум 64 символа` }),
  text: z.string(),
  isPublished: z.boolean(),
  isPinned: z.boolean()
})
