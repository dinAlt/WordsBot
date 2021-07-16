using System.Collections.Generic;

namespace WordsBot.Common.Models
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

    public int GameSessionId { get; set; }
    public long UserId { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string CurrentWord { get; set; } = string.Empty;
    public List<string> CurrentWordTranslations { get; } = new List<string>();
    public int CurrentWordNumber { get; set; }
    public int TotalWordsCount { get; set; }
    public int FailsCount { get; set; }
    public int GiveUpsCount { get; set; }
    public int SuccessCount { get; set; }
    public GameState State { get; set; }

    public void Reset()
    {
      CurrentWord = string.Empty;
      CurrentWordTranslations.Clear();
      CurrentWordNumber = 0;
      TotalWordsCount = 0;
      FailsCount = 0;
      SuccessCount = 0;
      GiveUpsCount = 0;
      State = GameState.Undefined;
    }
  }
}