import { AxiosResponse } from "axios"
import { Article } from "../model/article.ts"
import { httpClient } from "../../../shared/api/httpClient.ts"
import { CreateArticleRequest } from "../model/createArticleRequest.ts"
import { UpdateArticleRequest } from "../model/updateArticleRequest.ts"

export default class ArticleService {
  static getArticles = async (page: number, size: number) : Promise<AxiosResponse<Article[]>> =>
    await httpClient.get<Article[]>(`/articles?page=${page}&size=${size}`)

  static addArticle = async (request: CreateArticleRequest) : Promise<AxiosResponse<Article>> =>
    await httpClient.post<Article>('/articles', request)

  static getArticle = async (articleId: string) : Promise<AxiosResponse<Article>> =>
    await httpClient.get<Article>(`/articles/${articleId}`)

  static updateArticle = async (request : UpdateArticleRequest) : Promise<AxiosResponse> =>
    await httpClient.put('/articles', request)

  static deleteArticle = async (articleId: string) : Promise<AxiosResponse> =>
    await httpClient.delete(`/articles/${articleId}`)
}
