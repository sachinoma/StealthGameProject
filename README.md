# C# スタイルガイド

## 命名規則

* PascalCase (パスカルケース)
	* 例：SpawnPlayer
* camelCase (キャメルケース)
	* 例：isDead

| 種類 | スタイル | 例 | 備考 |
| ---- | ---- | ---- | ---- |
| 名前空間 | PascalCase | namespace MyNamespace | |
| インターフェース | PascalCase | interface IDisposable | プレフィックス `I` |
| クラス | PascalCase | class Player | |
| 列挙型 / 列挙型の値 | PascalCase | enum Direction { <br />&nbsp;&nbsp;&nbsp;&nbsp;Left, <br />&nbsp;&nbsp;&nbsp;&nbsp;Right, <br />}| |
| メソッド | PascalCase | void SpawnPlayer() | |
| ローカル変数 / パラメーター | camelCase | void PlusRandom(int num) <br />{ <br />&nbsp;&nbsp;&nbsp;&nbsp;int random = Random.Range(0, 10); <br />&nbsp;&nbsp;&nbsp;&nbsp;return num + random; <br />} | |
| private フィールド | &#95;camelCase | private int _health; | プレフィックス `_` |
| protected フィールド | &#95;camelCase | protected int _health; | プレフィックス `_` |
| public フィールド | camelCase | public int health; | |
| プロパティ | PascalCase | public int Health { get; set; }| |
| const フィールド | PascalCase | public const int MaxHealth; | |
| static フィールド | PascalCase | public static int SpawnedCount; | |

## スペース（空白文字、改行、インデント）についてのルール
``` CSharp
namespace MyNamespace
{
	public class Player
	{
		public enum Direction
		{
			Left,
			Right,
		}

		private Direction _direction;

		public Player()
		{
			_direction = Direction.Right;
		}

		public void ShowIfAndLoop(bool isLoop)
		{
			if(isLoop)
			{
				for(int i = 0; i < 10; ++i)
				{
					// 何かしよう
				}
			}
			else
			{
				// 何かしよう
			}
		}

		public void ShowSwitch()
		{
			switch(_direction)
			{
				case Direction.Left:
					// 何かしよう
					break;
				case Direction.Right:
					// 何かしよう
					break;
				default:
					break;
			}
		}
	}
}
```

## その他

* `cnt` / `num` の使用は可。他の言葉を省略することを控えてください。