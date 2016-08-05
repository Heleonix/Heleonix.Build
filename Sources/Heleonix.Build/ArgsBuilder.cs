/*
The MIT License (MIT)

Copyright (c) 2015-2016 Heleonix - Hennadii Lutsyshyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using System.Text;

namespace Heleonix.Build
{
    /// <summary>
    /// Builder for arguments to be passed to executable commands.
    /// </summary>
    internal class ArgsBuilder
    {
        #region Fields

        /// <summary>
        /// The <see cref="StringBuilder"/>.
        /// </summary>
        private readonly StringBuilder _builder = new StringBuilder();

        /// <summary>
        /// The arguments separator.
        /// </summary>
        private readonly char _argsSeparator;

        /// <summary>
        /// The key/value separator.
        /// </summary>
        private readonly char _keyValueSeparator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgsBuilder" /> class.
        /// </summary>
        /// <param name="argsSeparator">The arguments separator.</param>
        /// <param name="keyValueSeparator">The key/value separator.</param>
        private ArgsBuilder(char argsSeparator, char keyValueSeparator)
        {
            _argsSeparator = argsSeparator;
            _keyValueSeparator = keyValueSeparator;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="ArgsBuilder"/> with the specified separators.
        /// </summary>
        /// <param name="argsSeparator">The arguments separator.</param>
        /// <param name="keyValueSeparator">The key/value separator.</param>
        /// <returns>The <see cref="ArgsBuilder"/>.</returns>
        public static ArgsBuilder By(char argsSeparator, char keyValueSeparator)
            => new ArgsBuilder(argsSeparator, keyValueSeparator);

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isPath">If set to <c>true</c> adds a <paramref name="value"/> as quoted path.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder Add(object value, bool isPath = false, bool condition = true)
        {
            if (condition && !string.IsNullOrEmpty(value?.ToString()))
            {
                _builder.Append(isPath ? $"\"{value}\"{_argsSeparator}" : $"{value}{_argsSeparator}");
            }

            return this;
        }

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isPath">If set to <c>true</c> adds a <paramref name="value"/> as quoted path.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder Add(string value, bool isPath = false, bool condition = true)
        {
            Add((object) value, isPath, condition);

            return this;
        }

        /// <summary>
        /// Adds the <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="isPath">If set to <c>true</c> adds a <paramref name="values"/> as quoted path.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder Add(IEnumerable<object> values, bool isPath = false, bool condition = true)
        {
            if (!condition || values == null)
            {
                return this;
            }

            foreach (var value in values)
            {
                Add(value, isPath, condition);
            }

            return this;
        }

        /// <summary>
        /// Adds the <paramref name="values"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="isPath">If set to <c>true</c> adds a <paramref name="values"/> as quoted path.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder Add(string key, IEnumerable<object> values, bool isPath = false, bool condition = true)
        {
            if (!condition || values == null)
            {
                return this;
            }

            foreach (var value in values)
            {
                Add(key, value, isPath, condition);
            }

            return this;
        }

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="isPath">If set to <c>true</c> adds a <paramref name="value"/> as quoted path.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder Add(string key, object value, bool isPath = false, bool condition = true)
        {
            if (condition && !string.IsNullOrEmpty(value?.ToString()))
            {
                _builder.Append(isPath
                    ? $"{key}{_keyValueSeparator}\"{value}\"{_argsSeparator}"
                    : $"{key}{_keyValueSeparator}{value}{_argsSeparator}");
            }

            return this;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="ArgsBuilder"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(ArgsBuilder builder) => builder.ToString();

        #endregion

        #region object Members

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => _builder.ToString().TrimEnd(_argsSeparator);

        #endregion
    }
}