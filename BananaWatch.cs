using BananaWatch.Utils;
using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BananaWatch.Pages;

namespace BananaWatch
{
    [BepInPlugin("com.M4LivesAgain.BananaWatch", "BananaWatch", PluginInfo.Version)]
    public class BananaWatch : BaseUnityPlugin
    {
        public static BananaWatch Instance;
        private static AssetBundle assetBundle;
        private static GameObject BananaWatchPrefab;
        private GameObject InstantiatedBananaWatch;
        private Vector3 _PositionOffset = new Vector3(0.639f, -0.64f, -0.017f); // as perfect as i can get it without src
        private Vector3 _RotationOffset = new Vector3(90f, 180f, -90f);

        private static List<Type> TypeOfPage = new List<Type>();
        public List<BananaWatchPage> BananaWatchPages = new List<BananaWatchPage>();
        public BananaWatchPage CurrentPage;

        private Text _ScreenText = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/BananaWatch(Clone)/Plane/Canvas/Text")?.GetComponent<Text>();
        private static Vector3 OriginalScale;
        private bool isWatchOpen;
        public bool BananaWatchInitialized = false;



        public bool KolossalWasNeverACheatForGorillaTag = true;

        void Start()
        {
            StartCoroutine(InitializeBananaWatch());
        }

        void OnLoaded()
        {
            TypeOfPage.Add(typeof(StartPage));
            TypeOfPage.Add(typeof(ErrorPage));
            TypeOfPage.Add(typeof(MainMenu));
            TypeOfPage.Add(typeof(ScoreboardPage));
            TypeOfPage.Add(typeof(DetailsPage));
            TypeOfPage.Add(typeof(PlayerDetailsPage));
            TypeOfPage.Add(typeof(Disconnect));
            TypeOfPage.Add(typeof(ReportPlayerPage));

            Instance = this;
            Configuration.LoadSettings();

            GameObject Plane = InstantiatedBananaWatch.transform.Find("Plane")?.gameObject;
            Plane.SetActive(false);

            foreach (var ASSEMBLY in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var pageTypes = ASSEMBLY.GetTypes().Where(type => typeof(BananaWatchPage).IsAssignableFrom(type));
                    foreach (var pageType in pageTypes)
                    {
                        if (!TypeOfPage.Contains(pageType))
                        {
                            TypeOfPage.Add(pageType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"assembly failed to load: {ASSEMBLY.FullName}: {ex.Message}");
                }
            }

            InitializePages();
            NavigateToPage(typeof(StartPage));
        }

        void InitializePages()
        {
            foreach (var type in TypeOfPage)
            {
                try
                {
                    var watchPage = (BananaWatchPage)gameObject.AddComponent(type);
                    BananaWatchPages.Add(watchPage);
                }
                catch (Exception err)
                {
                    Debug.LogException(err);
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
                    BananaWatchPrefab = assetBundle.LoadAsset<GameObject>("BananaWatch");

                    if (BananaWatchPrefab != null)
                    {
                        InstantiatedBananaWatch = Instantiate(BananaWatchPrefab);

                        OriginalScale = InstantiatedBananaWatch.transform.localScale;

                        InstantiatedBananaWatch.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        InstantiatedBananaWatch.transform.localRotation = Quaternion.Euler(180, 0, 0);

                        InitializeScreenText();

                        GameObject buttonsParent = InstantiatedBananaWatch.transform.Find("Plane/Buttons")?.gameObject;
                        GameObject watchTop = InstantiatedBananaWatch.transform.Find("Watch/Top")?.gameObject;

                        watchTop.layer = 18;

                        if (buttonsParent != null && watchTop != null)
                        {
                            Transform[] childTransforms = buttonsParent.GetComponentsInChildren<Transform>(true);

                            watchTop.gameObject.AddComponent<WatchButton>();

                            foreach (Transform child in childTransforms)
                            {
                                if (child == buttonsParent.transform) continue;

                                WatchButton watchButton = child.gameObject.GetComponent<WatchButton>();
                                if (watchButton == null)
                                {
                                    watchButton = child.gameObject.AddComponent<WatchButton>();
                                    watchButton.gameObject.layer = 18;
                                }

                                switch (child.name)
                                {
                                    case "Up":
                                        watchButton.ButtonType = BananaWatchButton.Up;
                                        break;
                                    case "Down":
                                        watchButton.ButtonType = BananaWatchButton.Down;
                                        break;
                                    case "Left":
                                        watchButton.ButtonType = BananaWatchButton.Left;
                                        break;
                                    case "Right":
                                        watchButton.ButtonType = BananaWatchButton.Right;
                                        break;
                                    case "Back":
                                        watchButton.ButtonType = BananaWatchButton.Back;
                                        break;
                                    case "Enter":
                                        watchButton.ButtonType = BananaWatchButton.Enter;
                                        break;
                                    default:
                                        Debug.LogWarning($"button was registered that i dont understand: {child.name}");
                                        break;
                                }

                                watchButton.Awake();
                            }
                        }

                        BananaWatchInitialized = true;
                        OnLoaded();


                        foreach (var BananaWatchPage in BananaWatchPages)
                        {
                            try
                            {
                                BananaWatchPage.PostModLoaded();
                            }
                            catch (Exception err)
                            {
                                Debug.LogError($"page error: {BananaWatchPage.GetType().FullName}: {err}");
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.LogError($"asset loading failed: {err.Message}");
                Debug.LogException(err);
            }
        }

        private void InitializeScreenText()
        {
            if (InstantiatedBananaWatch != null)
            {
                _ScreenText = InstantiatedBananaWatch.transform.Find("Plane/Canvas/Text")?.GetComponent<Text>();
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

        void Update()
        {
            if (BananaWatchInitialized)
            {
                var watchAngle = Vector3.Angle(InstantiatedBananaWatch.transform.up, GorillaTagger.Instance.offlineVRRig.transform.up);
                if (watchAngle > Configuration.WatchClosingAngle.Value)
                {
                    isWatchOpen = false;
                    InstantiatedBananaWatch.transform.Find("Plane").gameObject.SetActive(false);
                    return;
                }
            }
        }

        public void LateUpdate()
        {
            if (BananaWatchInitialized && InstantiatedBananaWatch != null)
            {
                try
                {
                    Transform parentTransform = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L")?.transform;
                    Transform huntComputerTransform = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)")?.transform;

                    if (parentTransform != null && huntComputerTransform != null)
                    {
                        if (InstantiatedBananaWatch.transform.parent != parentTransform)
                        {
                            InstantiatedBananaWatch.transform.SetParent(parentTransform);
                        }
                        InstantiatedBananaWatch.transform.localPosition = Vector3.Lerp(InstantiatedBananaWatch.transform.localPosition, huntComputerTransform.localPosition + _PositionOffset, Time.deltaTime * 10f);
                        InstantiatedBananaWatch.transform.localRotation = Quaternion.Slerp(InstantiatedBananaWatch.transform.localRotation, huntComputerTransform.localRotation * Quaternion.Euler(_RotationOffset), Time.deltaTime * 10f);

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
                if (!BananaWatchInitialized)
                {
                    return;
                }
            }
        }


        public void BananaWatchButtonPress(BananaWatchButton type)
        {
            if (InstantiatedBananaWatch == null)
            {
                return;
            }

            GameObject plane = InstantiatedBananaWatch.transform.Find("Plane")?.gameObject;
            if (plane == null)
            {
                return;
            }

            if (type == BananaWatchButton.Watch)
            {
                isWatchOpen = !isWatchOpen;
                plane.SetActive(true);
                return;
            }

            if (CurrentPage == null)
            {
                Debug.LogError("404 Page Not Found");
                return;
            }
            CurrentPage.ButtonPressed(type);
            RefreshScreen();
        }


        public void NavigateToPage(BananaWatchPage screen)
        {
            if (screen == null) return;

            BananaWatch.Instance.CurrentPage = screen;
            screen.PageOpened();
            BananaWatch.Instance.RefreshScreen();
        }

        public void NavigateToPage(Type screenType)
        {
            if (BananaWatchPages?.Count > 0)
            {
                var screen = BananaWatchPages.FirstOrDefault(page => page.GetType() == screenType);
                if (screen != null)
                {
                    NavigateToPage(screen);
                }
                else
                {
                    Debug.LogError("no");
                }
            }
            else
            {
                Debug.LogError("also no lol");
            }
        }
        public void RefreshScreen()
        {
            try
            {
                var content = CurrentPage.RenderScreenContent();
                if (_ScreenText != null)
                {
                    _ScreenText.text = content;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"exception: {ex}";
                Debug.LogError(errorMessage);
                ErrorPage.page = errorMessage;
                NavigateToPage(typeof(ErrorPage));
            }
        }
    }
}