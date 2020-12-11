using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Implementation
{

[System.Serializable]
public struct Vector2Action
{
    
    public Vector2 value;

    public void Reset()
    {       
        value = Vector2.zero;
    }

    public void Update( float valueX , float valueY )
    {
        value = new Vector2( valueX , valueY );
        value = Vector2.ClampMagnitude( value , 1f );
    }

    public void Update( Vector2 value )
    {
        this.value = Vector2.ClampMagnitude( value , 1f );
    }

    
    public bool Detected
    {
        get
        {
            return value != Vector2.zero;
        }
    }

	public bool Right
    {
        get
        {
            return value.x > 0;
        }
    }

	public bool Left
    {
        get
        {
            return value.x < 0;
        }
    }

	public bool Up
    {
        get
        {
            return value.y > 0;
        }
    }

	public bool Down
    {
        get
        {
            return value.y < 0;
        }
    }

    
}


// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR


[CustomPropertyDrawer( typeof( Vector2Action ) )]
public class Vector2ActionEditor : PropertyDrawer
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