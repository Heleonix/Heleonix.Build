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
using Heleonix.Build.Properties;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Combines items: adds the right list of items as metadata into the left list.
    /// </summary>
    public class ItemCombine : BaseTask
    {
        #region Properties

        /// <summary>
        /// The operation on items.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Pairing</term></item>
        /// <item><term>CrossProduct</term></item>
        /// </list>
        /// </remarks>
        [Required]
        public string Operation { get; set; }

        /// <summary>
        /// The name of the target metadata to insert other items to.
        /// </summary>
        [Required]
        public string TargetMetadataName { get; set; }

        /// <summary>
        /// The name of the source metadata to get values from. Default is "Identity".
        /// </summary>
        public string SourceMetadataName { get; set; }

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
        /// Combines items: adds the right list of items as metadata into the left list.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var left = Left ?? new ITaskItem[0];
            var right = Right ?? new ITaskItem[0];

            var sourceMetadataName = string.IsNullOrEmpty(SourceMetadataName) ? "Identity" : SourceMetadataName;

            var result = new List<ITaskItem>();

            switch (Operation)
            {
                case "Pairing":
                    for (var i = 0; i < right.Length; i++)
                    {
                        if (i < left.Length)
                        {
                            var res = new TaskItem(left[i]);
                            res.SetMetadata(TargetMetadataName, right[i].GetMetadata(sourceMetadataName));
                            result.Add(res);
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case "CrossProduct":
                    foreach (var l in left)
                    {
                        foreach (var r in right)
                        {
                            var res = new TaskItem(l);
                            res.SetMetadata(TargetMetadataName, r.GetMetadata(sourceMetadataName));
                            result.Add(res);
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