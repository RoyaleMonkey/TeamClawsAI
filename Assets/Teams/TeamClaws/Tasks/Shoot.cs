using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace TeamClaws
{
    public class Shoot : Action
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
