
public class RoboInstanceDataEventArgs<T>
{
    public T Value { get; private set; }
    public RoboInstanceDataEventArgs(T value)
    {
        this.Value = value;
    }
}