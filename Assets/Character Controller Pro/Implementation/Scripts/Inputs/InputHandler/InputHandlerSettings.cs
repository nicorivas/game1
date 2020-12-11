using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Implementation
{

[System.Serializable]
public class InputHandlerSettings
{
	[SerializeField]
	InputHandler inputHandler = null;

	[SerializeField]
	HumanInputType humanInputType = HumanInputType.InputManager;

    public InputHandler InputHandler
    {
        get
        {
            return inputHandler;
        }
    }

	public void Initialize( GameObject gameObject )
	{
		if( inputHandler == null )
			inputHandler = InputHandler.CreateInputHandler( gameObject , humanInputType );
	}
}

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( InputHandlerSettings ) )]
public class InputHandlerSettingsEditor : PropertyDrawer
{

	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		EditorGUI.BeginProperty( position , label , property );

		SerializedProperty inputHandler = property.FindPropertyRelative("inputHandler");
		SerializedProperty humanInputType = property.FindPropertyRelative("humanInputType");		

		Rect fieldRect = position;
		fieldRect.height = EditorGUIUtility.singleLineHeight;

		CustomUtilities.DrawEditorHorizontalLine( ref fieldRect , Color.gray );

		EditorGUI.PropertyField( fieldRect , humanInputType );

		fieldRect.y += fieldRect.height;

		switch( (HumanInputType)humanInputType.enumValueIndex )
		{
			case HumanInputType.InputManager:				
				
				break;
			case HumanInputType.UIMobile:

				fieldRect.height = 3f * EditorGUIUtility.singleLineHeight;
				EditorGUI.HelpBox( 
					fieldRect , 
					"This mode will automatically search for buttons and axes from the scene. " + 
					"Make sure these elements \"action names\" match with the character actions you want to trigger." , 
					MessageType.Info 
				);

				fieldRect.y += fieldRect.height;
				
				break;
			case HumanInputType.Custom:

				
				EditorGUI.PropertyField( fieldRect , inputHandler );
				fieldRect.y += fieldRect.height;

				fieldRect.height = 3f * EditorGUIUtility.singleLineHeight;
				EditorGUI.HelpBox(
					fieldRect ,
					"Use your own InputHandler component (Monobehaviour)." , 
					MessageType.Info 
				);

				fieldRect.y += fieldRect.height;

				
				
				break;
		}

		fieldRect.height = EditorGUIUtility.singleLineHeight;
		
		CustomUtilities.DrawEditorHorizontalLine( ref fieldRect , Color.gray );

		EditorGUI.EndProperty();
         
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		SerializedProperty inputHandler = property.FindPropertyRelative("inputHandler");
		SerializedProperty humanInputType = property.FindPropertyRelative("humanInputType");	

		float height = 0f;

		switch( (HumanInputType)humanInputType.enumValueIndex )
		{
			case HumanInputType.InputManager:
				height = EditorGUIUtility.singleLineHeight;
				break;
			
			case HumanInputType.UIMobile:				
				height = 4f * EditorGUIUtility.singleLineHeight;
				break;
			
			default:			
				height = 5f * EditorGUIUtility.singleLineHeight;
				break;
		}

		height += 25f;

		return height;

		
	}

	public override bool CanCacheInspectorGUI(SerializedProperty property)
	{
		return false;
	}
}
#endif

}