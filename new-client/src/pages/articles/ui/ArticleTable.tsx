import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useReactTable,
} from "@tanstack/react-table"

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

import { Button } from "@/components/ui/button"
import { Article, useArticleStore } from "@/entities/article"
import { useArticleEditorStore } from "../model/useArticleEditorStore"

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
}

export function ArticleTable<TData, TValue>({
  columns,
  data,
}: DataTableProps<TData, TValue>) {
  const table = useReactTable({
    data,
    columns,
    initialState: { pagination: { pageSize: 15 } },
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel()
  })

  const loadNextPage = useArticleStore((state) => state.loadNextPage)
  const loadPrevPage = useArticleStore((state) => state.loadPrevPage)

  const totalCount = useArticleStore((state) => state.totalCount)
  const pageSize = useArticleStore((state) => state.pageSize)
  const pageNumber = useArticleStore(state => state.pageNumber)
  const maxPage = Math.ceil(totalCount / pageSize)

  const openArticleEditorWithEditMode = useArticleEditorStore(state => state.openEditorWithEditMode)

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => {
                return (
                  <TableHead key={header.id}>
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                  </TableHead>
                )
              })}
            </TableRow>
          ))}
        </TableHeader>
        <TableBody>
          {table.getRowModel().rows?.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow
                key={row.id}
                data-state={row.getIsSelected() && "selected"}
                onClick={() => openArticleEditorWithEditMode(row.original as Article)}
              >
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center">
                No results.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
      <div className="flex items-center justify-end space-x-2 py-2 pr-2 border-t">
        <Button variant="outline" size="sm" onClick={() => loadPrevPage()} disabled={pageNumber <= 1}>
          {"<"}
        </Button>
        <p className="text-sm">Страница: {pageNumber} из {maxPage}</p>
        <Button variant="outline" size="sm" onClick={() => loadNextPage()} disabled={pageNumber >= maxPage}>
          {">"}
        </Button>
      </div>
    </div>
  )
}
