using System.Collections;
using System.Collections.Generic;
using PlayableAnimation;

namespace ComboTree
{
    public class RuntimeComboTree
    {
        public ComboTreeController comboTree;
        public List<ComboState> states;
        public ComboState entry = new ComboState(null, "Entry");
        public ComboState any = new ComboState(null, "Any");

        public RuntimeComboTree (ComboTreeController comboTree)
        {
            var dict = new Dictionary<serializedState, ComboState>();
            this.comboTree = comboTree;

            foreach (var state in comboTree.States)
            {
                if (dict.ContainsKey(state))
                    continue;

                switch (state.name)
                {
                    case serializedState.EntryName:
                        dict.Add(state, entry);
                        break;
                    case serializedState.AnyName:
                        dict.Add(state, any);
                        break;
                    case serializedState.ExitName:
                        break;
                    default:
                        dict.Add(state, new ComboState(state.animationClip, state.name) { returnToDefault = state.returnToDefault });
                        break;
                }
            }

            foreach (var state in comboTree.States)
            {
                if (state.transitions is null || !dict.ContainsKey(state))
                    continue;
                foreach (var transition in state.transitions)
                {
                    var newTransition = new ComboTransition(transition.transitionParameters, transition.Name)
                    {
                        target = transition.Target.IsExit ? entry : dict[transition.Target],
                        inputWindowStart = transition.inputWindowStart,
                        inputWindowEnd = transition.inputWindowEnd,
                        hasInputWindow = transition.hasInputWindow
                    };

                    dict[state].transitions.Add(newTransition.nameHash, newTransition);
                }
            }

            states = new List<ComboState>();
            foreach (var item in dict)
            {
                states.Add(item.Value);
            }
        }

    }
}
