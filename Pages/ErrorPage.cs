using BananaWatch.Pages;

namespace BananaWatch.Pages
{
    public class ErrorPage : BananaWatchPage
    {
        public override string Title => "Error";
        public override bool PublicPage => false;

        public static string page = "Error: ";

        public override string RenderScreenContent()
        {
            return "<size=40>" + page;
        }

        public override void ButtonPressed(BananaWatchButton buttonType)
        {
            switch (buttonType)
            {
                case BananaWatchButton.Back:
                    NavigateToMainMenu();
                    break;
            }
        }
    }
}