namespace BananaWatch.Pages
{
    public class Disconnect : BananaWatchPage
    {
        public override string Title => "Disconnect";

        public override bool PublicPage => true;

        public override string RenderScreenContent()
        {
            NetworkSystem.Instance.ReturnToSinglePlayer();
            NavigateToMainMenu();
            return "";
        }
    }
}