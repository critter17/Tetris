using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CritterGames.UI
{
    public class UISystem : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public UIScreen startScreen;

        [Header("System Events")]
        public UnityEvent onSwitchedScreen = new UnityEvent();

        [Header("Fader Properties")]
        public Image fader;
        public bool useFader = true;
        public float fadeInDuration = 1f;
        public float fadeOutDuration = 1f;

        private Component[] screens;
        
        private UIScreen previousScreen;
        public UIScreen PreviousScreen { get { return previousScreen; } }
        
        private UIScreen currentScreen;
        public UIScreen CurrentScreen { get { return currentScreen; } }
        #endregion

        #region Main Methods
        private void Awake()
        {
            screens = GetComponentsInChildren<UIScreen>(true);

            if (fader && useFader)
            {
                fader.gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            SwitchScreens(startScreen);
        }

        private void OnDisable()
        {
            currentScreen = null;
            previousScreen = null;
        }
        #endregion

        #region Helper Methods
        public void SwitchScreens(UIScreen newScreen)
        {
            if (!newScreen)
            {
                return;
            }

            StartCoroutine(LoadScene(newScreen));
        }

        public void GoToPreviousScreen()
        {
            if (!previousScreen)
            {
                return;
            }

            SwitchScreens(previousScreen);
        }

        IEnumerator LoadScene(UIScreen newScreen)
        {
            if (currentScreen)
            {
                Debug.Log("Current Screen: " + currentScreen.name);
                FadeOut();
                currentScreen.CloseScreen(useFader);
                previousScreen = currentScreen;
                yield return new WaitForSeconds(fadeInDuration);
                currentScreen.gameObject.SetActive(false);
            }

            currentScreen = newScreen;
            currentScreen.gameObject.SetActive(true);
            currentScreen.StartScreen(useFader);
            yield return new WaitForSeconds(fadeInDuration * 0.25f);
            FadeIn();

            onSwitchedScreen?.Invoke();
        }

        public void FadeIn()
        {
            if (!useFader)
            {
                return;
            }

            fader.CrossFadeAlpha(0f, fadeInDuration, false);
        }

        public void FadeOut()
        {
            if (!useFader)
            {
                return;
            }

            fader.CrossFadeAlpha(1f, fadeOutDuration, false);
        }
        #endregion
    }
}
