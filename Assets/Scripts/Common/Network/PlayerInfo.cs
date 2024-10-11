using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Common.Network
{
    public class PlayerInfo : NetworkBehaviour
    {
        [Networked] public string userName { get; set; }

        public override void Spawned()
        {
            //データ保持
            DontDestroyOnLoad(this);

            //ユーザー情報登録
            if (Object.HasInputAuthority)
            {
                Rpc_SendData("name");
            }
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void Rpc_SendData(string name)
        {
            userName = name;
        }
    }
}
