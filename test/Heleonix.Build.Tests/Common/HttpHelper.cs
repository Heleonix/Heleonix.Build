// <copyright file="HttpHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public static class HttpHelper
{
    public static HttpListener LaunchHttpServer(params (
        string Url,
        Func<HttpListenerRequest, (string Content, HttpStatusCode StatusCode)>)[] mocks)
    {
        var listener = new HttpListener();

        Task.Run(() =>
        {
            foreach (var (url, _) in mocks)
            {
                listener.Prefixes.Add(url);
            }

            listener.Start();

            while (true)
            {
                var context = listener.GetContext();

                var request = context.Request;
                var response = context.Response;

                var (_, handler) = mocks.Single(m => m.Url.Contains(request.Url.AbsolutePath));

                var (content, statusCode) = handler(context.Request);

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
