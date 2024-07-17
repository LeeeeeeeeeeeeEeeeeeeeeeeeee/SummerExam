using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out Enemy _Enemy))
        {
            

            if (this.name.StartsWith("Search"))
            {
                transform.Translate(0, 20, 0);
                _Enemy.Animator.SetTrigger("Idle");
                _Enemy.Invoke("StartPatrol", 3);
            }
            else
            {
                
                if (_Enemy.patroll.Count == _Enemy.patrollIndex + 1)
                {
                    _Enemy.patroll[_Enemy.patrollIndex].gameObject.SetActive(false);
                    _Enemy.patroll[0].gameObject.SetActive(true);
                    _Enemy.Destination = _Enemy.patroll[0].position;
                    _Enemy.patrollIndex = 0;
                }
                else
                {
                    _Enemy.patroll[_Enemy.patrollIndex].gameObject.SetActive(false);
                    _Enemy.Destination = _Enemy.patroll[++_Enemy.patrollIndex].position;
                    _Enemy.patroll[_Enemy.patrollIndex].gameObject.SetActive(true);
                }

            }
            
        }
    }
}
