using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{

public enum SequenceType
{
    Duration ,
    OnWallHit
}

/// <summary>
/// This class represents a sequence action, executed by the AI brain in sequence behaviour mode.
/// </summary>
[System.Serializable]
public class CharacterAIAction
{
    public SequenceType sequenceType;

	[Range_NoSlider( true )]
	public float duration = 1;

	public CharacterActions action = new CharacterActions();
    
}

}