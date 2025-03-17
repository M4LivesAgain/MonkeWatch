using BananaWatch;
using BananaWatch.Pages;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BananaWatch.Pages
{
    public class MainMenu : BananaWatchPage
    {
        static SelectionHandler selectionHandler = new SelectionHandler();
        public static Dictionary<int, List<BananaWatchPage>> PageDictionary = new Dictionary<int, List<BananaWatchPage>>();
        const int MaximumPP = 12;
        private static int CurrentPage;
        public static int _currentPage
        {
            get { return CurrentPage; }
            set { CurrentPage = value; }
        }
        public override string MMHeader => PluginInfo.MM;
        public override bool MMDisplay => false;
        public override void PostModLoaded()
        {
            PageDictionary.Clear();
            int pageIndex = 0;
            int IOffset = 0;

            for (int i = 0; i < BananaWatch.Instance.watchPages.Count; i++)
            {
                if (!BananaWatch.Instance.watchPages[i].MMDisplay)
                {
                    IOffset++;
                    continue;
                }

                if ((i - IOffset) % MaximumPP == 0)
                {
                    if (!PageDictionary.ContainsKey(pageIndex))
                        PageDictionary.Add(pageIndex, new List<BananaWatchPage>());
                    pageIndex++;
                }
                PageDictionary[pageIndex - 1].Add(BananaWatch.Instance.watchPages[i]);
            }

            if (!PageDictionary.ContainsKey(_currentPage))
            {
                _currentPage = PageDictionary.Count > 0 ? 0 : -1;
            }

            if (_currentPage >= 0)
            {
                selectionHandler.maxIndex = PageDictionary[_currentPage].Count - 1;
                selectionHandler.currentIndex = 0;
            }
        }

        public override string RenderScreenContent()
        {
            Text screenText = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane/Canvas/Text")?.GetComponent<Text>();
            if (screenText != null)
            {
                string header = "<color=#ffff00>========================</color>\r\n       <color=#00ff00>MonkeWatch</color>\r\n     by: <color=#ff0000>RedBrumbler</color>\r\n<color=#ffff00>========================</color>\r\n";
                screenText.text = header;

                string content = header;

                if (!PageDictionary.ContainsKey(_currentPage))
                {
                    return "404 Not Found";
                }

                var screensPage = PageDictionary[_currentPage];
                for (int i = 0; i < screensPage.Count; i++)
                {
                    content += selectionHandler.SelectionArrow(i, $"{screensPage[i].MMHeader}") + "\n";
                }

                if (PageDictionary.Count > 1)
                {
                    content += $"{_currentPage}/{PageDictionary.Count - 1}\n";
                }

                return content;
            }
            else
            {
                return string.Empty;
            }
        }

        public override void ButtonPressed(BananaWatchButton ButtonType)
        {
            switch (ButtonType)
            {
                case BananaWatchButton.Up:
                    selectionHandler.MoveSelectionUp();
                    break;

                case BananaWatchButton.Down:
                    selectionHandler.MoveSelectionDown();
                    break;

                case BananaWatchButton.Left:
                    if (PageDictionary.Count <= 1) return;
                    _currentPage = (_currentPage - 1 + PageDictionary.Count) % PageDictionary.Count;
                    selectionHandler.maxIndex = PageDictionary[_currentPage].Count - 1;
                    selectionHandler.currentIndex = 0;
                    break;

                case BananaWatchButton.Right:
                    if (PageDictionary.Count <= 1) return;
                    _currentPage = (_currentPage + 1) % PageDictionary.Count;
                    selectionHandler.maxIndex = PageDictionary[_currentPage].Count - 1;
                    selectionHandler.currentIndex = 0;
                    break;

                case BananaWatchButton.Enter:
                    if (PageDictionary.ContainsKey(_currentPage) && selectionHandler.currentIndex < PageDictionary[_currentPage].Count)
                    {
                        var screen = PageDictionary[_currentPage][selectionHandler.currentIndex];
                        BananaWatch.Instance.NavigateToPage(screen.GetType());
                    }
                    break;
            }
        }
    }
}
