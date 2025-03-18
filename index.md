# AS-8X ծրագրավորողի ձեռնարկ
- [Համակարգային նկարագրություններ](SystemDefinitions.md)

```c#
public string Code
{
    get
    {
        return this.mCode;
    }
    set
    {
        if (this.mCode != value)
        {
            ResetForceValidationBit(nameof(this.Code));
            this.mCode = value == null ? string.Empty : (string)Correct(nameof(this.Code), value);
        }
        OnPropertyChanged();
    }
}
```