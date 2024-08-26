import { useEffect } from "react"
import { useArticleStore } from "../../../entities/article/model/useArticleStore.ts"
import { ArticleItem } from "./ArticleItem.tsx"

export const ArticlesList = () => {
  const articles = useArticleStore(state => state.articles)
  const getArticles = useArticleStore(state => state.getArticles)
  const pageNumber = useArticleStore(state => state.pageNumber)
  const pageSize = useArticleStore(state => state.pageSize)

  useEffect(() => {
    getArticles(pageNumber, pageSize).then()
  }, [pageNumber, pageSize])

  return (
    <table>
      <thead>
      <tr className={"flex pb-2 text-lg text-neutral-500 border-b border-black/20"}>
        <th className={"basis-1/2 p-0 text-left font-medium"}>Название статьи</th>
        <th className={"basis-1/4 p-0 text-right font-medium"}>Статус</th>
        <th className={"basis-1/4 p-0 text-right font-medium"}>Дата публикации</th>
      </tr>
      </thead>
      <tbody>
      { articles.map(article => <ArticleItem key={ article.articleId } article={ article } />) }
      </tbody>
    </table>
  );
};
