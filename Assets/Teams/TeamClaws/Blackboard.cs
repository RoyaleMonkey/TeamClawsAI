using System;
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

		public bool TriggerShoot { get; set; }
		public bool TriggerMakeShockwave { get; set; }
		public List<List<WayPoint>> Clusters { get; set; }

		public BehaviorTree _behaviorTree = null;
		public int _owner;
		public GameData _latestGameData = null;

		private double ShootStartTimer;
		private double ShockwaveStartTimer;

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
			Clusters = GetClusters(2.25f);
			ShootStartTimer = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds - 2000;
			ShockwaveStartTimer = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds - 2000;

			foreach (List<WayPoint> Cluster in Clusters)
            {
				Debug.Log("======================");
				foreach (WayPoint wayPoint in Cluster)
                {
					Debug.Log(wayPoint.name);
                }
            }
		}

		public void UpdateData(GameData gameData)
		{
			_latestGameData = gameData;
			if (gameData.SpaceShips[_owner].IsStun())
			{
				_behaviorTree.SetVariableValue("IsStun", true);
				_behaviorTree.SetVariableValue("NbWayPointToTakeInCluster", 0);
			}
			else
			{
				_behaviorTree.SetVariableValue("IsStun", false);

				TriggerShoot = CanHit(gameData, ShootTimeTolerance);
				if (TriggerShoot && new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds - ShootStartTimer >= 2000)
				{
					ShootStartTimer = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;
					_behaviorTree.SetVariableValue("TriggerShoot", TriggerShoot);
				}
				else
				{
					_behaviorTree.SetVariableValue("TriggerShoot", false);
				}

				TriggerMakeShockwave = CanMakeShockwave(gameData);
				if (TriggerMakeShockwave && new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds - ShockwaveStartTimer >= 2000)
				{
					ShockwaveStartTimer = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;
					_behaviorTree.SetVariableValue("TriggerMakeShockwave", TriggerMakeShockwave);
				}
				else
				{
					_behaviorTree.SetVariableValue("TriggerMakeShockwave", false);
				}
			}

			_behaviorTree.SetVariableValue("DistanceWithEnemy", Vector2.Distance(gameData.SpaceShips[0].Position, gameData.SpaceShips[1].Position));
		}

		public bool CanMakeShockwave(GameData gameData)
        {
			//gameData.SpaceShips[0]
			if (Vector2.Distance(gameData.SpaceShips[_owner].Position, Vector2.zero) < Vector2.Distance(gameData.SpaceShips[1 - _owner].Position, Vector2.zero))
				return true;
			else
				return false;
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

		public List<List<WayPoint>> GetClusters(float ClusterRadius)
		{
			List<List<WayPoint>> Clusters = new List<List<WayPoint>>();
			List<WayPoint> wayPoints = _latestGameData.WayPoints;

			for (int i = 0; i < wayPoints.Count; i++)
			{
				List<WayPoint> Cluster = new List<WayPoint>();
				Cluster.Add(wayPoints[i]);
				for (int j = 0; j < wayPoints.Count; j++)
				{
					if (i != j && Vector3.Distance(wayPoints[i].Position, wayPoints[j].Position) <= ClusterRadius)
					{
						Cluster.Add(wayPoints[j]);
					}
				}
				Clusters.Add(Cluster);
			}

			Clusters.Sort(delegate (List<WayPoint> list1, List<WayPoint> list2)
			{
				if (list1.Count < list2.Count)
					return 1;
				else if (list1.Count > list2.Count)
					return -1;
				else
					return 0;
			});

			List<WayPoint> tmp = new List<WayPoint>();

			for (int i = 0; i < Clusters.Count; i++)
			{
				int count = 0;
				for (int j = 0; j < Clusters[i].Count; j++)
				{
					if (!tmp.Contains(Clusters[i][j]))
					{
						count++;
						tmp.Add(Clusters[i][j]);
					}
				}
				if (count == 0)
				{
					Clusters.RemoveAt(i);
					i--;
				}
			}

			return (Clusters);
		}
	}
}