using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Network
{
    [Serializable]
    public sealed class SceneManagerTable : SerializableDictionary<int, GameObject> { }

    [CreateAssetMenu(menuName = "ScriptableObject/Network/Config")]
    public class Config : ScriptableObject
    {
        [Header("Config Prefabs")]
        public NetworkRunner runner; //NetworkRunnerのPrefab
        public HostMigrationHandler hostMigrationHandler; //HostMigrationを行うための中継
        public SceneManagerTable sceneManagerTables; //セッション時のシーンマネージャー
        [Header("Setting")]
        public bool useHostMigration = false; //HostMigrationを使うか
    }
}
