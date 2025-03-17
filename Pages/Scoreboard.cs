using BananaWatch.Pages;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BananaWatch.Pages
{
    public class ScoreboardPage : BananaWatchPage
    {
        public override string MMHeader => "Scoreboard";
        public override bool MMDisplay => true;

        private static SelectionHandler selectionHandler = new SelectionHandler();
        private List<Photon.Realtime.Player> players = new List<Photon.Realtime.Player>();

        public override void PageOpened()
        {
            selectionHandler.CurrentIndex = 0;
            UpdatePlayerList();
            InvokeRepeating("ForceUpdate", 0, 0.5f);
        }

        private void UpdatePlayerList()
        {
            players.Clear();
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
            {
                players.AddRange(PhotonNetwork.PlayerList);
            }
            selectionHandler.MaxIndex = players.Count - 1;
        }

        public override string RenderScreenContent()
        {
            UpdatePlayerList();

            string content = "<color=#ffff00>==</color> Scoreboard <color=#ffff00>==</color>\r\n";

            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
            {
                content += "\r\n<size=60>    You are not in a room! \r\n    Please enter a room for \r\n    the scoreboard to work \r\n    properly\r\n</size>";
            }
            else
            {
                content += $"\r\n<size=60>Room ID: - {PhotonNetwork.CurrentRoom.Name} - </size>\r\n\r\n";

                for (int i = 0; i < players.Count; i++)
                {
                    string playerDisplay = GetPlayerDisplay(players[i]);
                    content += SelectionArrow(i, playerDisplay) + "\n";
                }
            }
            return content;
        }

        private string GetPlayerDisplay(Photon.Realtime.Player player)
        {
            string nameColor = "#FFFFFF";

            if (GorillaGameManager.instance is GorillaTagManager gtm)
            {
                bool isTagged = gtm.currentInfectedArray.Any(taggedId => taggedId == player.ActorNumber);
                if (isTagged)
                {
                    nameColor = "#FF5D5D";
                }
            }

            string colorHex = "#FFFFFF"; 
            if (GorillaParent.instance.vrrigDict.TryGetValue(player, out VRRig rig))
            {
                Color playerColor = rig.materialsToChangeTo[0].color;
                colorHex = ColorUtility.ToHtmlStringRGB(playerColor);
            }

            return $"<color={nameColor}>{player.NickName}</color> <color=#{colorHex}>#</color>";
        }

        public string SelectionArrow(int index, string text)
        {
            return selectionHandler.CurrentIndex == index ? $" <color=#C16F66>></color> {text}" : $"   {text}";
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
                case BananaWatchButton.Enter:
                    OpenPlayerDetails();
                    break;
                case BananaWatchButton.Back:
                    NavigateToMM();
                    break;
            }
        }

        private void OpenPlayerDetails()
        {
            if (players.Count == 0) return;

            Photon.Realtime.Player selectedPlayer = players[selectionHandler.CurrentIndex];
            PlayerDetailsPage.SelectedPlayer = selectedPlayer;
            BananaWatch.Instance.NavigateToPage(typeof(PlayerDetailsPage));
        }

        private void ForceUpdate()
        {
            BananaWatch.Instance.RefreshScreen();
        }
    }
}
