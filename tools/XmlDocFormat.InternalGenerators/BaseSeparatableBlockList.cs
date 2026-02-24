namespace XmlDocFormat.InternalGenerators;

public abstract class BaseSeparatableBlockList
{
    private bool _hasPrevious = false;

    protected abstract void AppendSeparator();

    public void BeginNewBlock()
    {
        if (_hasPrevious)
        {
            AppendSeparator();
        }
    }

    public void CommitBlock()
    {
        _hasPrevious = true;
    }
}
