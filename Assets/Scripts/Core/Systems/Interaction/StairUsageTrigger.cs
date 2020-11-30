using C1L1.Core.Systems.Player;
using UnityEngine;

namespace C1L1.Core.Systems.Interaction
{
    public sealed class StairUsageTrigger : MonoBehaviour
    {
        public StairUsageTrigger OppositePoint = null;
        public Transform VerticalCheckPoint = null;
        public GameObject[] ObjectsToDisable = null;

        private Controller2D controller2D = null;

        private void Start()
        {
            controller2D = FindObjectOfType<Controller2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                controller2D.StairDestination = OppositePoint;
                controller2D.CanUseStairs = true;

                foreach (var go in ObjectsToDisable)
                {
                    if (!controller2D.ObjectsToDisable.Contains(go))
                        controller2D.ObjectsToDisable.Add(go);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                controller2D.CanUseStairs = false;
            }
        }
    }
}