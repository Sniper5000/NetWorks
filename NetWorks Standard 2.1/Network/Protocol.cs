using MemoryPack;
using NetWorks;
using System;
using System.Collections.Generic;

public class Protocol
{
    private int messageIdCounter = 1;
    private Dictionary<Type, int> messageTypeToId = new Dictionary<Type, int>();
    private Dictionary<int, Type> messageIdToType = new Dictionary<int, Type>();


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
        Protocol protocol = new Protocol();

        foreach(Type type in protocolType.GetNestedTypes())
        {
            protocol.RegisterMessage(type);
        }

        return protocol;
    }

    public static void Dispatch(Protocol protocol, byte[] payload, object target)
    {
        Message? messageObject = Transports.DeserializeBClass<Message>(payload) ?? throw new ArgumentException("The payload is invalid", nameof(payload));
        Dispatch(protocol, messageObject, target);
    }

    public static void Dispatch(Protocol protocol, Message messageObject, object target)
    {
        if (!protocol.IsValidMessageId(messageObject.Id))
            throw new ArgumentException("Attempted to dispatch an invalid protocol method", nameof(messageObject));
        
        Type messageType = protocol.GetMessageType(messageObject.Id);
        
        try
        {
            object? message = Transports.DeserializeBField(messageObject.Body, messageType)
                ?? throw new ArgumentException("The message contents are invalid", nameof(messageObject));
            protocol.Invoke(messageObject.Id, message, target);
        }
        catch (Exception e) when (e is ArgumentException)
        {
            throw new ArgumentException("The message contents are invalid", nameof(messageObject), e);
        }
    }

    public static byte[] Serialize(Protocol protocol, object message)
    {
        int id = protocol.GetMessageId(message.GetType());
        byte[] body = Transports.SerializeBClass(message);
        Message messageObject = new Message(id, body);
        return Transports.SerializeBClass(messageObject);
    }
}

[MemoryPackable]
public partial class Message 
{
    public int Id;
    public byte[] Body;
    public Message(int Id, byte[] Body)
    {
        this.Id = Id;
        this.Body = Body;
    }
};
