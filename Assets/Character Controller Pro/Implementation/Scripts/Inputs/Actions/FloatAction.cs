using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Implementation
{

[System.Serializable]
public struct FloatAction
{
    
    public float value;      

    /// <summary>
    /// Resets the values.
    /// </summary>
    public void Reset()
    {
        value = 0f;
    }

    /// <summary>
    /// Updates the fields based on the current button state.
    /// </summary>
    public void Update( float value )
    {
        this.value = value;        
    }
   
}


// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( FloatAction ) )]
public class FloatActionEditor : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( position , label , property );
        
        SerializedProperty value = property.FindPropertyRelative("value");
               
        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width = 100;

        EditorGUI.LabelField( fieldRect , label );
        
        fieldRect.x += 110;        

        EditorGUI.PropertyField( fieldRect , value , GUIContent.none );
        

        EditorGUI.EndProperty();
    }

   
 
}

#endif

}