using BananaWatch.Utils;
using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using BananaWatch.Pages;
using UnityEngine.InputSystem;
using GorillaNetworking;
using Photon.Pun;

namespace BananaWatch
{
    [BepInPlugin("com.M4LivesAgain.BananaWatch", "BananaWatch", "1.0.0")]
    public class BananaWatch : BaseUnityPlugin
    {
        public static List<Type> pageTypes = new List<Type>();
        public static BananaWatch Instance;
        private static AssetBundle assetBundle;
        private static GameObject bananaWatchPrefab;
        private GameObject instantiatedBananaWatch;
        private Text screenText = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane/Canvas/Text")?.GetComponent<Text>();
        public static Vector3 originalScale;
        bool isWatchOpen;

        private Vector3 positionOffset = new Vector3(0.639f, -0.64f, -0.017f); // as perfect as i can get it without src
        private Vector3 rotationOffset = new Vector3(90f, 180f, -90f);

        public List<BananaWatchPage> watchPages = new List<BananaWatchPage>();
        public BananaWatchPage currentPage;

        public bool isInitialized = false;

        void OnLoaded()
        {
            Instance = this;
            Configuration.LoadSettings();

            pageTypes.Add(typeof(StartPage));
            pageTypes.Add(typeof(ErrorPage));
            pageTypes.Add(typeof(MainMenu));
            pageTypes.Add(typeof(ScoreboardPage));
            pageTypes.Add(typeof(DetailsPage));
            pageTypes.Add(typeof(PlayerDetailsPage));
            pageTypes.Add(typeof(Disconnect));
            pageTypes.Add(typeof(ReportPlayerPage));
            foreach (var page in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = page.GetTypes().Where(type => type.IsSubclassOf(typeof(BananaWatchPage)));
                    foreach (var type in types)
                    {
                        if (!pageTypes.Contains(type))
                        {
                            pageTypes.Add(type);
                        }
                    }
                }
                catch (Exception err)
                {
                    Debug.LogError($"{page.FullName} errored while registering: " + err);
                }
            }

            CreatePages();

            NavigateToPage(typeof(StartPage));
        }

        void Start()
        {
            StartCoroutine(InitializeBananaWatch());
        }

        void CreatePages()
        {
            foreach (var type in pageTypes)
            {
                try
                {
                    var watchPage = (BananaWatchPage)gameObject.AddComponent(type);
                    watchPages.Add(watchPage);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    continue;
                }
            }
        }

        private IEnumerator InitializeBananaWatch()
        {
            while (GorillaTagger.Instance == null)
            {
                yield return null;
            }

            try
            {
                assetBundle = AssetTools.LoadAssetBundle("BananaWatch.Resources.bananawatch");

                if (assetBundle != null)
                {
                    bananaWatchPrefab = assetBundle.LoadAsset<GameObject>("BananaWatch");

                    if (bananaWatchPrefab != null)
                    {
                        instantiatedBananaWatch = Instantiate(bananaWatchPrefab);

                        originalScale = instantiatedBananaWatch.transform.localScale;

                        instantiatedBananaWatch.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        instantiatedBananaWatch.transform.localRotation = Quaternion.Euler(180, 0, 0);

                        InitializeScreenText();

                        GameObject buttonsParent = instantiatedBananaWatch.transform.Find("Plane/Buttons")?.gameObject;
                        GameObject watchTop = instantiatedBananaWatch.transform.Find("Watch/Top")?.gameObject;

                        watchTop.layer = 18;

                        if (buttonsParent != null && watchTop != null)
                        {
                            Transform[] childTransforms = buttonsParent.GetComponentsInChildren<Transform>(true);

                            watchTop.gameObject.AddComponent<WatchButton>();

                            foreach (Transform child in childTransforms)
                            {
                                if (child.name.Contains("Canvas") || child.name.Contains("Text")) continue;

                                WatchButton watchButton = child.gameObject.GetComponent<WatchButton>();
                                if (watchButton == null)
                                {
                                    watchButton = child.gameObject.AddComponent<WatchButton>();
                                    watchButton.gameObject.layer = 18;
                                }

                                switch (child.name)
                                {
                                    case "Up":
                                        watchButton.buttonType = BananaWatchButton.Up;
                                        break;
                                    case "Down":
                                        watchButton.buttonType = BananaWatchButton.Down;
                                        break;
                                    case "Left":
                                        watchButton.buttonType = BananaWatchButton.Left;
                                        break;
                                    case "Right":
                                        watchButton.buttonType = BananaWatchButton.Right;
                                        break;
                                    case "Back":
                                        watchButton.buttonType = BananaWatchButton.Back;
                                        break;
                                    case "Enter":
                                        watchButton.buttonType = BananaWatchButton.Enter;
                                        break;
                                    default:

                                        break;
                                }

                                watchButton.Init();
                            }
                        }



                        GameObject Plane = instantiatedBananaWatch.transform.Find("Plane")?.gameObject;
                        if (Plane != null)
                        {
                            Plane.SetActive(false);
                        }

                        isInitialized = true;
                        OnLoaded();


                        foreach (var page in watchPages)
                        {
                            try
                            {
                                page.PostModLoaded();
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"{page.GetType().FullName}: {e}");
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}");
                Debug.LogException(e);
            }
        }

        private void InitializeScreenText()
        {
            if (instantiatedBananaWatch != null)
            {
                screenText = instantiatedBananaWatch.transform.Find("Plane/Canvas/Text")?.GetComponent<Text>();
            }
        }

        public void OpenWatch()
        {
            GameObject Plane = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane");

            if (Plane != null)
                Plane.SetActive(true);
        }

        public void CloseWatch()
        {
            GameObject Plane = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane");

            if (Plane != null)
                Plane.SetActive(false);
        }

        public void UpdateScreen()
        {
            try
            {
                screenText.text = currentPage.RenderScreenContent();
            }
            catch (Exception exception)
            {
                var e = $"{exception}";
                Debug.LogError(e);
                ErrorPage.page = e;
                NavigateToPage(typeof(ErrorPage));
            }
        }

        void Update()
        {
            if (isInitialized)
            {
                var watchAngle = Vector3.Angle(instantiatedBananaWatch.transform.up, GorillaTagger.Instance.offlineVRRig.transform.up -= new Vector3(0f, 180f, 0f));
                if (watchAngle < Configuration.WatchClosingAngle.Value)
                {
                    isWatchOpen = false;
                    instantiatedBananaWatch.transform.Find("Plane").gameObject.SetActive(false);
                    return;
                }
            }
        }

        public void LateUpdate()
        {
            if (isInitialized && instantiatedBananaWatch != null)
            {
                try
                {
                    Transform parentTransform = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L")?.transform;
                    Transform huntComputerTransform = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)")?.transform;

                    if (parentTransform != null && huntComputerTransform != null)
                    {
                        if (instantiatedBananaWatch.transform.parent != parentTransform)
                        {
                            instantiatedBananaWatch.transform.SetParent(parentTransform);
                        }
                            instantiatedBananaWatch.transform.localPosition = Vector3.Lerp(instantiatedBananaWatch.transform.localPosition, huntComputerTransform.localPosition + positionOffset, Time.deltaTime * 10f);
                            instantiatedBananaWatch.transform.localRotation = Quaternion.Slerp(instantiatedBananaWatch.transform.localRotation, huntComputerTransform.localRotation * Quaternion.Euler(rotationOffset), Time.deltaTime * 10f);

                            GameObject Screen = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane");
                        GameObject head = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/head");
                        Vector3 targetLookAt = head.transform.position;
                        Vector3 smoothLookAt = Vector3.Lerp(Screen.transform.position, targetLookAt, Time.deltaTime * 10f);
                        Screen.transform.LookAt(smoothLookAt);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}");
                    Debug.LogException(e);
                }
            }
            else
            {
                if (!isInitialized)
                {
                    return;
                }
            }
        }


        public void PressButton(BananaWatchButton type)
        {

            /*var watchAngle = Vector3.Angle(instantiatedBananaWatch.transform.up, GorillaTagger.Instance.offlineVRRig.transform.up);
            if (watchAngle > Configuration.WatchClosingAngle.Value)
                return;*/

            if (type == BananaWatchButton.Watch)
            {
                isWatchOpen = !isWatchOpen;
                GameObject Plane1 = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane");
                if (Plane1 != null)
                    Plane1.SetActive(true);
                return;
            }
            currentPage.ButtonPressed(type);
            UpdateScreen();
        }

        public void NavigateToPage(BananaWatchPage screen)
        {
            if (screen == null)
            {
                return;
            }

            BananaWatch.Instance.currentPage = screen;
            screen.PageOpened();
            BananaWatch.Instance.UpdateScreen();
        }

        public void NavigateToPage(Type screenType)
        {
            if (watchPages == null)
            {
                return;
            }

            BananaWatchPage screen = watchPages.FirstOrDefault(page => page.GetType() == screenType);

            if (screen != null)
            {
                NavigateToPage(screen);
            }
            else
            {
                Debug.LogError($"{screenType}");
            }
        }
    }
}