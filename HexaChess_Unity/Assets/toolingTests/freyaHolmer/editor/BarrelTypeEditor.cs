using UnityEditor;
using UnityEngine;

namespace toolingTests.freyaHolmer
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BarrelType))]
    public class BarrelTypeEditor : Editor
    {
        SerializedObject so = null;
        SerializedProperty propRadius = null;
        SerializedProperty propDamage = null;
        SerializedProperty propColor = null;

        void OnEnable()
        {
            so = serializedObject;
            propRadius = so.FindProperty("radius");
            propDamage = so.FindProperty("damage");
            propColor = so.FindProperty("color");
        }

        public enum Things { Thing0, Thing1, Thing2 };
        Things things;
        float someValue;
        Color selectedColor = Color.white;

        public override void OnInspectorGUI()
        {
            BarrelType barrelType = (BarrelType)target;
            /// explicit positioning using Rect
            // GUI
            // EditorGUI
            /// implicit positioning, auto-layout
            // GUILayout
            // EditorGUILayout

            // base.OnInspectorGUI();
            ///How to mark the object as dirty when updating value + supporting undo
            //float newRadius = EditorGUILayout.FloatField("radius", barrelType.radius);
            /// Manual solution
            //if (newRadius != barrelType.radius)
            //{
            //    Undo.RecordObject(barrelType, "undo radius update");
            //    barrelType.radius = newRadius;
            //    // EditorUtility.SetDirty(barrelType);
            //}
            /// Automatic solution
            so.Update();
            EditorGUILayout.PropertyField(propRadius);
            EditorGUILayout.PropertyField(propDamage);
            EditorGUILayout.PropertyField(propColor);
            so.ApplyModifiedProperties();

            barrelType.radius = EditorGUILayout.FloatField("radius", barrelType.radius);
            // barrelType.damage = EditorGUILayout.FloatField("radius", barrelType.damage);
            // barrelType.color = EditorGUILayout.ColorField("color", barrelType.color);

            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(10, 300, 200, 20), "start of custom inspector ?");
            GUILayout.Label("alternate text");
            if (GUILayout.Button("Trigger OnValidate"))
            {
                barrelType.TriggerValidated?.Invoke();
            }
            GUILayout.EndHorizontal();

            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                things = (Things)EditorGUILayout.EnumPopup(things, GUILayout.Width(100));
                someValue = GUILayout.HorizontalSlider(someValue, 0f, 100f);
            }


            GUILayout.Label("styles", EditorStyles.boldLabel);
            GUILayout.Label("styles", EditorStyles.colorField);
            selectedColor = EditorGUILayout.ColorField("Pick a color", selectedColor);
            var transform = EditorGUILayout.ObjectField("assign here", null, typeof(Transform), true);
        }
    }
}
