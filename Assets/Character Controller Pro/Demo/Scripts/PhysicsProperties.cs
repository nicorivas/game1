using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Demo
{

[System.Serializable]
public struct SurfacePhysicsProperties
{
    [Min(0f)]
    public float acceleration;

    // [Min(0f)]
    // public float planarDrag;

    [Min(0f)]
    public float planarDrag;

    public SurfacePhysicsProperties( float acceleration , float planarDrag )
    {
        this.acceleration = acceleration;
        this.planarDrag = planarDrag;
    }
}

[System.Serializable]
public struct VolumePhysicsProperties
{
    [Min(0f)]
    public float acceleration;

    [Min(0f)]
    public float planarDrag;

    [Min(0f)]
    public float verticalDrag;

    public VolumePhysicsProperties( float acceleration , float planarDrag , float verticalDrag )
    {
        this.acceleration = acceleration;
        this.planarDrag = planarDrag;
        this.verticalDrag = verticalDrag;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( SurfacePhysicsProperties ) )]
public class SurfacePhysicsPropertiesEditor : PropertyDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( position , label , property );

        SerializedProperty acceleration = property.FindPropertyRelative("acceleration");
        SerializedProperty planarDrag = property.FindPropertyRelative("planarDrag");
        SerializedProperty verticalDrag = property.FindPropertyRelative("planarDrag");

        GUI.Box( position , GUIContent.none , EditorStyles.helpBox );

        Rect fieldRect = position;
        
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width -= 10f;
        fieldRect.x += 5f;
        fieldRect.y += 0.5f * EditorGUIUtility.singleLineHeight;
        
        EditorGUI.LabelField( fieldRect , label );
        fieldRect.y += 1.5f * fieldRect.height;

        EditorGUI.PropertyField( fieldRect , acceleration );
        fieldRect.y += fieldRect.height;

        EditorGUI.PropertyField( fieldRect , planarDrag );
        
        fieldRect.y += 2f * fieldRect.height;

        float maxPlanarSpeed = acceleration.floatValue == 0f && planarDrag.floatValue == 0f ? 0f : acceleration.floatValue * ( 1 / planarDrag.floatValue - Time.fixedDeltaTime );

        GUI.enabled = false;

        EditorGUI.LabelField( fieldRect , "Planar speed = " + maxPlanarSpeed.ToString("F3") );
        fieldRect.y += fieldRect.height;

        GUI.enabled = true;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 6.5f * EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer( typeof( VolumePhysicsProperties ) )]
public class VolumePhysicsPropertiesEditor : PropertyDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( position , label , property );

        SerializedProperty acceleration = property.FindPropertyRelative("acceleration");
        SerializedProperty planarDrag = property.FindPropertyRelative("planarDrag");
        SerializedProperty verticalDrag = property.FindPropertyRelative("verticalDrag");

        GUI.Box( position , GUIContent.none , EditorStyles.helpBox );

        Rect fieldRect = position;
        
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width -= 10f;
        fieldRect.x += 5f;
        fieldRect.y += 0.5f * EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField( fieldRect , label );
        fieldRect.y += 1.5f * fieldRect.height;

        EditorGUI.PropertyField( fieldRect , acceleration );
        fieldRect.y += fieldRect.height;

        EditorGUI.PropertyField( fieldRect , planarDrag );
        fieldRect.y += fieldRect.height;

        EditorGUI.PropertyField( fieldRect , verticalDrag );
        
        fieldRect.y += 2f * fieldRect.height;

        float maxPlanarSpeed = acceleration.floatValue == 0f && planarDrag.floatValue == 0f ? 0f : acceleration.floatValue * ( 1 / planarDrag.floatValue - Time.fixedDeltaTime );
        float maxVerticalSpeed = acceleration.floatValue == 0f && verticalDrag.floatValue == 0f ? 0f : 10f * ( 1 / verticalDrag.floatValue - Time.fixedDeltaTime );

        GUI.enabled = false;

        EditorGUI.LabelField( fieldRect , "Planar speed = " + maxPlanarSpeed.ToString("F3") );
        fieldRect.y += fieldRect.height;

        EditorGUI.LabelField( fieldRect , "Vertical speed = " + maxVerticalSpeed.ToString("F3") );
        fieldRect.y += fieldRect.height;

        GUI.enabled = true;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 8.5f * EditorGUIUtility.singleLineHeight;
    }
}

#endif



}