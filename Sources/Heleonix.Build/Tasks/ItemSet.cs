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

using System;
using System.Collections.Generic;
using System.Linq;
using Heleonix.Build.Properties;
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
        /// The name of the metadata to handle items by. Default is "ItemSpec".
        /// </summary>
        public string MetadataName { get; set; }

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
            var left = Left ?? new ITaskItem[0];
            var right = Right ?? new ITaskItem[0];

            var metadataName = string.IsNullOrEmpty(MetadataName) ? "Identity" : MetadataName;

            var result = new List<ITaskItem>();

            switch (Operation)
            {
                case "Union":
                    result.AddRange(left.Select(l => new TaskItem(l)));
                    foreach (var r in right)
                    {
                        if (!result.Exists(res => string.Equals(res.GetMetadata(metadataName),
                            r.GetMetadata(metadataName), StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(r);
                        }
                    }
                    break;

                case "Intersection":
                    foreach (var l in left)
                    {
                        if (right.Any(r => string.Equals(r.GetMetadata(metadataName), l.GetMetadata(metadataName),
                                StringComparison.OrdinalIgnoreCase))
                            &&
                            !result.Exists(r => string.Equals(r.GetMetadata(metadataName), l.GetMetadata(metadataName),
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(l));
                        }
                    }
                    break;
                case "RelativeComplement":
                    foreach (var l in left)
                    {
                        if (!right.Any(r => string.Equals(r.GetMetadata(metadataName), l.GetMetadata(metadataName),
                                StringComparison.OrdinalIgnoreCase))
                            &&
                            !result.Exists(r => string.Equals(r.GetMetadata(metadataName), l.GetMetadata(metadataName),
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(l));
                        }
                    }
                    break;
                case "SymmetricDifference":
                    foreach (var l in left)
                    {
                        if (!right.Any(r => string.Equals(r.GetMetadata(metadataName), l.GetMetadata(metadataName),
                                StringComparison.OrdinalIgnoreCase))
                            && !result.Exists(r => string.Equals(r.GetMetadata(metadataName),
                                l.GetMetadata(metadataName), StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(l));
                        }
                    }
                    foreach (var r in right)
                    {
                        if (!left.Any(l => string.Equals(l.GetMetadata(metadataName), r.GetMetadata(metadataName),
                                StringComparison.OrdinalIgnoreCase))
                            && !result.Exists(res => string.Equals(res.GetMetadata(metadataName),
                                r.GetMetadata(metadataName), StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new TaskItem(r));
                        }
                    }
                    break;
                default:
                    Log.LogError(Resources.OperationIsNotRecognized, nameof(Operation));
                    return;
            }

            Result = result.ToArray();
        }

        #endregion
    }
}