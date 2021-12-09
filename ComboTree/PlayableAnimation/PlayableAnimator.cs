using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace PlayableAnimation
{


    [RequireComponent(typeof(Animator))]
    public class PlayableAnimator : MonoBehaviour
    {
        /// <summary>
        /// Callback that is triggered when Transition() is called (includes TransitionToDefault()).
        /// </summary>
        public Action OnTransitionEnter;
        /// <summary>
        /// Callback that is triggered once the transition has reached its exittime.
        /// </summary>
        public Action OnActiveTransitionEnter;
        /// <summary>
        /// Callback that is triggered once the transitions completed.
        /// </summary>
        public Action OnTransitionExit;
        /// <summary>
        /// The default transitionparameters used when transitioning to default.
        /// </summary>
        public TransitionParameters defaultTransition = new TransitionParameters()
        {
            transitionOffset = 0,
            transitionTime = 0.2f,
            hasExitTime = true,
            exitTime = 0.8f
        };

        /// <summary>
        /// The playablegraph used.
        /// </summary>
        public PlayableGraph Graph { get; private set; }
        /// <summary>
        /// The current state.
        /// </summary>
        public State Current => current;
        /// <summary>
        /// The next state (null when not transitioning),
        /// </summary>
        public State Next => next;
        /// <summary>
        /// The transitionstate.
        /// </summary>
        public TransitionState TransitionState { get; protected set; }

        protected PlayableTransitionSampler transitionSampler;
        protected PlayablePooler playablePooler;
        protected State defaultState = new State(null);
        protected State current;
        protected State next;
        protected AnimationMixerPlayable transitionMixer;


        /// <summary>
        /// Transition from the current to a new state.
        /// </summary>
        /// <param name="state">The new state to transition to.</param>
        /// <param name="transitionParameters">The transitionparameters to use to transition.</param>
        /// <param name="cachePlayable">Wether or not the animator saves the generated playable for later reuse.</param>
        public void Transition(State state, TransitionParameters transitionParameters, bool cachePlayable = true)
        {
            SetNextPlayable(cachePlayable ? playablePooler.Get(state) : AnimationClipPlayable.Create(Graph, next.animationClip));

            transitionSampler = new PlayableTransitionSampler(transitionMixer, transitionParameters);
            TransitionState = TransitionState.InTransition;
            next = state;

            OnTransitionEnter?.Invoke();
        }
        /// <inheritdoc/>
        /// <summary>
        /// Transition from the current to a new state.
        /// </summary>
        /// <param name="animationClip">The animation clip to transition to.</param>
        /// <param name="returnToDefault">Wether or not to transition to default after the transition.</param>
        public void Transition(AnimationClip animationClip, TransitionParameters transitionParameters, bool returnToDefault = true, bool cachePlayable = true)
        {
            Transition(new Transition(transitionParameters)
            {
                target = new State(animationClip)
                {
                    returnToDefault = returnToDefault
                }
            }, cachePlayable);
        }
        /// <inheritdoc/>
        /// <summary>
        /// Transition from the current to a new state.
        /// </summary>
        /// <param name="transition">The transition used.</param>
        public void Transition(Transition transition, bool cachePlayable = true)
        {
            Transition(transition.target, transition.transitionParameters, cachePlayable);
        }
        /// <summary>
        /// Transition back to the animator controller.
        /// </summary>
        /// <param name="transitionParameters">The transitionparameters to use to transition.</param>
        public void TransitionToDefault(TransitionParameters transitionParameters)
        {
            Transition(defaultState, transitionParameters, true);
        }
        /// <summary>
        /// Transition back to the animator controller using the default transitionparameters.
        /// </summary>
        public void TransitionToDefault()
        {
            TransitionToDefault(defaultTransition);
        }

        void EnterTransition()
        {
            if (TransitionState is TransitionState.InActiveTransition)
                return;

            TransitionState = TransitionState.InActiveTransition;

            OnActiveTransitionEnter?.Invoke();
        }
        void SampleTransition()
        {
            if (TransitionState is TransitionState.None || transitionSampler is null)
                return;

            var state = transitionSampler.Sample();

            if (state is TransitionState.InActiveTransition)
                EnterTransition();
            else if (state is TransitionState.Done)
                FinishTransition();
        }
        protected void FinishTransition()
        {
            if (TransitionState is TransitionState.None)
                return;

            TransitionState = TransitionState.None;

            SetCurrentToNext();

            OnTransitionExit?.Invoke();
            if (current.returnToDefault)
                TransitionToDefault(defaultTransition);
        }
        void SetCurrentToNext()
        {
            var nextPlayable = transitionMixer.GetInput(1);

            transitionMixer.DisconnectInput(1);
            SetCurrentPlayable(nextPlayable);

            transitionMixer.SetInputWeight(0, 1);
            transitionMixer.SetInputWeight(1, 0);

            current = next;
            next = null;
            transitionSampler = null;
        }
        void SetCurrentPlayable(Playable playable)
        {
            transitionMixer.DisconnectInput(0);
            playablePooler.Dispose(current);

            if (!playable.IsNull())
                transitionMixer.ConnectInput(0, playable, 0);
        }
        void SetNextPlayable(Playable playable)
        {
            transitionMixer.DisconnectInput(1);
            playablePooler.Dispose(next);

            if (!playable.IsNull())
                transitionMixer.ConnectInput(1, playable, 0);
        }

        void Init()
        {
            var animator = GetComponent<Animator>();

            Graph = PlayableGraph.Create();
            Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);


            playablePooler = new PlayablePooler(Graph);
            playablePooler.Add(defaultState, AnimatorControllerPlayable.Create(Graph, animator.runtimeAnimatorController));


            transitionMixer = AnimationMixerPlayable.Create(Graph, 2, true);
            transitionMixer.SetPropagateSetTime(true);
            transitionMixer.ConnectInput(0, playablePooler.Get(defaultState), 0, 1);
            current = defaultState;

            var output = AnimationPlayableOutput.Create(Graph, "Animator", animator);
            output.SetSourcePlayable(transitionMixer);

            Graph.Play();
        }


        protected virtual void Awake()
        {
            Init();
        }
        void Update()
        {
            SampleTransition();
        }
        private void OnDestroy()
        {
            Graph.Destroy();
        }


        protected class PlayablePooler
        {
            PlayableGraph graph;
            Dictionary<State, Playable> cache = new Dictionary<State, Playable>();


            public PlayablePooler(PlayableGraph graph)
            {
                this.graph = graph;
            }

            public Playable Get(State key)
            {
                if (cache.TryGetValue(key, out Playable value))
                    return value;
                else
                {
                    var entry = AnimationClipPlayable.Create(graph, key.animationClip);
                    cache.Add(key, entry);
                    return entry;
                }
            }
            public bool Contains(State key)
            {
                return cache.ContainsKey(key);
            }
            public void Dispose(State key)
            {
                if (key is null)
                    return;

                if (!cache.TryGetValue(key, out Playable value))
                    graph.DestroySubgraph(value);
            }
            public void Add(State key, Playable value)
            {
                if (cache.ContainsKey(key))
                    return;

                cache.Add(key, value);
            }
            public void Remove(State key)
            {
                if (key is null)
                    return;

                if (!cache.ContainsKey(key))
                    return;

                cache.Remove(key);
            }
        }
    }


}
