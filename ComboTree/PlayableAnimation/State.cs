using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayableAnimation
{
    [System.Serializable]
    public class State
    {
        public AnimationClip animationClip;

        public State(AnimationClip animationClip)
        {
            this.animationClip = animationClip;
        }

        public virtual void OnEnter(PlayableAnimator animator) { }
        public virtual void OnUpdate(PlayableAnimator animator) { }
        public virtual void OnExit(PlayableAnimator animator) { }
    }
}
