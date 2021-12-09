using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ComboTree
{
    [Serializable]
    public class serializedState : ScriptableObject
    { 
        public const string EntryName = "Entry";
        public const string AnyName = "Any";
        public const string ExitName = "Exit";

        [NonReorderable]
        public List<SerializedTransition> transitions = new List<SerializedTransition>();
        public AnimationClip animationClip;
        public bool returnToDefault = true;
        public Vector2 position;
        public bool IsDefault
        {
            get
            {
                return IsEntry || IsAny || IsExit;
            }
        }
        public bool IsEntry
        {
            get
            {
                return name.Equals(EntryName);
            }
        }
        public bool IsAny
        {
            get
            {
                return name.Equals(AnyName);
            }
        }
        public bool IsExit
        {
            get
            {
                return name.Equals(ExitName);
            }
        }

        public static serializedState State(string name, Vector2 position)
        {
            var state = CreateInstance<serializedState>();
            state.name = name;
            state.position = position;
            return state;
        }
        public static serializedState State(string name)
        {
            return State(name, Vector2.zero);
        }
        public static serializedState Entry()
        {
            return State(EntryName);
        }
        public static serializedState Entry(Vector2 position)
        {
            return State(EntryName, position);
        }
        public static serializedState Any()
        {
            return State(AnyName);
        }
        public static serializedState Any(Vector2 position)
        {
            return State(AnyName, position);
        }
        public static serializedState Exit()
        {
            var state = State(ExitName);
            state.transitions = null;
            return state;
        }
        public static serializedState Exit(Vector2 position)
        {
            var state = State(ExitName, position);
            state.transitions = null;
            return state;
        }


        public virtual bool CanTransitionTo(serializedState target)
        {
            return !(IsDefault && target.IsDefault);
        }
    }
}
