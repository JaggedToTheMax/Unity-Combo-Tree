using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ComboTree.Editor
{
    [CustomEditor(typeof(EditorTransitionWrapper))]
    public class TransitionWrapperEditor : UnityEditor.Editor
    {
        SerializedProperty prop;
        SerializedObject sO;
        string propertyPath;

        SerializedTransition t
        {
            get
            {
                if (target == null)
                    return null;
                return ((EditorTransitionWrapper)target).editorTransition;
            }
        }
        string path
        {
            get
            {
                return ComboTreeEditorWindow.AssetPath + "Editors/";
            }
        }

        protected override void OnHeaderGUI()
        {
            Repaint();
        }
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "TransitionEditor.uxml").CloneTree(root);

            root.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("d_AnimatorStateTransition Icon").image as Texture2D);

            var nameField = root.Q<TextField>("NameField");
            var nameProp = prop.FindPropertyRelative("name");

            nameField.BindProperty(nameProp);
            nameField.isDelayed = true;
            nameField.RegisterValueChangedCallback(e =>
            {
                var newValue = t.ValidateName(e.newValue);
                nameProp.stringValue = newValue;
                nameField.SetValueWithoutNotify(newValue);
                sO.ApplyModifiedProperties();
            });

            var canInterrupt = root.Q<Toggle>("CanInterrupt");
            canInterrupt.bindingPath = propertyPath + canInterrupt.bindingPath;

            var hasIntpuWindow = root.Q<Toggle>("HasInputWindow");
            hasIntpuWindow.bindingPath = propertyPath + hasIntpuWindow.bindingPath;

            var inputStart = root.Q<FloatField>("InputStart");
            inputStart.bindingPath = propertyPath + inputStart.bindingPath;
            inputStart.SetEnabled(t.hasInputWindow);

            var inputEnd = root.Q<FloatField>("InputEnd");
            inputEnd.bindingPath = propertyPath + inputEnd.bindingPath;
            inputEnd.SetEnabled(t.hasInputWindow);

            var hasExitTime = root.Q<Toggle>("HasExitTime");
            hasExitTime.bindingPath = propertyPath + hasExitTime.bindingPath;

            var exitTime = root.Q<FloatField>("ExitTime");
            exitTime.bindingPath = propertyPath + exitTime.bindingPath;
            exitTime.SetEnabled(t.transitionParameters.hasExitTime);

            var transitionTime = root.Q<FloatField>("TransitionTime");
            transitionTime.bindingPath = propertyPath + transitionTime.bindingPath;

            var transitionOffset = root.Q<FloatField>("TransitionOffset");
            transitionOffset.bindingPath = propertyPath + transitionOffset.bindingPath;

            hasIntpuWindow.RegisterValueChangedCallback(e =>
            {
                inputStart.SetEnabled(e.newValue);
                inputEnd.SetEnabled(e.newValue);
            });
            inputStart.RegisterValueChangedCallback(e =>
            {
                if (e.newValue < 0)
                    inputStart.value = Mathf.Clamp(e.newValue, 0, float.MaxValue);
                if (e.newValue > inputEnd.value)
                    inputEnd.value = e.newValue;
            });
            inputEnd.RegisterValueChangedCallback(e =>
            {
                if (e.newValue < 0)
                    inputEnd.value = Mathf.Clamp(e.newValue, 0, float.MaxValue);
                else if (e.newValue < inputStart.value)
                    inputStart.value = e.newValue;
                if (e.newValue > exitTime.value)
                    exitTime.value = e.newValue;
            });
            hasExitTime.RegisterValueChangedCallback(e =>
            {
                exitTime.SetEnabled(e.newValue);
            });
            exitTime.RegisterValueChangedCallback(e =>
            {
                if (e.newValue < 0)
                    exitTime.value = Mathf.Clamp(e.newValue, 0, float.MaxValue);
                else if (e.newValue < inputEnd.value)
                    inputEnd.value = e.newValue;
            });
            transitionTime.RegisterValueChangedCallback(e =>
            {
                if (e.newValue < 0)
                    transitionTime.value = Mathf.Clamp(e.newValue, 0, float.MaxValue);
            });
            transitionOffset.RegisterValueChangedCallback(e =>
            {
                if (e.newValue < 0)
                    transitionOffset.value = Mathf.Clamp(e.newValue, 0, float.MaxValue);
            });

            root.Bind(sO);
            return root;
        }
        public override bool UseDefaultMargins()
        {
            return false;
        }

        void ClampPropertyValue(SerializedProperty property)
        {
            property.floatValue = Mathf.Clamp(property.floatValue, 0, float.MaxValue);
        }
        private void OnEnable()
        {
            var i = t.Owner.transitions.IndexOf(t);
            sO = new SerializedObject(t.Owner);
            sO.Update();
            prop = sO.FindProperty("transitions").GetArrayElementAtIndex(i);
            propertyPath = prop.propertyPath + ".";
        }
    }
}