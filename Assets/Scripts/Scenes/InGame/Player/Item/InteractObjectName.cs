using UnityEngine;

namespace Scenes.Ingame.Player
{
    public class InteractObjectName : MonoBehaviour
    {
        [SerializeField, Tooltip("�C���^���N�g���Ƀ|�b�v�ŏo�Ă��閼�O")]
        private string interactObjectName;

        public string getName { get => interactObjectName; }
    }
}