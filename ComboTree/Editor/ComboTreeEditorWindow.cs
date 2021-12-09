using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace ComboTree.Editor
{
    public class ComboTreeEditorWindow : EditorWindow
    {
        public static string AssetPath
        {
            get
            {
                var longPath = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();

                return longPath.Remove(0, longPath.IndexOf("Assets")).Replace("ComboTreeEditorWindow.cs", "");
            }
        }

        ComboTreeController target;

        public static void EditSelected()
        {
            if (Selection.activeObject is ComboTreeController comboTree)
                Edit(comboTree);
        }
        public static void Edit(ComboTreeController comboTree)
        {
            var window = GetWindow<ComboTreeEditorWindow>();
            window.target = comboTree;
            window.Init();
        }


        void Init()
        {
            var root = rootVisualElement;
            var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath + "ComboTreeEditorWindow.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetPath + "ComboTreeEditorWindow.uss");

            root.Clear();
            treeAsset.CloneTree(root);

            root.styleSheets.Add(styleSheet);

            var graph = root.Q<TreeView>();
            graph.BindTree(target);
        }


        private void OnSelectionChange()
        {
            if (Selection.activeObject is ComboTreeController controller)
                Edit(controller);
        }
    }
}
