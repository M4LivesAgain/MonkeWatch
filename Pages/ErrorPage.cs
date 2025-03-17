using BananaWatch.Pages;

namespace BananaWatch.Pages
{
    public class ErrorPage : BananaWatchPage
    {
        public override string MMHeader => "Error";
        public override bool MMDisplay => false;

        public static string page = "Error: ";

        public override string RenderScreenContent()
        {
            return "<size=40>" + page;
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