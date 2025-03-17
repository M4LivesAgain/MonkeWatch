using BananaWatch.Utils;
using BepInEx;
using System;
using System.Collections;
using UnityEngine;

namespace BananaWatch
{
    public enum BananaWatchButton
    {
        Watch = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        Back = 5,
        Enter = 6
    }

    public class WatchButton : MonoBehaviour
    {
        private static readonly Color pressedColor = new Color(0.5f, 0.5f, 0.5f);
        private const float KEY_BUMP_AMOUNT = 0.02f;
        public BananaWatchButton buttonType;
        public Renderer ButtonRenderer;
        public GameObject ButtonObject;
        public float ButtonTime;
        public bool isPressed;
        public bool CanPress;

        private BoxCollider collider;
        private bool _bumped;

        private Material _Material;
        private Color _OriginalColor;
        private Color _PressedButtonColor;

        void Update()
        {
            if (!CanPress)
            {
                ButtonTime += Time.deltaTime;
                if (ButtonTime > Configuration.ButtonCooldown.Value)
                {
                    ButtonTime = 0;
                    CanPress = true;
                }
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (!CanPress) return;

            var component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
            if (component != null && !component.isLeftHand && collider.gameObject.name == "RightHandTriggerCollider")
            {
                CanPress = false;
                if (Configuration.HapticResponse.Value) // haptic feedback
                {
                    GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
                }
                BumpIn(); // bumping is broken for some reason, the text stops it from bumping but if you click on the side of the button it will bump
                Active(true);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.GetComponent<GorillaTriggerColliderHandIndicator>() == null) return;

            BumpOut();
            Active(false);
            BananaWatch.Instance._CurrentPage.ButtonReleased(buttonType);
        }

        void Active(bool val)
        {
            isPressed = val;
            if (val)
            {
                BananaWatch.Instance?.PressButton(buttonType);
            }
            UpdateColor();
        }

        private void BumpIn()
        {
            if (!_bumped)
            {
                _bumped = true;
                var pos = transform.localPosition;
                pos.z -= KEY_BUMP_AMOUNT;
                transform.localPosition = pos;

                if (collider != null)
                {
                    collider.center -= new Vector3(0, 0, KEY_BUMP_AMOUNT / 1.125f);
                }
            }
        }

        private void BumpOut()
        {
            if (_bumped)
            {
                _bumped = false;
                var pos = transform.localPosition;
                pos.z += KEY_BUMP_AMOUNT;
                transform.localPosition = pos;

                if (collider != null)
                {
                    collider.center += new Vector3(0, 0, KEY_BUMP_AMOUNT / 1.125f);
                }
            }
        }

        void UpdateColor()
        {
            if (buttonType == BananaWatchButton.Watch || ButtonRenderer == null)
                return;

            _Material.color = isPressed
                ? Configuration.PressedColor.Value
                : buttonType switch
                {
                    BananaWatchButton.Enter => Configuration.EnterColor.Value,
                    BananaWatchButton.Back => Configuration.BackColor.Value,
                    _ => Configuration.ArrowColor.Value
                };
        }

        public void Interface()
        {
            if (ButtonRenderer == null)
                ButtonRenderer = GetComponent<Renderer>();

            if (ButtonRenderer == null)
            {
                return;
            }

            ButtonObject = this.gameObject;
            if (ButtonObject == null)
            {
                return;
            }

            if (collider == null)
                collider = GetComponent<BoxCollider>();

            if (collider == null)
            {
                return;
            }

            _Material = ButtonRenderer.material; // setting this will cause null refenrencen error
            _OriginalColor = _Material.color;
            _PressedButtonColor = pressedColor;

            UpdateColor();
        }
    }
}