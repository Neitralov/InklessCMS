import { Outlet, NavLink } from "react-router";
import { BookOpenText, Bot, Braces, LogOut, Newspaper, Rss, SquareLibrary } from "lucide-react";
import { CornerDecorator, useAuthStore } from "@/shared";
import { Separator } from "@/components/ui/separator";

export const Layout = () => {
  const logout = useAuthStore((state) => state.logout)

  return (
    <div className="shaded">
      <div className="flex flex-col gap-5 m-auto w-7xl min-h-svh border-x border-black/10 bg-white">
        <nav className="px-12 py-4 border-b">
          <div className="flex justify-between">
            <div className="flex gap-3 items-center">
              <NavLink
                to={"/inkless"} end
                title="Статьи"
                className={({isActive}) => isActive ? "relative p-2.5 bg-black/5" : "relative p-2.5 hover:bg-black/5" }>
                <CornerDecorator />
                <Newspaper />
              </NavLink>
              <NavLink
                to={"collections"}
                title="Коллекции"
                className={({isActive}) => isActive ? "relative p-2.5 bg-black/5" : "relative p-2.5 hover:bg-black/5" }>
                <CornerDecorator />
                <SquareLibrary />
              </NavLink>

              <Separator className="mx-3 bg-black/50" orientation="vertical"/>

              <NavLink
                to={"rss"}
                title="RSS"
                className={({isActive}) => isActive ? "relative p-2.5 bg-black/5" : "relative p-2.5 hover:bg-black/5" }>
                <CornerDecorator />
                <Rss />
              </NavLink>
              <NavLink
                to={"telegrambot"}
                title="TelegramBot"
                className={({isActive}) => isActive ? "relative p-2.5 bg-black/5" : "relative p-2.5 hover:bg-black/5" }>
                <CornerDecorator />
                <Bot />
              </NavLink>

              <Separator className="mx-3 bg-black/50" orientation="vertical"/>

              <a
                href="https://github.com/Neitralov/InklessCMS/wiki"
                title="Wiki"
                target="_blank"
                className="relative p-2.5 hover:bg-black/5">
                <CornerDecorator />
                <BookOpenText />
              </a>
              <a
                href="http://localhost:8080/swagger"
                title="SwaggerUI"
                target="_blank"
                className="relative p-2.5 hover:bg-black/5">
                <CornerDecorator />
                <Braces />
              </a>
            </div>

            <div title="Выход" className="relative p-2.5 hover:bg-black/5 cursor-pointer" onClick={logout}>
              <CornerDecorator />
              <LogOut />
            </div>
          </div>
        </nav>
        <div className="px-12">
          <Outlet />
        </div>
      </div>
    </div>
  )
}
