using UnityEditor;
using UnityEditor.Callbacks;

namespace ComboTree.Editor
{
    [CustomEditor(typeof(ComboTreeController))]
    public partial class ComboTreeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
        }
        [OnOpenAsset()]
        static bool OpenEditor(int instanceID, int _)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is ComboTreeController controller)
            {
                ComboTreeEditorWindow.Edit(controller);
                return true;
            }

            return false;
        }
    }
}
