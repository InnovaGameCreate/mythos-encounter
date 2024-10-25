using Scenes.Ingame.Manager;
using System.Linq;
namespace Scenes.Ingame.Player
{
    public class CapEffect : ItemEffect
    {
        private ResultManager _resultManager;
        public override void OnPickUp()
        {
            _resultManager = FindObjectOfType<ResultManager>();
            _resultManager.SetRingFlag(true);
        }

        public override void OnThrow()
        {
            //　アイテムスロット内に冠が残っているかどうか判定
           　var _isHavecap = ownerPlayerItem.ItemSlots
                .Where((item, i) => i != ownerPlayerItem.nowIndex && item.myItemData != null)
                .Any(item => item.myItemData.itemID == 22);

            //冠が残っていなければ効果消滅
            if (!_isHavecap)
            {
                _resultManager.SetRingFlag(false);
            }
        }

        public override void Effect()
        {

        }
    }
}