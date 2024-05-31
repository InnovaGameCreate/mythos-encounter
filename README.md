# mythos-encounter
## 開発環境
- Unityバージョン 2022.3.1f1
## ブランチの切り方
- 命名規則はスネークケースを使ってください。
- 機能追加の場合は feature/(追加機能内容)で書いてください。
  
  例.セーブ機能実装の場合→feature/data_save
- バグ修正の場合は fix/(修正内容)で書いてください。
- 緊急のバグ修正（レビュー無視でマージ）の場合は hotfix/(緊急の修正内容)で書いてください。
## コーディング規約
### スクリプトの作成位置について
 - 基本的に特定のシーンにしか作用しない内容の場合はScenesの中の各シーンフォルダ内で作成してください。
### ネームスペース
 - スクリプト作成時は必ずネームスペースをつけてください。
 - ネームスペースはフォルダ階層を示すように書いてください

  例. Scenes.Ingame.Playerなど

### 命名規則
 - Script名はパスカルケースで書いてください
 - 関数名(Voidなど）はパスカルケースで書いてください
 - 定数(const)は全て大文字で書いてください
 - private変数は_から始まるキャメルケースで書いてください
 - public変数はキャメルケースで書いてください
 - 過度な略称は使わないでください(rigidBody2D→rb2など）
 - IObservableなどは"On"から始めてください(OnDeadなど)

## プルリクエストの書き方
基本的にテンプレートに書いてある枠に沿って書いてください
<--- --->はコメントアウトを意味します、この間に書いてむもプルリクエスト上には反映されません。
### Reviewersの設定
プルリクエスト作成後、マージされるためには2人以上のreviewが必要な設定になっています。作成は下記のようにReviewerを設定してください。

![image](https://github.com/InnovaGameCreate/mythos-encounter/assets/67269447/4bebb78c-d38a-406f-90bf-ff0a6d9c6e2e)

この際に設定するのは下記の3人です。

tibigamePoppo

iwamotoHinata

Tyunosause

### Request Changeを貰ったら
Reviewerから作業内容についてアドバイスや修正があることがあります。この際にRequest Changeというものをもらいます。
問題がなければApproveをもらいます。
Request Changeを貰ったら内容に沿って修正を行なってください。

## 使用アセット
### スクリプト関係
- TextMeshPro
- UniRx
- DoTween
- QuickOutline
### モデル関係
- Decrepit Dungeon LITE
- Adventure Character
- Horror House
- Creature Insect
- Horror Creatures
  
