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
        public NetworkRunner runner; //NetworkRunner��Prefab
        public HostMigrationHandler hostMigrationHandler; //HostMigration���s�����߂̒��p
        public SceneManagerTable sceneManagerTables; //�Z�b�V�������̃V�[���}�l�[�W���[
        [Header("Setting")]
        public bool useHostMigration = false; //HostMigration���g����
    }
}
