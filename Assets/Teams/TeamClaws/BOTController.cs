using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace TeamClaws
{

	public class BOTController : BaseSpaceShipController
	{
		public InputData inputData;
		private Blackboard _blackboard;

		public override void Initialize(SpaceShip spaceship, GameData data)
		{
			_blackboard = GetComponent<Blackboard>();
			_blackboard.Initialize(spaceship, data);
		}

		public override InputData UpdateInput(SpaceShip spaceship, GameData data)
		{
			_blackboard.UpdateData(data);
			return (inputData);
		}
	}


}
