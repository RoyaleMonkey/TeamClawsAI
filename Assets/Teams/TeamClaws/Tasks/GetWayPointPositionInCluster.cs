using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class GetWayPointPositionInCluster : Action
    {
        public SharedInt ClusterId;
        public SharedInt NbPointToTake;

        public override TaskStatus OnUpdate()
        {
            Blackboard blackboard = GetComponent<BOTController>()._blackboard;

            int i = blackboard.Clusters[ClusterId.Value].Count - NbPointToTake.Value;
            if (ClusterId.Value < 0 || ClusterId.Value > blackboard.Clusters.Count - 1)
                ClusterId.Value = 0;
            if (i < 0 || i > blackboard.Clusters[ClusterId.Value].Count - 1)
                i = 0;
            blackboard._behaviorTree.SetVariableValue("TargetWayPointTransform", blackboard.Clusters[ClusterId.Value][i].transform);

            return TaskStatus.Success;
        }
    }
}
