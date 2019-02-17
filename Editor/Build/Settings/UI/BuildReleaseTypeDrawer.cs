using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildReleaseType))]
public class BuildReleaseTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Limit valid characters.
        // TODO: This might not be necessary since name will need to be sanitized for different needs later (as an enum entry, pre-processor define, etc.)
        //char chr = Event.current.character;
        //if ((chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9') && chr != '-' && chr != '_' && chr != ' ')
        //{
        //    Event.current.character = '\0';
        //}

        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader(property.FindPropertyRelative("typeName").stringValue, ref show, false);
        property.isExpanded = show;

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            GUILayout.Label("Basic Info", UnityBuildGUIUtility.midHeaderStyle);

            SerializedProperty typeName = property.FindPropertyRelative("typeName");
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Type Name");
            typeName.stringValue = BuildProject.SanitizeFolderName(GUILayout.TextArea(typeName.stringValue));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(property.FindPropertyRelative("bundleIdentifier"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("productName"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("binaryNameOverride"), new GUIContent("Binary Name Override", "This will replace the product name in the binary file names"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("companyName"));

            GUILayout.Space(20);
            GUILayout.Label("Build Options", UnityBuildGUIUtility.midHeaderStyle);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("customDefines"));

            SerializedProperty buildOptions = property.FindPropertyRelative("buildOptions");
            
            bool enableHeadlessMode = ((BuildOptions)buildOptions.intValue & BuildOptions.EnableHeadlessMode) == BuildOptions.EnableHeadlessMode;
            bool developmentBuild = ((BuildOptions)buildOptions.intValue & BuildOptions.Development) == BuildOptions.Development;
            bool allowDebugging = ((BuildOptions)buildOptions.intValue & BuildOptions.AllowDebugging) == BuildOptions.AllowDebugging;
            
            enableHeadlessMode = EditorGUILayout.ToggleLeft(" Server Build", enableHeadlessMode);
            if (enableHeadlessMode) buildOptions.intValue |= (int)BuildOptions.EnableHeadlessMode;
            else buildOptions.intValue &= ~(int)BuildOptions.EnableHeadlessMode;
            
            developmentBuild = EditorGUILayout.ToggleLeft(" Development Build", developmentBuild);
            if (developmentBuild) buildOptions.intValue |= (int)BuildOptions.Development;
            else buildOptions.intValue &= ~(int)BuildOptions.Development;

            EditorGUI.BeginDisabledGroup(!developmentBuild);
            allowDebugging = EditorGUILayout.ToggleLeft(" Script Debugging", allowDebugging);
            EditorGUI.EndDisabledGroup();
            if (allowDebugging) buildOptions.intValue |= (int)BuildOptions.AllowDebugging;
            else buildOptions.intValue &= ~(int)BuildOptions.AllowDebugging;
            
            buildOptions.intValue = (int)(BuildOptions)EditorGUILayout.EnumFlagsField("Advanced", (BuildOptions)buildOptions.intValue);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("sceneList"));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.MaxWidth(150)))
            {
                BuildReleaseType[] types = BuildSettings.releaseTypeList.releaseTypes;
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].typeName == property.FindPropertyRelative("typeName").stringValue)
                    {
                        ArrayUtility.RemoveAt<BuildReleaseType>(ref BuildSettings.releaseTypeList.releaseTypes, i);
                        GUIUtility.keyboardControl = 0;
                        break;
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            property.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}