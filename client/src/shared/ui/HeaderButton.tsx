import { ReactNode } from "react"

export const HeaderButton = ({ children, OnClick }: { children: ReactNode, OnClick: () => void }) => {
  return (
    <button onClick={ OnClick } className={"flex items-center gap-1 px-3.5 py-1.5 text-base font-medium outline outline-1 outline-black hover:text-white hover:bg-black rounded-md hover:fill-white"}>
      { children }
    </button>
  )
}
