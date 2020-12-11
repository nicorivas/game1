#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;



namespace Lightbug.Utilities
{

[CustomPropertyDrawer(typeof(Range_NoSliderAttribute))]
public class Range_NoSliderAttributeEditor : PropertyDrawer
{
	Range_NoSliderAttribute target;
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{		
		if( target == null )
			target = attribute as Range_NoSliderAttribute;

		if( property.propertyType == SerializedPropertyType.Integer )
		{
			property.intValue = Mathf.Clamp( property.intValue , target.minInteger , target.maxInteger );
		}
		else if( property.propertyType == SerializedPropertyType.Float )
		{
			property.floatValue = Mathf.Clamp( property.floatValue , target.minFloat , target.maxFloat );
		}
		else if( property.propertyType == SerializedPropertyType.Vector2 )
		{
			property.vector2Value = new Vector2( 
				Mathf.Clamp( property.vector2Value.x , target.minFloat , target.maxFloat ) ,
				Mathf.Clamp( property.vector2Value.y , target.minFloat , target.maxFloat )
			);
		}
		else if( property.propertyType == SerializedPropertyType.Vector3 )
		{
			property.vector3Value = new Vector3( 
				Mathf.Clamp( property.vector3Value.x , target.minFloat , target.maxFloat ) ,
				Mathf.Clamp( property.vector3Value.y , target.minFloat , target.maxFloat ) ,
				Mathf.Clamp( property.vector3Value.z , target.minFloat , target.maxFloat )
			);
		}
		else
		{
			GUI.Label( position , "This attribute doesn't work properly with the chosen field type." );
			return;
		}

		EditorGUI.PropertyField( position , property );
	}
}

}

#endif
