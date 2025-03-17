using System;
using UnityEngine;

namespace BananaWatch.Pages
{
    public abstract class BananaWatchPage : MonoBehaviour
    {
        private static readonly SelectionHandler SelectionManager = new SelectionHandler();

        public abstract string MMHeader { get; }
        public abstract bool MMDisplay { get; }
        public abstract string RenderScreenContent();

        public void NavigateToMM() => NavigateToPage(typeof(MainMenu));

        public void NavigateToPage(BananaWatchPage screen)
            => BananaWatch.Instance.NavigateToPage(screen);

        public void NavigateToPage(Type screenType)
            => BananaWatch.Instance.NavigateToPage(screenType);

        public virtual void ButtonPressed(BananaWatchButton ButtonType) { }

        public virtual void ButtonReleased(BananaWatchButton ButtonType) { }

        public virtual void PostModLoaded() { }

        public virtual void PageOpened() { }
    }
}
