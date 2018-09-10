using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleSort
    {
        public int SortColumnIndex { get; set; } = -1;
        public ConsoleSortType SortType { get; set; } = Enums.ConsoleSortType.ASCENDING;
        public ConsoleSortDataType DataType { get; set; } = ConsoleSortDataType.STRING;
    }
}
