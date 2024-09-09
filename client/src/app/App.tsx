import { createRoot } from 'react-dom/client'
import './assets/index.css'
import { Routing } from "./routes/Routing.tsx"

const App = () => {
  return(
    <>
      { import.meta.env.DEV &&
        <div className="flex justify-center bg-green-300">
          <a href="http://localhost:8080/swagger" target={"_blank"} className="my-1 hover:underline">Открыть Swagger UI</a>
        </div>
      }

      <Routing />
    </>
  )
}

createRoot(document.getElementById('root')!).render(<App />)
