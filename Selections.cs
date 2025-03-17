namespace BananaWatch.Pages
{
    public class SelectionHandler
    {
        public int MaxIndex { get; set; } = 0;
        public int CurrentIndex { get; set; }
        private string SelectionText => " <color=#FD0000>></color> ";

        public string GetSelectedText(int index, string text)
            => CurrentIndex == index ? SelectionText + text : text;

        public string SelectionArrow(int index, string text)
            => CurrentIndex == index ? $" <color=#FD0000>></color> {text}" : $"  {text}";

        public bool IsSelectionIndex(int index)
            => CurrentIndex == index;

        public int CheckSelection()
        {
            if (CurrentIndex < 0)
                CurrentIndex = Configuration.WrapSelectArrow.Value ? MaxIndex : 0;

            if (CurrentIndex > MaxIndex)
                CurrentIndex = Configuration.WrapSelectArrow.Value ? 0 : MaxIndex;

            return CurrentIndex;
        }

        public int MoveSelectionUp()
        {
            CurrentIndex--;
            return CheckSelection();
        }

        public int MoveSelectionDown()
        {
            CurrentIndex++;
            return CheckSelection();
        }
    }
}
