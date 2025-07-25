﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace NetConfigus;

/// <summary>
/// Парсер параметров командной строки
/// </summary>
public static class CommandLineParser
{
    /// <summary>
    /// Внутренний класс для хранения метаданных свойства
    /// </summary>
    private class PropertyData
    {
        public required PropertyInfo Property { get; set; }
        public required CommandLineAttribute Attribute { get; set; }
        public bool IsSet { get; set; }
        public Type Type => Property.PropertyType;
    }

    private static CultureInfo enUsFormat = new CultureInfo("en-US");

    /// <summary>
    /// Загружает параметры конфигурации из файла и/или командной строки.
    /// </summary>
    /// <typeparam name="T">Тип объекта конфигурации, должен иметь конструктор по умолчанию.</typeparam>
    /// <param name="fn">Путь к файлу конфигурации (JSON). Если null или файл не найден, используется объект по умолчанию.</param>
    /// <param name="args">Аргументы командной строки для парсинга. Если null, парсинг не выполняется.</param>
    /// <param name="isConfigFileRequired">Если true, выбрасывает исключение при отсутствии файла конфигурации.</param>
    /// <exception cref="Exception">Если файл конфигурации обязателен, но не найден.</exception>
    /// <exception cref="ArgumentException">При ошибках в аргументах командной строки или отсутствии обязательных параметров.</exception>
    /// <exception cref="InvalidOperationException">При конфликте заполнения параметров.</exception>
    /// <exception cref="FormatException">При ошибках преобразования типов параметров.</exception>
    /// <remarks>
    /// Сначала загружает параметры из файла, затем из переменных среды, затем из командной строки.
    /// </remarks>

    public static T Load<T>(string? fn, string[]? args, bool isConfigFileRequired = false) where T : class, new()
    {
        if (isConfigFileRequired && !File.Exists(fn))
            throw new Exception("Configuration file not found!");

        T options;
        if (File.Exists(fn))
            options = JsonSerializer.Deserialize<T>(File.ReadAllText(fn)) ?? new T();
        else
            options = new T();

        List<PropertyData> properties = GetProperties(options);
        ValidateProperties(properties);

        ParseEnvironmen(options, properties);

        if (args != null)
            Parse(options, args, properties);

        return options;
    }

    /// <summary>
    /// Заполняет свойства объекта конфигурации значениями из переменных среды.
    /// </summary>
    /// <typeparam name="T">Тип объекта конфигурации.</typeparam>
    /// <param name="options">Экземпляр объекта конфигурации для заполнения.</param>
    /// <param name="properties">Список метаданных свойств, полученных через атрибуты.</param>
    /// <remarks>
    /// Для каждого свойства, у которого задано имя переменной среды, устанавливает значение из соответствующей переменной среды.
    /// Если переменная среды отсутствует или пуста, свойство не изменяется.
    /// </remarks>
    /// <exception cref="FormatException">
    /// Возникает при ошибке преобразования значения переменной среды к типу свойства.
    /// </exception>
    private static void ParseEnvironmen<T>(T options, List<PropertyData> properties) where T : class, new()
    {
        foreach (var prop in properties)
        {
            if (string.IsNullOrWhiteSpace(prop.Attribute.EnvironmentVariableName)) continue;
            var evnVal = Environment.GetEnvironmentVariable(prop.Attribute.EnvironmentVariableName);
            if (string.IsNullOrWhiteSpace(evnVal)) continue;
            object convertedValue = ConvertValue(evnVal, prop.Type);
            prop.Property.SetValue(options, convertedValue);
            prop.IsSet = true;
        }
    }

    /// <summary>
    /// Парсит аргументы командной строки и заполняет свойства объекта
    /// </summary>
    /// <typeparam name="T">Тип объекта с параметрами</typeparam>
    /// <param name="options">Экземпляр объекта для заполнения</param>
    /// <param name="args">Аргументы командной строки</param>
    /// <exception cref="ArgumentNullException">
    /// Возникает если options или args равны null
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Возникает при отсутствии обязательных параметров или неверном количестве позиционных аргументов
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Возникает при конфликте заполнения параметров
    /// </exception>
    /// <exception cref="FormatException">
    /// Возникает при ошибках преобразования типов
    /// </exception>
    /// <example>
    /// <code>
    /// var options = new Options();
    /// CommandLineParser.Parse(options, args);
    /// </code>
    /// </example>
    public static void Parse<T>(T options, string[] args) where T : class
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (args == null) throw new ArgumentNullException(nameof(args));

        List<PropertyData> properties = GetProperties(options);
        ValidateProperties(properties);

        Parse(options, args, properties);
    }


    /// <summary>
    /// Парсит аргументы командной строки и заполняет свойства объекта
    /// </summary>
    /// <typeparam name="T">Тип объекта с параметрами</typeparam>
    /// <param name="options">Экземпляр объекта для заполнения</param>
    /// <param name="args">Аргументы командной строки</param>  
    /// <param name="properties">Массив свойств</param>  
    private static void Parse<T>(T options, string[] args, List<PropertyData> properties) where T : class
    {
        var (positionalArgs, namedArgs) = SplitArguments(args);
        ProcessPositionalArguments(options, properties, positionalArgs);
        ProcessNamedArguments(options, properties, namedArgs);
        CheckRequiredProperties(properties);
    }

    /// <summary>
    /// Извлекает свойства с атрибутом CommandLine из объекта
    /// </summary>
    private static List<PropertyData> GetProperties<T>(T options)
    {
#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
        return typeof(T)
            .GetProperties()
            .Where(p => p.IsDefined(typeof(CommandLineAttribute), false))
            .Select(p => new PropertyData
            {
                Property = p,
                Attribute = p.GetCustomAttribute<CommandLineAttribute>(),
                IsSet = false
            })
            .ToList();
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
    }

    /// <summary>
    /// Валидирует корректность конфигурации свойств
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Возникает при недопустимой конфигурации (bool + Position)
    /// </exception>
    private static void ValidateProperties(List<PropertyData> properties)
    {
        foreach (var prop in properties)
        {
            if (prop.Type == typeof(bool) && prop.Attribute.Position > -1)
            {
                throw new InvalidOperationException(
                    $"Boolean property '{prop.Property.Name}' cannot use Position");
            }

            if (prop.Attribute.Position > -1 && properties.Count(p => p.Attribute.Position == prop.Attribute.Position) > 1)
            {
                throw new InvalidOperationException(
                    $"Several properties are competing for position {prop.Attribute.Position}");
            }
        }
    }

    /// <summary>
    /// Разделяет аргументы на позиционные и именованные
    /// </summary>
    /// <remarks>
    /// Позиционные аргументы - все до первого именованного параметра
    /// </remarks>
    private static (List<string> positional, List<string> named) SplitArguments(string[] args)
    {
        List<string> positional = new List<string>();
        List<string> named = new List<string>();
        bool namedSection = false;

        foreach (var arg in args)
        {
            if (arg.StartsWith("-"))
            {
                namedSection = true;
                named.Add(arg);
            }
            else if (!namedSection)
            {
                positional.Add(arg);
            }
            else
            {
                named.Add(arg);
            }
        }

        return (positional, named);
    }

    /// <summary>
    /// Обрабатывает позиционные аргументы
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Возникает при отсутствии обязательного позиционного аргумента
    /// </exception>
    private static void ProcessPositionalArguments<T>(
        T options, List<PropertyData> properties, List<string> positionalArgs)
    {
        var positionalProps = properties
            .Where(p => p.Attribute.Position != -1)
            .OrderBy(p => p.Attribute.Position)
            .ToList();

        for (int i = 0; i < positionalProps.Count; i++)
        {
            if (i >= positionalArgs.Count)
            {
                if (positionalProps[i].Attribute.Required)
                    throw new ArgumentException(
                        $"Missing required positional argument at position {i}");
                break;
            }

            SetPropertyValue(
                options,
                positionalProps[i],
                positionalArgs[i],
                isPositional: true
            );
        }
    }

    /// <summary>
    /// Обрабатывает именованные аргументы
    /// </summary>
    /// <exception cref="FormatException">
    /// Возникает при отсутствии значения для параметра
    /// </exception>
    private static void ProcessNamedArguments<T>(
        T options, List<PropertyData> properties, List<string> namedArgs)
    {
        int index = 0;
        while (index < namedArgs.Count)
        {
            string arg = namedArgs[index++];
            bool isLong = arg.StartsWith("--");
            bool isShort = !isLong && arg.StartsWith("-");

            if (!isLong && !isShort) continue;

            string key;
            string? value = null;
            int separatorIndex;

            if (isLong)
            {
                key = arg.Substring(2);
                separatorIndex = key.IndexOf('=');
                if (separatorIndex >= 0)
                {
                    value = key.Substring(separatorIndex + 1);
                    key = key.Substring(0, separatorIndex);
                }
                else if (index < namedArgs.Count && !namedArgs[index].StartsWith("-"))
                {
                    value = namedArgs[index++];
                }
            }
            else
            {
                key = arg.Substring(1);
                separatorIndex = key.IndexOf('=');
                if (separatorIndex >= 0)
                {
                    value = key.Substring(separatorIndex + 1);
                    key = key.Substring(0, separatorIndex);
                }
                else if (index < namedArgs.Count && !namedArgs[index].StartsWith("-"))
                {
                    value = namedArgs[index++];
                }
            }

            var property = FindProperty(properties, key, isLong);
            if (property == null)
            {
                if (!isLong)
                {
                    var boolPropertys = key.Select(c => FindProperty(properties, c.ToString(), isLong))
                        .Where(p => p?.Type == typeof(bool)).ToArray();
                    if (boolPropertys.Length == key.Length)
                        foreach (var prop in boolPropertys)
                            if (prop != null)
                                SetPropertyValue(options, prop, "true", isPositional: false);

                }

                continue;
            }

            if (property.Type == typeof(bool) && value == null)
            {
                value = "true";
            }

            if (value == null)
            {
                throw new FormatException(
                    $"Missing value for parameter '{key}'");
            }

            SetPropertyValue(options, property, value, isPositional: false);
        }
    }

    /// <summary>
    /// Находит свойство по имени параметра
    /// </summary>
    /// <returns>Найденное свойство или null</returns>
    private static PropertyData? FindProperty(
        List<PropertyData> properties, string key, bool isLong)
    {
        StringComparison comparison = StringComparison.OrdinalIgnoreCase;

        return properties.FirstOrDefault(p =>
        {
            if (isLong)
            {
                return p.Attribute.LongName != null &&
                       p.Attribute.LongName.Equals(key, comparison);
            }
            return p.Attribute.ShortName != '\0' &&
                   p.Attribute.ShortName.ToString().Equals(key, comparison);
        });
    }

    /// <summary>
    /// Устанавливает значение свойства
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Возникает при конфликте перезаписи не-массива
    /// </exception>
    /// <exception cref="FormatException">
    /// Возникает при ошибках преобразования типов
    /// </exception>
    private static void SetPropertyValue<T>(
        T options, PropertyData property, string value, bool isPositional)
    {
        if (property.IsSet && !property.Type.IsArray)
        {
            throw new InvalidOperationException(
                $"Property '{property.Property.Name}' is already set");
        }

        try
        {
            object convertedValue = ConvertValue(value, property.Type);

            if (property.Type.IsArray)
            {
                HandleArrayValue(options, property, convertedValue);
            }
            else
            {
                property.Property.SetValue(options, convertedValue);
                property.IsSet = true;
            }
        }
        catch (Exception ex)
        {
            throw new FormatException(
                $"Invalid value '{value}' for property '{property.Property.Name}'", ex);
        }
    }

    /// <summary>
    /// Конвертирует строковое значение в целевой тип
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Возникает при попытке конвертации в неподдерживаемый тип
    /// </exception>
    private static object ConvertValue(string value, Type targetType)
    {
        if (targetType == typeof(string)) return value;
        if (targetType == typeof(int)) return int.Parse(value);
        if (targetType == typeof(bool)) return bool.Parse(value);
        if (targetType == typeof(double)) return double.Parse(value, enUsFormat);

        if (targetType.IsArray)
        {
            return ConvertValue(value, targetType.GetElementType() ?? throw new Exception("Nellable Element Type"));
        }

        throw new NotSupportedException(
            $"Type {targetType} is not supported");
    }

    /// <summary>
    /// Обрабатывает значения для массивов (добавляет в конец)
    /// </summary>
    private static void HandleArrayValue<T>(
        T options, PropertyData property, object value)
    {
        var current = property.Property.GetValue(options) as Array;
        int newLength = current?.Length + 1 ?? 1;

        var newArray = Array.CreateInstance(
            property.Type.GetElementType() ?? throw new Exception("Nellable Element Type"),
            newLength
        );

        if (current != null)
        {
            Array.Copy(current, newArray, current.Length);
        }

        newArray.SetValue(value, newLength - 1);
        property.Property.SetValue(options, newArray);
        property.IsSet = true;
    }

    /// <summary>
    /// Проверяет заполнение обязательных свойств
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Возникает при отсутствии обязательного параметра
    /// </exception>
    private static void CheckRequiredProperties(List<PropertyData> properties)
    {
        foreach (var prop in properties.Where(p => p.Attribute.Required))
        {
            if (!prop.IsSet)
            {
                throw new ArgumentException(
                    $"Required property '{prop.Property.Name}' is not set");
            }
        }
    }
}