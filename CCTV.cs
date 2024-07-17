using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : MonoBehaviour
{
    [SerializeField] private Enemy[] Enemy;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Enemy[FindnearEnemy(Enemy)].RayHitted();
        }
    }

    public int FindnearEnemy(Enemy[] Enemy)
    {
        float nearDis = 10000.0f;
        int Index = 0;

        for (int i = 0; i < Enemy.Length; i++)
        {
            float distance = Vector3.Distance(Enemy[i].transform.position, transform.position);
            if (distance < nearDis)
            {
                Index = i;
                nearDis = distance;
            }
        }
        return Index;
    }
}
