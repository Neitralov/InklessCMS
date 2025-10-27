<div align="center">
  
  ![image](https://github.com/user-attachments/assets/144aa0ba-64f2-4f62-a16e-2e8854c8458d)
  
  <h3 align="center">
    Headless CMS для создания и управления блогами
  </h3>
</div>

---

# 🔥 Особенности
* Headless - система управления контентом предоставляет только API для взаимодействия с контентом, оставляя разработчику свободу выбора внешней реализации и отображения контента.
* Текстовые статьи, как основная единица контента. Их можно создавать, редактировать, удалять и т.д.
* Разделение типов статей на опубликованные и черновики, чтобы можно было хранить свои наброски приватно от читателей.
* Статьи обладают счетчиком, фиксирующим количество просмотров/чтений.
* Реализована постраничная пагинация для статей.
* Статьи можно группировать в коллекции.
* API полностью на GraphQL.
* Интерактивная документация в Nitro.
* Аутентификация посредством JWT токенов (access + refresh).

# 🛠️ Сборка
1. Установите [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) версии 9.0 или новее.
2. Убедитесь, что имеете [Docker](https://www.docker.com/).
4. Убедитесь, что имеете [Make](https://en.wikipedia.org/wiki/Make_(software)).
5. Клонируйте репозиторий `git clone https://github.com/Neitralov/InklessCMS.git`.
6. Перейдите в папку проекта `cd InklessCMS`
7. Запустите БД `make run-db`
8. Запустите сборку `make`

Зайти в интерактивную документацию можно по адресу: `http://localhost:8080/graphql`

# 🧰 Стек технологий
Backend:

* [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
* [EF Core](https://learn.microsoft.com/ru-ru/ef/core/)
* [PostgreSQL](https://hub.docker.com/_/postgres)
* [ErrorOr](https://github.com/amantinband/error-or)
* [GraphQL (HotChocolate)](https://github.com/ChilliCream/graphql-platform)

Тесты:
* [xUnit](https://github.com/xunit/xunit)
* [Moq](https://github.com/devlooped/moq)
* [Shouldly](https://github.com/shouldly/shouldly)
* [Respawn](https://github.com/jbogard/Respawn)
* [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet)
* [GraphQL Client](https://github.com/graphql-dotnet/graphql-client)

# 📊 Статистика по количеству строк кода

## Основной код (без тестов и миграций)
```
-------------------------------------------------------------------------------
Language                     files          blank        comment           code
-------------------------------------------------------------------------------
C#                              44            328              0           1320
MSBuild script                   4             18              0             77
JSON                             4              0              0             68
Dockerfile                       2              0              0              9
-------------------------------------------------------------------------------
SUM:                            54            346              0           1474
-------------------------------------------------------------------------------
```

## Отдельно тесты

```
-------------------------------------------------------------------------------
Language                     files          blank        comment           code
-------------------------------------------------------------------------------
C#                              51            464            264           2128
MSBuild script                   2             10              0             57
-------------------------------------------------------------------------------
SUM:                            53            474            264           2185
-------------------------------------------------------------------------------
```

# 📃 Лицензия
Программа распространяется под лицензией [Apache License 2.0](https://github.com/Neitralov/InklessCMS/blob/master/LICENSE).
