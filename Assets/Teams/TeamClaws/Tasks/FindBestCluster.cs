using UnityEngine;
using System.Collections.Generic;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class FindBestCluster : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedBool CheckPoints = true;
        public SharedFloat PointsForce = 1;

        public override TaskStatus OnUpdate()
        {
            ref Blackboard blackboard = ref GetComponent<BOTController>()._blackboard;
            List<List<WayPoint>> Clusters = blackboard.Clusters;
            int AiId = blackboard._owner;

            Vector2 AIPosition = blackboard._latestGameData.SpaceShips[blackboard._owner].Position;
            List<float> ClustersNote = new List<float>();
            int BestCluster = -1;

            foreach (List<WayPoint> Cluster in Clusters)
            {
                // Sort WayPoints in CLuster

                Cluster.Sort(delegate (WayPoint point1, WayPoint point2)
                {
                    if (point1.Owner == AiId)
                        return 1;
                    else if (point1.Owner == AiId)
                        return -1;
                    else if (Vector2.Distance(AIPosition, point1.Position) < Vector2.Distance(AIPosition, point2.Position))
                        return -1;
                    else if (Vector2.Distance(AIPosition, point1.Position) > Vector2.Distance(AIPosition, point2.Position))
                        return 1;
                    else
                        return 0;
                });

                // Get the Cluster note
                float Note = 1;

                int Points = 0;
                Vector2 ClusterPosition = new Vector2();
                float Distance;

                foreach (WayPoint wayPoint in Cluster)
                {
                    if (CheckPoints.Value)
                    {
                        if (wayPoint.Owner == -1)
                            Points += 1;
                        else if (wayPoint.Owner == (1 - AiId))
                            Points += 2;
                    }
                    ClusterPosition += wayPoint.Position;
                }
                ClusterPosition = ClusterPosition / Cluster.Count;
                Distance = Vector2.Distance(AIPosition, ClusterPosition);
                Note = (Note + Points * PointsForce.Value) / Distance;
                ClustersNote.Add(Note);
            }

            float BestNote = -1;
            for (int i = 0; i < ClustersNote.Count; i++)
            {
                if (ClustersNote[i] > BestNote)
                {
                    BestNote = ClustersNote[i];
                    BestCluster = i;
                }
            }

            // Get number of point to take in Best Cluster
            int NumberPointToTake = 0;

            for (int i = 0; i < Clusters[BestCluster].Count; i++)
            {
                if (Clusters[BestCluster][i].Owner == -1 || Clusters[BestCluster][i].Owner == (1 - AiId))
                    NumberPointToTake++;
            }

            if (BestNote == -1)
                return TaskStatus.Failure;
            else
            {
                blackboard._behaviorTree.SetVariableValue("BestClusterId", BestCluster);
                blackboard._behaviorTree.SetVariableValue("NbWayPointToTakeInCluster", NumberPointToTake);
                Debug.Log("+++++++++++++++++");
                Debug.Log("BestCluster: " + BestCluster);
                for (int i = 0; i < Clusters[BestCluster].Count; i++)
                {
                    Debug.Log(Clusters[BestCluster][i].name);
                }
                Debug.Log("NumberPointToTake: " + NumberPointToTake);
                Debug.Log("+++++++++++++++++");
                return TaskStatus.Success;
            }
        }

    }
}
