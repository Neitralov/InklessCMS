import { createRoot } from 'react-dom/client'
import './assets/index.css'
import {Routing} from "./routes/Routing.tsx";

const App = () => {
  return(
    <Routing />
  )
}

createRoot(document.getElementById('root')!).render(<App />)