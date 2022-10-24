namespace ExplogineCore;

public class SerialBlob
{
    private const char SeparatorChar = ' ';

    private readonly Dictionary<IDescriptor, object> _assignedVariables = new();
    private readonly Dictionary<string, IDescriptor> _declaredVariables = new();

    public Descriptor<T> Declare<T>(string variableName)
    {
        if (variableName.Contains(SerialBlob.SeparatorChar))
        {
            throw new Exception($"\"{variableName}\" contains illegal character {SerialBlob.SeparatorChar}.");
        }

        if (_declaredVariables.ContainsKey(variableName))
        {
            throw new Exception($"Duplicate variable declaration {variableName}");
        }

        var descriptor = new Descriptor<T>(variableName);
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
    }

    private void SetUnsafe(IDescriptor descriptor, object value)
    {
        ConfirmDeclared(descriptor);

        if (value == null)
        {
            throw new ArgumentNullException();
        }

        _assignedVariables[descriptor] = value;
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

    public void Read(IFileSystem fileSystem, string fileName)
    {
        var lines = fileSystem.ReadFile(fileName).SplitLines();
        foreach (var line in lines)
        {
            var split = line.Split(SerialBlob.SeparatorChar);
            if (split.Length != 2)
            {
                throw new Exception("Blob was in an unexpected format");
            }

            var name = split[0];
            var data = split[1];

            if (_declaredVariables.ContainsKey(name))
            {
                var descriptor = _declaredVariables[name];

                var type = descriptor.GetUnderlyingType();
                var result = Convert.ChangeType(data, type);
                SetUnsafe(descriptor, result);
            }
        }
    }

    public string[] DataAsStrings()
    {
        var lines = new List<string>();

        foreach (var data in _assignedVariables)
        {
            lines.Add($"{data.Key.Name}{SerialBlob.SeparatorChar}{data.Value}");
        }

        return lines.ToArray();
    }

    /// <summary>
    ///     This is here because we need a non-generic to key the Dictionary on
    /// </summary>
    private interface IDescriptor
    {
        public string Name { get; }
        public Type GetUnderlyingType();
    }

    public readonly record struct Descriptor<T>(string Name) : IDescriptor
    {
        public Type GetUnderlyingType()
        {
            return typeof(T);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
