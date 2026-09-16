using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace L4th4m
{
    /// <summary>
    /// Arena opening: character entrance -> jump/mount animation -> cycle activation
    /// -> countdown -> gameplay. Supports both timed fallback and animation-event timing.
    /// </summary>
    public class OpeningSequenceDirector : MonoBehaviour
    {
        [Header("Scene references")]
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private GameObject characterRoot;
        [SerializeField] private GameObject lightcycleRoot;
        [SerializeField] private Camera cinematicCamera;
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private MonoBehaviour[] gameplayScripts;

        [Header("Animation")]
        [SerializeField] private string mountTrigger = "MountLightcycle";
        [SerializeField] private float preMountDelay = 0.75f;
        [SerializeField] private float mountDuration = 2.0f;
        [Tooltip("When enabled, animation events call CycleMountPoint() and MountAnimationFinished().")]
        [SerializeField] private bool useAnimationEvents = true;

        [Header("Cycle reveal")]
        [SerializeField] private bool hideCycleUntilMount = false;
        [SerializeField] private Behaviour[] cycleEffects;
        [SerializeField] private float timedCycleActivationDelay = 0.35f;

        [Header("Countdown")]
        [SerializeField] private Text countdownText;
        [SerializeField] private float countdownStepSeconds = 0.65f;
        [SerializeField] private string goText = "GO";
        [SerializeField] private float goHoldSeconds = 0.5f;

        [Header("Events")]
        public UnityEvent onSequenceStarted;
        public UnityEvent onMountStarted;
        public UnityEvent onCycleActivated;
        public UnityEvent onGameplayStarted;

        private bool running;
        private bool cycleActivated;
        private bool mountFinished;

        private void Awake()
        {
            SetGameplayEnabled(false);

            if (cinematicCamera != null) cinematicCamera.enabled = true;
            if (gameplayCamera != null) gameplayCamera.enabled = false;
            if (countdownText != null) countdownText.gameObject.SetActive(false);
            if (hideCycleUntilMount && lightcycleRoot != null) lightcycleRoot.SetActive(false);

            SetCycleEffects(false);
        }

        private void Start() => StartOpening();

        public void StartOpening()
        {
            if (!running) StartCoroutine(RunOpening());
        }

        private IEnumerator RunOpening()
        {
            running = true;
            cycleActivated = false;
            mountFinished = false;
            onSequenceStarted?.Invoke();

            yield return new WaitForSeconds(preMountDelay);

            onMountStarted?.Invoke();
            if (characterAnimator != null && !string.IsNullOrWhiteSpace(mountTrigger))
                characterAnimator.SetTrigger(mountTrigger);

            if (useAnimationEvents)
            {
                // Safety fallback prevents a bad/missing Animation Event from hanging the intro.
                float timeout = Mathf.Max(0.25f, mountDuration + 1.0f);
                float elapsed = 0f;
                while (!mountFinished && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (!cycleActivated) ActivateCycle();
            }
            else
            {
                yield return new WaitForSeconds(timedCycleActivationDelay);
                ActivateCycle();
                yield return new WaitForSeconds(Mathf.Max(0f, mountDuration - timedCycleActivationDelay));
            }

            CompleteMountAndStartCountdown();
            yield return StartCoroutine(RunCountdown());

            SetGameplayEnabled(true);
            onGameplayStarted?.Invoke();
            running = false;
        }

        /// <summary>
        /// Add an Animation Event with function name "CycleMountPoint" at the frame where
        /// the rider reaches/touches the lightcycle. This reveals/energises the bike exactly
        /// in sync with the supplied jump-on-lightcycle animation.
        /// </summary>
        public void CycleMountPoint()
        {
            ActivateCycle();
        }

        /// <summary>
        /// Add an Animation Event with function name "MountAnimationFinished" on the final
        /// seated frame of the supplied mount clip.
        /// </summary>
        public void MountAnimationFinished()
        {
            mountFinished = true;
        }

        private void ActivateCycle()
        {
            if (cycleActivated) return;
            cycleActivated = true;

            if (lightcycleRoot != null) lightcycleRoot.SetActive(true);
            SetCycleEffects(true);
            onCycleActivated?.Invoke();
        }

        private void CompleteMountAndStartCountdown()
        {
            if (characterRoot != null) characterRoot.SetActive(false);
            SwitchToGameplayCamera();
        }

        private IEnumerator RunCountdown()
        {
            if (countdownText == null) yield break;

            countdownText.gameObject.SetActive(true);
            for (int value = 3; value >= 1; value--)
            {
                countdownText.text = value.ToString();
                yield return new WaitForSeconds(countdownStepSeconds);
            }

            countdownText.text = goText;
            yield return new WaitForSeconds(goHoldSeconds);
            countdownText.gameObject.SetActive(false);
        }

        private void SwitchToGameplayCamera()
        {
            if (cinematicCamera != null) cinematicCamera.enabled = false;
            if (gameplayCamera != null) gameplayCamera.enabled = true;
        }

        private void SetGameplayEnabled(bool enabled)
        {
            if (gameplayScripts == null) return;
            foreach (MonoBehaviour script in gameplayScripts)
                if (script != null) script.enabled = enabled;
        }

        private void SetCycleEffects(bool enabled)
        {
            if (cycleEffects == null) return;
            foreach (Behaviour effect in cycleEffects)
                if (effect != null) effect.enabled = enabled;
        }
    }
}
