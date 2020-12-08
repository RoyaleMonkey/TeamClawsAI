using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{

	public class GameConfiguration : MonoBehaviour
	{
		public static GameConfiguration Instance = null;

		[System.NonSerialized]
		public BaseSpaceShipController controller1;
		[System.NonSerialized]
		public BaseSpaceShipController controller2;
		[System.NonSerialized]
		public string levelName;

		private void Awake()
		{
			if (Instance != null)
			{
				GameObject.Destroy(this);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

}