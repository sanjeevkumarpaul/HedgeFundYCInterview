
namespace TagUtility.Entities
{
    public interface ITagResult
    {
        string FilePath { get; set; }

        void Action(TagOptions options);
    }
}
