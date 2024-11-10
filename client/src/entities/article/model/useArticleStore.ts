import { create } from 'zustand'
import { Article } from "./article.ts"
import ArticleService from "../api/articleService.ts"
import { CreateArticleRequest } from "./createArticleRequest.ts"
import { UpdateArticleRequest } from "./updateArticleRequest.ts"
import { UseFormSetError } from "react-hook-form"

interface UseArticleStore {
  articles: Article[]
  totalCount: number
  pageNumber: number
  pageSize: number
  setPageNumber: (newPageNumber: number) => void
  loadPrevPage: () => void
  loadNextPage: () => void
  addArticle: (request: CreateArticleRequest, setError: UseFormSetError<CreateArticleRequest>) => Promise<boolean>
  getArticles: (pageNumber: number, pageSize: number) => Promise<void>
  getArticle: (articleId: string) => Promise<Article | undefined>
  updateArticle: (request: CreateArticleRequest) => Promise<void>
  deleteArticle: (articleId: string) => Promise<void>
}

export const useArticleStore = create<UseArticleStore>((set, get) => ({
  articles: [],
  totalCount: 0,
  pageNumber: 1,
  pageSize: 15,
  setPageNumber: (newPageNumber: number) => {
    set(state => {
      if (newPageNumber >= 1 && newPageNumber <= Math.ceil(state.totalCount / state.pageSize)) {
        return { pageNumber: newPageNumber }
      }
      console.warn("Указан несуществующий номер страницы. Номер страницы остался тем же")
      return state
    })
  },
  loadPrevPage: () => {
    set(state => {
      if (state.pageNumber > 1) {
        return { pageNumber: state.pageNumber - 1 };
      }
      return state
    })
  },
  loadNextPage: () => {
    set(state => {
      if (state.pageNumber < Math.ceil(state.totalCount / 10)) {
        return { pageNumber: state.pageNumber + 1 };
      }
      return state
    })
  },
  addArticle: async (request, setError) => {
    if (get().articles.some(article => article.articleId === request.articleId)) {
      setError("articleId", {type: "uniqueId", message: "Статья с таким ID уже существует"})
      return false
    }

    try {
      const response = await ArticleService.addArticle(request)
      set(state => ({ articles: [...state.articles, response.data] })) 
      return true
    } catch (error) {
      console.error(error)
      return false
    }
  },
  getArticles: async (pageNumber, pageSize) => {
    try {
      const response = await ArticleService.getArticles(pageNumber, pageSize)
      set({ articles: response.data, totalCount: response.headers['x-total-count'] })
    } catch (error) {
      console.error(error)
    }
  },
  getArticle: async (articleId: string) => {
    try {
      const response = await ArticleService.getArticle(articleId)
      return response.data
    } catch (error) {
      console.error(error)
    }
  },
  updateArticle: async (request: UpdateArticleRequest) => {
    try {
      await ArticleService.updateArticle(request)
    } catch (error) {
      console.error(error)
    }
  },
  deleteArticle: async (articleId: string) => {
    try {
      await ArticleService.deleteArticle(articleId)
    } catch (error) {
      console.error(error)
    }
  }
}))