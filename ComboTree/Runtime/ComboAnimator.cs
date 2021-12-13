using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayableAnimation;
using UnityEngine.Playables;

namespace ComboTree
{
    public class ComboAnimator : PlayableAnimator
    {
        [SerializeField] ComboTreeController comboTree;
        RuntimeComboTree runtimeCombo;

        State anyState;

        /// <summary>
        /// Used to change the runtimecombotree.
        /// </summary>
        public void SetComboTree(ComboTreeController comboTree)
        {
            runtimeCombo = new RuntimeComboTree(comboTree);
            defaultState = runtimeCombo.entry;
            anyState = runtimeCombo.any;
        }
        /// <summary>
        /// Searches the current, next and anystate (in that order) for a valid transition.
        /// </summary>
        /// <param name="name">The name of the transition to search for</param>
        /// <returns>Wether or not a valid transition was found (a valid transition needs a matching name and inputwindow).</returns>
        public bool Transition(string name)
        {
            var hash = Animator.StringToHash(name);

            if (TransitionCurrent(current as ComboState, hash))
                return true;
            else if (TransitionNext(next as ComboState, hash))
                return true;
            else if (TransitionCurrent(anyState as ComboState, hash))
                return true;

            return false;
        }


        bool TransitionCurrent(ComboState state, int hash)
        {
            if (state is null)
                return false;


            if (state.transitions.TryGetValue(hash, out ComboTransition transition))
            {
                if (transition.target == current || transition.target == next)
                    return false;

                if (transition.InInputWindow(current.animationClip is null ? 0 : (float)transitionMixer.GetInput(0).GetTime() / current.animationClip.length))
                {
                    if (TransitionState is TransitionState.InActiveTransition && !transition.canInterrupt)
                        return false;

                    Transition(transition, true);

                    return true;
                }
            }

            return false;
        }
        bool TransitionNext(ComboState state, int hash)
        {
            if (state is null || TransitionState != TransitionState.InActiveTransition)
                return false;


            if (state.transitions.TryGetValue(hash, out ComboTransition transition))
            {
                if (transition.target == current || transition.target == next)
                    return false;

                if (transition.InInputWindow(next.animationClip is null ? 0 : (float)transitionMixer.GetInput(1).GetTime() / next.animationClip.length))
                {
                    if (transition.canInterrupt)
                        FinishTransition();
                    else
                        return false;

                    Transition(transition, true);

                    return true;
                }
            }

            return false;
        }


        protected override void Awake()
        {
            SetComboTree(comboTree);
            base.Awake();
        }
    }
}
