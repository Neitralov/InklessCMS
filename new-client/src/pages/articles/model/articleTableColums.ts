import { Article } from "@/entities/article"
import { ColumnDef } from "@tanstack/react-table"

export const articleTableColumns: ColumnDef<Article>[] = [
  {
    accessorKey: "title",
    header: "Название",
  },
  {
    accessorKey: "views",
    header: "Просмотры"
  },
  {
    accessorKey: "isPinned",
    header: "Закреплена?",
    cell: ({ row }) => row.getValue("isPinned") ? "Да" : "Нет"
  },
  {
    accessorKey: "isPublished",
    header: "Статус",
    cell: ({ row }) => row.getValue("isPublished") ? "Опубликовано" : "Черновик"
  },
  {
    accessorKey: "publishDate",
    header: "Дата публикации",
    cell: ({ row }) => {
      const publishDate : Date = row.getValue("publishDate")
      return publishDate != null ? new Date(publishDate).toLocaleDateString() : "N/A"
    }
  }
]
