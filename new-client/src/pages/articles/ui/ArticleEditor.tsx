import { Button } from "@/components/ui/button"
import { useArticleEditorStore } from "../model/useArticleEditorStore"
import { Undo2, Save } from "lucide-react"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { useArticleForm } from "../model/useArticleForm";
import { Input } from "@/components/ui/input";
import { Checkbox } from "@/components/ui/checkbox";
import { MarkdownEditor } from "./MarkdownEditor";
import { useEffect } from "react";

export const ArticleEditor = () => {
  const { form, onSubmit, clearForm } = useArticleForm()
  const isEditMode = useArticleEditorStore(state => state.isEditMode)
  const article = useArticleEditorStore(state => state.article)
  const closeArticleEditor = useArticleEditorStore(state => state.closeEditor)

  useEffect(() => {
    if (isEditMode)
      form.reset(article)
  }, [form, isEditMode, article])

  return (
    <div>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} >
          <div className="flex gap-4 mb-4">
            <aside className="basis-1/3 space-y-4">
              <FormField
                control={form.control}
                name="articleId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>ID статьи</FormLabel>
                    <FormControl>
                      <Input placeholder="Введите ID..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="title"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Название статьи</FormLabel>
                    <FormControl>
                      <Input placeholder="Введите название..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="description"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Описание статьи</FormLabel>
                    <FormControl>
                      <Input placeholder="Введите описание..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="isPinned"
                render={({ field }) => (
                  <FormItem className="flex items-center">
                    <FormControl>
                      <Checkbox checked={field.value} onCheckedChange={field.onChange} />
                    </FormControl>
                    <FormLabel>Закрепить статью?</FormLabel>
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="isPublished"
                render={({ field }) => (
                  <FormItem className="flex items-center">
                    <FormControl>
                      <Checkbox checked={field.value} onCheckedChange={field.onChange} />
                    </FormControl>
                    <FormLabel>Опубликовать статью?</FormLabel>
                  </FormItem>
                )}
              />
            </aside>
            <FormField
              control={form.control}
              name="text"
              render={({ field }) => (
                <FormItem className="basis-2/3">
                  <FormControl>
                    <MarkdownEditor value={field.value} onChange={field.onChange} />
                  </FormControl>
                </FormItem>
              )}
            />
          </div>
          <div className="flex justify-between">
            <Button type="button" variant={"outline"} onClick={() => {
              closeArticleEditor()
              clearForm()
            }}>
              <Undo2 />
              Закрыть
            </Button>
            <Button type="submit">
              <Save />
              Сохранить
            </Button>
          </div>
        </form>
      </Form>
    </div>
  )
}
