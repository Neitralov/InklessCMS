import {Logo} from "../../../shared/ui/Logo.tsx";
import {LoginUserRequest} from "../../../shared/model/LoginUserRequest.ts";
import {useAuthStore} from "../../../shared/model/useAuthStore.ts";
import {useNavigate} from "react-router-dom";
import {useForm} from "react-hook-form";

export const LoginPage = () => {
  const {
    register,
    formState: { errors },
    handleSubmit,
    reset
  } = useForm<LoginUserRequest>({ defaultValues: { email: "", password: "" }});
  const login = useAuthStore(state => state.login)
  const isLoginDenied = useAuthStore(state => state.isLoginDenied)
  const navigate = useNavigate()

  const submit = (data: LoginUserRequest) => {
    login(data, navigate)
    reset()
  }

  return(
    <div className={"flex justify-center items-center min-h-screen"}>
      <form onSubmit={handleSubmit(submit)} className={"flex flex-col gap-3 w-96 px-5 py-5 border border-black rounded-md"}>
        <div className={"flex justify-center pb-2 border-b border-black/20"}>
          <Logo />
        </div>

        <div className={"flex flex-col gap-1"}>
          <label htmlFor={"email"} className={"flex gap-2 text-lg font-medium"}>
            Email
            { errors.email?.type === "required" && (<p className={"font-normal text-red-600"}>Поле обязательно</p>)}
          </label>
          <input
            { ...register("email", { required: true })}
            id={"email"}
            name={"email"}
            className={`${errors.email != undefined && "outline outline-2 outline-red-600"} text-lg bg-neutral-200 px-2 py-1 rounded`}
            placeholder={"Введите email..."}/>
        </div>

        <div className={"flex flex-col gap-1"}>
          <label htmlFor={"password"} className={`flex gap-2 text-lg font-medium`}>
            Пароль
            { errors.password?.type === "required" && (<p className={"font-normal text-red-600"}>Поле обязательно</p>)}
          </label>
          <input
            { ...register("password", { required: true })}
            id={"password"}
            name={"password"}
            type={"password"}
            className={`${errors.password != undefined && "outline outline-2 outline-red-600"} text-lg bg-neutral-200 px-2 py-1 rounded`}
            placeholder={"Введите пароль..."}/>
        </div>

        { isLoginDenied && (<p className={"font-medium text-red-600"}>Логин или пароль указаны неверно</p>)}
        <button className={"text-lg font-medium text-black border border-black hover:text-white hover:bg-black py-1.5 rounded-md"}>
          Войти
        </button>
      </form>
    </div>
  )
}