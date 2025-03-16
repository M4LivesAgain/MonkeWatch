using BananaWatch;
using BananaWatch.Pages;
using System;
using System.Linq;
using UnityEngine;
using static BananaWatch.BananaWatch;

namespace BananaWatch.Pages
{
    public abstract class BananaWatchPage : MonoBehaviour
    {

        static SelectionHandler selectionManager = new SelectionHandler();
        public abstract string Title { get; } // displayed on main menu
        public abstract bool PublicPage { get; } // should be displayed on main menu?
        public abstract string RenderScreenContent();

        public void NavigateToMainMenu() => NavigateToPage(typeof(MainMenu));
        public void NavigateToPage(BananaWatchPage screen) => BananaWatch.Instance.NavigateToPage(screen);
        public void NavigateToPage(Type screenType) => BananaWatch.Instance.NavigateToPage(screenType);
        public virtual void PostRoomUpdate() { } // happens when leave or join room
        public virtual void ButtonPressed(BananaWatchButton buttonType) { }
        public virtual void ButtonReleased(BananaWatchButton buttonType) { }
        public virtual void PostModLoaded() { }
        public virtual void PageOpened() { }
    }
}