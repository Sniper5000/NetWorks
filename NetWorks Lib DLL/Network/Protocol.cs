using System.Text.Json;

public class Protocol
{
    private int messageIdCounter = 1;
    private Dictionary<Type, int> messageTypeToId = new();
    private Dictionary<int, Type> messageIdToType = new();


    public void RegisterMessage(Type messageType)
    {
        int messageId = messageIdCounter++;
        messageTypeToId[messageType] = messageId;
        messageIdToType[messageId] = messageType;
    }

    public int GetMessageId(Type messageType)
    {
        return messageTypeToId[messageType];
    }
    
    public Type GetMessageType(int messageId)
    {
        return messageIdToType[messageId];
    }

    public bool IsValidMessageId(int messageId)
    {
        return messageIdToType.ContainsKey(messageId);
    }

    public void Invoke(int messageId, object message, object target)
    {
        var methodInfo = GetMessageType(messageId).GetMethod("Receive");
        
        if(methodInfo == null)
            throw new TypeAccessException("The message class lacks a Receive method");

        methodInfo.Invoke(message, new object[] { target });
    }
}

public static class ProtocolMethods
{

    public static Protocol BuildFromType(Type protocolType)
    {
        Protocol protocol = new();

        foreach(Type type in protocolType.GetNestedTypes())
        {
            protocol.RegisterMessage(type);
        }

        return protocol;
    }

    public static void Dispatch(Protocol protocol, byte[] payload, object target)
    {
        Message? messageObject = JsonSerializer.Deserialize<Message>(payload) ?? throw new ArgumentException("The payload is invalid", nameof(payload));
        Dispatch(protocol, messageObject, target);
    }

    public static void Dispatch(Protocol protocol, Message messageObject, object target)
    {
        if (!protocol.IsValidMessageId(messageObject.Id))
            throw new ArgumentException("Attempted to dispatch an invalid protocol method", nameof(messageObject));
        
        Type messageType = protocol.GetMessageType(messageObject.Id);
        
        try
        {
            object? message = JsonSerializer.Deserialize(messageObject.Body, messageType)
                ?? throw new ArgumentException("The message contents are invalid", nameof(messageObject));
            protocol.Invoke(messageObject.Id, message, target);
        }
        catch (Exception e) when (e is JsonException || e is ArgumentException)
        {
            throw new ArgumentException("The message contents are invalid", nameof(messageObject), e);
        }
    }

    public static byte[] Serialize(Protocol protocol, object message)
    {
        int id = protocol.GetMessageId(message.GetType());
        string body = JsonSerializer.Serialize(message);
        Message messageObject = new(id, body);
        return JsonSerializer.SerializeToUtf8Bytes(messageObject);
    }
}

public record Message(int Id, string Body);
