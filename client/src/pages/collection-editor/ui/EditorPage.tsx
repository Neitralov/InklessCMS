import { useNavigate, useParams } from "react-router-dom"
import { useEffect, useState } from "react"
import { HeaderButton } from "../../../shared/ui/HeaderButton.tsx"
import { useForm } from "react-hook-form"
import { EditorModes } from "../model/editorModes.ts"
import { ApplyIcon, FileIcon, TrashBinIcon } from "../../../shared/ui/Icons.tsx"
import { CreateCollectionRequest } from "../../../entities/collection/model/createCollectionRequest.ts"
import { UpdateCollectionRequest } from "../../../entities/collection/model/updateCollectionRequest.ts"
import { useCollectionStore } from "../../../entities/collection/model/useCollectionStore.ts"
import { Select } from "./Select.tsx"
import { useArticleStore } from "../../../entities/article/model/useArticleStore.ts"

export const EditorPage = ({ editorMode }: { editorMode: EditorModes }) => {
  const {
    register,
    formState: { errors },
    handleSubmit,
    reset,
    setError,
  } = useForm<CreateCollectionRequest | UpdateCollectionRequest>({
    defaultValues: {
      collectionId: "",
      title: ""
    }
  })

  const articles = useArticleStore(state => state.articles)
  const getArticles = useArticleStore(state => state.getArticles)
  const collection = useCollectionStore(state => state.collection)
  const createCollection = useCollectionStore(state => state.createCollection)
  const updateCollection = useCollectionStore(state => state.updateCollection)
  const getCollection = useCollectionStore(state => state.getCollection)
  const deleteCollection = useCollectionStore(state => state.deleteCollection)
  const addArticleToCollection = useCollectionStore(state => state.addArticleToCollection)
  const deleteArticleFromCollection = useCollectionStore(state => state.deleteArticleFromCollection)
  const { collectionId } = useParams()
  const navigate = useNavigate()
  const [isLoaded, setIsLoaded] = useState(false)

  useEffect(() => {
    loadCollectionToEditor().then(() => getArticles(1, 100).then(() => setIsLoaded(true)))
  }, [collectionId]);

  const loadCollectionToEditor = async () => {
    if (editorMode === EditorModes.Editing && collectionId != undefined) {
      reset(await getCollection(collectionId))
    }
  };

  const save =
    editorMode === EditorModes.Addititon
      ? handleSubmit(async (data: CreateCollectionRequest) => {
          const success = await createCollection(data, setError)
          if (success) navigate("/inkless/collections")
        })
      : handleSubmit(async (data: UpdateCollectionRequest) => {
          await updateCollection(data)
          navigate("/inkless/collections")
        });

  const remove = handleSubmit(async (data: CreateCollectionRequest) => {
    await deleteCollection(data.collectionId);
    navigate("/inkless/collections");
  });

  return (
    <>
      { isLoaded && (
        <div className={"flex flex-col w-full h-screen"}>
          <header className={"flex justify-between items-center px-5 py-5 border-b border-black/20"}>
            <h1 className={"text-2xl font-medium"}>
              { editorMode === EditorModes.Addititon
                ? "Добавление новой коллекции"
                : `Редактирование коллекции "${ collection?.title }"` }
            </h1>
            <div className={"flex gap-2"}>
              { editorMode === EditorModes.Editing && (
                <HeaderButton OnClick={ remove }>
                  Удалить <TrashBinIcon width="18px" height="18px" />
                </HeaderButton>) }
              <HeaderButton OnClick={ save }>
                Сохранить <ApplyIcon />
              </HeaderButton>
            </div>
          </header>

          <div className={"flex flex-col gap-3 h-[calc(100vh-117px)] mx-5 my-5"}>
          { editorMode === EditorModes.Addititon
            ? <h2 className={"text-lg font-medium"}>Сначала создайте коллекцию, а после в нее можно будет добавлять статьи!</h2>
            : <>
                <Select
                  header={"Добавить статью"}
                  onSelect={(article) => addArticleToCollection(collectionId!, article)}
                  data={ articles.filter(article => !collection!.articles!.map(x => x.articleId).includes(article.articleId)) }
                />

                { collection!.articles!.length > 0 &&
                  <div className={"flex flex-col px-3.5 py-1 border border-black/20 rounded-md overflow-y-auto"}>
                    { collection!.articles!.map(article =>
                      <div key={ article.articleId } className={"flex justify-between py-2 items-center border-b last:border-none border-black/20"}>
                        <div className={"flex gap-2 items-center"}>
                          <FileIcon width="24px" height="24px" />
                          <h2 className={"text-lg font-medium"}>{ article.title }</h2>
                        </div>
                        <div
                          onClick={async () => await deleteArticleFromCollection(collectionId!, article.articleId)}
                          className={"fill-neutral-500 hover:fill-black cursor-pointer"}>
                          <TrashBinIcon width="24px" height="24px" />
                        </div>
                      </div>) }
                  </div> }
              </> }
          </div>
        </div>) }

      { isLoaded && (
        <aside className={"flex flex-col gap-3 px-5 py-5 min-w-80 h-screen border-l border-black/20"}>
          <h1 className={"text-xl font-medium"}>Настройки коллекции</h1>
          <div className={"flex flex-col gap-1"}>
            <h2 className={"font-medium"}>
              ID коллекции
              { errors.collectionId?.type === "required" && (<p className={"text-sm font-normal text-red-600"}>Поле обязательно</p>) }
              { errors.collectionId?.type === "minLength" && (<p className={"text-sm font-normal text-red-600"}>ID не может быть короче 3 символов</p>) }
              { errors.collectionId?.type === "maxLength" && (<p className={"text-sm font-normal text-red-600"}>ID не может быть длиннее 32 символов</p>) }
              { errors.collectionId?.type === "pattern" && (<p className={"text-sm font-normal text-red-600"}>ID должен соответствовать шаблону <br />/^[a-zA-Z0-9-]+$/</p>) }
              { errors.collectionId?.type === "uniqueId" && (<p className={"text-sm font-medium text-red-600"}>{ errors.collectionId?.message }</p>) }
            </h2>
            <input
              { ...register("collectionId", { required: true, minLength: 3, maxLength: 32, pattern: /^[a-zA-Z0-9-]+$/ }) }
              readOnly={ editorMode === EditorModes.Editing }
              className={"px-2 py-1 bg-neutral-200 rounded read-only:outline-none read-only:cursor-default read-only:text-neutral-500"}
              placeholder={"Введите ID"}/>
          </div>
          <div className={"flex flex-col gap-1"}>
            <h2 className={"font-medium"}>
              Название коллекции
              { errors.title?.type === "required" && (<p className={"text-sm font-normal text-red-600"}>Поле обязательно</p>) }
              { errors.title?.type === "minLength" && (<p className={"text-sm font-normal text-red-600"}>Название не может быть короче 3 символов</p>) }
              { errors.title?.type === "maxLength" && (<p className={"text-sm font-normal text-red-600"}>Название не может быть длиннее 32 символов</p>) }
            </h2>
            <input
              { ...register("title", { required: true, minLength: 3, maxLength: 32 }) }
              className={"px-2 py-1 bg-neutral-200 rounded"}
              placeholder={"Введите название"}/>
          </div>
        </aside>) }
    </>
  )
}
