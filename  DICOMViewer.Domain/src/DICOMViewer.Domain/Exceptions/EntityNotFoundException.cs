using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Domain.Exceptions;

[Serializable]
public class EntityNotFoundException : DomainException
{
    public Type EntityType { get; }
    public object? Id { get; }

    public EntityNotFoundException(Type entityType, object? id) 
        : base($"{entityType.Name} with ID {id} not found")
    {
        EntityType = entityType;
        Id = id;
    }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        EntityType = Type.GetType(info.GetString("EntityType")!)!;
        Id = info.GetValue("Id", typeof(object));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("EntityType", EntityType.AssemblyQualifiedName);
        info.AddValue("Id", Id);
    }
}