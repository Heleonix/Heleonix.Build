if(args.Any(a => a.Contains("ERROR")))
{
    Console.Error.Write("ERROR details");

    Console.Write("ERROR simulated");

    return 1;
}

Console.Write(string.Join(' ', args));

return 0;