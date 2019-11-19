using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEO.MODULE;
using System.IO;

public class AssetBundleExample : MonoBehaviour
{
    //资源配置文件所在的位置（Resources目录下的相对路径）
    string assetBundleDataPath = "assetBundleData";

    AssetBundleManager manager;
    GameObject managerGO;

    const float yGap = 2f;
    const float xGap = 2f;

    public Transform hook;
    List<GameObject> TestGOs1;
    List<GameObject> TestGOs2;
    List<GameObject> TestGOs3;
    List<GameObject> TestGOs4;

    public AssetBundleData.ID id;
    public AssetBundleManager.Mode mode;

    UnityEngine.U2D.SpriteAtlas atlas;

    #region 初始化
    void Start()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();

        Initialize();
    }

    void Initialize()
    {
        managerGO = new GameObject("AssetBundle Manager");

        string assetBundleRootPath = Path.Combine(Application.streamingAssetsPath, "neo");
        manager = new AssetBundleManager(assetBundleDataPath,
            assetBundleRootPath, mode,
            managerGO.transform, ProgressListener);

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
        if (id == AssetBundleData.ID.Test4)
        {
            Texture2D texture = manager.GetAsset<Texture2D>(id);
            GameObject go = new GameObject();
            go.transform.parent = hook;
            go.transform.position = new Vector3(xGap * GOs.Count, yGap * (int)id, 0f);
            go.AddComponent<SpriteRenderer>();
            go.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture,
                new Rect(0f,0f,texture.width,texture.height),Vector2.zero);
            go.SetActive(true);
            GOs.Add(go);
        }
        else
        {
            GameObject go = manager.GetAsset<GameObject>(id);
            go.transform.parent = hook;
            go.transform.position = new Vector3(xGap * GOs.Count, yGap * (int)id, 0f);

            /*if(id == AssetBundleData.ID.Test1)
            {
                go.GetComponent<SpriteRenderer>().sprite = atlas.GetSprite("sprite1");
            }else if (id == AssetBundleData.ID.Test2)
            {
                go.GetComponent<SpriteRenderer>().sprite = atlas.GetSprite("sprite2");
            }else if (id == AssetBundleData.ID.Test3)
            {
                go.GetComponent<SpriteRenderer>().sprite = atlas.GetSprite("sprite3");
            }*/
            go.SetActive(true);
            GOs.Add(go);
        }
    }

    public void CreateAsync()
    {
        if (manager.Initialized() == false)
            return;
        List<GameObject> GOs = SelectList();
        if (id == AssetBundleData.ID.Test4)
        {
            manager.GetAssetAsync<Texture2D>(id, (texture) => {
                GameObject go = new GameObject();
                go.transform.parent = hook;
                go.transform.position = new Vector3(xGap * GOs.Count, yGap * (int)id, 0f);
                go.AddComponent<SpriteRenderer>();
                go.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture,
                new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
                go.SetActive(true);
                GOs.Add(go);
            });
        }
        else
        {
            manager.GetAssetAsync<GameObject>(id, (go) => {
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
            case AssetBundleData.ID.Test1:
                return TestGOs1;
            case AssetBundleData.ID.Test2:
                return TestGOs2;
            case AssetBundleData.ID.Test3:
                return TestGOs3;
            case AssetBundleData.ID.Test4:
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
        if (id == AssetBundleData.ID.Test4)
        {
            Texture2D texture = go.GetComponent<SpriteRenderer>().sprite.texture;
            manager.Release(texture);
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
        if (id == AssetBundleData.ID.Test4)
        {
            for (int i = 0; i < GOs.Count; i++)
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
        
        if(id == AssetBundleData.ID.test5)
        {
            manager.Destroy(id);
        }
        else
        {
            List<GameObject> GOs = SelectList();
            if (id == AssetBundleData.ID.Test4)
            {
                for (int i = 0; i < GOs.Count; i++)
                {
                    GameObject.Destroy(GOs[i]);
                }
            }
            GOs.Clear();
            manager.Destroy(id);
        }
    }

    public void UnloadUnused()
    {
        AssetBundleManager.ClearUnused();
    }
    #endregion
}
