using Dev.Nicklaj.Butter;
using UnityEngine;

public class IntEventInvoker : MonoBehaviour
{
    public IntEvent Event;
    public uint Channel;
    public int Value;
    
    public void Invoke(uint channel, int value) => Event?.Raise(value, channel);

    public void Invoke() => Invoke(Channel, Value);
}
