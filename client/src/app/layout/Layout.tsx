import { NavLink, Outlet } from "react-router-dom"
import { NavGroup } from "./NavGroup.tsx"
import { Logo } from "../../shared/ui/Logo.tsx"
import { useAuthStore } from "../../shared/model/useAuthStore.ts"
import { Icon } from "@iconify/react";

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
                to={"/inkless"}
                className={({ isActive }) => (isActive ? "font-medium " : "") + "flex items-center gap-1.5 hover:font-medium"}>
                <Icon icon={"material-symbols:description-outline"} className={"fill-inherit text-xl"} />
                Статьи
              </NavLink>
              <NavLink
                end
                to={"/inkless/collections"}
                className={({ isActive }) => (isActive ? "font-medium " : "") + "flex items-center gap-1.5 hover:font-medium"}>
                <Icon icon={"material-symbols:collections-bookmark-outline"} className={"fill-inherit text-xl"} />
                Коллекции
              </NavLink>
            </NavGroup>
          </div>

          <div className={"flex flex-col mb-2"}>
            <div className={"flex flex-col"}>
              <p
                className={"flex items-center gap-1.5 hover:font-medium cursor-pointer"}
                onClick={ logout }>
                <Icon icon={"material-symbols:logout"} className={"fill-inherit text-xl"} />
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
