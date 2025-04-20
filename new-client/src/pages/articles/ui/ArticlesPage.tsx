import { useArticleStore } from "@/entities/article"
import { useArticleEditorStore } from "../model/useArticleEditorStore"
import { ArticleTable } from "./ArticleTable"
import { articleTableColumns } from "../model/articleTableColums"
import { useEffect } from "react"
import { FilePlus2 } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Modal } from "@/widgets/modal"
import { ArticleEditor } from "./ArticleEditor"

export const ArticlesPage = () => {
  const articles = useArticleStore(state => state.articles)
  const getArticles = useArticleStore(state => state.getArticles)

  const pageSize = useArticleStore((state) => state.pageSize)
  const pageNumber = useArticleStore(state => state.pageNumber)

  const isArticleEditorOpen = useArticleEditorStore(state => state.isEditorOpen)
  const openArticleEditor = useArticleEditorStore(state => state.openEditor)

  useEffect(() => {
    getArticles(pageNumber, pageSize).then()
  }, [pageNumber, pageSize, getArticles, articles])

  return(
    <main className="flex flex-col w-full gap-3">
      <div className="flex justify-between items-center">
        <h1 className="text-xl">Статьи</h1>
        <Button variant={"outline"} onClick={openArticleEditor}>
          <FilePlus2 />
          Добавить статью
        </Button>
      </div>

      <ArticleTable columns={articleTableColumns} data={articles} />

      <Modal isOpen={isArticleEditorOpen}>
        <ArticleEditor />
      </Modal>
    </main>
  )
}
