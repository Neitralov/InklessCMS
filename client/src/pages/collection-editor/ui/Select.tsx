import { useState } from "react"
import { Article } from "../../../entities/article/model/article"
import { Icon } from "@iconify/react";

export const Select = ({ header, onSelect, data }: { header: string, onSelect: (selectedArticle: Article) => void, data: Article[] }) => {
  const [isOpen, setIsOpen] = useState(false)

  const selectItem = (selectedArticle: Article) => {
    onSelect(selectedArticle)
    setIsOpen(false)
  }

  return (
    <div className={"relative"}>
      <button
        className={`flex items-center justify-between w-full px-3.5 py-1.5 hover:text-white hover:bg-black hover:fill-white font-medium border border-black rounded-md select-none`}
        type={"button"}
        onClick={() => setIsOpen(!isOpen)}
        onBlur={() => setIsOpen(false)}>
        { header }
        <Icon icon={"material-symbols:arrow-drop-down"} className={"fill-inherit text-2xl"} />
      </button>

      { data.length > 0
        ? <div
            className={`absolute ${isOpen ? "" : "hidden"} w-full max-h-48 mt-2 px-2 py-1.5 bg-white border border-black overflow-x-hidden overflow-y-auto rounded-md z-10`}>
            { data.map(item =>
              <p
                key={item.articleId}
                className={"min-w-max hover:bg-neutral-100 px-2 py-0.5 select-none cursor-pointer"}
                onMouseDown={() => selectItem(item)}>{ item.title }
              </p>) }
          </div>
        : <div
            className={`absolute ${isOpen ? "" : "hidden"} w-full max-h-48 mt-2 px-2 py-1.5 bg-white border border-black overflow-x-hidden overflow-y-auto rounded-md z-10`}>
            <p className={"px-2 py-0.5 select-none"}>Нет статей, которые можно добавить</p>
          </div> }
    </div>
  )
}
