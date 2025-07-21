# **NetConfigus**  

**Универсальный парсер конфигурации для .NET**  
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)  

**NetConfigus** — это библиотека для удобного парсинга аргументов командной строки, переменных среды и файлов конфигурации в объекты C#. Она позволяет легко комбинировать настройки из разных источников с приоритетом: **CLI > Environment > JSON**.  

## **Особенности**  
✅ **Гибкая конфигурация**  
- Поддержка **позиционных** и **именованных** аргументов (`-f`, `--file`, `value1 value2`)  
- Автоматическая загрузка из **переменных среды** (`EnvironmentVariableName`)  
- Чтение **JSON-конфигурации** с возможностью переопределения через CLI  

✅ **Простота использования**  
- Декларативный стиль через атрибуты `[CommandLine]`  
- Автоматическая **конвертация типов** (`int`, `bool`, `double`, массивы)  
- Валидация **обязательных параметров** (`Required = true`)  

✅ **Безопасность и надежность**  
- Проверка конфликтов (дублирование параметров, несовместимые типы)  
- Поддержка **массивов** (накопление значений)  
- Четкие сообщения об ошибках  

---

## **Установка**  
Добавьте пакет через NuGet (или вручную подключите исходники):  
```bash
dotnet add package NetConfigus
```

---

## **Использование**  

### 1. **Определение конфигурации**  
Создайте класс и пометьте свойства атрибутом `[CommandLine]`:  

```csharp
using NetConfigus;

public class AppConfig
{
    // Позиционный аргумент (обязательный)
    [CommandLine(Position = 0, Required = true, Description = "Input file path")]
    public string InputFile { get; set; }

    // Именованный аргумент (--output или -o)
    [CommandLine(ShortName = 'o', LongName = "output", Description = "Output file path")]
    public string OutputFile { get; set; }

    // Флаг (--verbose или -v)
    [CommandLine(ShortName = 'v', LongName = "verbose")]
    public bool Verbose { get; set; }

    // Переменная среды + CLI (--port или ENV=APP_PORT)
    [CommandLine(LongName = "port", EnvironmentVariableName = "APP_PORT")]
    public int Port { get; set; } = 8080;

    // Массив значений (--item=value1 --item=value2)
    [CommandLine(LongName = "item")]
    public string[] Items { get; set; }
}
```

### 2. **Загрузка конфигурации**  
Используйте `CommandLineParser.Load` для загрузки из разных источников:  

```csharp
var config = new AppConfig();

// Загрузка из JSON + CLI (если файл есть)
CommandLineParser.Load(config, "config.json", args);

// Или только из аргументов командной строки
CommandLineParser.Parse(config, args);
```

### 3. **Запуск приложения**  
```bash
# Примеры вызова:
./app.exe input.txt --output=result.txt -v
./app.exe data.json --port 9000 --item=apple --item=orange
```

---

## **Примеры**  

### **1. Позиционные + именованные аргументы**  
```bash
./app.exe input.txt -o output.txt --verbose
```
→ `InputFile = "input.txt"`, `OutputFile = "output.txt"`, `Verbose = true`  

### **2. Переменные среды**  
```bash
export APP_PORT=3000
./app.exe --items=one --items=two
```
→ `Port = 3000`, `Items = ["one", "two"]`  

### **3. JSON-конфигурация**  
**config.json**:  
```json
{ "Port": 5000, "Verbose": true }
```
→ Значения будут переопределяться аргументами CLI.  

---

## **Обработка ошибок**  
Библиотека выбрасывает исключения в случаях:  
- `ArgumentException` – отсутствует обязательный параметр.  
- `FormatException` – неверный формат значения (например, строка вместо числа).  
- `InvalidOperationException` – конфликт параметров (дублирование, несовместимые типы).  

---

## **Лицензия**  
MIT License © 2024 [virst](https://github.com/virst)  
```text
Разрешено свободное использование, модификация и распространение.
```  

---

## **Вклад в проект**  
Приветствуются пул-реквесты и issue!  

🚀 **Ссылки**:  
- [GitHub](https://github.com/virst/NetConfigus)  
- [NuGet](https://www.nuget.org/packages/NetConfigus)  

--- 

**NetConfigus** — удобный инструмент для работы с конфигурацией в .NET-приложениях.  
Попробуйте и упростите обработку параметров! 🎉