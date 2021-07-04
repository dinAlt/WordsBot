using System.Collections.Generic;

namespace WordsBot.Common
{
  public class GameSession
  {
    public enum GameState
    {
      Undefined,
      Running,
      Ended,
      Paused,
      WaitingCount,
    }

    public int SessionId { get; set; }
    public long UserId { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string CurrentWord { get; set; }
    public List<string> CurrentWordTranslations { get; } = new List<string>();
    public int CurrentWordNumber { get; set; }
    public int TotalWordsCount { get; set; }
    public int FailsCount { get; set; }
    public GameState State { get; set; }

    public void Reset()
    {
      CurrentWord = string.Empty;
      CurrentWordTranslations.Clear();
      CurrentWordNumber = 0;
      TotalWordsCount = 0;
      FailsCount = 0;
      State = GameState.Undefined;
    }
  }
}