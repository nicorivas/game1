#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

[CustomEditor( typeof( CharacterActionsAsset ) )]
public class CharacterActionsAssetEditor : Editor
{
    const string TemplateTextFilePath = "Assets/Character Controller Pro/Implementation/Resources/template-character-actions.txt";
    const string DirectoryPath = "Assets/Character Controller Pro/Implementation/Scripts/Character/Actions/";
    const string FileName = "CharacterActions";
    const string FullFileName = FileName + ".cs";

    SerializedProperty boolActions = null;
    SerializedProperty floatActions = null;
    SerializedProperty vector2Actions = null;
    

    void OnEnable()
    {
        boolActions = serializedObject.FindProperty( "boolActions" );
        floatActions = serializedObject.FindProperty( "floatActions" );
        vector2Actions = serializedObject.FindProperty( "vector2Actions" );
    }

    public override void OnInspectorGUI()
    {
        CustomUtilities.DrawScriptableObjectField<CharacterActionsAsset>( (CharacterActionsAsset)target );

        serializedObject.Update();

        EditorGUILayout.PropertyField( boolActions , true );
        EditorGUILayout.PropertyField( floatActions , true );
        EditorGUILayout.PropertyField( vector2Actions , true );

        CustomUtilities.DrawEditorLayoutHorizontalLine( Color.gray );

        EditorGUILayout.HelpBox( 
            "Click the button to replace the original \"CharacterActions.cs\" file. This can be useful if you need to create custom actions, without modifing the code. " , 
            MessageType.Info 
        ); 

        if( GUILayout.Button("Create actions") )
        {
            bool result = EditorUtility.DisplayDialog( 
                "Create actions" , 
                "Warning: This will replace the original \"CharacterActions\" file. Are you sure you want to continue?" , "Yes" , "No" );
            
            if( result )
                CreateCSharpClass();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void CreateCSharpClass()
    {
        string output = GenerateOutput( FileName );        


        if( !Directory.Exists( DirectoryPath ) )
        {
            Debug.Log( "Missing directory : " + DirectoryPath + ". Reimport the package." );
            return;
        }
        
        FileStream fileStream = File.Open( DirectoryPath + FullFileName , FileMode.Truncate, FileAccess.ReadWrite );        

        StreamWriter file = new StreamWriter( fileStream );
        
        file.Write( output );
        file.Close();

        AssetDatabase.Refresh();


    }
    

    string GenerateOutput( string className )
    {
        StreamReader reader = new StreamReader( TemplateTextFilePath );

        string output = reader.ReadToEnd();
        reader.Close();

        output = Regex.Replace( output , @"@\s*struct-name\s*@" , FileName );

        // -----------------------------------------------------------------------------------------------------------------------------------
        // Bool Actions ----------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------------------------------------------------------

        string definitionsString = "";
        string resetString = "";
        string newString = "";
        string setValueString = "";
        string copyValueString = "";
        string updateString = "";

        for( int i = 0 ; i < boolActions.arraySize ; i++ )
        {            
            string actionName = boolActions.GetArrayElementAtIndex( i ).stringValue;

            string[] words = actionName.Split( ' ' );

            string variableName = "@";
            for( int j = 0 ; j < words.Length ; j++ )
            {
                string word = words[j];

                if( j == 0 )
                    variableName += System.Char.ToLowerInvariant( word[0] ) + word.Substring( 1 ).ToLower();
                else
                    variableName += System.Char.ToUpperInvariant( word[0] ) + word.Substring( 1 ).ToLower();
            }
            
                        
            definitionsString += "\tpublic BoolAction " + variableName + ";\n";
            resetString += "\t\t" + variableName + ".Reset();\n";
            newString += "\t\t" + variableName + " = new BoolAction();\n";
            setValueString += "\t\t" + variableName + ".value = inputHandler.GetBool( \"" + actionName + "\" );\n";
            copyValueString += "\t\t" + variableName + ".value = characterActions." + variableName.Substring(1) + ".value;\n";
            updateString += "\t\t" + variableName + ".Update();\n";
        }
        
        // Write bool actions
        output = Regex.Replace( output , @"@\s*bool-actions-definitions\s*@" , definitionsString );
        output = Regex.Replace( output , @"@\s*bool-actions-reset\s*@" , resetString );
        output = Regex.Replace( output , @"@\s*bool-actions-new\s*@" , newString );
        output = Regex.Replace( output , @"@\s*bool-actions-setValue\s*@" , setValueString );
        output = Regex.Replace( output , @"@\s*bool-actions-copyValue\s*@" , copyValueString );
        output = Regex.Replace( output , @"@\s*bool-actions-update\s*@" , updateString );

        // -----------------------------------------------------------------------------------------------------------------------------------
        // Float Actions ---------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------------------------------------------------------

        definitionsString = "";
        resetString = "";
        newString = "";
        setValueString = "";
        copyValueString = "";
        updateString = "";

        for( int i = 0 ; i < floatActions.arraySize ; i++ )
        {            
            string actionName = floatActions.GetArrayElementAtIndex( i ).stringValue;

            string[] words = actionName.Split( ' ' );

            string variableName = "@";
            for( int j = 0 ; j < words.Length ; j++ )
            {
                string word = words[j];

                if( j == 0 )
                    variableName += System.Char.ToLowerInvariant( word[0] ) + word.Substring( 1 ).ToLower();
                else
                    variableName += System.Char.ToUpperInvariant( word[0] ) + word.Substring( 1 ).ToLower();
            }
            
                        
            definitionsString += "\tpublic FloatAction " + variableName + ";\n";
            resetString += "\t\t" + variableName + ".Reset();\n";
            newString += "\t\t" + variableName + " = new FloatAction();\n";
            setValueString += "\t\t" + variableName + ".value = inputHandler.GetFloat( \"" + actionName + "\" );\n";
            copyValueString += "\t\t" + variableName + ".value = characterActions." + variableName.Substring(1) + ".value;\n";
        }
        
        // Write bool actions
        output = Regex.Replace( output , @"@\s*float-actions-definitions\s*@" , definitionsString );
        output = Regex.Replace( output , @"@\s*float-actions-reset\s*@" , resetString );
        output = Regex.Replace( output , @"@\s*float-actions-new\s*@" , newString );
        output = Regex.Replace( output , @"@\s*float-actions-setValue\s*@" , setValueString );
        output = Regex.Replace( output , @"@\s*float-actions-copyValue\s*@" , copyValueString );

        // -----------------------------------------------------------------------------------------------------------------------------------
        // Vector2 Actions -------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------------------------------------------------------

        definitionsString = "";
        resetString = "";
        newString = "";
        setValueString = "";
        updateString = "";

        for( int i = 0 ; i < vector2Actions.arraySize ; i++ )
        {            
            string actionName = vector2Actions.GetArrayElementAtIndex( i ).stringValue;

            string[] words = actionName.Split( ' ' );

            string variableName = "@";
            for( int j = 0 ; j < words.Length ; j++ )
            {
                string word = words[j];

                if( j == 0 )
                    variableName += System.Char.ToLowerInvariant( word[0] ) + word.Substring( 1 ).ToLower();
                else
                    variableName += System.Char.ToUpperInvariant( word[0] ) + word.Substring( 1 ).ToLower();
            }
            
                        
            definitionsString += "\tpublic Vector2Action " + variableName + ";\n";
            resetString += "\t\t" + variableName + ".Reset();\n";
            newString += "\t\t" + variableName + " = new Vector2Action();\n";
            setValueString += "\t\t" + variableName + ".value = inputHandler.GetVector2( \"" + actionName + "\" );\n";
            copyValueString += "\t\t" + variableName + ".value = characterActions." + variableName.Substring(1) + ".value;\n";
            
        }
        
        // Write bool actions
        output = Regex.Replace( output , @"@\s*vector2-actions-definitions\s*@" , definitionsString );
        output = Regex.Replace( output , @"@\s*vector2-actions-reset\s*@" , resetString );
        output = Regex.Replace( output , @"@\s*vector2-actions-new\s*@" , newString );
        output = Regex.Replace( output , @"@\s*vector2-actions-setValue\s*@" , setValueString );
        output = Regex.Replace( output , @"@\s*vector2-actions-copyValue\s*@" , copyValueString );

        return output;
        
    }

}

}

#endif
