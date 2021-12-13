using PlayableAnimation;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ComboTree
{
    [Serializable]
    public class SerializedTransition
    {
        public SerializedState Owner => owner;
        public SerializedState Target => target;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = ValidateName(value);
            }
        }

        public TransitionParameters transitionParameters;
        public float inputWindowStart;
        public float inputWindowEnd;
        public bool hasInputWindow = false;
        public bool canInterrupt = false;

        [SerializeField] [HideInInspector] SerializedState owner;
        [SerializeField] [HideInInspector] SerializedState target;
        [SerializeField] string name;

        public SerializedTransition(SerializedState from, SerializedState to)
        {
            owner = from;
            target = to;
            from.transitions.Add(this);

            transitionParameters = new TransitionParameters()
            {
                transitionTime = 0.2f,
                exitTime = 0.8f,
                hasExitTime = true
            };

            Name = from.name + " -> " + to.name;
        }

        public string ValidateName(string name)
        {
            if (owner is null)
                return "No Owner";

            bool isValid = !owner.transitions.Exists(transition =>
            {
                if (transition == this)
                    return false;

                return transition.name == name;
            });


            if (isValid)
                return name;

            var last = Regex.Match(name, @"\d*\Z").Value;

            if (last.Equals(string.Empty))
                return ValidateName(name + "0");
            else
            {
                var number = int.Parse(last);
                var newName = name.Replace(last, (number + 1).ToString());

                return ValidateName(newName);
            }
        }
    }
}
