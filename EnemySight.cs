using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class EnemySight : MonoBehaviour
{
    public Transform Eye;

    public event Action OnRayHitted;
    public event Action RayHittedAfterOneSecond;

    private Dictionary<Transform, Coroutine> SearchCor = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, RaycastHit> StillWatching = new Dictionary<Transform, RaycastHit>();

    private RaycastHit hit;
    private int layermask;

    public int SearchCount = 0;

    float WatchTime=0;

    public bool Isee
    {
        get
        {
            foreach (var hitEntry in StillWatching)
            {
                if (ChooseLayer(hitEntry.Value.transform.gameObject))
                {
                    return true;
                }
            }
            return false;
        }
    }




    private void Awake()
    {
        layermask = LayerMask.GetMask("Default", "Player");
    }








    private void OnTriggerEnter(Collider other)
    {
        if (ChooseLayer(other.gameObject))
        {
            SearchCor.Add(other.transform, StartCoroutine(SearchingYou(other.transform)));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (ChooseLayer(other.gameObject))
        {
            ClearDictionary(other.transform);
        }
    }

    private void OnDisable()
    {
        if (SearchCor != null)
        {
            ClearDictionary(null);
        }
    }

    private void ItsGone(bool playerState)
    {
        ClearDictionary(null);
    }




    public enum KindOfThis
    {
        Player = 0,
        Enemy = 1
    }




    private void ClearDictionary(Transform that)
    {
        if (that == null)
        {
            SearchCor.Clear();
            StillWatching.Clear();
            StopAllCoroutines();
        }
        else
        {
            if (SearchCor.TryGetValue(that.transform, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
            }
            SearchCor.Remove(that.transform);

            StillWatching.Remove(that.transform);
        }
    }
    [ContextMenu(nameof(Log))]
    private void Log()
    {
        foreach (var item in SearchCor)
        {
            Debug.Log(item.Key);
        }
        foreach (var item in StillWatching)
        {
            Debug.Log(item.Key);
        }
        foreach (var item in StillWatching)
        {
            Debug.Log(item.Value.transform.name);
        }
    }

    private bool ChooseLayer(GameObject youare)
    {
        return youare.layer == LayerMask.NameToLayer("Player");
    }








    public IEnumerator SearchingYou(Transform other)
    {
        while (gameObject)
        {
            Vector3 rayDirection = other.transform.position - Eye.position;
            Vector3 rayOrigin = Eye.position;

            Debug.DrawRay(rayOrigin, rayDirection, Color.red);


            if (Physics.Raycast(rayOrigin, rayDirection, out hit, float.PositiveInfinity, layermask))
            {
                //Debug.Log("hit : " + hit.transform.gameObject.name);
                //Debug.Log(hit.collider.gameObject.name);
                if (ChooseLayer(hit.collider.transform.gameObject))
                {
                    //Debug.Log("hit layer is : " + LayerMask.LayerToName(hit.transform.gameObject.layer));
                    
                        //Debug.Log("i see no : " + hit.transform.gameObject.name);
                        //UnityEditor.EditorApplication.isPaused = true;
                        OnRayHitted();
                    
                }

                if (!StillWatching.ContainsKey(other))
                {
                    //Debug.Log("still watching add : " + other.transform.gameObject.name);
                    StillWatching.Add(other, hit);
                }
                else
                {
                    //Debug.Log("still watching update : " + other.transform.gameObject.name);
                    StillWatching[other] = hit;
                }
            }
            yield return null;
        }
    }
}
