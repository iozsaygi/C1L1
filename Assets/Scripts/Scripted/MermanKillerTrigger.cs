using C1L1.Core.Systems.AI;
using UnityEngine;

namespace C1L1.Scripted
{
    public class MermanKillerTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Merman>() != null)
                Destroy(collision.gameObject);
        }
    }
}