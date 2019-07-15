using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mati36.CustomIcons
{
    [CreateAssetMenu(menuName = "Mati36/CustomIcons/Database", order = 35), System.Serializable]
    public class CustomIconsDatabase : ScriptableObject
    {

        [SerializeField]
        private List<CustomAssetData> database = new List<CustomAssetData>();

        public Texture2D defaultIcon;

        public int ContainsGUID(string guid)
        {
            for (int i = 0; i < database.Count; i++)
            {
                if (database[i].guid == guid) return i;
            }
            return -1;
        }

        public void AddAssetData(string guid, Texture2D icon, Texture2D icon_16, string name)
        {
            database.Add(new CustomAssetData() { guid = guid, icon = icon, icon_16 = icon_16, assetName = name });
        }

        public void DeleteAssetData(int index)
        {
            database.RemoveAt(index);
        }

        public CustomAssetData GetAssetData(int index)
        {
            return database[index];
        }

        public void SetIcon(int index, Texture2D icon, Texture2D icon_16)
        {
            database[index].icon = icon;
            database[index].icon_16 = icon_16;
        }

        public void SetName(int index, string name)
        {
            database[index].assetName = name;
        }
    }

    [System.Serializable]
    public class CustomAssetData
    {
        public string guid;
        public Texture2D icon;
        public Texture2D icon_16;
        public string assetName;
    }
}