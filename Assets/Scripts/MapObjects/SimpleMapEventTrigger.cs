public class SimpleMapEventTrigger : MapEventTriggerBase
{
    protected override void Trigger()
    {
        MainSceneEventManager.TriggerEvent(_eventId, this, null);
    }
}
