import { createArticleRequest } from "@/entities/article/model/createArticleRequest"
import { useArticleCRUD } from "@/entities/article/model/useArticleCRUD"
import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { z } from "zod"
import { useArticleEditorStore } from "./useArticleEditorStore"

export const useArticleForm = () => {
  const { addArticle } = useArticleCRUD()
  const closeArticleEditor = useArticleEditorStore(state => state.closeEditor)

  const form = useForm<z.infer<typeof createArticleRequest>>({
    resolver: zodResolver(createArticleRequest),
    defaultValues: {
      articleId: "",
      title: "",
      description: "",
      text: "",
      isPublished: false,
      isPinned: false
    },
  })

  const clearForm = () => {
    form.reset()
    form.clearErrors()
  }

  const onSubmit = async (request: z.infer<typeof createArticleRequest>) => {
    const isSuccess = await addArticle(request, form.setError)
    if (isSuccess) {
      clearForm()
      closeArticleEditor()
    }
  }

  return { form, onSubmit, clearForm }
}
