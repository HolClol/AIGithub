using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(CharacteristicTraits))]
public class CustomTraitEnumFilter : Editor
{
    private SerializedProperty _nameProp;
    private SerializedProperty _statsProp;
    private SerializedProperty _oppositeProp;

    private ReorderableList _list;

    private void OnEnable()
    {
        _nameProp = serializedObject.FindProperty("TraitName");
        _statsProp = serializedObject.FindProperty("Stats");
        _oppositeProp = serializedObject.FindProperty("OppositeTrait");

        // set up the ReorderableList
        _list = new ReorderableList(serializedObject, _statsProp, draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true);

        _list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Value Changers");
        };

        _list.elementHeightCallback = index =>
        {
            // two lines (Stats + Value/Percentage), plus padding
            return EditorGUIUtility.singleLineHeight * 2 + 8;
        };

        _list.drawElementCallback = (rect, index, active, focused) =>
        {
            var element = _statsProp.GetArrayElementAtIndex(index);
            var statsProp = element.FindPropertyRelative("Stats");
            var valueProp = element.FindPropertyRelative("Value");
            var pctProp = element.FindPropertyRelative("PercentageValue");

            // split the rect into two lines
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var r1 = new Rect(rect.x, rect.y + 2, rect.width, lineHeight);
            var r2 = new Rect(rect.x, rect.y + 2 + lineHeight, rect.width, lineHeight);

            // 1) Stats enum
            EditorGUI.PropertyField(r1, statsProp, GUIContent.none);

            // 2) filtered field
            var stat = (Stat)statsProp.enumValueIndex;
            if (ShouldShowPercentage(stat))
                EditorGUI.PropertyField(r2, pctProp, new GUIContent("Percentage"));
            else
                EditorGUI.PropertyField(r2, valueProp, new GUIContent("Value"));
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
   
        EditorGUILayout.PropertyField(_nameProp); // Draw the string name
        EditorGUILayout.PropertyField(_oppositeProp); // Draw the opposite trait
        EditorGUILayout.PropertyField(_statsProp.FindPropertyRelative("Array.size"), new GUIContent("Stats Size")); // Draw the array size field so user can resize:
        
        _list.DoLayoutList(); // Draw the whole list in one block

        serializedObject.ApplyModifiedProperties();   
    }

    private bool ShouldShowPercentage(Stat stat)
    {
        // <-- Put your enum-filter logic here.
        // For example:
        switch (stat)
        {
            case Stat.WorkFailChance:
            case Stat.WorkAbandonChance:
            case Stat.WorkBonusChance:
            case Stat.WanderOffChance:
            case Stat.AssistChance:
                return true;    // show the PercentageValue slider
            default:
                return false;   // show the raw Value field
        }
    }
}
