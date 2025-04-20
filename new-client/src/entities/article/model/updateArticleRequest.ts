export interface UpdateArticleRequest {
  articleId: string
  title: string
  description: string
  text: string
  isPublished: boolean
  isPinned: boolean
}