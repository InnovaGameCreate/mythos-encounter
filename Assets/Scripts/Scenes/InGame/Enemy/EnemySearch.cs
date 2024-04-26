using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class EnemySearch : MonoBehaviour
    {
        private EnemyVisibilityMap _myVisivilityMap;
        //çıìGçsìÆÇÃÉNÉâÉXÇ≈Ç∑
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_myVisivilityMap != null) 
            { 
            
            }
        }

        public void SetVisivilityMap(EnemyVisibilityMap setVisivilityMap) 
        { 
            _myVisivilityMap = setVisivilityMap;
        }
    }
}
