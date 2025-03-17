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
        public override string MMHeader => "MainMenu";

        public override bool MMDisplay => false;

        static SelectionHandler selectionHandler = new SelectionHandler();
        public static Dictionary<int, List<BananaWatchPage>> ScreenPageDict = new Dictionary<int, List<BananaWatchPage>>();
        public readonly BananaWatchButton[] secretCode = new BananaWatchButton[] { BananaWatchButton.Up, BananaWatchButton.Up, BananaWatchButton.Down, BananaWatchButton.Down, BananaWatchButton.Left, BananaWatchButton.Right, BananaWatchButton.Left, BananaWatchButton.Right, BananaWatchButton.Back, BananaWatchButton.Enter };
        List<BananaWatchButton> lastPressedButtons = new List<BananaWatchButton>();
        const int maxPageItemCount = 8;
        private static int CurrentPage;
        public static int _CurrentPage
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
            ScreenPageDict.Clear(); 

            int pageIndex = 1; // start from 1, since CurrentPage also starts from 1
            int indexOffset = 0;

            for (int i = 0; i < BananaWatch.Instance.BananaWatchPages.Count; i++)
            {
                if (!BananaWatch.Instance.BananaWatchPages[i].MMDisplay)
                {
                    indexOffset++;
                    continue;
                }

                if ((i - indexOffset) % maxPageItemCount == 0)
                {
                    if (!ScreenPageDict.ContainsKey(pageIndex))
                        ScreenPageDict.Add(pageIndex, new List<BananaWatchPage>());
                    pageIndex++;
                }

                ScreenPageDict[pageIndex - 1].Add(BananaWatch.Instance.BananaWatchPages[i]);
            }

            if (!ScreenPageDict.ContainsKey(CurrentPage))
            {
                CurrentPage = 1;
            }

            selectionHandler.MaxIndex = ScreenPageDict[CurrentPage].Count - 1;
            selectionHandler.CurrentIndex = 0;
        }

        public override string RenderScreenContent()
        {
            Text _ScreenText = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane/Canvas/Text")?.GetComponent<Text>();
            if (_ScreenText != null)
            {
                string header = "<color=#ffff00>========================</color>\r\n       <color=#00ff00>MonkeWatch</color>\r\n     by: <color=#ff0000>RedBrumbler</color>\r\n<color=#ffff00>========================</color>\r\n";

                _ScreenText.text = header;

                string content = header;

                if (!ScreenPageDict.ContainsKey(CurrentPage))
                {
                    return "404 Not Found";
                }

                var screensPage = ScreenPageDict[CurrentPage];

                for (int i = 0; i < screensPage.Count; i++)
                {
                    content += selectionHandler.SelectionArrow(i, $"{screensPage[i].MMHeader}") + "\n";
                }

                if (ScreenPageDict.Count > 1)
                {
                    content += $"{CurrentPage}/{ScreenPageDict.Count}\n";
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
            switch(ButtonType)
            {
                case BananaWatchButton.Up:
                    selectionHandler.MoveSelectionUp();
                    break;
                case BananaWatchButton.Down:
                    selectionHandler.MoveSelectionDown();
                    break;
                case BananaWatchButton.Left:
                    if (ScreenPageDict.Count <= 1)
                    {
                        return;
                    }

                    int previousPage = CurrentPage - 1;

                    if (!ScreenPageDict.ContainsKey(previousPage))
                    {
                        return;
                    }

                    CurrentPage = previousPage;
                    selectionHandler.MaxIndex = ScreenPageDict[CurrentPage].Count - 1;
                    selectionHandler.CurrentIndex = 0;
                    break;
                case BananaWatchButton.Right:
                    if (ScreenPageDict.Count <= 1)
                    {
                        return;
                    }

                    int nextPage = CurrentPage + 1;

                    if (!ScreenPageDict.ContainsKey(nextPage))
                    {
                        return;
                    }

                    CurrentPage = nextPage;
                    selectionHandler.MaxIndex = ScreenPageDict[CurrentPage].Count - 1;
                    selectionHandler.CurrentIndex = 0;
                    break;
                case BananaWatchButton.Enter:
                    var screen = ScreenPageDict[CurrentPage][selectionHandler.CurrentIndex];
                    BananaWatch.Instance.NavigateToPage(screen.GetType());
                    break;
            }
        }
    }
}