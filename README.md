# MonkeWatch
### MonkeWatch is a port of popular old quest mod named MonkeComputer, Created by M4.


### Simply input the .dll from releases into your BepInEx plugins folder and then it should work. Integrations should be put in the same plugins folder.


## Need help?
### Make sure to join the discord to get help with integrations or installing MonkeWatch.
https://discord.gg/kZmDh5etNy

## For Integrations

### Below is a Template for adding your very own integration to MonkeWatch!

```csharp
using System.Collections.Generic;
using BananaWatch;
using BananaWatch.Pages;
using BepInEx;

namespace Template
{
    [BepInPlugin("M4.Template", "TemplateIntegration", "1.0.0")] // BepInEx Plugin Header needed for loading the page into the MonkeWatch.
    public class Template : BaseUnityPlugin { } // same again from ^
    public class TemplatePage : BananaWatchPage
    {
        public override string Title => "Template"; // name displayed on main menu (can use rich text)
        public override bool PublicPage => true; // should be displayed on main menu

        private SelectionHandler selectionHandler = new SelectionHandler();

        private List<string> menuOptions = new List<string> // list of all options
        {
            "Option 1",
            "Option 2",
            "Option 3",
            "Option 4"
        };

        public override void PageOpened()
        {
            selectionHandler.maxIndex = menuOptions.Count - 1;
            selectionHandler.currentIndex = 0;
        }

        public override string RenderScreenContent()
        {
            string header = "<color=#00ff00>==</color> Template <color=#00ff00>==</color>\n\n"; // header at the top of the screen

            string content = header;
            for (int i = 0; i < menuOptions.Count; i++)
            {
                content += selectionHandler.SelectionArrow(i, menuOptions[i]) + "\n"; // adding the selection arrow to all menu options
            }

            return content;
        }

        public override void ButtonPressed(BananaWatchButton buttonType)
        {
            switch (buttonType)
            {
                case BananaWatchButton.Up:
                    selectionHandler.MoveSelectionUp();
                    break;
                case BananaWatchButton.Down:
                    selectionHandler.MoveSelectionDown();
                    break;
                case BananaWatchButton.Enter:
                    // handle selections for each menu button here here
                    break;
                case BananaWatchButton.Back:
                    NavigateToMainMenu();
                    break;
            }
        }
    }
}
```
