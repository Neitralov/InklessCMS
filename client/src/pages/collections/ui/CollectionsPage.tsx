import { useNavigate } from "react-router-dom"
import { HeaderButton } from "../../../shared/ui/HeaderButton.tsx"
import { CollectionsList } from "./CollectionsList.tsx"
import { Icon } from "@iconify/react";

export const CollectionsPage = () => {
  const navigate = useNavigate()

  return (
    <div className={"flex flex-col w-full h-screen"}>
      <header className={"flex justify-between items-center px-5 py-5 border-b border-black/20"}>
        <h1 className={"text-2xl font-medium"}>Коллекции</h1>
        <HeaderButton
          OnClick={() => { navigate("/inkless/collections/editor/new") }}>
          Новая коллекция
          <Icon icon={"material-symbols:add"} className={"fill-inherit text-lg"} />
        </HeaderButton>
      </header>

      <div className={"flex flex-col gap-3 h-screen px-5 py-5"}>
        <CollectionsList />
      </div>
    </div>
  );
};
