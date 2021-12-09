using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayableAnimation;

namespace PlayableAnimation
{
    public class Transition
    {
        public State target;
        public TransitionParameters transitionParameters;


        public Transition(TransitionParameters transitionParameters)
        {
            this.transitionParameters = transitionParameters;
        }
    }
}