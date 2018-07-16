namespace APICalls.Entities.Interfaces
{
    public interface IAPIParallelProgress
    {
         float Percentage { get; set; }
         int Tasks { get; set; }
         string Url { get; set; }
    }
}
