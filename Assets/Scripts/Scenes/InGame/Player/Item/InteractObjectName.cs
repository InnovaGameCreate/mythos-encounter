using UnityEngine;

namespace Scenes.Ingame.Player
{
    public class InteractObjectName : MonoBehaviour
    {
        [SerializeField, Tooltip("インタラクト時にポップで出てくる名前")]
        private string interactObjectName;

        public string getName { get => interactObjectName; }
    }
}