/*
The MIT License (MIT)

Copyright (c) 2015-present Heleonix - Hennadii Lutsyshyn

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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using static System.FormattableString;

namespace Heleonix.Build
{
    /// <summary>
    /// Builder for arguments to be passed to executable commands.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class ArgsBuilder
    {
        #region Fields

        /// <summary>
        /// The <see cref="StringBuilder"/>.
        /// </summary>
        private readonly StringBuilder _builder = new StringBuilder();

        /// <summary>
        /// The key prefix.
        /// </summary>
        private readonly string _keyPrefix;

        /// <summary>
        /// The key/value separator.
        /// </summary>
        private readonly string _keyValueSeparator;

        /// <summary>
        /// The value separator.
        /// </summary>
        private readonly string _valueSeparator;

        /// <summary>
        /// The path wrapper.
        /// </summary>
        private readonly string _pathWrapper;

        /// <summary>
        /// The arguments separator.
        /// </summary>
        private readonly string _argsSeparator;

        /// <summary>
        /// Determines whether this instance was modified.
        /// </summary>
        private bool _wasModified;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgsBuilder" /> class.
        /// </summary>
        /// <param name="keyPrefix">The key prefix.</param>
        /// <param name="keyValueSeparator">The key/value separator.</param>
        /// <param name="valueSeparator">The value separator.</param>
        /// <param name="pathWrapper">The path wrapper.</param>
        /// <param name="argsSeparator">The arguments separator.</param>
        private ArgsBuilder(string keyPrefix, string keyValueSeparator, string valueSeparator,
            string pathWrapper, string argsSeparator)
        {
            _keyPrefix = keyPrefix;
            _keyValueSeparator = keyValueSeparator;
            _valueSeparator = valueSeparator;
            _pathWrapper = pathWrapper;
            _argsSeparator = argsSeparator;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="ArgsBuilder"/> with the specified separators.
        /// </summary>
        /// <param name="keyPrefix">The key prefix.</param>
        /// <param name="keyValueSeparator">The key/value separator.</param>
        /// <param name="valueSeparator">The value separator.</param>
        /// <param name="pathWrapper">The path wrapper.</param>
        /// <param name="argsSeparator">The arguments separator.</param>
        /// <returns>The <see cref="ArgsBuilder"/>.</returns>
        public static ArgsBuilder By(string keyPrefix, string keyValueSeparator, string valueSeparator,
            string pathWrapper, string argsSeparator)
            => new ArgsBuilder(keyPrefix, keyValueSeparator, valueSeparator, pathWrapper, argsSeparator);

        /// <summary>
        /// Creates a new <see cref="ArgsBuilder"/> with the specified separators.
        /// </summary>
        /// <param name="keyPrefix">The key prefix.</param>
        /// <param name="keyValueSeparator">The key/value separator.</param>
        /// <param name="valueSeparator">The value separator.</param>
        /// <returns>The <see cref="ArgsBuilder"/>.</returns>
        public static ArgsBuilder By(string keyPrefix, string keyValueSeparator, string valueSeparator)
            => By(keyPrefix, keyValueSeparator, valueSeparator, "\"", " ");

        /// <summary>
        /// Creates a new <see cref="ArgsBuilder"/> with the specified separators.
        /// </summary>
        /// <param name="keyPrefix">The key prefix.</param>
        /// <param name="keyValueSeparator">The key/value separator.</param>
        /// <returns>The <see cref="ArgsBuilder"/>.</returns>
        public static ArgsBuilder By(string keyPrefix, string keyValueSeparator)
            => By(keyPrefix, keyValueSeparator, ";");

        /// <summary>
        /// Adds the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddKey(string key, bool condition)
        {
            if (!condition || string.IsNullOrEmpty(key)) return this;

            return Append(Invariant($"{_keyPrefix}{key}"));
        }

        /// <summary>
        /// Adds the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddKey(string key) => AddKey(key, true);

        /// <summary>
        /// Adds the <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddKeys(IEnumerable<string> keys, bool condition)
        {
            if (!condition || keys == null) return this;

            var add = string.Join(_argsSeparator,
                from k in keys where !string.IsNullOrEmpty(k) select Invariant($"{_keyPrefix}{k}"));

            return string.IsNullOrEmpty(add) ? this : Append(add);
        }

        /// <summary>
        /// Adds the <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddKeys(IEnumerable<string> keys) => AddKeys(keys, true);

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddValue(object value, bool condition)
        {
            var val = value?.ToString();

            if (!condition || string.IsNullOrEmpty(val)) return this;

            return Append(val);
        }

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddValue(object value) => AddValue(value, true);

        /// <summary>
        /// Adds the <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddValues(IEnumerable<object> values, bool condition)
        {
            if (!condition || values == null) return this;

            var add = string.Join(_argsSeparator,
                from v in values let vs = v?.ToString() where !string.IsNullOrEmpty(vs) select vs);

            return string.IsNullOrEmpty(add) ? this : Append(add);
        }

        /// <summary>
        /// Adds the <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddValues(IEnumerable<object> values) => AddValues(values, true);

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddArgument(string key, object value, bool condition)
        {
            var val = value?.ToString();

            if (!condition || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val)) return this;

            return Append(Invariant($"{_keyPrefix}{key}{_keyValueSeparator}{val}"));
        }

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddArgument(string key, object value) => AddArgument(key, value, true);

        /// <summary>
        /// Adds the <paramref name="values"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="multipleTimes">Determines whether to add each path with separate key.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddArguments(string key, IEnumerable<object> values, bool multipleTimes, bool condition)
        {
            if (!condition || string.IsNullOrEmpty(key) || values == null) return this;

            if (multipleTimes)
            {
                foreach (var value in values)
                {
                    Append(Invariant($"{_keyPrefix}{key}{_keyValueSeparator}{value}"));
                }

                return this;
            }

            var add = string.Join(_valueSeparator,
                from v in values let vs = v?.ToString() where !string.IsNullOrEmpty(vs) select vs);

            return string.IsNullOrEmpty(add) ? this : Append(Invariant($"{_keyPrefix}{key}{_keyValueSeparator}{add}"));
        }

        /// <summary>
        /// Adds the <paramref name="values"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="multipleTimes">Determines whether to add each path with separate key.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddArguments(string key, IEnumerable<object> values, bool multipleTimes)
            => AddArguments(key, values, multipleTimes, true);

        /// <summary>
        /// Adds the <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPath(string path, bool condition)
        {
            if (!condition || string.IsNullOrEmpty(path)) return this;

            return Append(Invariant($"{_pathWrapper}{path}{_pathWrapper}"));
        }

        /// <summary>
        /// Adds the <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPath(string path) => AddPath(path, true);

        /// <summary>
        /// Adds the <paramref name="paths"/>.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPaths(IEnumerable<string> paths, bool condition)
        {
            if (!condition || paths == null) return this;

            var add = string.Join(_argsSeparator,
                from p in paths where !string.IsNullOrEmpty(p) select Invariant($"{_pathWrapper}{p}{_pathWrapper}"));

            return string.IsNullOrEmpty(add) ? this : Append(add);
        }

        /// <summary>
        /// Adds the <paramref name="paths"/>.
        /// </summary>
        /// <param name="paths">The values.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPaths(IEnumerable<string> paths) => AddPaths(paths, true);

        /// <summary>
        /// Adds the <paramref name="path"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="path">The path.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPath(string key, string path, bool condition)
        {
            if (!condition || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(path)) return this;

            return Append(Invariant($"{_keyPrefix}{key}{_keyValueSeparator}{_pathWrapper}{path}{_pathWrapper}"));
        }

        /// <summary>
        /// Adds the <paramref name="path"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="path">The path.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPath(string key, string path) => AddPath(key, path, true);

        /// <summary>
        /// Adds the <paramref name="paths"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="multipleTimes">Determines whether to add each path with separate key.</param>
        /// <param name="condition">A condition to add.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPaths(string key, IEnumerable<string> paths, bool multipleTimes, bool condition)
        {
            if (!condition || string.IsNullOrEmpty(key) || paths == null) return this;

            if (multipleTimes)
            {
                foreach (var path in paths)
                {
                    Append(Invariant($"{_keyPrefix}{key}{_keyValueSeparator}{_pathWrapper}{path}{_pathWrapper}"));
                }

                return this;
            }

            var add = string.Join(_valueSeparator,
                from p in paths where !string.IsNullOrEmpty(p) select Invariant($"{_pathWrapper}{p}{_pathWrapper}"));

            return string.IsNullOrEmpty(add) ? this : Append(Invariant($"{_keyPrefix}{key}{_keyValueSeparator}{add}"));
        }

        /// <summary>
        /// Adds the <paramref name="paths"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="multipleTimes">Determines whether to add each path with separate key.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        public ArgsBuilder AddPaths(string key, IEnumerable<string> paths, bool multipleTimes)
            => AddPaths(key, paths, multipleTimes, true);

        /// <summary>
        /// Appends the <paramref name="arg"/>.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>This <see cref="ArgsBuilder"/>.</returns>
        private ArgsBuilder Append(string arg)
        {
            _builder.Append(arg).Append(_argsSeparator);

            _wasModified = true;

            return this;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="ArgsBuilder"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(ArgsBuilder builder) => builder?.ToString();

        #endregion

        #region object Members

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
            => _wasModified ? _builder.Remove(_builder.Length - 1, 1).ToString() : _builder.ToString();

        #endregion
    }
}