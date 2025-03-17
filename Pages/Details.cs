using BananaWatch.Pages;
using GorillaNetworking;
using Photon.Pun;
using System;

namespace BananaWatch.Pages
{
    public class DetailsPage : BananaWatchPage
    {
        public override string MMHeader => "Details";
        public override bool MMDisplay => true;

        public static string page;
        string pagetemp;

        public override string RenderScreenContent()
        {
            string gameVersion = !string.IsNullOrEmpty(GorillaComputer.instance.version) ? GorillaComputer.instance.version : "Unavailable";
            string playerName = PhotonNetwork.LocalPlayer != null && !string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? PhotonNetwork.LocalPlayer.NickName : "Unavailable";
            string roomName = PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "Not in room";
            string playersOnline = PhotonNetwork.CountOfPlayers > 0 ? PhotonNetwork.CountOfPlayers.ToString() : "Unavailable";
            string currentTime = DateTime.Now.ToString("ddd MMM d HH:mm:ss yyyy");

            pagetemp = $"<color=#ffff00>==</color> Details <color=#ffff00>==</color>\r\n" +
                    "<size=43> Refresh by reopening this menu </size>\r\n\r\n" +
                    $"<size=70> Current Time: \r\n {currentTime}\r\n</size>\r\n" +
                    $"<size=70> Current Game Version:\r\n live{gameVersion}</size>\r\n" +
                    $"<size=70> Current Name:\r\n {playerName}</size>\r\n\r\n" +
                    $"<size=70> {roomName}</size>\r\n" +
                    $"<size=70> Players Online:\r\n {playersOnline}</size>\r\n";

            page = pagetemp;

            return page;
        }


        public override void ButtonPressed(BananaWatchButton ButtonType)
        {
            switch (ButtonType)
            {
                case BananaWatchButton.Back:
                    NavigateToMM();
                    break;
            }
        }
    }
}
