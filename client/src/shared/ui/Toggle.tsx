export const Toggle = ({ title, value, onChange } : { title: string, value: boolean, onChange: (value: boolean) => void }) => {
  return(
    <div className={"flex justify-between"}>
      <h2 className={"font-medium"}>
        { title }
      </h2>
      <div onClick={() => onChange(!value)} className={`${ value ? "justify-end bg-black" : "bg-neutral-300"} flex w-12 p-1 rounded-full cursor-pointer`}>
        <div className={"size-[18px] bg-white rounded-full"}/>
      </div>
    </div>
  )
}
