using BananaWatch.Pages;
using Photon.Pun;
using UnityEngine;

namespace BananaWatch.Pages
{
    public class ReportPlayerPage : BananaWatchPage
    {
        public override string Title => "Report";
        public override bool PublicPage => false;

        public static Photon.Realtime.Player SelectedPlayer;
        private int selectedOption = 0;

        private string[] reportReasons = { "Hate Speech", "Cheating", "Toxicity" };

        public override string RenderScreenContent()
        {
            SelectedPlayer = PlayerDetailsPage.SelectedPlayer;

            if (SelectedPlayer == null)
                return "<color=#ffff00>==</color> Report Player <color=#ffff00>==</color>\n\n<size=60> No player selected </size>";

            string hateSpeechArrow = selectedOption == 0 ? "<color=#C16F66>></color> " : "  ";
            string cheatingArrow = selectedOption == 1 ? "<color=#C16F66>></color> " : "  ";
            string toxicityArrow = selectedOption == 2 ? "<color=#C16F66>></color> " : "  ";

            return $@"<color=#ffff00>==</color> Report Player <color=#ffff00>==</color>

Report {SelectedPlayer.NickName} ?

<size=43> Select a reason and press enter to confirm 
Or press back to stop the report</size>

<size=95> 
{hateSpeechArrow}Hate Speech
{cheatingArrow}Cheating
{toxicityArrow}Toxicity
</size>";
        }

        public override void ButtonPressed(BananaWatchButton buttonType)
        {
            switch (buttonType)
            {
                case BananaWatchButton.Up:
                    selectedOption = Mathf.Max(0, selectedOption - 1);
                    break;
                case BananaWatchButton.Down:
                    selectedOption = Mathf.Min(2, selectedOption + 1);
                    break;
                case BananaWatchButton.Enter:
                    HandleReport();
                    break;
                case BananaWatchButton.Back:
                    BananaWatch.Instance.NavigateToPage(typeof(PlayerDetailsPage));
                    break;
            }
        }

        private void HandleReport()
        {
            if (SelectedPlayer != null)
            {
                GorillaPlayerLineButton.ButtonType reportType = GorillaPlayerLineButton.ButtonType.HateSpeech;

                switch (selectedOption)
                {
                    case 0:
                        reportType = GorillaPlayerLineButton.ButtonType.HateSpeech;
                        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                        {
                            if (line.linePlayer.UserId == SelectedPlayer.UserId)
                            {
                                line.PressButton(true, reportType);
                                break;
                            }
                        }
                        break;
                    case 1:
                        reportType = GorillaPlayerLineButton.ButtonType.Cheating;
                        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                        {
                            if (line.linePlayer.UserId == SelectedPlayer.UserId)
                            {
                                line.PressButton(true, reportType);
                                break;
                            }
                        }
                        break;
                    case 2:
                        reportType = GorillaPlayerLineButton.ButtonType.Toxicity;
                        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                        {
                            if (line.linePlayer.UserId == SelectedPlayer.UserId)
                            {
                                line.PressButton(true, reportType);
                                break;
                            }
                        }
                        break;
                }

                GorillaPlayerScoreboardLine.ReportPlayer(SelectedPlayer.UserId, reportType, SelectedPlayer.NickName);
                NavigateToPage(typeof(PlayerDetailsPage));
            }
        }
    }
}
