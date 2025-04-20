import { Article } from '@/entities/article'
import { create } from 'zustand'

interface UseArticleEditorStore {
  article?: Article,
  isEditMode: boolean,
  isEditorOpen: boolean,
  openEditor: () => void,
  openEditorWithEditMode: (article: Article) => void,
  closeEditor: () => void
}

export const useArticleEditorStore = create<UseArticleEditorStore>((set) => ({
  isEditMode: false,
  isEditorOpen: false,
  openEditor: () => { set(() => ({ isEditorOpen: true, isEditMode: false }) ) },
  openEditorWithEditMode: (article: Article) => { set(() => ({ isEditorOpen: true, article: article, isEditMode: true }) ) },
  closeEditor: () => { set(() => ({ isEditorOpen: false }) ) }
}))
