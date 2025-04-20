import { ChangeEventHandler } from "react"

export const MarkdownEditor = ({ value, onChange } : { value: string, onChange: ChangeEventHandler }) => {
  return (
    <div className={"flex w-full min-h-96 border"}>
      <div className={"w-8 leading-5 pr-2 text-sm border-r border-black/20"}>
        { value.split('\n').map((_, index) => <p key={ index } className={"min-w-4 text-right font-mono"}>{ index + 1 }</p>) }
      </div>
      <textarea
        className={"pl-2 w-full text-sm font-mono leading-5 outline-none overflow-y-hidden resize-none"}
        rows={1}
        value={ value }
        onChange={ onChange }
        placeholder={"Введите текст"} />
    </div>
  )
}
