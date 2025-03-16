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
        public Renderer buttonRenderer;
        public GameObject buttonObject;
        public float buttonTime;
        public bool isPressed;
        public bool canPress;

        private BoxCollider collider;
        private bool _bumped;

        private Material _material;
        private Color _originalColor;
        private Color _pressedButtonColor;

        public void Init()
        {
            if (buttonRenderer == null)
                buttonRenderer = GetComponent<Renderer>();

            if (buttonRenderer == null)
            {
                return;
            }

            buttonObject = this.gameObject;
            if (buttonObject == null)
            {
                return;
            }

            if (collider == null)
                collider = GetComponent<BoxCollider>();

            if (collider == null)
            {
                return;
            }

            _material = buttonRenderer.material;
            _originalColor = _material.color;
            _pressedButtonColor = pressedColor;

            UpdateColor();
        }

        void Update()
        {
            if (!canPress)
            {
                buttonTime += Time.deltaTime;
                if (buttonTime > Configuration.ButtonCooldown.Value)
                {
                    buttonTime = 0;
                    canPress = true;
                }
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (!canPress) return;

            var component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
            if (component != null && !component.isLeftHand && collider.gameObject.name == "RightHandTriggerCollider")
            {
                canPress = false;
                BumpIn(); // bumping is broken for some reason, the text stops it from bumping but if you click on the side of the button it will bump
                Active(true);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.GetComponent<GorillaTriggerColliderHandIndicator>() == null) return;

            BumpOut();
            Active(false);
            BananaWatch.Instance.currentPage.ButtonReleased(buttonType);
        }

        void Active(bool value)
        {
            isPressed = value;
            if (value)
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
            if (buttonType == BananaWatchButton.Watch || buttonRenderer == null)
                return;

            _material.color = isPressed
                ? Configuration.PressedColor.Value
                : buttonType switch
                {
                    BananaWatchButton.Enter => Configuration.EnterColor.Value,
                    BananaWatchButton.Back => Configuration.BackColor.Value,
                    _ => Configuration.ArrowColor.Value
                };
        }
    }
}
