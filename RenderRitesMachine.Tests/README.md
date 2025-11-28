# RenderRitesMachine.Tests

Проект тестов для RenderRites Machine.

## Запуск тестов

### Через командную строку

Из корня решения:

```bash
dotnet test
```

Из директории проекта тестов:

```bash
cd RenderRitesMachine.Tests
dotnet test
```

### С различными опциями

**Подробный вывод:**

```bash
dotnet test --verbosity normal
dotnet test --verbosity detailed
```

**Запуск конкретного класса тестов:**

```bash
dotnet test --filter "FullyQualifiedName~PerspectiveCameraTests"
```

**Запуск конкретного теста:**

```bash
dotnet test --filter "FullyQualifiedName~PerspectiveCameraTests.Constructor_InitializesWithDefaultValues"
```

**Запуск только пройденных тестов (исключая пропущенные):**

```bash
dotnet test --filter "FullyQualifiedName!~SystemSharedObjectTests"
```

**С покрытием кода:**

```bash
dotnet test /p:CollectCoverage=true
```

### Через Visual Studio

1. Откройте Test Explorer (Test → Test Explorer или `Ctrl+E, T`)
2. Нажмите "Run All Tests" или запустите конкретные тесты

### Через Visual Studio Code

1. Установите расширение ".NET Core Test Explorer"
2. Тесты появятся в боковой панели
3. Запускайте тесты через интерфейс расширения

## Структура тестов

- **PerspectiveCameraTests** - Тесты для камеры (40 тестов)
- **SceneManagerTests** - Тесты для менеджера сцен (20 тестов)
- **AssetsServiceTests** - Тесты для сервиса ресурсов (47 тестов)
- **RenderConstantsTests** - Тесты для констант (10 тестов)
- **SystemSharedObjectTests** - Тесты для общего объекта систем (18 тестов)

## Решение проблемы тестирования OpenGL

Для тестирования методов, требующих OpenGL контекста, был применен подход **Dependency Injection**:

1. **Создан интерфейс `IOpenGLWrapper`** - абстракция для OpenGL вызовов
2. **Создан класс `OpenGLWrapper`** - реализация, использующая реальные OpenGL вызовы
3. **Рефакторинг `SystemSharedObject`** - теперь принимает `IOpenGLWrapper` как опциональный параметр
4. **В тестах используется мок `IOpenGLWrapper`** - позволяет тестировать логику без реального OpenGL контекста

Этот подход позволяет:

- ✅ Тестировать логику управления шейдерами без OpenGL контекста
- ✅ Проверять правильность вызовов OpenGL методов через моки
- ✅ Сохранить обратную совместимость (по умолчанию используется реальный `OpenGLWrapper`)

## Статистика

- **Всего тестов:** 262
- **Пройдено:** 262
- **Пропущено:** 0
- **Провалено:** 0

### Покрытие тестами

- **PerspectiveCameraTests** - 40 тестов (включая edge cases)
- **SceneManagerTests** - 20 тестов (включая edge cases)
- **AssetsServiceTests** - 47 тестов (включая edge cases)
- **SystemSharedObjectTests** - 18 тестов (включая edge cases)
- **RayTests** - 10 тестов (edge cases для работы с лучами)
- **LoggerTests** - 20 тестов (включая edge cases)
- **SceneFactoryTests** - 5 тестов
- **RenderConstantsTests** - 10 тестов
- **AudioServiceTests** - 32 теста

