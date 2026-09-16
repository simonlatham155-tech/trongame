using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace L4th4m
{
    /// <summary>
    /// Runs the intended arena opening:
    /// character entrance -> jump/mount animation -> cycle activation -> countdown -> gameplay.
    ///
    /// This controller does not depend on a specific FBX path or animation clip name.
    /// Wire the supplied Animator and trigger name in the Inspector.
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
        [Tooltip("Animator trigger that starts the supplied jump/mount-lightcycle animation.")]
        [SerializeField] private string mountTrigger = "MountLightcycle";
        [Tooltip("Delay after the sequence starts before the mount animation is triggered.")]
        [SerializeField] private float preMountDelay = 0.75f;
        [Tooltip("Time from mount trigger until the rider is considered seated on the bike.")]
        [SerializeField] private float mountDuration = 2.0f;

        [Header("Cycle reveal")]
        [SerializeField] private bool hideCycleUntilMount = false;
        [SerializeField] private Behaviour[] cycleEffects;
        [SerializeField] private float cycleActivationDelay = 0.35f;

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

        private void Awake()
        {
            SetGameplayEnabled(false);

            if (cinematicCamera != null)
                cinematicCamera.enabled = true;

            if (gameplayCamera != null)
                gameplayCamera.enabled = false;

            if (countdownText != null)
                countdownText.gameObject.SetActive(false);

            if (hideCycleUntilMount && lightcycleRoot != null)
                lightcycleRoot.SetActive(false);

            SetCycleEffects(false);
        }

        private void Start()
        {
            StartOpening();
        }

        public void StartOpening()
        {
            if (!running)
                StartCoroutine(RunOpening());
        }

        private IEnumerator RunOpening()
        {
            running = true;
            onSequenceStarted?.Invoke();

            yield return new WaitForSeconds(preMountDelay);

            onMountStarted?.Invoke();
            if (characterAnimator != null && !string.IsNullOrWhiteSpace(mountTrigger))
                characterAnimator.SetTrigger(mountTrigger);

            yield return new WaitForSeconds(cycleActivationDelay);

            if (lightcycleRoot != null)
                lightcycleRoot.SetActive(true);

            SetCycleEffects(true);
            onCycleActivated?.Invoke();

            float remainingMountTime = Mathf.Max(0f, mountDuration - cycleActivationDelay);
            if (remainingMountTime > 0f)
                yield return new WaitForSeconds(remainingMountTime);

            // Once the mount animation has completed, the separate entrance character
            // can be hidden if the gameplay lightcycle already contains the rider model.
            if (characterRoot != null)
                characterRoot.SetActive(false);

            SwitchToGameplayCamera();
            yield return StartCoroutine(RunCountdown());

            SetGameplayEnabled(true);
            onGameplayStarted?.Invoke();
            running = false;
        }

        private IEnumerator RunCountdown()
        {
            if (countdownText == null)
                yield break;

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
            if (cinematicCamera != null)
                cinematicCamera.enabled = false;

            if (gameplayCamera != null)
                gameplayCamera.enabled = true;
        }

        private void SetGameplayEnabled(bool enabled)
        {
            if (gameplayScripts == null)
                return;

            foreach (MonoBehaviour script in gameplayScripts)
            {
                if (script != null)
                    script.enabled = enabled;
            }
        }

        private void SetCycleEffects(bool enabled)
        {
            if (cycleEffects == null)
                return;

            foreach (Behaviour effect in cycleEffects)
            {
                if (effect != null)
                    effect.enabled = enabled;
            }
        }
    }
}
