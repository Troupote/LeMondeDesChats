using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent _gameEvent;
    [SerializeField] private UnityEvent _unityEvent;

    private void Awake() => _gameEvent.Register(this);

    private void OnDestroy() => _gameEvent.Deregister(this);

    public void RaiseEvent() => _unityEvent.Invoke();
}