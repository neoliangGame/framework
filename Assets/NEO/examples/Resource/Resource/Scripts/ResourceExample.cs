using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NEO.MODULE;

public class ResourceExample : MonoBehaviour
{
    //资源配置文件所在的位置（Resources目录下的相对路径）
    string resourceDataPath = "resourceData";

    ResourceManager manager;
    GameObject managerGO;

    const float yGap = 2f;
    const float xGap = 2f;

    public Transform hook;
    List<GameObject> TestGOs1;
    List<GameObject> TestGOs2;
    List<GameObject> TestGOs3;
    List<GameObject> TestGOs4;

    public ResourceData.ID id;

    #region 初始化
    void Start()
    {
        managerGO = new GameObject("Resource Manager");
        manager = managerGO.AddComponent<ResourceManager>();
        manager.SetProgressListener(ProgressListener);
        manager.Initialize(resourceDataPath);

        TestGOs1 = new List<GameObject>();
        TestGOs2 = new List<GameObject>();
        TestGOs3 = new List<GameObject>();
        TestGOs4 = new List<GameObject>();
    }

    void ProgressListener(float progress, bool isDone)
    {
        if (isDone)
        {
            Debug.Log("<color=green>load complete</color>");
        }
        else
        {
            Debug.Log("<color=orange>loading... " + progress + "</color>");
        }
    }
    #endregion

    #region 操作
    public void Create()
    {
        if (manager.Initialized() == false)
            return;
        List<GameObject> GOs = SelectList();
        if (id == ResourceData.ID.TestID4)
        {
            manager.Get(id, out Sprite sprite);
            GameObject go = new GameObject();
            go.transform.parent = hook;
            go.transform.position = new Vector3(xGap * GOs.Count, yGap * (int)id, 0f);
            go.AddComponent<SpriteRenderer>();
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            go.SetActive(true);
            GOs.Add(go);
        }
        else
        {
            manager.Get(id, out GameObject go);
            go.transform.parent = hook;
            go.transform.position = new Vector3(xGap * GOs.Count, yGap * (int)id, 0f);
            go.SetActive(true);
            GOs.Add(go);
        }
    }

    public void CreateAsync()
    {
        if (manager.Initialized() == false)
            return;
        List<GameObject> GOs = SelectList();
        if (id == ResourceData.ID.TestID4)
        {
            manager.GetAsync<Sprite>(id, (sprite) => {
                GameObject go = new GameObject();
                go.transform.parent = hook;
                go.transform.position = new Vector3(xGap * GOs.Count, yGap * (int)id, 0f);
                go.AddComponent<SpriteRenderer>();
                go.GetComponent<SpriteRenderer>().sprite = sprite;
                go.SetActive(true);
                GOs.Add(go);
            });
        }
        else
        {
            manager.GetAsync<GameObject>(id, (go) => {
                go.transform.parent = hook;
                go.transform.position = new Vector3(xGap * GOs.Count, yGap * (int)id, 0f);
                go.SetActive(true);
                GOs.Add(go);
            });
        }
    }

    List<GameObject> SelectList()
    {
        switch (id)
        {
            case ResourceData.ID.TestID1:
                return TestGOs1;
            case ResourceData.ID.TestID2:
                return TestGOs2;
            case ResourceData.ID.TestID3:
                return TestGOs3;
            case ResourceData.ID.TestID4:
                return TestGOs4;
        }
        return null;
    }

    public void Release()
    {
        if (manager.Initialized() == false)
            return;
        List<GameObject> GOs = SelectList();
        if (GOs.Count == 0)
            return;
        GameObject go = GOs[GOs.Count - 1];
        GOs.Remove(go);
        go.SetActive(false);
        if (id == ResourceData.ID.TestID4)
        {
            Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
            manager.Release(sprite);
        }
        else
        {
            manager.Release(go);
        }
    }

    public void Clear()
    {
        if (manager.Initialized() == false)
            return;
        List<GameObject> GOs = SelectList();
        if (GOs.Count == 0)
            return;
        if(id == ResourceData.ID.TestID4)
        {
            for(int i = 0;i < GOs.Count; i++)
            {
                Debug.Log("destroy:" + GOs[i].name);
                Destroy(GOs[i]);
            }
        }
        GOs.Clear();
        manager.Clear(id);
    }

    public void Destroy()
    {
        if (manager.Initialized() == false)
            return;
        List<GameObject> GOs = SelectList();
        if (id == ResourceData.ID.TestID4)
        {
            for (int i = 0; i < GOs.Count; i++)
            {
                GameObject.Destroy(GOs[i]);
            }
        }
        GOs.Clear();
        manager.Destroy(id);
    }

    public void UnloadUnused()
    {
        ResourceManager.ClearUnused();
    }
    #endregion
}
