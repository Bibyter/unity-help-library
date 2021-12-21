using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2
{
    [CreateAssetMenu]
    public sealed class WindowDatabase : ScriptableObject
    {
        [System.Serializable]
        public sealed class AssetInfo
        {
            public string guid;
            [System.NonSerialized] public WindowDatabase datebase;
            [SerializeField] List<StateInfo> _infoList;

            public StateInfo GetStateInfo(string guid)
            {
                for (int i = 0; i < _infoList.Count; i++)
                {
                    if (_infoList[i].guid == guid)
                        return _infoList[i];
                }

                var info = new StateInfo();
                info.guid = guid;
                _infoList.Add(info);
                datebase.Save();
                return info;
            }
        }

        [SerializeField] List<AssetInfo> _assetInfoList;

        public AssetInfo GetAssetInfo(string guid)
        {
            for (int i = 0; i < _assetInfoList.Count; i++)
            {
                if (_assetInfoList[i].guid == guid)
                {
                    _assetInfoList[0].datebase = this;
                    return _assetInfoList[i];
                }
            }

            var info = new AssetInfo();
            info.guid = guid;
            info.datebase = this;
            _assetInfoList.Add(info);
            Save();
            return info;
        }

        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [System.Serializable]
        public sealed class StateInfo
        {
            public string guid;
            public Vector2 position;
        }

        public static WindowDatabase GetDatabase()
        {
            var database = UnityEditor.AssetDatabase.LoadAssetAtPath<WindowDatabase>("Assets/Bibyter/Fsm2/Editor/Database.asset");

            if (database == null)
            {
                database = CreateInstance<WindowDatabase>();
                UnityEditor.AssetDatabase.CreateAsset(database, "Assets/Bibyter/Fsm2/Editor/Database.asset");
                //UnityEditor.EditorUtility.SetDirty(database);
            }

            return database;
        }
    }
}