using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR

using UnityEditor;


namespace Lightbug.CharacterControllerPro.Implementation
{
 

[CustomEditor( typeof(CharacterBrain) ) , CanEditMultipleObjects]
public class CharacterBrainEditor : Editor
{    
    
    SerializedProperty isAI = null;

    SerializedProperty inputData = null;

    SerializedProperty inputHandlerSettings = null;

    SerializedProperty aiBehaviour = null;

    SerializedProperty characterActions = null;


    void OnEnable()
    {
        isAI = serializedObject.FindProperty("isAI");
        inputData = serializedObject.FindProperty("inputData");        
        inputHandlerSettings = serializedObject.FindProperty("inputHandlerSettings");
        characterActions = serializedObject.FindProperty("characterActions");

        aiBehaviour = serializedObject.FindProperty("aiBehaviour");
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CustomUtilities.DrawMonoBehaviourField<CharacterBrain>( (CharacterBrain)target);

        GUILayout.Space(10);



        GUILayout.BeginHorizontal();

        GUI.color = isAI.boolValue ? Color.white : Color.green;
        if( GUILayout.Button( "Human" , EditorStyles.miniButton ) )
        {
            isAI.boolValue = false;
        }
        
        GUI.color = !isAI.boolValue ? Color.white : Color.green;
        if( GUILayout.Button( "AI" , EditorStyles.miniButton ) )
        {
            isAI.boolValue = true;
        }

        GUI.color = Color.white;


        GUILayout.EndHorizontal();

        CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

        GUILayout.Space(15);

        if( isAI.boolValue )
        {
            EditorGUILayout.PropertyField( aiBehaviour );

            GUILayout.Space(10);

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

        }
        else
        {
            EditorGUILayout.PropertyField( inputHandlerSettings );
            GUILayout.Space(10);
            
        }   

        GUI.enabled = false;
        EditorGUILayout.PropertyField( characterActions , true );
        GUI.enabled = true;

        GUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
    }

    
    
}

}

#endif
