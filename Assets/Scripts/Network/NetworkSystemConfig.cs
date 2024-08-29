using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    [CreateAssetMenu(menuName = "ScriptableObjects/NetworkSystemConfig")]
    public class NetworkSystemConfig : ScriptableObject
    {
        public int sessionPlayerMax;
        [Header("SceneRefBuildIndex")]
        public int inGameScene;
    }
}
