import {useNavigate} from "react-router-dom";
import {EditIcon} from "../../../shared/ui/Icons.tsx";
import {ArticlesList} from "./ArticlesList.tsx";
import {PageNavigation} from "./PageNavigation.tsx";
import {useArticleStore} from "../../../entities/article/model/useArticleStore.ts";
import {HeaderButton} from "../../../shared/ui/HeaderButton.tsx";

export const ArticlesPage = () => {
  const router = useNavigate()
  const totalCount = useArticleStore(state => state.totalCount)
  const pageSize = useArticleStore(state => state.pageSize)
  const maxPage = Math.ceil(totalCount / pageSize)

  return (
    <div className={"flex flex-col w-full h-screen"}>
      <header className={"flex justify-between items-center px-5 py-5 border-b border-black/20"}>
        <h1 className={"text-2xl font-medium"}>
          Статьи
        </h1>
        <HeaderButton OnClick={() => {router("/editor/new")}}>Новая статья <EditIcon /></HeaderButton>
      </header>

      <div className={"flex flex-col gap-3 justify-between h-screen px-5 py-5"}>
        <ArticlesList />
        { maxPage > 1 && <PageNavigation /> }
      </div>
    </div>
  )
}