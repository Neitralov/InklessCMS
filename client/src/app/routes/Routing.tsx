import {BrowserRouter, Route, Routes} from "react-router-dom";
import {ArticlesPage, EditorPage, LoginPage} from "../../pages";
import {Layout} from "../layout/Layout.tsx";
import {PrivateRoutes} from "./PrivateRoutes.tsx";
import {EditorModes} from "../../pages/article-editor/model/editorModes.ts";

export const Routing = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path={"login"} element={<LoginPage />}/>
        <Route element={<PrivateRoutes />}>
          <Route path={"/"} element={<Layout />}>
            <Route index element={<ArticlesPage />}/>
            <Route path={"editor/edit/:articleId"} element={<EditorPage editorMode={EditorModes.Editing}/>}/>
            <Route path={"editor/new"} element={<EditorPage editorMode={EditorModes.Addititon}/>}/>
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  )
}