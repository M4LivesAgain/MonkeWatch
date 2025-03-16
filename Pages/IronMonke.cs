using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

namespace BananaWatch.Pages
{
    internal class IronMonke : BananaWatchPage
    {
        public override string Title => "Iron Monke";
        public override bool PublicPage => true;
        public bool isEnabled;
        static SelectionHandler selectionHandler = new SelectionHandler();


        public override void PostModLoaded()
        {
            selectionHandler.maxIndex = 1;
            force = 7;
        }

        public override string RenderScreenContent()
        {
            string ptext;

            ptext = "<color=#ffff00>==</color> Iron Monke <color=#ffff00>==</color>\r\n ";

            ptext += selectionHandler.SelectionArrow(0, isEnabled ? "<color=#00FF00>Enabled</color>" : "<color=#FF0000>Disabled</color>") + "\r\n";
            ptext += selectionHandler.SelectionArrow(1, "\r\n Thrust:\r\n    " + force);

            return ptext;
        }
        public float force;

        public override void ButtonPressed(BananaWatchButton buttonType)
        {
            switch (buttonType)
            {
                case BananaWatchButton.Down:
                    selectionHandler.MoveSelectionDown();
                    break;

                case BananaWatchButton.Up:
                    selectionHandler.MoveSelectionUp();
                    break;

                case BananaWatchButton.Enter:
                    if (selectionHandler.currentIndex == 0)
                    {
                        isEnabled = !isEnabled;
                    }
                    break;
                case BananaWatchButton.Right:
                    if (selectionHandler.currentIndex == 1)
                    {
                        if (force > 100)
                        {
                            force -= 1f;
                        }
                        else
                        {
                            force += 1f;
                        }
                    }

                    break;
                case BananaWatchButton.Left:
                    if (selectionHandler.currentIndex == 1)
                    {
                        if (force < 5)
                        {
                            force += 1f;
                        }
                        else
                        {
                            force -= 1f;
                        }
                    }
                    break;

                case BananaWatchButton.Back:
                    NavigateToMainMenu();
                    break;
            }
        }
        void Update()
        {
            if (isEnabled)
            {
                if (ControllerInputPoller.instance.rightControllerPrimaryButton)
                {
                    GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity += GorillaLocomotion.Player.Instance.rightControllerTransform.transform.right * force * Time.deltaTime;
                }
                if (ControllerInputPoller.instance.leftControllerPrimaryButton)
                {
                    GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity += GorillaLocomotion.Player.Instance.leftControllerTransform.transform.right * -force * Time.deltaTime;
                }
            }
        }
    }
}