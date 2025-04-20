import { Button } from "@/components/ui/button"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { useLoginForm } from "../model/useLoginForm";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Logo } from "@/shared";

export const LoginPage = () => {
  const { form, onSubmit, isLoginDenied } = useLoginForm()

  return (
    <div className="grid grid-cols-12 gap-4 m-auto w-7xl px-12 min-h-svh content-center">
      <Card className="col-start-5 col-span-4">
        <CardHeader className="flex items-center">
          <Logo />
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Почта</FormLabel>
                    <FormControl>
                      <Input placeholder="Введите почту" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="password"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Пароль</FormLabel>
                    <FormControl>
                      <Input placeholder="Введите пароль" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              { isLoginDenied && (<p className={"text-sm font-medium text-red-600"}>Логин или пароль указаны неверно</p>) }
              <Button type="submit" className="w-full">Войти</Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
