using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace PlayableAnimation
{

    [System.Serializable]
    public class PlayableTransitionSampler : TransitionSampler
    {
        AnimationMixerPlayable transitionMixer;
        Playable from;
        Playable to;

        public PlayableTransitionSampler(AnimationMixerPlayable transitionMixer, TransitionParameters transitionParameters) 
            : base(GetPlayableDuration(transitionMixer.GetInput(0)), (float)transitionMixer.GetInput(0).GetTime(), transitionParameters)
        {
            if (!ValidMixer(transitionMixer))
                throw new System.Exception("Tried to transition non valid transitionmixer.\n" + transitionMixer);

            this.transitionMixer = transitionMixer;
            from = transitionMixer.GetInput(0);
            to = transitionMixer.GetInput(1);
            to.SetTime(transitionParameters.transitionOffset);
        }

        public static bool ValidMixer(AnimationMixerPlayable mixerPlayable)
        {
            return mixerPlayable.GetInputCount() == 2
                && ValidPlayable(mixerPlayable.GetInput(0))
                && ValidPlayable(mixerPlayable.GetInput(1));
        }

        public TransitionState Sample()
        {
            var completion = Sample((float)from.GetTime(), out TransitionState transitionState);

            SetCompletion(transitionMixer, completion);

            if (completion == 0)
                to.Pause();
            else
                to.Play();

            return transitionState;
        }


        static void SetCompletion(AnimationMixerPlayable transitionMixer, float completion)
        {
            transitionMixer.SetInputWeight(0, 1 - completion);
            transitionMixer.SetInputWeight(1, completion);
        }
        static bool ValidPlayable(Playable playable)
        {
            return playable.IsPlayableOfType<AnimationClipPlayable>() || playable.IsPlayableOfType<AnimatorControllerPlayable>();
        }
        static float GetPlayableDuration(Playable playable)
        {
            if (playable.IsPlayableOfType<AnimationClipPlayable>() && ((AnimationClipPlayable)playable).GetAnimationClip() is AnimationClip animationClip)
                return animationClip.length;

            if (playable.IsPlayableOfType<AnimatorControllerPlayable>())
                return ((AnimatorControllerPlayable)playable).GetCurrentAnimatorStateInfo(0).length;

            return 0;
        }
    }
}
