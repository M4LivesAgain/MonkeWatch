namespace BananaWatch.Pages
{
    public class SelectionHandler
    {
        public int maxIndex { get; set; } = 0;
        public int currentIndex { get; set; }
        private string SelectionText => " <color=#FD0000>></color> ";

        public string GetSelectedText(int index, string text)
            => currentIndex == index ? SelectionText + text : text;

        public string SelectionArrow(int index, string text)
            => currentIndex == index ? $" <color=#FD0000>></color> {text}" : $"  {text}";

        public bool IsSelectionIndex(int index)
            => currentIndex == index;

        public int CheckSelection()
        {
            if (currentIndex < 0)
                currentIndex = Configuration.WrapSelectArrow.Value ? maxIndex : 0;

            if (currentIndex > maxIndex)
                currentIndex = Configuration.WrapSelectArrow.Value ? 0 : maxIndex;

            return currentIndex;
        }

        public int MoveSelectionUp()
        {
            currentIndex--;
            return CheckSelection();
        }

        public int MoveSelectionDown()
        {
            currentIndex++;
            return CheckSelection();
        }
    }
}
