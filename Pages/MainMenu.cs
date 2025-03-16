using BananaWatch;
using BananaWatch.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BananaWatch.Pages
{
    public class MainMenu : BananaWatchPage
    {
        public override string Title => "MainMenu";

        public override bool PublicPage => false;

        static SelectionHandler selectionHandler = new SelectionHandler();
        public static Dictionary<int, List<BananaWatchPage>> screenPageDict = new Dictionary<int, List<BananaWatchPage>>();
        public readonly BananaWatchButton[] secretCode = new BananaWatchButton[] { BananaWatchButton.Up, BananaWatchButton.Up, BananaWatchButton.Down, BananaWatchButton.Down, BananaWatchButton.Left, BananaWatchButton.Right, BananaWatchButton.Left, BananaWatchButton.Right, BananaWatchButton.Back, BananaWatchButton.Enter };
        List<BananaWatchButton> lastPressedButtons = new List<BananaWatchButton>();
        const int maxPageItemCount = 8;
        private static int CurrentPage;
        public static int currentPage
        {
            get
            {
                return CurrentPage + 1;
            }
            set
            {
                CurrentPage = value;
            }
        }

        public override void PageOpened()
        {
            lastPressedButtons.Clear();
        }
        public override void PostModLoaded()
        {
            screenPageDict.Clear(); 

            int pageIndex = 1; // start from 1, since currentPage also starts from 1
            int indexOffset = 0;

            for (int i = 0; i < BananaWatch.Instance.watchPages.Count; i++)
            {
                if (!BananaWatch.Instance.watchPages[i].PublicPage)
                {
                    indexOffset++;
                    continue;
                }

                if ((i - indexOffset) % maxPageItemCount == 0)
                {
                    if (!screenPageDict.ContainsKey(pageIndex))
                        screenPageDict.Add(pageIndex, new List<BananaWatchPage>());
                    pageIndex++;
                }

                screenPageDict[pageIndex - 1].Add(BananaWatch.Instance.watchPages[i]);
            }

            if (!screenPageDict.ContainsKey(currentPage))
            {
                currentPage = 1;
            }

            selectionHandler.maxIndex = screenPageDict[currentPage].Count - 1;
            selectionHandler.currentIndex = 0;
        }

        public override string RenderScreenContent()
        {
            Text screenText = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane/Canvas/Text")?.GetComponent<Text>();
            if (screenText != null)
            {
                string header = "<color=#ffff00>========================</color>\r\n       <color=#00ff00>MonkeWatch</color>\r\n     by: <color=#ff0000>RedBrumbler</color>\r\n<color=#ffff00>========================</color>\r\n";

                screenText.text = header;

                string content = header;

                if (!screenPageDict.ContainsKey(currentPage))
                {
                    return "404 Not Found";
                }

                var screensPage = screenPageDict[currentPage];

                for (int i = 0; i < screensPage.Count; i++)
                {
                    content += selectionHandler.SelectionArrow(i, $"{screensPage[i].Title}") + "\n";
                }

                if (screenPageDict.Count > 1)
                {
                    content += $"{currentPage}/{screenPageDict.Count}\n";
                }

                return content;
            }
            else
            {
                return string.Empty;
            }
        }

        public override void ButtonPressed(BananaWatchButton buttonType)
        {
            switch(buttonType)
            {
                case BananaWatchButton.Up:
                    selectionHandler.MoveSelectionUp();
                    break;
                case BananaWatchButton.Down:
                    selectionHandler.MoveSelectionDown();
                    break;
                case BananaWatchButton.Left:
                    if (screenPageDict.Count <= 1)
                    {
                        return;
                    }

                    int previousPage = currentPage - 1;

                    if (!screenPageDict.ContainsKey(previousPage))
                    {
                        return;
                    }

                    currentPage = previousPage;
                    selectionHandler.maxIndex = screenPageDict[currentPage].Count - 1;
                    selectionHandler.currentIndex = 0;
                    break;
                case BananaWatchButton.Right:
                    if (screenPageDict.Count <= 1)
                    {
                        return;
                    }

                    int nextPage = currentPage + 1;

                    if (!screenPageDict.ContainsKey(nextPage))
                    {
                        return;
                    }

                    currentPage = nextPage;
                    selectionHandler.maxIndex = screenPageDict[currentPage].Count - 1;
                    selectionHandler.currentIndex = 0;
                    break;
                case BananaWatchButton.Enter:
                    var screen = screenPageDict[currentPage][selectionHandler.currentIndex];
                    BananaWatch.Instance.NavigateToPage(screen.GetType());
                    break;
            }
        }
    }
}