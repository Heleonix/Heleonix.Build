// <copyright file="CommunicationHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Provides functionality for working with requests, responses, emulated services etc.
/// </summary>
public static class CommunicationHelper
{
    /// <summary>
    /// Launches an http server.
    /// </summary>
    /// <param name="mocks">An array of mocks to emulate responses for each url.</param>
    /// <returns>A task to manage launched server.</returns>
    public static HttpListener LaunchHttpServer(params (
        string Url,
        Predicate<HttpListenerRequest> IsSuccess,
        (string Content, HttpStatusCode StatusCode) OnSuccess,
        (string Content, HttpStatusCode StatusCode) OnFail)[] mocks)
    {
        var listener = new HttpListener();

        Task.Run(() =>
        {
            foreach (var (url, _, _, _) in mocks)
            {
                listener.Prefixes.Add(url);
            }

            listener.Start();

            while (true)
            {
                var context = listener.GetContext();

                var request = context.Request;
                var response = context.Response;

                var (_, isSuccess, onSuccess, onFail) = mocks.Single(m => m.Url.Contains(request.Url.AbsolutePath));

                var (content, statusCode) =
                isSuccess(context.Request) ? onSuccess : onFail;

                var buffer = Encoding.UTF8.GetBytes(content);

                response.StatusCode = (int)statusCode;
                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;

                var output = response.OutputStream;

                output.Write(buffer, 0, buffer.Length);

                output.Close();
            }
        });

        return listener;
    }
}
