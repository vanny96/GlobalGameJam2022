using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    private static DataHolder dataHolder;

    private Dictionary<string, object> data = new Dictionary<string, object>();

    private void Start()
    {
        if(dataHolder == null)
        {
            dataHolder = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddData(string name, object value)
    {
        data[name] = value;
    }

    public T GetData<T>(string name)
    {
        return (T) data[name];
    }
}
