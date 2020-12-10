using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace TeamClaws
{

	public class BOTController : BaseSpaceShipController
	{
		public InputData inputData;
		public Blackboard _blackboard;

		private bool DropedMine = false;

		public override void Initialize(SpaceShip spaceship, GameData data)
		{
			_blackboard = GetComponent<Blackboard>();
			_blackboard.Initialize(spaceship, data);
		}

		public override InputData UpdateInput(SpaceShip spaceship, GameData data)
		{
			_blackboard.UpdateData(data);
			if (inputData.dropMine && !DropedMine)
			{
				DropedMine = true;
			}
			else if (inputData.dropMine && DropedMine)
			{
				inputData.dropMine = false;
				DropedMine = false;
			}
			
			return (inputData);
		}
	}


}
