using UnityEngine;

namespace PlayableAnimation
{
    [System.Serializable]
    public enum TransitionState { None, InTransition, InActiveTransition, Done }
    public class TransitionSampler
    {
        readonly TransitionParameters transitionParameters;
        [SerializeField] float transitionTime;
        [SerializeField] float exitTime;
        float fromDuration;
        float startTime;

        public TransitionSampler(float fromDuration, float startTime, TransitionParameters transitionParameters)
        {
            this.transitionParameters = transitionParameters;
            this.fromDuration = fromDuration;
            this.startTime = startTime;
            SetTransitionTime(transitionParameters.transitionTime);
            HasExitTime(transitionParameters.hasExitTime);
        }

        public void SetTransitionTime(float transitionTime)
        {
            this.transitionTime = Mathf.Clamp(transitionTime, 0.0000001f, float.MaxValue);
        }
        public void SetExitTime(float exitTime)
        {
            this.exitTime = exitTime;
            if (this.exitTime == 0 && startTime != 0)
                this.exitTime = fromDuration;

            if (startTime > exitTime)
                this.exitTime += fromDuration * (Mathf.Floor((startTime - this.exitTime) / fromDuration) + 1);
        }
        public void SetNormalizedExitTime(float normalizedExitTime)
        {
            exitTime = normalizedExitTime * fromDuration;
            SetExitTime(exitTime);
        }
        public void HasExitTime(bool hasExitTime)
        {
            if (hasExitTime)
                SetNormalizedExitTime(transitionParameters.exitTime);
            else
                exitTime = startTime;
        }

        public float Sample(float time)
        {
            var completion = Mathf.Clamp01((time - exitTime) / transitionTime);

            return completion;
        }
        public float Sample(float time, out TransitionState transitionState)
        {
            var completion = Sample(time);

            transitionState = completion == 0 ? TransitionState.InTransition : completion < 1 ? TransitionState.InActiveTransition : TransitionState.Done;

            return completion;
        }
    }
}
