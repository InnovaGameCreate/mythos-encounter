using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public enum EnemyState
    {
        /// <summary>
        /// Ç»Ç…Ç‡ìoò^Ç≥ÇÍÇƒÇ¢Ç»Ç¢èÛë‘
        /// </summary>
        None,
        /// <summary>
        /// èÑâÒ
        /// </summary>
        Patrolling,
        /// <summary>
        /// çıìG
        /// </summary>
        Searching,
        /// <summary>
        /// í«ê’
        /// </summary>
        Chase,
        /// <summary>
        /// çUåÇ
        /// </summary>
        Attack,
        /// <summary>
        /// ëﬁéU
        /// </summary>
        FallBack,
        /// <summary>
        /// ì¡éÍçsìÆ
        /// </summary>
        Special,
        /// <summary>
        /// î≠å©
        /// </summary>
        Discover
    }


}
