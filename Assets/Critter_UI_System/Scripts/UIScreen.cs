using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CritterGames.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public Selectable startSelectable;

        [Header("Screen Events")]
        public UnityEvent onScreenStart = new UnityEvent();
        public UnityEvent onScreenClose = new UnityEvent();

        private Animator animator;
        #endregion

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }        

        #region Main Methods
        void Start()
        {
            if (startSelectable)
            {
                EventSystem.current.SetSelectedGameObject(startSelectable.gameObject);
            }
        }
        #endregion

        #region Helper Methods
        public virtual void StartScreen(bool usingFader)
        {
            onScreenStart?.Invoke();
            animator.SetTrigger("show");
        }

        public virtual void CloseScreen(bool usingFader)
        {
            onScreenClose?.Invoke();
            if (!usingFader) animator.SetTrigger("hide");
        }
        #endregion
    }
}

