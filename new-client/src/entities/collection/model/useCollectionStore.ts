import { create } from 'zustand'
import { Collection } from "./collection.ts"
import CollectionService from "../api/collectionService.ts"
import { CreateCollectionRequest } from './createCollectionRequest.ts'
import { UpdateCollectionRequest } from './updateCollectionRequest.ts'
import { AxiosError } from "axios"
import { UseFormSetError } from "react-hook-form"
import { Article } from '../../article/model/article.ts'
import { AddArticleToCollectionRequest } from './addArticleToCollectionRequest.ts'

interface UseCollectionStore {
  collections: Collection[]
  collection: Collection | undefined
  getCollections: () => Promise<void>
  getCollection: (collectionId: string) => Promise<Collection | undefined>
  createCollection: (request: CreateCollectionRequest, setError: UseFormSetError<CreateCollectionRequest>) => Promise<boolean>
  updateCollection: (request: UpdateCollectionRequest) => Promise<void>
  deleteCollection: (collectionId: string) => Promise<void>
  addArticleToCollection: (collectionId: string, article: Article) => Promise<void>
  deleteArticleFromCollection: (collectionId: string, articleId: string) => Promise<void>
}

export const useCollectionStore = create<UseCollectionStore>(set => ({
  collections: [],
  collection: undefined,
  getCollections: async () => {
    try {
      const response = await CollectionService.getCollections()
      set({ collections: response.data })
    } catch (error) {
      console.error(error)
    }
  },
  getCollection: async (collectionId) => {
    try {
      const response = await CollectionService.getCollection(collectionId)
      set({ collection: response.data })
      return response.data
    } catch (error) {
      console.error(error)
    }
  },
  createCollection: async (request, setError) => {
    try {
      await CollectionService.createCollection(request)
      return true
    } catch (error) {
      console.error(error)
      if (Object.keys((((error as AxiosError).response!.data) as ProblemDetails).errors)[0] === "Collection.NonUniqueId") {
        setError("collectionId", { type: "uniqueId", message: "Коллекция с таким ID уже существует" })
      }
      return false
    }
  },
  updateCollection: async (request) => {
    try {
      await CollectionService.updateCollection(request)
    } catch (error) {
      console.error(error)
    }
  },
  deleteCollection: async (collectionId) => {
    try {
      await CollectionService.deleteCollection(collectionId)
    } catch (error) {
      console.error(error)
    }
  },
  addArticleToCollection: async (collectionId, article) => {
    try {
      const request: AddArticleToCollectionRequest = { articleId: article.articleId }
      await CollectionService.addArticleToCollection(collectionId, request)
      set(state => ({ collection: { ...state.collection!, articles: [...state.collection!.articles, article ] } }))
    } catch (error) {
      console.error(error)
    }
  },
  deleteArticleFromCollection: async (collectionId, articleId) => {
    try {
      await CollectionService.deleteArticleFromCollection(collectionId, articleId)
      set(state => ({ collection: { ...state.collection!, articles: state.collection!.articles.filter(article => article.articleId != articleId) }}))
    } catch (error) {
      console.error(error)
    }
  }
}))

interface ProblemDetails {
  errors: { value: string }
}
