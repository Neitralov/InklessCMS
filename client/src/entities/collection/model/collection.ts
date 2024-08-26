import { Article } from "../../article/model/article"

export interface Collection {
  collectionId: string,
  title: string,
  articles: Article[]
}
