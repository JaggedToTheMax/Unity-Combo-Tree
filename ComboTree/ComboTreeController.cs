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
        public List<serializedState> States
        {
            get
            {
                if(scriptableStates is null)
                {
                    scriptableStates = new List<serializedState>();

                    var entry = AddState(serializedState.EntryName);
                    entry.position = new Vector2(75, 200);
                    var any = AddState(serializedState.AnyName);
                    any.position = new Vector2(75, 25);
                    var exit = AddState(serializedState.ExitName);
                    exit.position = new Vector2(500, 200);
                }

                return scriptableStates;
            }
        }

        [SerializeField] 
        [HideInInspector]
        List<serializedState> scriptableStates;

        public serializedState AddState(string name)
        {
            serializedState state;

            switch (name)
            {
                case serializedState.EntryName:
                    state = serializedState.Entry();
                    break;
                case serializedState.AnyName:
                    state = serializedState.Any();
                    break;
                case serializedState.ExitName:
                    state = serializedState.Exit();
                    break;
                default:
                    state = serializedState.State(name);
                    break;
            }
            scriptableStates.Add(state);
            state.hideFlags = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(state, this);
            AssetDatabase.SaveAssets();

            return state;
        }
        public void RemoveState(serializedState state)
        {
            if (state is null)
                return;

            scriptableStates.Remove(state);
            DestroyImmediate(state, true);
        }
    }
}