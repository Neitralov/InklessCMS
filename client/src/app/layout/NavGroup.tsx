import { FC, ReactNode } from "react"

interface Props {
  title: string
  children: ReactNode
}

export const NavGroup: FC<Props> = ({ title, children }) => {
  return (
    <div className={"flex flex-col gap-2"}>
      <h2 className={"text-sm text-neutral-500"}>
        { title }
      </h2>
      { children }
    </div>
  )
}
