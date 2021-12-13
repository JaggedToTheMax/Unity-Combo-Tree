using PlayableAnimation;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ComboTree
{
    public class ComboState : State
    {
        public Dictionary<int, ComboTransition> transitions = new Dictionary<int, ComboTransition>();
        public int nameHash;

        public ComboState(AnimationClip animationClip, string name) : base(animationClip)
        {
            this.animationClip = animationClip;
            nameHash = Animator.StringToHash(name);
        }
        public ComboState(AnimationClip animationClip) : base(animationClip)
        {
            this.animationClip = animationClip;
            nameHash = Animator.StringToHash(animationClip is null ? "" : animationClip.name);
        }

        public override void OnEnter(PlayableAnimator animator)
        {
            void onTransitionComplete()
            {
                animator.OnTransitionExit -= onTransitionComplete;

                if (transitions.TryGetValue(Animator.StringToHash("Default"), out var transition))
                    animator.Transition(transition);
            }

            animator.OnTransitionExit += onTransitionComplete;
        }

        public bool IsName(string name)
        {
            return Animator.StringToHash(name) == nameHash;
        }
    }
}
