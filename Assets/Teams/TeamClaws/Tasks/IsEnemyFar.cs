using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class IsEnemyFar : Conditional
    {
        public SharedFloat DistanceWithEnemy;
        public float Distance;
        public override TaskStatus OnUpdate()
        {
            if (DistanceWithEnemy.Value > Distance)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
