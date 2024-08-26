import { NavLink, Outlet } from "react-router-dom"
import {
  ApiIcon,
  CollectionsIcon,
  ExitIcon,
  FileIcon,
} from "../../shared/ui/Icons.tsx"
import { NavGroup } from "./NavGroup.tsx"
import { Logo } from "../../shared/ui/Logo.tsx"
import { useAuthStore } from "../../shared/model/useAuthStore.ts"

export const Layout = () => {
  const logout = useAuthStore((state) => state.logout)

  return (
    <div className={"flex min-h-screen"}>
      <aside className={"flex flex-col min-w-60 h-screen border-r border-black/20"}>
        <div className={"flex justify-center py-5 border-b border-black/20"}>
          <Logo />
        </div>

        <nav className={"flex flex-col justify-between h-full px-7 py-5"}>
          <div className={"flex flex-col gap-5"}>
            <NavGroup title={"Контент"}>
              <NavLink
                to={""}
                className={({ isActive }) => (isActive ? "font-medium " : "") + "flex items-center gap-1.5 hover:font-medium"}>
                <FileIcon width="20px" height="20px" />
                Статьи
              </NavLink>
              <NavLink
                end
                to={"/collections"}
                className={({ isActive }) => (isActive ? "font-medium " : "") + "flex items-center gap-1.5 hover:font-medium"}>
                <CollectionsIcon />
                Коллекции
              </NavLink>
            </NavGroup>
            <NavGroup title={"API"}>
              <a
                href={"http://localhost:8080/swagger"}
                target={"_blank"}
                className={"flex items-center gap-1.5 hover:font-medium"}>
                <ApiIcon />
                REST API
              </a>
            </NavGroup>
          </div>

          <div className={"flex flex-col mb-2"}>
            <div className={"flex flex-col"}>
              <p
                className={"flex items-center gap-1.5 hover:font-medium cursor-pointer"}
                onClick={ logout }>
                <ExitIcon />
                Выход
              </p>
            </div>
          </div>
        </nav>
      </aside>
      <Outlet />
    </div>
  )
}
