using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Events;

public class ConfigurationEditor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        ConfigureNode[] nodes = new ConfigureNode[]
        {
            new ConfigureNode(){
                name = "/example.csv",
                path = "Assets/NEO/examples/Configuration/Resources/configuration/example.asset",
                toType = typeof(ConfigurationExampleData),
                Parse = (asset, text) =>{
                    ((ConfigurationExampleData)asset).items = NEO.MODULE.Configuration.ParseCSV<ConfigurationExampleData.Item> (text);
                }
            }
            //. ......
        };

        for (int i = 0;i < importedAssets.Length; ++i)
        {
            for(int n = 0;n < nodes.Length; ++n)
            {
                if(importedAssets[i].IndexOf(nodes[n].name) != -1)
                {
                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(importedAssets[i]);
                    UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(nodes[n].path, nodes[n].toType);
                    if (asset == null)
                    {
                        asset = Activator.CreateInstance(nodes[n].toType) as UnityEngine.Object;
                        AssetDatabase.CreateAsset(asset, nodes[n].path);
                    }
                    nodes[n].Parse(asset, data.text);
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }

    class ConfigureNode
    {
        /// <summary>
        /// 资源包含的名称，可以唯一标识
        /// </summary>
        public string name;

        /// <summary>
        /// 解析后的文件存储路径（含文件名和后缀）
        /// </summary>
        public string path;

        /// <summary>
        /// 要转换成的格式
        /// </summary>
        public Type toType;

        /// <summary>
        /// 每种类型单独提取出解析逻辑
        /// </summary>
        public UnityAction<UnityEngine.Object, string> Parse;
    }
}
