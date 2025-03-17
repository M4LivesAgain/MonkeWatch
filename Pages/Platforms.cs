using BananaWatch.Pages;
using UnityEngine;

namespace BananaWatch.Pages
{
    public class AirJump : BananaWatchPage
    {
        public enum PlatformSize
        {
            Default,
            One,
            Two,
            Three,
            Four,
            HeftyChonk
        }

        public enum PlatformColor
        {
            Black,
            Red,
            Green,
            Blue,
            Yellow,
            Purple
        }

        public override string MMHeader => "<color=#ffff00>Platform Monke V2</color>";
        public override bool MMDisplay => true;

        static SelectionHandler selectionHandler = new SelectionHandler();
        private int platformSizeIndex = 0;
        private int platformColorIndex = 0;
        private PlatformSize platformSize = PlatformSize.Default;
        private PlatformColor platformColor = PlatformColor.Black;

        public override void PostModLoaded()
        {
            selectionHandler.MaxIndex = 7;
        }

        public bool Platforms { get; private set; }
        public static GameObject RightPlatform;
        public static bool RightPlatformEnabled;
        public static GameObject LeftPlatform;
        public static bool LeftPlatformEnabled;

        public string SelectionArrow(int index, string text)
        {
            if (selectionHandler.CurrentIndex == index)
            {
                string arrow = (index == 0) ? "<color=#C16F66>></color>" : "<color=#C16F66><></color>";
                return $"{arrow} {text}";
            }
            return text;
        }


        public void Update()
        {
            if (Platforms)
            {
                if (ControllerInputPoller.instance.rightGrab)
                {
                    HandlePlatform(ref RightPlatform, ref RightPlatformEnabled, GorillaLocomotion.Player.Instance.rightControllerTransform, 45f);
                }
                else if (RightPlatformEnabled)
                {
                    Destroy(RightPlatform);
                    RightPlatformEnabled = false;
                }

                if (ControllerInputPoller.instance.leftGrab)
                {
                    HandlePlatform(ref LeftPlatform, ref LeftPlatformEnabled, GorillaLocomotion.Player.Instance.leftControllerTransform, 45f);
                }
                else if (LeftPlatformEnabled)
                {
                    Destroy(LeftPlatform);
                    LeftPlatformEnabled = false;
                }
            }
        }

        private void HandlePlatform(ref GameObject platform, ref bool platformEnabled, Transform controllerTransform, float rotationX)
        {
            if (!platformEnabled)
            {
                platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                platform.transform.position = controllerTransform.position + new Vector3(0f, -0.02f, 0f);
                platform.transform.rotation = controllerTransform.rotation * Quaternion.Euler(rotationX, 0f, -90f);
                platform.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                platformEnabled = true;

                ApplyPlatformSettings(platform);
            }
        }

        private void ApplyPlatformSettings(GameObject platform)
        {
            switch (platformSize)
            {
                case PlatformSize.Default:
                    platform.transform.localScale = new Vector3(0.28f, 0.015f, 0.38f);
                    break;
                case PlatformSize.One:
                    platform.transform.localScale = new Vector3(0.10f, 0.005f, 0.12f);
                    break;
                case PlatformSize.Two:
                    platform.transform.localScale = new Vector3(0.14f, 0.007f, 0.19f);
                    break;
                case PlatformSize.Three:
                    platform.transform.localScale = new Vector3(0.35f, 0.018f, 0.45f);
                    break;
                case PlatformSize.Four:
                    platform.transform.localScale = new Vector3(0.45f, 0.02f, 0.55f);
                    break;
                case PlatformSize.HeftyChonk:
                    platform.transform.localScale = new Vector3(0.9f, 0.05f, 1.1f);
                    break;
            }

            Color newColor = Color.black;
            switch (platformColor)
            {
                case PlatformColor.Black: newColor = Color.black; break;
                case PlatformColor.Red: newColor = Color.red; break;
                case PlatformColor.Green: newColor = Color.green; break;
                case PlatformColor.Blue: newColor = Color.blue; break;
                case PlatformColor.Yellow: newColor = Color.yellow; break;
                case PlatformColor.Purple: newColor = new Color(0.5f, 0f, 0.5f); break;
            }
            platform.GetComponent<Renderer>().material.color = newColor;
        }

        public override string RenderScreenContent()
        {
            string pagetext = "<color=#ffff00> Platform Monke v2:</color>\r\n";
            pagetext += SelectionArrow(0, Platforms ? "<color=#00FF00>Enabled</color>" : "<color=#FF0000>Disabled</color>") + "\r\n";
            pagetext += SelectionArrow(1, $"Platform Size: <color=#00FF00>{platformSize}</color>") + "\r\n";
            pagetext += SelectionArrow(2, $"Platform Color: <color=#00FF00>{platformColor}</color>") + "\r\n";

            return pagetext;
        }

        public override void ButtonPressed(BananaWatchButton ButtonType)
        {
            switch (ButtonType)
            {
                case BananaWatchButton.Up:
                    selectionHandler.MoveSelectionUp();
                    break;
                case BananaWatchButton.Down:
                    selectionHandler.MoveSelectionDown();
                    break;
                case BananaWatchButton.Left:
                    AdjustSelection(-1);
                    break;
                case BananaWatchButton.Right:
                    AdjustSelection(1);
                    break;
                case BananaWatchButton.Enter:
                    if (selectionHandler.CurrentIndex == 0)
                    {
                        Platforms = !Platforms;
                    }
                    break;
                case BananaWatchButton.Back:
                    NavigateToMM();
                    break;
            }
        }

        private void AdjustSelection(int direction)
        {
            if (selectionHandler.CurrentIndex == 1)
            {
                platformSizeIndex += direction;
                if (platformSizeIndex < 0) platformSizeIndex = System.Enum.GetValues(typeof(PlatformSize)).Length - 1;
                if (platformSizeIndex >= System.Enum.GetValues(typeof(PlatformSize)).Length) platformSizeIndex = 0;
                platformSize = (PlatformSize)platformSizeIndex;
            }
            else if (selectionHandler.CurrentIndex == 2)
            {
                platformColorIndex += direction;
                if (platformColorIndex < 0) platformColorIndex = System.Enum.GetValues(typeof(PlatformColor)).Length - 1;
                if (platformColorIndex >= System.Enum.GetValues(typeof(PlatformColor)).Length) platformColorIndex = 0;
                platformColor = (PlatformColor)platformColorIndex;
            }
        }
    }
}
