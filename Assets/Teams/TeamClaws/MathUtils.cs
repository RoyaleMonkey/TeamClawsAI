using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamClaws
{
	public class MathUtils : MonoBehaviour
	{
		public static bool ComputeIntersection(Vector2 A, Vector2 dirA, Vector2 B, Vector2 dirB, out Vector2 C)
		{
			C = Vector2.zero;
			if (dirA.sqrMagnitude == 0 || dirB.sqrMagnitude == 0)
				return false;
			dirA.Normalize();
			dirB.Normalize();

			float denominator = dirA.x * dirB.y - dirA.y * dirB.x;
			if (denominator == 0)
				return false; // can't divide by 0;

			float k = ((A.y - B.y) * dirB.x + (B.x - A.x) * dirB.y) / denominator;
			C = A + dirA * k;
			return true;
		}
	}
}