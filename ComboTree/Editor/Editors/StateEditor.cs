using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using PlayableAnimation;

namespace ComboTree.Editor
{
    [CustomEditor(typeof(serializedState))]
    public partial class StateEditor : UnityEditor.Editor
    {
        SerializedObject sO;

        serializedState t => target as serializedState;
        string path
        {
            get
            {
                return ComboTreeEditorWindow.AssetPath + "Editors/";
            }
        }

        protected override void OnHeaderGUI()
        {
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "StateEditor.uxml").CloneTree(root);
            root.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("d_AnimatorState Icon").image as Texture2D);

            root.Bind(sO);

            return root;
        }
        public override bool UseDefaultMargins()
        {
            return false;
        }

        private void OnEnable()
        {
            sO = new SerializedObject(target);
        }
    }
}
