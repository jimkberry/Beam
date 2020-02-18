﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastMgr : MonoBehaviour
{
    public const int maxToasts = 4;
    public const float defDisplaySecs = 2.0f;

    public GameObject toastPrefab;

    public List<Toast> toasts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void FixupPositions()
    {
        int idx = toasts.Count - 1;;
        foreach (Toast t in toasts)
            t.SetIndex(idx--); 
    }

    public void ShowToast(string msg, Toast.ToastColor color=Toast.ToastColor.kBlue, float displaySecs=defDisplaySecs)
    {
        GameObject toastGo = GameObject.Instantiate(toastPrefab, transform);        
        toastGo.transform.SetParent(transform.parent);        
        Toast toast= (Toast)toastGo.transform.GetComponent<Toast>();
		toast.Setup(this, msg, color, displaySecs);
        if (toasts.Count >= maxToasts)
            RemoveToast(toasts[maxToasts-1]);
        toasts.Add(toast);
        toastGo.SetActive(true);    
        FixupPositions();            
    }    
    
    public void RemoveToast(Toast theToast)
    {
        toasts.Remove(theToast);
        GameObject.Destroy(theToast.gameObject); 
        FixupPositions();       
    }
}
