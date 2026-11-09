using LocalSM;
using UnityEditor;
using UnityEngine;

namespace LocalSM.EditorTools
{
    /// <summary>
    /// Custom inspector for <see cref="LocalStateMachine"/>. In play mode it shows a live
    /// readout of the current / previous / next state and lets you jump to any state with a button.
    /// Outside play mode it just draws the default inspector.
    /// </summary>
    [CustomEditor(typeof(LocalStateMachine))]
    public class LocalStateMachineEditor : Editor
    {
        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            var sm = (LocalStateMachine)target;

            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Live State", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Current",  Name(sm.currentStateNode));
                EditorGUILayout.LabelField("Previous", Name(sm.previousStateNode));
                EditorGUILayout.LabelField("Next",     Name(sm.nextStateNode));
                EditorGUILayout.Space();

                foreach (var s in sm.states)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        bool active = s == sm.currentStateNode;
                        EditorGUILayout.LabelField(active ? "▶ " + Name(s) : Name(s));
                        using (new EditorGUI.DisabledScope(active || s == null))
                            if (GUILayout.Button("Go", GUILayout.Width(40)))
                                sm.SetState(s);
                    }
                }
                EditorGUILayout.Space();
            }

            DrawDefaultInspector();
        }

        private static string Name(LocalStateNode n) => n ? n.name : "<none>";
    }
}
