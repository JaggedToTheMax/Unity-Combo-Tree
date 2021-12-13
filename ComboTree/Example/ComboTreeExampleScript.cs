using UnityEngine;
using ComboTree;
using UnityEditor;

public class ComboTreeExampleScript : MonoBehaviour
{
    public ComboAnimator animator;
}

[CustomEditor(typeof(ComboTreeExampleScript))]
public class ComboTreeExampleInspector : Editor
{
    ComboTreeExampleScript t => target as ComboTreeExampleScript;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Button("Primary");
        Button("Secondary");
        Button("Dodge Left");
        Button("Dodge Right");
    }

    void Button(string name)
    {
        if (GUILayout.Button(name))
            t.animator.Transition(name);
    }
}