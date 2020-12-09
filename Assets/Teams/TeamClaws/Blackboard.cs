using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
	public class Blackboard : MonoBehaviour
	{
		public float ShootTimeTolerance = 0.2f;

		public bool TriggerShoot { get; private set; }

		BehaviorTree _behaviorTree = null;
		int _owner;
		GameData _latestGameData = null;

		bool _debugCanShootIntersect = false;
		Vector2 _debugIntersection = Vector2.zero;
		float _debugTimeDiff = 0;

		public void Awake()
		{
			_behaviorTree = GetComponent<BehaviorTree>();
		}

		public void Initialize(SpaceShip aiShip, GameData gameData)
		{
			_latestGameData = gameData;
			_owner = aiShip.Owner;
		}

		public void UpdateData(GameData gameData)
		{
			_latestGameData = gameData;

			//_behaviorTree.SetVariableValue("SomeVariable", 1.5f);
			//_stateMachine.SetFloat("SomeVariable", 1.5f);

			TriggerShoot = CanHit(gameData, ShootTimeTolerance);
		}

		public bool CanHit(GameData gameData, float timeTolerance)
		{
			_debugCanShootIntersect = false;

			SpaceShip aiShip = gameData.SpaceShips[_owner];
			SpaceShip enemyShip = gameData.SpaceShips[1 - _owner];

			float shootAngle = Mathf.Deg2Rad * aiShip.Orientation;
			Vector2 shootDir = new Vector2(Mathf.Cos(shootAngle), Mathf.Sin(shootAngle));

			Vector2 intersection;
			bool canIntersect = MathUtils.ComputeIntersection(aiShip.Position, shootDir, enemyShip.Position, enemyShip.Velocity, out intersection);
			if (!canIntersect)
			{
				return false;
			}
			Vector2 aiToI = intersection - aiShip.Position;
			Vector2 enemyToI = intersection - enemyShip.Position;
			if (Vector2.Dot(aiToI, shootDir) <= 0)
				return false;

			float bulletTimeToI = aiToI.magnitude / Bullet.Speed;
			float enemyTimeToI = enemyToI.magnitude / enemyShip.Velocity.magnitude;
			enemyTimeToI *= Vector2.Dot(enemyToI, enemyShip.Velocity) > 0 ? 1 : -1;

			_debugCanShootIntersect = canIntersect;
			_debugIntersection = intersection;

			float timeDiff = bulletTimeToI - enemyTimeToI;
			_debugTimeDiff = timeDiff;
			return Mathf.Abs(timeDiff) < timeTolerance;
		}

		private void OnDrawGizmos()
		{
			if (_debugCanShootIntersect)
			{
				SpaceShip aiShip = _latestGameData.SpaceShips[_owner];
				SpaceShip enemyShip = _latestGameData.SpaceShips[1 - _owner];
				Gizmos.DrawLine(aiShip.Position, _debugIntersection);
				Gizmos.DrawLine(enemyShip.Position, _debugIntersection);
				Gizmos.DrawSphere(_debugIntersection, Mathf.Clamp(Mathf.Abs(_debugTimeDiff), 0.5f, 0));
			}
		}
	}
}