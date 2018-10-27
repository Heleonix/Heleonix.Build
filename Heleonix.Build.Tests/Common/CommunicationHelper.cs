// <copyright file="CommunicationHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality for working with requests, responses, emulated services etc.
    /// </summary>
    public static class CommunicationHelper
    {
        /// <summary>
        /// Launches an http server.
        /// </summary>
        /// <param name="url">A url to launch server on.</param>
        /// <param name="isSuccess">Determines if request shoul succeed.</param>
        /// <param name="success">A success response.</param>
        /// <param name="fail">A fail response.</param>
        /// <returns>A task to manage launched server.</returns>
#pragma warning disable CA1054 // Uri parameters should not be strings
        public static Task LaunchHttpServer(
            string url,
            Predicate<HttpListenerRequest> isSuccess,
            (string ContentType, string Content, HttpStatusCode StatusCode) success,
            (string ContentType, string Content, HttpStatusCode StatusCode) fail)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            return Task.Run(() =>
            {
                using (var listener = new HttpListener())
                {
                    listener.Prefixes.Add(url);

                    listener.Start();

                    var context = listener.GetContext();

                    var response = context.Response;

                    var(contentType, content, statusCode) =
                        isSuccess != null && isSuccess(context.Request)
                            ? success
                            : fail;

                    var buffer = Encoding.UTF8.GetBytes(content);

                    response.StatusCode = (int)statusCode;
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentType = contentType;
                    response.ContentLength64 = buffer.Length;
                    var output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                    Thread.Sleep(1 * 1000);
                }
            });
        }
    }
}
