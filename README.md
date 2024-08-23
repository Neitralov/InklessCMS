<div align="center">
  
  ![image](https://github.com/user-attachments/assets/144aa0ba-64f2-4f62-a16e-2e8854c8458d)
  
  <h3 align="center">
    Headless CMS для создания блогов.
  </h3>
</div>

---

# 🔥 Особенности
* Headless - система управления контентом работает как репозиторий с контентом, доступ к которому осуществляется посредством REST API.
* Текстовые статьи, как основная единица контента. Их можно создавать, редактировать, удалять и т.д.
* Разделение типов статей на опубликованные и черновики, чтобы можно было хранить свои наброски приватно от читателей.
* Статьи обладают счетчиком, фиксирующим количество просмотров/чтений.
* Реализована постраничная пагинация для статей.
* Встроенный Markdown редактор.
* Интерактивная документация SwaggerUI.
* Аутентификация посредством JWT токенов (access + refresh).

# 🌆 Скриншоты
![image](https://github.com/user-attachments/assets/b14a80e0-46bc-4ead-a378-6e75971f8729)

# 📑 Документация
* [Макет сайта](https://www.figma.com/design/EjGgp0cHXhUf67yHmIk6JJ/Inkless)

# 🛠️ Сборка
1. Установите [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) версии 8.0 или новее.
2. Убедитесь, что имеете [Podman](https://podman.io).
3. Убедитесь, что имеете [Bun](https://bun.sh).
4. Убедитесь, что имеете [Make](https://en.wikipedia.org/wiki/Make_(software)).
5. Клонируйте репозиторий `git clone https://github.com/Neitralov/InklessCMS.git`.
6. Перейдите в папку проекта `cd InklessCMS`
7. Запустите сборку `make`

Приложение будет доступно по адресу: `http://localhost:5173`

Данные для входа в админку  ->  email: `admin@example.ru` password: `admin`

# 🧰 Стек технологий
Frontend:

* [React](https://react.dev)
* [React Router](https://reactrouter.com/en/main/start/overview)
* [React Hook Form](https://react-hook-form.com)
* [Zustand](https://zustand.docs.pmnd.rs/getting-started/introduction)
* [Tailwindcss](https://tailwindcss.com/)
* [Vite](https://vitejs.dev)

Backend:

* [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
* [EF Core](https://learn.microsoft.com/ru-ru/ef/core/)
* [PostgreSQL](https://hub.docker.com/_/postgres)
* [ErrorOr](https://github.com/amantinband/error-or)
* [Mapster](https://github.com/MapsterMapper/Mapster)
* [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* [Swashbuckle.AspNetCore.Filters](https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters)

Тесты:
* [xUnit](https://github.com/xunit/xunit)
* [Moq](https://github.com/devlooped/moq)
* [FluentAssertions](https://github.com/fluentassertions/fluentassertions)

# 📊 Статистика по количеству строк кода
## Frontend

```
-------------------------------------------------------------------------------
Language                     files          blank        comment           code
-------------------------------------------------------------------------------
TypeScript                      33             83              2            834
JSON                             4              4              0             89
Text                             1             15              0             81
JavaScript                       3              2              1             46
CSS                              1              7              0             41
SVG                              1              1              1             32
HTML                             1              0              0             13
-------------------------------------------------------------------------------
SUM:                            44            112              4           1136
-------------------------------------------------------------------------------
```
## Backend (без тестов)
```
-------------------------------------------------------------------------------
Language                     files          blank        comment           code
-------------------------------------------------------------------------------
C#                              39            281             95           1190
JSON                             3              0              0             65
MSBuild script                   4             13              0             61
-------------------------------------------------------------------------------
SUM:                            46            294             95           1316
-------------------------------------------------------------------------------
```

# 📃 Лицензия
Программа распространяется под лицензией [Apache License 2.0](https://github.com/Neitralov/InklessCMS/blob/master/LICENSE).

За исключением шрифта приложения Ubuntu [UBUNTU FONT LICENCE Version 1.0](https://github.com/Neitralov/GameReviewLib/blob/master/client/src/assets/UFL.txt).
