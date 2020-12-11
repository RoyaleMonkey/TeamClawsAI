using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class CantMakeShockwave : Conditional
    {
        public SharedFloat DistanceWithEnemy;
        public SharedBool TriggerMakeShockwave;
        public float Distance;
        public override TaskStatus OnUpdate()
        {
            if (DistanceWithEnemy.Value > Distance || !TriggerMakeShockwave.Value)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
