import {ChangeEventHandler} from "react";

export const MarkdownEditor = ({ value, onChange } : { value: string, onChange: ChangeEventHandler }) => {
  return (
    <div className={"flex"}>
      <div className={"w-8 leading-5 pr-2 border-r border-black/20"}>
        {
          value.split('\n').map((_, index) => <p className={"min-w-4 text-right font-mono"} key={index}>{index + 1}</p>)
        }
      </div>
      <textarea
        className={"pl-2 w-full font-mono leading-5 outline-none overflow-y-hidden resize-none"}
        rows={1}
        value={value}
        onChange={onChange}
        placeholder={"Введите текст"}/>
    </div>
  )
}