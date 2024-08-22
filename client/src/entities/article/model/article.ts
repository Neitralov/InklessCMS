export interface Article {
  articleId: string
  title: string
  description: string
  isPublished: boolean
  publishDate: Date | null
  views: number
  isPinned: boolean
}