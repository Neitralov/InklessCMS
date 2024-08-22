import {useArticleStore} from "./useArticleStore.ts";

export const useArticleCRUD = () => {
  const addArticle = useArticleStore(state => state.addArticle)
  const getArticle = useArticleStore(state => state.getArticle)
  const updateArticle = useArticleStore(state => state.updateArticle)
  const deleteArticle = useArticleStore(state => state.deleteArticle)

  return { addArticle, getArticle, updateArticle, deleteArticle }
}