using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayableAnimation
{
    [System.Serializable]
    public struct TransitionParameters
    {
        public float transitionTime;
        public float exitTime;
        public float transitionOffset;
        public bool hasExitTime;
    }
}
