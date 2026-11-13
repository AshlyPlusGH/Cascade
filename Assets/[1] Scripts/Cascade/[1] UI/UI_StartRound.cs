using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using PrimeTween;
using UnityEngine.UI;

namespace Cascade {
    public class UI_StartRound : UI
    {
        [SerializeField] private GameObject waitingForPlayersOverlay;
        [SerializeField] private Image blackScreenOverlay;
            [SerializeField] TweenSettings<float> TweenSettings_fadeIn;

        [SerializeField] private TextMeshProUGUI TMP_vs;
            [SerializeField] TweenSettings<float> TweenSettings_vs;
        [SerializeField] private TextMeshProUGUI TMP_playerOne;
            [SerializeField] TweenSettings<Vector3> TweenSettings_playerOne;
        [SerializeField] private TextMeshProUGUI TMP_playerTwo;
            [SerializeField] TweenSettings<Vector3> TweenSettings_playerTwo;
        [SerializeField] private TextMeshProUGUI TMP_gameStart;
            [SerializeField] TweenSettings<float> TweenSettings_gameStart;

        public void Setup()
        {
            TMP_playerOne.text = Account.username;
            TMP_playerTwo.text = GameManager.GetOpponentUsername();
        }

        void SetBlackScreenAlpha(float alpha){ blackScreenOverlay.color = new Color(0,0,0,alpha); }

        public override async UniTask Play()
        {
            waitingForPlayersOverlay.SetActive(false);

            Setup();
            
            blackScreenOverlay.raycastTarget = false;
            
            if (fastStart)
            {
                blackScreenOverlay.color = new Color();

                TMP_playerOne.gameObject.SetActive(false);
                return;
            }

            await UniTask.WaitForSeconds(1); //To Allow for workings

            await Tween.Custom(TweenSettings_fadeIn,SetBlackScreenAlpha);

                TMP_playerOne.gameObject.SetActive(true);
            await Tween.LocalPosition(TMP_playerOne.rectTransform, TweenSettings_playerOne);
                TMP_playerTwo.gameObject.SetActive(true);
            await Tween.LocalPosition(TMP_playerTwo.rectTransform, TweenSettings_playerTwo);
                TMP_vs.gameObject.SetActive(true);
            await Tween.Scale(TMP_vs.transform, TweenSettings_vs);
                TMP_playerOne.gameObject.SetActive(false);
                TMP_playerTwo.gameObject.SetActive(false);
                TMP_vs.gameObject.SetActive(false);

                TMP_gameStart.gameObject.SetActive(true);
            await Tween.Scale(TMP_gameStart.transform, TweenSettings_gameStart);
                TMP_gameStart.gameObject.SetActive(false);
        }
    }
}