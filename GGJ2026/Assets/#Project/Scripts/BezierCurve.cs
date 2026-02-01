using System.Collections.Generic;
using UnityEngine;

// Class that implements the deCasteljau algorithm for
// computing the value of a Bezier curve of arbitrary
// degree
public class BezierCurve
{
	// Calculate value of Bezier curve
	/// <summary>
	/// Return a value along the bezier curve defined by controlPoints according to value t
	/// </summary>
	/// <param name="t"></param>
	/// <param name="controlPoints"></param>
	/// <returns></returns>
	public static Vector3 DeCasteljau(List<Vector3> controlPoints, float t)
	{
		var array = controlPoints.ToArray();
		return DeCasteljau(array, t);
	}

	/// <summary>
	/// Return a value along the bezier curve defined by controlPoints according to value t
	/// </summary>
	/// <param name="t"></param>
	/// <param name="controlPoints"></param>
	/// <returns></returns>
	public static Vector3 DeCasteljau(Vector3[] controlPoints, float t)
	{
		// if only one point remains then return that
		if (controlPoints.Length == 1) return controlPoints[0];

		// get the points of the bezier curve one degree lower 
		Vector3[] newPoints = new Vector3[controlPoints.Length - 1];
		for (int i = 0; i < newPoints.Length; i++)
		{
			newPoints[i] = Vector3.Lerp(controlPoints[i], controlPoints[i + 1], t);
		}

		// recursive way to get the final bezier point
		return DeCasteljau(newPoints, t);
	}

	/// <summary>
	/// Get the length of the Bezier Curve defined by controlPoints, 
	/// using a smaller <paramref name="step"> step </paramref> results in a higher accuracy, but reduced performance
	/// </summary>
	/// <param name="controlPoints"></param>
	/// <param name="step"></param>
	/// <returns></returns>
	public static float GetLength(Vector3[] controlPoints, float step)
	{
		float length = 0f;

		for (float i = 0; i < 1; i += step)
		{
			// get the current and next point of the curve by using the step as offset
			var curPoint = DeCasteljau(controlPoints, i);
			var nextPoint = DeCasteljau(controlPoints, i + step);

			// add the distance to approximate the total length of the curve
			length += Vector3.Distance(curPoint, nextPoint);
		}

		return length;
	}
}
