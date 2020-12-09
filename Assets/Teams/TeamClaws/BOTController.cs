using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace TeamClaws
{

	public class BOTController : BaseSpaceShipController
	{
		private Blackboard _blackboard;

		public override void Initialize(SpaceShip spaceship, GameData data)
		{
			_blackboard = GetComponent<Blackboard>();
			_blackboard.Initialize(spaceship, data);
		}

		public override InputData UpdateInput(SpaceShip spaceship, GameData data)
		{
			float thrust = 1.0f;
			float targetOrient = spaceship.Orientation + 90.0f;

			_blackboard.UpdateData(data);

			return new InputData(thrust, targetOrient, _blackboard.TriggerShoot, false, false);
		}
	}


}
