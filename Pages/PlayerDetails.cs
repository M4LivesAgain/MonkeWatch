using BananaWatch.Pages;
using GorillaNetworking;
using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace BananaWatch.Pages
{
    public class PlayerDetailsPage : BananaWatchPage
    {
        public override string Title => "Player";
        public override bool PublicPage => false;

        public static Photon.Realtime.Player SelectedPlayer;
        private int selectedOption = 0;
        private bool isMuted = false;

        public override string RenderScreenContent()
        {
            if (SelectedPlayer == null || !PhotonNetwork.PlayerList.Contains(SelectedPlayer))
            {
                BananaWatch.Instance.NavigateToPage(typeof(ScoreboardPage));
                return "<color=#ffff00>==</color> Player <color=#ffff00>==</color>\n\n<size=60> No player selected </size>";
            }
            if (SelectedPlayer == PhotonNetwork.LocalPlayer)
            {
                // does not really do anything other than look, but who actually cares about voice chat settings?
                string voiceChatStatus = PlayerPrefs.GetString("voiceChatOn", "false") == "true" ? "On" : "Off";
                string voiceChatColor = voiceChatStatus == "On" ? "#00FF00" : "#FF0000";
                string pttType = PlayerPrefs.GetString("pttType", "ALL CHAT");

                return $@"<color=#ffff00>==</color> Player <color=#ffff00>==</color>

This is You!,
you can't mute or 
report yourself

Press enter to toggle:
Voice Chat <color={voiceChatColor}>{voiceChatStatus}</color>

Use < / > to change:
< {pttType} >";
            }

            string color9 = "0 0 0";
            string color255 = "0 0 0";
            if (GorillaParent.instance.vrrigDict.TryGetValue(SelectedPlayer, out VRRig rig))
            {
                Color playerColor = rig.materialsToChangeTo[0].color;

                int r = Mathf.Min(Mathf.FloorToInt(playerColor.r * 10), 9);
                int g = Mathf.Min(Mathf.FloorToInt(playerColor.g * 10), 9);
                int b = Mathf.Min(Mathf.FloorToInt(playerColor.b * 10), 9);

                color9 = $"{r} {g} {b}";
                color255 = $"{(int)(playerColor.r * 255)} {(int)(playerColor.g * 255)} {(int)(playerColor.b * 255)}";
            }

            string muteText = isMuted ? "Unmute" : "Mute";
            string muteArrow = selectedOption == 0 ? "<color=#C16F66>></color> " : "  ";
            string reportArrow = selectedOption == 1 ? "<color=#C16F66>></color> " : "  ";

            return $@"<color=#ffff00>==</color> Player <color=#ffff00>==</color>

Player: {SelectedPlayer.NickName}

{muteArrow}{muteText}
{reportArrow}Report

Color (0, 9):
{color9}

Color (0, 255):
{color255}";
        }

        public override void ButtonPressed(BananaWatchButton buttonType)
        {
            if (SelectedPlayer == null || !PhotonNetwork.PlayerList.Contains(SelectedPlayer))
            {
                BananaWatch.Instance.NavigateToPage(typeof(ScoreboardPage));
                return;
            }

            switch (buttonType)
            {
                case BananaWatchButton.Up:
                    selectedOption = Mathf.Max(0, selectedOption - 1);
                    break;
                case BananaWatchButton.Down:
                    selectedOption = Mathf.Min(1, selectedOption + 1);
                    break;
                case BananaWatchButton.Enter:
                    HandleSelection();
                    break;
                case BananaWatchButton.Back:
                    BananaWatch.Instance.NavigateToPage(typeof(ScoreboardPage));
                    break;
            }
        }
        public static void MutePlayer()
        {
            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.linePlayer.UserId == SelectedPlayer.UserId)
                {
                    line.PressButton(true, GorillaPlayerLineButton.ButtonType.Mute);
                    break;
                }
            }
        }

        public static void UnmutePlayer()
        {
            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.linePlayer.UserId == SelectedPlayer.UserId)
                {
                    line.PressButton(false, GorillaPlayerLineButton.ButtonType.Mute);
                    break;
                }
            }
        }


        private void HandleSelection()
        {
            if (SelectedPlayer == null || !PhotonNetwork.PlayerList.Contains(SelectedPlayer))
            {
                BananaWatch.Instance.NavigateToPage(typeof(ScoreboardPage));
                return;
            }

            if (SelectedPlayer == PhotonNetwork.LocalPlayer)
            {
                return;
            }

            if (selectedOption == 0)
            {
                isMuted = !isMuted;
                if (isMuted)
                {
                    MutePlayer();
                }
                else
                {
                    UnmutePlayer();
                }

                int muteValue = isMuted ? 1 : 0;
                UpdateMuteButtonUI(muteValue);
                PlayerPrefs.SetInt(SelectedPlayer.UserId, muteValue);
                PlayerPrefs.Save();

                BananaWatch.Instance.UpdateScreen();
            }
            else if (selectedOption == 1)
            {
                NavigateToPage(typeof(ReportPlayerPage));
            }
        }


        private void UpdateMuteButtonUI(int muteValue)
        {
            string muteText = muteValue == 1 ? "Unmute" : "Mute";
        }

        public void UpdateVoiceChatSettings(BananaWatchButton buttonType)
        {
            string currentSetting = PlayerPrefs.GetString("pttType", "ALL CHAT");

            switch (buttonType)
            {
                case BananaWatchButton.Left:
                    if (currentSetting == "ALL CHAT") PlayerPrefs.SetString("pttType", "PUSH TO TALK");
                    else if (currentSetting == "PUSH TO TALK") PlayerPrefs.SetString("pttType", "PUSH TO MUTE");
                    else PlayerPrefs.SetString("pttType", "ALL CHAT");
                    break;

                case BananaWatchButton.Right:
                    if (currentSetting == "ALL CHAT") PlayerPrefs.SetString("pttType", "PUSH TO MUTE");
                    else if (currentSetting == "PUSH TO TALK") PlayerPrefs.SetString("pttType", "ALL CHAT");
                    else PlayerPrefs.SetString("pttType", "PUSH TO TALK");
                    break;

                case BananaWatchButton.Enter:
                    bool isVoiceChatOn = PlayerPrefs.GetString("voiceChatOn", "FALSE") == "TRUE";
                    PlayerPrefs.SetString("voiceChatOn", isVoiceChatOn ? "FALSE" : "TRUE");
                    break;
            }

            PlayerPrefs.Save();
            BananaWatch.Instance.UpdateScreen();
        }
    }
}
