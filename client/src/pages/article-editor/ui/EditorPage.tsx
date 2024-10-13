import { MarkdownEditor } from "./MarkdownEditor.tsx"
import { Toggle } from "../../../shared/ui/Toggle.tsx"
import { useNavigate, useParams } from "react-router-dom"
import { useEffect, useState } from "react"
import { HeaderButton } from "../../../shared/ui/HeaderButton.tsx"
import { Controller, useForm } from "react-hook-form"
import { CreateArticleRequest } from "../../../entities/article/model/createArticleRequest.ts"
import { EditorModes } from "../model/editorModes.ts"
import { UpdateArticleRequest } from "../../../entities/article/model/updateArticleRequest.ts"
import { useArticleCRUD } from "../../../entities/article/model/useArticleCRUD.ts"
import { Icon } from "@iconify/react";

export const EditorPage = ({ editorMode }: { editorMode: EditorModes }) => {
  const {
    register,
    formState: { errors },
    handleSubmit,
    control,
    reset,
    getValues,
    setError
  } = useForm<CreateArticleRequest | UpdateArticleRequest>({
    defaultValues: { articleId: '', title: '', description: '', text: '', isPinned: false, isPublished: false }
  })

  const { addArticle, getArticle, updateArticle, deleteArticle } = useArticleCRUD()
  const { articleId } = useParams()
  const navigate = useNavigate()
  const [isLoaded, setIsLoaded] = useState(false)

  useEffect(() => {
    loadArticleToEditor().then(() => setIsLoaded(true))
  }, [articleId])

  const loadArticleToEditor = async () => {
    if (editorMode === EditorModes.Editing && articleId != undefined) {
      const article = await getArticle(articleId)
      reset(article)
    }
  }

  const publish = editorMode === EditorModes.Addititon
    ? handleSubmit(async (data: CreateArticleRequest) => {
      const finalData = { ...data, isPublished: true }
      const success = await addArticle(finalData, setError)
      if (success) navigate('/inkless')
    })
    : handleSubmit(async (data: UpdateArticleRequest) => {
      const finalData = { ...data, isPublished: true }
      await updateArticle(finalData)
      navigate('/inkless')
    })

  const save = editorMode === EditorModes.Addititon
    ? handleSubmit(async (data: CreateArticleRequest) => {
      const finalData = { ...data, isPublished: false }
      const success = await addArticle(finalData, setError)
      if (success) navigate('/inkless')
    })
    : handleSubmit(async (data: UpdateArticleRequest) => {
      const finalData = { ...data, isPublished: false }
      await updateArticle(finalData)
      navigate('/inkless')
    })

  const remove = handleSubmit(async (data: CreateArticleRequest) => {
    await deleteArticle(data.articleId)
    navigate('/inkless')
  })

  return(
    <>
      { isLoaded &&
        <div className={"flex flex-col w-full h-screen"}>
          <header className={"flex justify-between items-center px-5 py-5 border-b border-black/20"}>
            <h1 className={"text-2xl font-medium"}>
              {
                editorMode === EditorModes.Addititon
                  ? "Добавление новой статьи"
                  : "Редактирование статьи"
              }
            </h1>
            <div className={"flex gap-2"}>
              { editorMode === EditorModes.Editing &&
                <HeaderButton OnClick={ remove }>
                  Удалить
                  <Icon icon={"material-symbols:delete-outline"} className={"fill-inherit text-lg"} />
                </HeaderButton> }
              <HeaderButton OnClick={ save }>
                Сохранить
                <Icon icon={"material-symbols:draft-outline"} className={"fill-inherit text-lg"} />
              </HeaderButton>
              { getValues("isPublished") === false &&
                <HeaderButton OnClick={ publish }>
                  Опубликовать
                  <Icon icon={"material-symbols:check"} className={"fill-inherit text-lg"} />
                </HeaderButton> }
            </div>
          </header>

          <div className={"flex flex-col gap-3 h-[calc(100vh-117px)] mx-5 my-5"}>
            <div className={"h-full border border-black/20 overflow-y-auto"}>
              <Controller
                control={ control }
                render={ ({ field: { value, onChange } }) => <MarkdownEditor value={ value } onChange={ onChange } /> }
                name={"text"}/>
            </div>
          </div>
        </div>
      }

      { isLoaded &&
        <aside className={"flex flex-col gap-3 px-5 py-5 min-w-80 h-screen border-l border-black/20"}>
          <h1 className={"text-xl font-medium"}>Настройки статьи</h1>
            <div className={"flex flex-col gap-1"}>
              <h2 className={"font-medium"}>
                ID статьи
                { errors.articleId?.type === "required" && (<p className={"text-sm font-normal text-red-600"}>Поле обязательно</p>) }
                { errors.articleId?.type === "minLength" && (<p className={"text-sm font-normal text-red-600"}>ID не может быть короче 3 символов</p>) }
                { errors.articleId?.type === "maxLength" && (<p className={"text-sm font-normal text-red-600"}>ID не может быть длиннее 64 символов</p>) }
                { errors.articleId?.type === "pattern" && (<p className={"text-sm font-normal text-red-600"}>ID должен соответствовать шаблону <br />/^[a-zA-Z0-9-]+$/</p>) }
                { errors.articleId?.type === "uniqueId" && (<p className={"text-sm font-medium text-red-600"}>Статья с таким ID уже существует</p>) }
              </h2>
              <input
                { ...register("articleId", { required: true, minLength: 3, maxLength: 64, pattern: /^[a-zA-Z0-9-]+$/ }) }
                readOnly={ editorMode === EditorModes.Editing }
                className={"px-2 py-1 bg-neutral-200 rounded read-only:outline-none read-only:cursor-default read-only:text-neutral-500"}
                placeholder={"Введите ID"} />
            </div>
            <div className={"flex flex-col gap-1"}>
              <h2 className={"font-medium"}>
                Заголовок статьи
                { errors.title?.type === "required" && (<p className={"text-sm font-normal text-red-600"}>Поле обязательно</p>) }
                { errors.title?.type === "minLength" && (<p className={"text-sm font-normal text-red-600"}>Заголовок не может быть короче 3 символов</p>) }
                { errors.title?.type === "maxLength" && (<p className={"text-sm font-normal text-red-600"}>Заголовок не может быть длиннее 64 символов</p>) }
              </h2>
              <input
                { ...register("title", { required: true, minLength: 3, maxLength: 64 }) }
                className={"px-2 py-1 bg-neutral-200 rounded"}
                placeholder={"Введите заголовок"} />
            </div>
            <div className={"flex flex-col gap-1"}>
              <h2 className={"font-medium"}>
                Описание статьи
                { errors.description?.type === "maxLength" && (<p className={"text-sm font-normal text-red-600"}>Описание не может быть длиннее 64 символов</p>) }
              </h2>
                <textarea
                  { ...register("description", { maxLength: 64 }) }
                  className={"px-2 py-1 bg-neutral-200 rounded resize-none"} rows={2}
                  placeholder={"Введите описание"}/>
            </div>
            <Controller
              control={ control }
              render={ ({ field: { value, onChange } }) => <Toggle title={"Закрепить статью?"} value={ value } onChange={ onChange } /> }
              name={"isPinned"} />
        </aside> }
    </>
  )
}
