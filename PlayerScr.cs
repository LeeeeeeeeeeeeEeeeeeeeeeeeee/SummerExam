using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScr : MonoBehaviour
{
    bool ClearCondition = false;
    [SerializeField] private List<GameObject> Enemys;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Ghost")
        {
            other.enabled = false;
            ClearCondition= true;
        }
        if(other.name == "Check" && ClearCondition)
        {
            Debug.Log("Clear!!!");
            Enemys.ForEach(obj => obj.SetActive(false));
        }
    }
}
