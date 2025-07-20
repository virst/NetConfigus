using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus
{
    /// <summary>
    /// Атрибут для определения параметров командной строки
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandLineAttribute : Attribute
    {
        /// <summary>
        /// Краткое имя параметра (одиночный символ)
        /// </summary>
        public char ShortName { get; set; } = '\0';
        /// <summary>
        /// Полное имя параметра
        /// </summary>
        public string? LongName { get; set; }
        /// <summary>
        /// Описание параметра для справки
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Обязательность параметра (по умолчанию false)
        /// </summary>
        public bool Required { get; set; } = false;
        /// <summary>
        /// Позиция аргумента (начиная с 0)
        /// </summary>
        public int Position { get; set; } = -1;
    }
}
