using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ComboTree
{

    [Serializable]
    public class SerializedState : ScriptableObject
    { 
        public const string EntryName = "Entry";
        public const string AnyName = "Any";
        public const string ExitName = "Exit";

        [NonReorderable]
        public List<SerializedTransition> transitions = new List<SerializedTransition>();
        public AnimationClip animationClip;
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

        public static SerializedState State(string name, Vector2 position)
        {
            var state = CreateInstance<SerializedState>();
            state.name = name;
            state.position = position;
            return state;
        }
        public static SerializedState State(string name)
        {
            return State(name, Vector2.zero);
        }
        public static SerializedState Entry()
        {
            return State(EntryName);
        }
        public static SerializedState Entry(Vector2 position)
        {
            return State(EntryName, position);
        }
        public static SerializedState Any()
        {
            return State(AnyName);
        }
        public static SerializedState Any(Vector2 position)
        {
            return State(AnyName, position);
        }
        public static SerializedState Exit()
        {
            var state = State(ExitName);
            state.transitions = null;
            return state;
        }
        public static SerializedState Exit(Vector2 position)
        {
            var state = State(ExitName, position);
            state.transitions = null;
            return state;
        }


        public virtual bool CanTransitionTo(SerializedState target)
        {
            return !((IsEntry && target.IsExit) || IsExit);
        }
    }
}
