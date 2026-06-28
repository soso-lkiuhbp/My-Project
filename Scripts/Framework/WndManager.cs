using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WndManager :Singleton<WndManager>
{

    Dictionary<string, BaseWnd> _allWnds;
    Transform _canvas;



    public void Initial(Transform canvas)
    {
        _canvas = canvas;
        _allWnds = new Dictionary<string, BaseWnd>();
    }


    public void OpenWnd<T>()where T : BaseWnd,new()
    {
        string wndName = typeof(T).Name;
        if (_allWnds.ContainsKey(wndName))
        {
            _allWnds[wndName].OpenWnd();
        }
        else
        {
            T wnd = new T();
            wnd.CreateWnd(wndName, _canvas);
            wnd.Initial();
            _allWnds.Add(wndName, wnd);
        }
    }

    public void CloseWnd<T>()where T: BaseWnd,new()
    {
        string wndName = typeof(T).Name;
        if (_allWnds.ContainsKey(wndName))
        {
            _allWnds[wndName].CloseWnd();
        }
    }

    public T GetWnd<T>() where T : BaseWnd, new()
    {
        string wndName = typeof(T).Name;
        if (_allWnds.ContainsKey(wndName))
        {
            return _allWnds[wndName] as T;
        }
        return null; 
    }

    public void DeleteWnd<T>() where T : BaseWnd, new()
    {
        string wndName = typeof (T).Name;
        if (_allWnds.ContainsKey(wndName))
        {
            _allWnds[wndName].DeleteWnd();
            _allWnds.Remove(wndName);
        }
   
    }




}
