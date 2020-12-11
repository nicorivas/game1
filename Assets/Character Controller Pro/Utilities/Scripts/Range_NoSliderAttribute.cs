using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// A min max range attribute without a slider interface.
/// </summary>
public class Range_NoSliderAttribute : PropertyAttribute
{
	public float minFloat;
	public float maxFloat;

	public int minInteger;
	public int maxInteger;

	public Range_NoSliderAttribute( bool positive )
	{
		if(positive)
		{
			minFloat = 0;
			maxFloat = Mathf.Infinity;

			minInteger = 0;
			maxInteger = int.MaxValue;
		}
		else
		{
			minFloat = Mathf.NegativeInfinity;
			maxFloat = 0;

			minInteger = int.MinValue;
			maxInteger = 0;
		}

		
	}

	public Range_NoSliderAttribute( float min , float max )
	{
		minFloat = min;
		maxFloat = max;
	}

	public Range_NoSliderAttribute( int min , int max )
	{
		minInteger = min;
		maxInteger = max;

	}
	
}

}
