using System;
using System.Collections.Generic;

namespace WordsBot.Common
{
  public class CommandBuilder : ICommandBuilder
  {
    readonly char _delimiter;
    readonly List<string> _parts;

    public CommandBuilder(char delimiter, params string[] prefix)
    {
      _delimiter = delimiter;
      _parts = new List<string>(prefix);
    }

    public ICommandBuilder Add(params string[] args)
    {
      _parts.AddRange(args);
      return this;
    }

    public string Build() => string.Join(_delimiter, _parts);
  }
}