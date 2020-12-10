using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class DecrementWayPointToTake : Action
    {
        public SharedInt NbPointToTake;

        public override TaskStatus OnUpdate()
        {
            Blackboard blackboard = GetComponent<BOTController>()._blackboard;

            int i = NbPointToTake.Value - 1;
            blackboard._behaviorTree.SetVariableValue("NbWayPointToTakeInCluster", i);

            return TaskStatus.Success;
        }
    }
}