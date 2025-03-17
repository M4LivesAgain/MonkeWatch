namespace BananaWatch.Pages
{
    public class Disconnect : BananaWatchPage
    {
        public override string MMHeader => "Disconnect";

        public override bool MMDisplay => true;

        public override string RenderScreenContent()
        {
            NetworkSystem.Instance.ReturnToSinglePlayer();
            NavigateToMM();
            return "";
        }
    }
}