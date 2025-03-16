using BananaWatch;
using BepInEx;
using System;
using System.Text;

namespace BananaWatch.Pages
{
    public class SelectionHandler
    {
        public int maxIndex = 0;
        public int currentIndex;
        public string selectionText = " <color=#FD0000>></color> ";

        public string GetSelectedText(int index, string text)
        {
            return currentIndex == index ? selectionText + text : text;
        }
        public string SelectionArrow(int index, string text)
        {
            return currentIndex == index ? $" <color=#FD0000>></color> {text}" : $"  {text}";
        }
        public bool IsSelectionIndex(int index)
        {
            return currentIndex == index;
        }

        public int CheckSelection()
        {
            if (currentIndex < 0)
            {
                currentIndex = Configuration.WrapSelectArrow.Value ? maxIndex : 0;
            }
            if (currentIndex > maxIndex)
            {
                currentIndex = Configuration.WrapSelectArrow.Value ? 0 : maxIndex;
            }
            return currentIndex;
        }
        public int MoveSelectionUp()
        {
            currentIndex--;
            CheckSelection();
            return currentIndex;
        }
        public int MoveSelectionDown()
        {
            currentIndex++;
            CheckSelection();
            return currentIndex;
        }
    }
}