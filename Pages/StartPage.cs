using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaWatch.Pages
{
    public class StartPage : BananaWatchPage
    {
        public override string MMHeader => "";
        public override bool MMDisplay => false;

        public static string page =
@"<color=#f0f000><size=40>
                    ██                    
                  ██<color=#a0a000>▒▒</color>██                 
              ████████                    
            ██<color=#a0a000>░░</color>  ████                    
            ██<color=#a0a000>░░</color>  ██████                  
          ██<color=#a0a000>░░░░</color>  ████  ████              
          ██<color=#a0a000>░░░░</color>  ██  ██    ██████       
          ██<color=#a0a000>░░░░</color>  ██<color=#a0a000>░░</color>  ████      ████████
          ██<color=#a0a000>░░░░</color>    ██<color=#a0a000>░░</color>    ██████      ██
          ██<color=#a0a000>░░░░░░</color>    ████        ██████  
          ██<color=#a0a000>░░░░░░░░</color>      ██████<color=#a0a000>░░</color>    ██  
            ██<color=#a0a000>░░░░░░░░░░</color>        ██████    
              ██<color=#a0a000>░░░░░░░░░░░░░░</color>    ██      
                ████<color=#a0a000>░░░░░░░░░░</color>████        
                    ██████████            
</size>
    <color=#ffff00>B A N A N A</color>  <color=#ffffff>O S</color>
</color>";


        public override string RenderScreenContent()
        {
            return page;
        }

        public override void ButtonPressed(BananaWatchButton ButtonType)
        {
            switch (ButtonType)
            {
                case BananaWatchButton.Enter:
                    NavigateToMM();
                    break;
            }
        }

    }
}
