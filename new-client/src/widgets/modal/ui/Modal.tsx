import React, {FC} from "react";
import { createPortal } from 'react-dom';

interface Props {
  isOpen: boolean
  children: React.ReactNode
}

export const Modal: FC<Props> = ({isOpen, children}) => {
  return createPortal(
    <div className={`fixed ${isOpen ? "" : "hidden"} w-full bg-black/25 z-50`}>
      <div className={"grid grid-cols-12 gap-4 m-auto w-7xl px-12 min-h-svh content-center"}>
        <div className={"col-start-2 col-span-10 bg-white p-4"}>
          { children }
        </div>
      </div>
    </div>,
    document.getElementById("modal") as Element
  )
}
