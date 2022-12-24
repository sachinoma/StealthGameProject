using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private TriggerEvent onTriggerStay = new TriggerEvent();

    /*
     * 一般的に、EnemyAIはEnemyModelをコントロールする。
     * EnemyModelはEnemyAIの存在を知らなくても大丈夫だ。
     * でも、今の状況は逆にEnemyAIはEnemyModelのことを知らない。
     */
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        // InspectorタブのonTriggerStayで指定された処理を実行する
        onTriggerStay.Invoke(other);
    }

    /*
     * UnityEventとC# eventと比べてどっちの方がいいのかがウェブ上いろいろな論議がある。
     * UnityEventはInspectorウインドウ上に表示できて、Inspectorでevent handlerを紐つけることができる。でも、C# eventより遅い。
     * 僕の意見では、汎用なコンポーネントとかを作る時は、UnityEventを使うのがいいかもしれない。
     * でも、一般的に、C# eventを使った方がいいと思う。

     * UnityEventが遅いことの他に、もう一つの大きな欠点がある。
     * Visual Studioで追跡できないということだ。
     * こっちの場合は、inspectorでEnemyModel.PlayerSarchとEnemyModel.SoundSarchに紐ついた。
     * でも、Visual Studioで参照を探すと、何の参照も探さない。
     * もし他の人(又は一ヶ月の自分)がこのコードを見ると、EnemyModel.PlayerSarchはもう無用だろう(obsolete code)と思うかもしれない。

     * ちらみに、UIButtonのonClickとかも同じ状況だ。
     * inspectorでonClickの処理を紐つけるというやり方の代わりに、[SerializeField]でButtonを参照してから、
     * onClick.AddListenerとonClick.RemoveListenerを用いるとのような形で、個人的におすすめだ。

     * また、inspectorでしか紐つかない場合（Anim eventとか）は、命名制約をやった方がいいと思う。他には、コメントを付けるのもいいだろう。

     */
    // UnityEventを継承したクラスに[Serializable]属性を付与することで、Inspectorウインドウ上に表示できるようになる。
    [Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    {
    }
}
