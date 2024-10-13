import { BrowserRouter, Route, Routes } from "react-router-dom"
import {
  NotFound,
  ArticlesPage,
  ArticleEditorPage,
  ArticleEditorModes,
  LoginPage,
  CollectionsPage,
  CollectionEditorPage,
  CollectionEditorModes,
} from "../../pages"
import { Layout } from "../layout/Layout.tsx"
import { PrivateRoutes } from "./PrivateRoutes.tsx"

export const Routing = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path={"*"} element={<NotFound />} />
        <Route path={"inkless/login"} element={<LoginPage />} />
        <Route element={<PrivateRoutes />}>
          <Route path={"inkless"} element={<Layout />}>
            <Route index element={<ArticlesPage />} />
            <Route path={"articles/editor/edit/:articleId"} element={<ArticleEditorPage editorMode={ArticleEditorModes.Editing} />} />
            <Route path={"articles/editor/new"} element={<ArticleEditorPage editorMode={ArticleEditorModes.Addititon} />} />
            <Route path={"collections"} element={<CollectionsPage />} />
            <Route path={"collections/editor/edit/:collectionId"} element={<CollectionEditorPage editorMode={CollectionEditorModes.Editing} />} />
            <Route path={"collections/editor/new"} element={<CollectionEditorPage editorMode={CollectionEditorModes.Addititon} />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  )
}
