using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NEO.MODULE
{
    /// <summary>
    /// 策划配置文件管理方案
    /// CSV表格编辑->ScriptableObject资源
    /// </summary>
    /// 属性名称不以大小写区分
    /// Int型的内容不能填float内容（不允许有小数点）
    /// 支持文本类型：Int，float，string，enum
    /// CSV中与属性名不匹配的任何列会被忽略
    public class Configuration
    {
        //这里添加CSV文本解析方法
        public static T[] ParseCSV<T>(string text)
        {
            string[] lines = Lines(text);
            Dictionary<string, int> propertyNAMEtoINDEX = PropertyToIndex(lines[0]);
            lines = ContentLines(lines);

            FieldInfo[] fields = typeof(T).GetFields();
            T[] targets = new T[lines.Length];
            for(int l = 0;l < lines.Length; ++l)
            {
                targets[l] =  Activator.CreateInstance<T>();
                string[] line = lines[l].Split(',');
                line = TrimArray(line);

                for(int f = 0;f < fields.Length; ++f)
                {
                    if(propertyNAMEtoINDEX.TryGetValue(fields[f].Name,out int index))
                    {
                        SetField(targets[l], fields[f], line[index]);
                    }
                }
            }
            return targets;
        }

        //分割原始文本成行，删除多余的换行符
        static string[] Lines(string text)
        {
            string[] originLines = text.Split('\n');
            List<string> cleanLines = new List<string>();
            for (int i = 0; i < originLines.Length; i++)
            {
                originLines[i] = originLines[i].Replace("\r", "");
                if (originLines[i].Length > 0)
                {
                    cleanLines.Add(originLines[i]);
                }
            }
            return cleanLines.ToArray();
        }

        //文本第一行为内容为属性名
        //分割属性，去除每个属性的前后多余空格
        static Dictionary<string, int> PropertyToIndex(string propertyLine)
        {
            string[] properties = propertyLine.Split(',');
            properties = TrimArray(properties);
            Dictionary<string, int> PROPERTYtoINDEX = new Dictionary<string, int>();
            for (int i = 0; i < properties.Length; ++i)
            {
                PROPERTYtoINDEX[properties[i]] = i;
            }
            return PROPERTYtoINDEX;
        }

        //文本第二行开始为属性内容
        static string[] TrimArray(string[] fullArray)
        {
            for(int i = 0;i < fullArray.Length; ++i)
            {
                fullArray[i] = fullArray[i].Trim();
            }
            return fullArray;
        }

        static string[] ContentLines(string[] lines)
        {
            List<string> contents = new List<string>();
            for(int i = 1;i < lines.Length; ++i)
            {
                contents.Add(lines[i]);
            }
            return contents.ToArray();
        }

        //设置属性值
        static void SetField(object target, FieldInfo field, string value)
        {
            if (field.FieldType.IsEnum)
            {
                field.SetValue(target, Enum.Parse(field.FieldType, value));
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(target, value);
            }
            else if (field.FieldType == typeof(short) ||
               field.FieldType == typeof(int) ||
               field.FieldType == typeof(long) ||
               field.FieldType == typeof(float) ||
               field.FieldType == typeof(double))
            {
                field.SetValue(target, Convert.ChangeType(value, field.FieldType));
            }
            //扩展时机需求的属性......
        }
    }
}

