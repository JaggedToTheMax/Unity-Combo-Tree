using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using ComboTree;

namespace ComboTree.Editor
{
    public class TransitionView : Edge
    {
        public SerializedTransition transition;



        public override void OnSelected()
        {
            if (transition is null)
                return;

            var wrapper = ScriptableObject.CreateInstance<EditorTransitionWrapper>();
            wrapper.editorTransition = transition;

            Selection.activeObject = wrapper;
        }
        public override void OnUnselected()
        {
            if(Selection.activeObject is EditorTransitionWrapper wrapper && wrapper.editorTransition == transition)
            {
                Selection.activeObject = null;
                Object.DestroyImmediate(wrapper);
            }
        }
    }
}