using UnityEngine;
namespace Scenes.Ingame.Player
{
    /// <summary>
    /// アイテムの効果・データを管理する為の抽象クラス
    /// 子クラスの名前は「アイテム名 + Effect」とすること
    /// Startメソッドで必ずSetUp関数を呼ぶこと. 呼び方:「base.SetUp();」
    /// </summary>
    [RequireComponent(typeof(ItemInstract))]
    public abstract class ItemEffect : MonoBehaviour
    {
        public ItemData myItemData;
        [HideInInspector]public PlayerStatus ownerPlayerStatus;

        public void SetUp()
        {
            myItemData.thisItemEffect = this;
            this.gameObject.layer = LayerMask.NameToLayer("Item");
        }

        public ItemData GetItemData()
        {
            return myItemData;
        }

        /// <summary>
        /// 拾われたときに実行する処理を記述
        /// </summary>
        public abstract void OnPickUp();

        /// <summary>
        /// アイテムの効果を実装する関数
        /// </summary>
        public abstract void Effect();
    }
}

