import { useNavigate } from "react-router-dom"
import { useEffect } from "react"
import { useCollectionStore } from "../../../entities/collection/model/useCollectionStore"

export const CollectionsList = () => {
  const collections = useCollectionStore(state => state.collections)
  const getCollections = useCollectionStore(state => state.getCollections)
  const navigate = useNavigate()

  useEffect(() => {
    getCollections().then()
  }, [])

  return (
    collections.map(collection =>
      <div
        key={ collection.collectionId }
        className={"px-3 py-2 text-lg font-medium border border-black/20 rounded-md"}>
        <h2
          className={"w-fit hover:underline decoration-2 underline-offset-2 cursor-pointer"}
          onClick={() => navigate(`/inkless/collections/editor/edit/${collection.collectionId}`)}>
          { collection.title }
        </h2>
      </div>)
  )
}
