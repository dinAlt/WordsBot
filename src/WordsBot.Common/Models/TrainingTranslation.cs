namespace WordsBot.Common.Models
{
  public class TrainingTranslation
  {
    public TrainingTranslation(long userId, string word)
    {
      UserId = userId;
      Word = word;
    }

    public long UserId { get; set; }
    public string Word { get; set; }
  }
}