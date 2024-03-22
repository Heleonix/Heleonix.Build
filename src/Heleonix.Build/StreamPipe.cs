// <copyright file="StreamPipe.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build;

internal sealed class StreamPipe
{
    private const int BufferSize = 2048;

    private readonly TextReader source;

    private readonly TextWriter destination;

    public StreamPipe(TextReader source, TextWriter destination)
    {
        this.source = source;
        this.destination = destination;
    }

    public void Connect()
    {
        System.Threading.Tasks.Task.Run(
            async () =>
            {
                var buffer = new char[StreamPipe.BufferSize];

                while (true)
                {
                    var count = await this.source.ReadAsync(buffer, 0, BufferSize);

                    if (count <= 0)
                    {
                        break;
                    }

                    await this.destination.WriteAsync(buffer, 0, count);
                    await this.destination.FlushAsync();
                }
            });
    }
}
