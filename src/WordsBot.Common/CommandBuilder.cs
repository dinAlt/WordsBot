using System;
using System.Collections.Generic;

namespace WordsBot.Common
{
  public class CommandBuilder : ICommandBuilder
  {
    readonly char _delimiter;
    readonly string[] _prefix;
    readonly List<string> _parts = new();

    public CommandBuilder(char delimiter, params string[] prefix)
    {
      _delimiter = delimiter;
      _prefix = prefix;
    }

    public ICommandBuilder Add(params string[] args)
    {
      _parts.AddRange(args);
      return this;
    }

    public ICommandBuilder Clear()
    {
      _parts.Clear();
      return this;
    }

    public string Build() => string.Join(_delimiter, _prefix) + _delimiter +
      string.Join(_delimiter, _parts);
  }
}