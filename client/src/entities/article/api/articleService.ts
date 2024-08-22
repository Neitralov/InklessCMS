import {AxiosResponse} from "axios";
import {Article} from "../model/article.ts";
import {httpClient} from "../../../shared/api/httpClient.ts";
import {CreateArticleRequest} from "../model/createArticleRequest.ts";
import {UpdateArticleRequest} from "../model/updateArticleRequest.ts";

export default class ArticleService {
  static async getArticles(page: number, size: number) : Promise<AxiosResponse<Article[]>> {
    return await httpClient.get<Article[]>(`/articles?page=${page}&size=${size}`)
  }

  static async addArticle(request: CreateArticleRequest) : Promise<AxiosResponse<Article>> {
    return await httpClient.post<Article>('/articles', request)
  }

  static async getArticle(articleId: string) : Promise<AxiosResponse<Article>> {
    return await httpClient.get<Article>(`/articles/${articleId}`)
  }

  static async updateArticle(request : UpdateArticleRequest) : Promise<AxiosResponse> {
    return await httpClient.put('/articles', request)
  }

  static async deleteArticle(articleId: string) : Promise<AxiosResponse> {
    return await httpClient.delete(`/articles/${articleId}`)
  }
}