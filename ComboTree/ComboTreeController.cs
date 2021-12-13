using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace ComboTree
{
    [CreateAssetMenu()]
    public class ComboTreeController : ScriptableObject
    {
        public List<SerializedState> States
        {
            get
            {
                if(scriptableStates is null)
                {
                    scriptableStates = new List<SerializedState>();

                    var entry = AddState(SerializedState.EntryName);
                    entry.position = new Vector2(75, 200);
                    var any = AddState(SerializedState.AnyName);
                    any.position = new Vector2(75, 25);
                    var exit = AddState(SerializedState.ExitName);
                    exit.position = new Vector2(500, 200);
                }

                return scriptableStates;
            }
        }

        [SerializeField] 
        [HideInInspector]
        List<SerializedState> scriptableStates;

        public SerializedState AddState(string name)
        {
            SerializedState state;

            switch (name)
            {
                case SerializedState.EntryName:
                    state = SerializedState.Entry();
                    break;
                case SerializedState.AnyName:
                    state = SerializedState.Any();
                    break;
                case SerializedState.ExitName:
                    state = SerializedState.Exit();
                    break;
                default:
                    state = SerializedState.State(name);
                    break;
            }
            scriptableStates.Add(state);
            state.hideFlags = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(state, this);
            AssetDatabase.SaveAssets();

            return state;
        }
        public void RemoveState(SerializedState state)
        {
            if (state is null)
                return;

            scriptableStates.Remove(state);
            DestroyImmediate(state, true);
        }
    }
}