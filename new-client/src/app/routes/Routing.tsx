import { BrowserRouter, Route, Routes } from "react-router";
import { LoginPage } from "@/pages/login";
import { ArticlesPage } from "@/pages/articles";
import { CollectionsPage } from "@/pages/collections";
import { NotFoundPage } from "@/pages/404";
import { Layout } from "../layout/Layout.tsx";
import { PrivateRoutes } from "./PrivateRoutes.tsx";

export const Routing = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path={"*"} element={<NotFoundPage />} />
        <Route path={"inkless/login"} element={<LoginPage />} />
        <Route element={<PrivateRoutes />}>
          <Route path={"inkless"} element={<Layout />}>
            <Route index element={<ArticlesPage />} />
            <Route path={"collections"} element={<CollectionsPage />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  )
}
