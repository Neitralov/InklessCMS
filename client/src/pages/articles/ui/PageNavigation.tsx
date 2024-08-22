import {useArticleStore} from "../../../entities/article/model/useArticleStore.ts";

export const PageNavigation = () => {
  const loadPrevPage = useArticleStore(state => state.loadPrevPage)
  const loadNextPage = useArticleStore(state => state.loadNextPage)
  const pageNumber = useArticleStore(state => state.pageNumber)
  const pageSize = useArticleStore(state => state.pageSize)
  const totalCount = useArticleStore(state => state.totalCount)
  const setPageNumber = useArticleStore(state => state.setPageNumber)
  const maxPage = Math.ceil(totalCount / pageSize)

  const pagesArray = []

  for (let i = 0; i < maxPage; i++) {
    pagesArray[i] = i + 1
  }

  return (
    <div className={"flex gap-1"}>
      <p onClick={loadPrevPage}
         className={"px-2 py-1 text-neutral-500 bg-neutral-200 hover:bg-neutral-300 rounded-l cursor-pointer aria-disabled:cursor-not-allowed"}
         aria-disabled={pageNumber == 1}>Назад</p>
      {
        pagesArray.map(page =>
          <div key={page} onClick={() => setPageNumber(page)}
               className={`${page == pageNumber ? "bg-neutral-300 text-black" : "bg-neutral-200 text-neutral-500"} px-2 py-1 hover:bg-neutral-300 cursor-pointer`}>
            {page}
          </div>
        )
      }
      <p onClick={loadNextPage}
         className={"px-2 py-1 text-neutral-500 bg-neutral-200 hover:bg-neutral-300 rounded-r cursor-pointer aria-disabled:cursor-not-allowed"}
         aria-disabled={pageNumber == maxPage}>Вперед</p>
    </div>
  )
}