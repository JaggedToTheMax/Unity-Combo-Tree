using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayableAnimation;

namespace ComboTree
{
    public class ComboTransition : Transition
    {
        public int nameHash;
        public float inputWindowStart;
        public float inputWindowEnd;
        public bool hasInputWindow;


        public ComboTransition(TransitionParameters transitionParameters, string name) : base(transitionParameters)
        {
            nameHash = Animator.StringToHash(name);
        }

        public static int GetHash(string name)
        {
            return Animator.StringToHash(name);
        }
        public bool IsName(string name)
        {
            return Animator.StringToHash(name) == nameHash;
        }
        public bool InInputWindow(float time)
        {
            return !hasInputWindow || time >= inputWindowStart && time <= inputWindowEnd;
        }
    }
}