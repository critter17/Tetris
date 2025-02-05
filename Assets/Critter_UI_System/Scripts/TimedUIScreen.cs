using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CritterGames.UI
{
    public class TimedUIScreen : UIScreen
    {
        [Header("Timed Screen Properties")]
        public float screenTime = 2f;
        public UnityEvent onTimeCompleted = new UnityEvent();
        private float startTime;

        public override void StartScreen(bool usingFader)
        {
            base.StartScreen(usingFader);

            startTime = Time.time;
            StartCoroutine(WaitForTime());
        }

        IEnumerator WaitForTime()
        {
            yield return new WaitForSeconds(screenTime);

            onTimeCompleted?.Invoke();
        }
    }
}
