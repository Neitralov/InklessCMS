import { AxiosResponse } from "axios"
import { Collection } from "../model/collection.ts"
import { httpClient } from "../../../shared/api/httpClient.ts"
import { CreateCollectionRequest } from "../model/createCollectionRequest.ts"
import { UpdateCollectionRequest } from "../model/updateCollectionRequest.ts"
import { AddArticleToCollectionRequest } from "../model/addArticleToCollectionRequest.ts"

export default class CollectionService {
  static getCollections = async () : Promise<AxiosResponse<Collection[]>> =>
    await httpClient.get<Collection[]>(`api/collections`)

  static getCollection = async(collectionId: string) : Promise<AxiosResponse<Collection>> =>
    await httpClient.get<Collection>(`api/collections/${collectionId}`)

  static createCollection = async (request: CreateCollectionRequest) : Promise<AxiosResponse<Collection>> =>
    await httpClient.post<Collection>('api/collections', request)

  static updateCollection = async (request: UpdateCollectionRequest) : Promise<AxiosResponse<Collection>> =>
    await httpClient.put('api/collections', request)

  static deleteCollection = async (collectionId: string) : Promise<AxiosResponse> =>
    await httpClient.delete(`api/collections/${collectionId}`)

  static addArticleToCollection = async (collectionId: string, request: AddArticleToCollectionRequest) : Promise<AxiosResponse> =>
    await httpClient.post(`api/collections/${collectionId}`, request)

  static deleteArticleFromCollection = async (collectionId: string, articleId: string) : Promise<AxiosResponse> =>
    await httpClient.delete(`api/collections/${collectionId}/articles/${articleId}`)
}
