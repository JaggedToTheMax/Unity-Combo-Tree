using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayableAnimation
{
    [System.Serializable]
    public class State
    {
        public AnimationClip animationClip;
        public bool returnToDefault;

        public State(AnimationClip animationClip)
        {
            this.animationClip = animationClip;
        }
    }
}
