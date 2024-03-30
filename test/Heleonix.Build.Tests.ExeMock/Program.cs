// <copyright file="Program.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

if (Array.Exists(args, a => a.Contains("ERROR")))
{
    Console.Error.Write("ERROR details");

    Console.Write("ERROR simulated");

    return 1;
}

Console.Write(string.Join(' ', args));

return 0;