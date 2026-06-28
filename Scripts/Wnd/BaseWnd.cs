using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWnd 
{
    protected RectTransform SelfTransform { get;private set; }


    public void CreateWnd(string wndName,Transform canvas)
    {
        GameObject origin = Resources.Load<GameObject>("Wnd/"+wndName);
        GameObject clone = GameObject.Instantiate(origin);
        SelfTransform  = clone.GetComponent<RectTransform>();
        SelfTransform.SetParent(canvas, false);
    }

    public abstract void Initial();

    public void OpenWnd()
    {
        SelfTransform.gameObject.SetActive(true);
        OnOpenWnd();
    }

    protected virtual void OnOpenWnd()
    {

    }

    public void CloseWnd()
    {
        SelfTransform.gameObject.SetActive(false);
        OnCloseWnd();
    }

    protected virtual void OnCloseWnd()
    {

    }

    public void DeleteWnd()
    {
        GameObject.Destroy(SelfTransform.gameObject);
    }
}
