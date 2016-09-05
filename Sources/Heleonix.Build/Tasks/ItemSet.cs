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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Handles items as sets.
    /// </summary>
    public class ItemSet : BaseTask
    {
        #region Properties

        /// <summary>
        /// The operation on items.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Union</term></item>
        /// <item><term>Intersection</term></item>
        /// <item><term>RelativeComplement</term></item>
        /// <item><term>SymmetricDifference</term></item>
        /// </list>
        /// </remarks>
        [Required]
        public string Operation { get; set; }

        /// <summary>
        /// Left-side items.
        /// </summary>
        public ITaskItem[] Left { get; set; }

        /// <summary>
        /// Right-side items.
        /// </summary>
        public ITaskItem[] Right { get; set; }

        /// <summary>
        /// [Output] The resulting items.
        /// </summary>
        [Output]
        public ITaskItem[] Result { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Handles items as sets.
        /// </summary>
        protected override void ExecuteInternal()
        {
            if (Left == null)
            {
                Log.LogMessage($"'{nameof(Left)}' is null.");

                return;
            }

            if (Right == null)
            {
                Log.LogMessage($"'{nameof(Right)}' is null.");

                return;
            }

            var result = new List<ITaskItem>();

            switch (Operation)
            {
                case "Union":
                    result.AddRange(Left.Select(i => new TaskItem(i)));
                    foreach (var right in Right)
                    {
                        if (!result.Exists(r => string.Equals(r.ItemSpec, right.ItemSpec,
                            StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(right);
                        }
                    }
                    break;

                case "Intersection":
                    foreach (var left in Left)
                    {
                        if (Right.Any(r => string.Equals(r.ItemSpec, left.ItemSpec,
                            StringComparison.OrdinalIgnoreCase))
                            && !result.Exists(r => string.Equals(r.ItemSpec, left.ItemSpec,
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(left));
                        }
                    }
                    break;
                case "RelativeComplement":
                    foreach (var left in Left)
                    {
                        if (!Right.Any(r => string.Equals(r.ItemSpec, left.ItemSpec,
                            StringComparison.OrdinalIgnoreCase))
                            && !result.Exists(r => string.Equals(r.ItemSpec, left.ItemSpec,
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(left));
                        }
                    }
                    break;
                case "SymmetricDifference":
                    foreach (var left in Left)
                    {
                        if (!Right.Any(r => string.Equals(r.ItemSpec, left.ItemSpec,
                            StringComparison.OrdinalIgnoreCase))
                            && !result.Exists(r => string.Equals(r.ItemSpec, left.ItemSpec,
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(left));
                        }
                    }
                    foreach (var right in Right)
                    {
                        if (!Left.Any(l => string.Equals(l.ItemSpec, right.ItemSpec,
                            StringComparison.OrdinalIgnoreCase))
                            && !result.Exists(r => string.Equals(r.ItemSpec, right.ItemSpec,
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(right));
                        }
                    }
                    break;
                default:
                    Log.LogError($"Operation '{nameof(Operation)}' is not recognized.");
                    return;
            }

            Result = result.ToArray();
        }

        #endregion
    }
}