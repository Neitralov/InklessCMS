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
* Интерактивная документация Scalar.
* Аутентификация посредством JWT токенов (access + refresh).

# 🛠️ Сборка
1. Установите [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) версии 9.0 или новее.
2. Убедитесь, что имеете [Docker](https://www.docker.com/).
4. Убедитесь, что имеете [Make](https://en.wikipedia.org/wiki/Make_(software)).
5. Клонируйте репозиторий `git clone https://github.com/Neitralov/InklessCMS.git`.
6. Перейдите в папку проекта `cd InklessCMS`
7. Запустите сборку `make`

Зайти в интерактивную документацию можно по адресу: `http://localhost:8080/scalar`

# 🧰 Стек технологий
Backend:

* [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
* [EF Core](https://learn.microsoft.com/ru-ru/ef/core/)
* [PostgreSQL](https://hub.docker.com/_/postgres)
* [ErrorOr](https://github.com/amantinband/error-or)
* [Scalar](https://github.com/scalar/scalar)
* [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* [Swashbuckle.AspNetCore.Filters](https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters)

Тесты:
* [xUnit](https://github.com/xunit/xunit)
* [Moq](https://github.com/devlooped/moq)
* [Shouldly](https://github.com/shouldly/shouldly)
* [Respawn](https://github.com/jbogard/Respawn)

# 📊 Статистика по количеству строк кода

## Основной код (без тестов и миграций)
```
-------------------------------------------------------------------------------
Language                     files          blank        comment           code
-------------------------------------------------------------------------------
C#                              49            318            114           1370
MSBuild script                   5             20              0             90
JSON                             4              0              0             68
Dockerfile                       2              0              0              9
-------------------------------------------------------------------------------
SUM:                            60            338            114           1537
-------------------------------------------------------------------------------
```

## Отдельно тесты

```
-------------------------------------------------------------------------------
Language                     files          blank        comment           code
-------------------------------------------------------------------------------
C#                              31            382            265           1549
MSBuild script                   2             10              0             52
-------------------------------------------------------------------------------
SUM:                            33            392            265           1601
-------------------------------------------------------------------------------
```

# 📃 Лицензия
Программа распространяется под лицензией [Apache License 2.0](https://github.com/Neitralov/InklessCMS/blob/master/LICENSE).
