import { FC } from "react"
import { Article } from "../../../entities/article/model/article.ts"
import { PinIcon, ViewsIcon } from "../../../shared/ui/Icons.tsx"
import { useNavigate } from "react-router-dom"

interface Props {
  article: Article
}

export const ArticleItem: FC<Props> = ({ article }) => {
  const navigate = useNavigate()

  return (
    <tr
      key={ article.articleId }
      className={"flex py-2 text-lg border-b last:border-none border-black/20"}>
      <td className={"flex gap-3 basis-1/2 p-0 text-left font-medium"}>
        <h2
          onClick={() => navigate(`articles/editor/edit/${article.articleId}`)}
          className={"hover:underline decoration-2 underline-offset-2 cursor-pointer"}>
          { article.title }
        </h2>
        { article.isPublished &&
          <div className={"flex gap-0.5 items-center w-fit h-7 px-2 py-1.5 text-sm border border-black rounded-full"}>
            <ViewsIcon />
            { article.views }
          </div> }
        { article.isPinned &&
          <div className={"flex gap-0.5 items-center w-fit h-7 px-2 py-1.5 text-sm border border-black rounded-full"}>
            <PinIcon />
            Закреплено
          </div> }
      </td>
      <td className={"basis-1/4 p-0 text-right font-medium"}>
        { article.publishDate == null ? "Черновик" : "Опубликовано" }
      </td>
      <td className={"basis-1/4 p-0 text-right font-medium"}>
        { article.publishDate != null
          ? new Date(article["publishDate"]).toLocaleDateString()
          : "N/A" }
      </td>
    </tr>
  )
}
