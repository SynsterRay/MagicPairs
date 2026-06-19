using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class InvitePanel : MonoBehaviour
    {
        private const string LastShareKey = "MagicPairs_LastShare";
        private const int ShareReward = 50;
        private const string StoreUrl = "https://play.google.com/store/apps/details?id=com.Mateusz_Bajak.MagicPairs";

        private GameObject _panel;
        private Text _statusText;

        public void Show(System.Action onBack)
        {
            if (_panel != null) { Destroy(_panel); _panel = null; }
            CreatePanel(onBack);
        }

        private void CreatePanel(System.Action onBack)
        {
            var canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            if (canvas == null) return;

            _panel = new GameObject("InvitePanel");
            _panel.transform.SetParent(canvas.transform, false);
            var bg = _panel.AddComponent<Image>();
            bg.color = new Color(1f, 1f, 1f, 1f);
            var rect = _panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Reward description
            var desc = UIFactory.CreateText("Desc", Localization.Get("inviteReward"), _panel.transform,
                new Vector2(0.1f, 0.65f), new Vector2(0.9f, 0.80f), TextAnchor.MiddleCenter, 32);
            desc.color = new Color(0.2f, 0.2f, 0.2f);

            // Coin icon + amount
            var coinIcon = new GameObject("CoinIcon");
            coinIcon.transform.SetParent(_panel.transform, false);
            var ciImg = coinIcon.AddComponent<Image>();
            ciImg.sprite = UIIcons.Get("coin_icon");
            ciImg.preserveAspect = true;
            ciImg.raycastTarget = false;
            var ciRect = coinIcon.GetComponent<RectTransform>();
            ciRect.anchorMin = new Vector2(0.38f, 0.55f);
            ciRect.anchorMax = new Vector2(0.50f, 0.64f);
            ciRect.offsetMin = Vector2.zero;
            ciRect.offsetMax = Vector2.zero;

            var coinText = UIFactory.CreateText("CoinAmount", $"+{ShareReward}", _panel.transform,
                new Vector2(0.50f, 0.55f), new Vector2(0.65f, 0.64f), TextAnchor.MiddleLeft, 36);
            coinText.color = new Color(0.85f, 0.65f, 0.1f);

            // Share button
            var shareBtn = UIFactory.CreateIconButton("ShareBtn", "referal_link", _panel.transform,
                new Vector2(0.25f, 0.32f), new Vector2(0.75f, 0.54f));
            shareBtn.onClick.AddListener(OnShare);

            // Status text
            _statusText = UIFactory.CreateText("Status", "", _panel.transform,
                new Vector2(0.1f, 0.24f), new Vector2(0.9f, 0.32f), TextAnchor.MiddleCenter, 22);
            _statusText.color = new Color(0.4f, 0.4f, 0.4f);

            // Check cooldown
            if (!CanShare())
            {
                _statusText.text = Localization.Get("inviteCooldown");
            }

            // Back button
            var backBtn = UIFactory.CreateIconButton("BackBtn", "back", _panel.transform,
                new Vector2(0.30f, 0.14f), new Vector2(0.70f, 0.23f));
            backBtn.onClick.AddListener(() => { Destroy(_panel); onBack?.Invoke(); });
        }

        private bool CanShare()
        {
            string last = PlayerPrefs.GetString(LastShareKey, "");
            if (string.IsNullOrEmpty(last)) return true;
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");
            return last != today;
        }

        private void OnShare()
        {
            string msg = Localization.Get("inviteMsg") + "\n" + StoreUrl;

#if UNITY_ANDROID && !UNITY_EDITOR
            using (var intent = new AndroidJavaObject("android.content.Intent"))
            {
                intent.Call<AndroidJavaObject>("setAction", "android.intent.action.SEND");
                intent.Call<AndroidJavaObject>("setType", "text/plain");
                intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", msg);
                using (var chooser = new AndroidJavaClass("android.content.Intent")
                    .CallStatic<AndroidJavaObject>("createChooser", intent, Localization.Get("invite")))
                {
                    using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    {
                        var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                        activity.Call("startActivity", chooser);
                    }
                }
            }
#else
            Debug.Log($"[Invite] Share: {msg}");
#endif

            // Grant reward only once per day
            if (CanShare())
            {
                PlayerPrefs.SetString(LastShareKey, System.DateTime.Now.ToString("yyyy-MM-dd"));
                PlayerPrefs.Save();
                PlayerWallet.Add(ShareReward);

                if (_statusText != null)
                    _statusText.text = Localization.Get("inviteShared");
            }
        }
    }
}
