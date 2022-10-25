using Newtonsoft.Json;

namespace ExplogineCore;

public class SerialBlob
{
    private readonly Dictionary<IDescriptor, object> _assignedVariables = new();
    private readonly Dictionary<string, IDescriptor> _declaredVariables = new();

    public event Action? WasValueSet;

    public Descriptor<T> Declare<T>(string variableName)
    {
        if (_declaredVariables.ContainsKey(variableName))
        {
            throw new Exception($"Duplicate variable declaration {variableName}");
        }

        var descriptor = new Descriptor<T>(variableName, this);
        _declaredVariables.Add(variableName, descriptor);
        return descriptor;
    }

    public Descriptor<T> Declare<T>(string variableName, T startingValue)
    {
        var descriptor = Declare<T>(variableName);
        Set(descriptor, startingValue);
        return descriptor;
    }

    public void Set<T>(Descriptor<T> descriptor, T value)
    {
        ConfirmDeclared(descriptor);

        if (value == null)
        {
            throw new ArgumentNullException();
        }

        _assignedVariables[descriptor] = value;
        WasValueSet?.Invoke();
    }

    private void SetUnsafe(IDescriptor descriptor, object value)
    {
        ConfirmDeclared(descriptor);

        if (value == null)
        {
            throw new ArgumentNullException();
        }

        var type = descriptor.GetUnderlyingType();

        if (type.IsEnum)
        {
            // enums are special, ugh
            value = Enum.Parse(type, value.ToString()!);
        }

        _assignedVariables[descriptor] = Convert.ChangeType(value, type);
        WasValueSet?.Invoke();
    }

    private void ConfirmDeclared(IDescriptor descriptor)
    {
        if (!IsDeclared(descriptor))
        {
            throw new Exception($"{descriptor.Name} is not declared in this blob");
        }
    }

    private bool IsDeclared(IDescriptor descriptor)
    {
        return _declaredVariables.ContainsKey(descriptor.Name);
    }

    public T Get<T>(Descriptor<T> descriptor)
    {
        ConfirmDeclared(descriptor);
        ConfirmHasCorrectType(descriptor);

        if (!IsAssigned(descriptor))
        {
            throw new Exception($"{descriptor.Name} is not assigned in this blob");
        }

        return (T) _assignedVariables[descriptor];
    }

    public T? GetOrDefault<T>(Descriptor<T> descriptor)
    {
        ConfirmDeclared(descriptor);

        if (!IsAssigned(descriptor))
        {
            return default;
        }

        return Get(descriptor);
    }

    private bool IsAssigned<T>(Descriptor<T> descriptor)
    {
        return _assignedVariables.ContainsKey(descriptor);
    }

    private void ConfirmHasCorrectType<T>(Descriptor<T> descriptor)
    {
        if (_assignedVariables[descriptor].GetType() != typeof(T))
        {
            // This should be impossible but you never know
            throw new Exception(
                $"{descriptor.Name} had type {_assignedVariables[descriptor].GetType().Name}, expected {typeof(T).Name}");
        }
    }

    public void Dump(IFileSystem fileSystem, string fileName)
    {
        fileSystem.CreateOrOverwriteFile(fileName);
        fileSystem.WriteToFile(fileName, DataAsStrings());
    }

    public bool TryRead(IFileSystem fileSystem, string fileName)
    {
        try
        {
            Read(fileSystem, fileName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Read(IFileSystem fileSystem, string fileName)
    {
        var fileContent = fileSystem.ReadFile(fileName);
        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent);

        if (dictionary == null)
        {
            throw new Exception($"Json deserialize failed {fileName}");
        }

        foreach (var keyVal in dictionary)
        {
            var name = keyVal.Key;
            if (_declaredVariables.ContainsKey(name))
            {
                var descriptor = _declaredVariables[name];
                var data = dictionary[name];
                SetUnsafe(descriptor, data);
            }
        }
    }

    public string[] DataAsStrings()
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var keyValue in _assignedVariables)
        {
            var name = keyValue.Key.Name;
            var value = keyValue.Value;

            dictionary[name] = value;
        }

        return new[] {JsonConvert.SerializeObject(dictionary, Formatting.Indented)};
    }

    /// <summary>
    ///     This is here because we need a non-generic to key the Dictionary on
    /// </summary>
    private interface IDescriptor
    {
        public string Name { get; }
        public Type GetUnderlyingType();
    }

    public readonly record struct Descriptor<T>(string Name, SerialBlob Blob) : IDescriptor
    {
        public Type GetUnderlyingType()
        {
            return typeof(T);
        }

        public void Set(T value)
        {
            Blob.Set(this, value);
        }

        public T Get()
        {
            return Blob.Get(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
