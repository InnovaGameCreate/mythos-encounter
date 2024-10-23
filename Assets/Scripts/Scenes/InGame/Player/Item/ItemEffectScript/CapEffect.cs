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
            //�@�A�C�e���X���b�g���Ɋ����c���Ă��邩�ǂ�������
           �@var _isHavecap = ownerPlayerItem.ItemSlots
                .Where((item, i) => i != ownerPlayerItem.nowIndex && item.myItemData != null)
                .Any(item => item.myItemData.itemID == 22);

            //�����c���Ă��Ȃ���Ό��ʏ���
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