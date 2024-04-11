using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


// Server to Client
public enum ServerPackets
{
    // Server packets
    welcome = 1,
    spawnPlayer,
    playerPosition,
    playerRotation,
    playerDisconnected,
    createCoinSpawner,
    coinSpawned,
    coinPickedUp

}

// Client to Server
public enum ClientPackets
{
    // Client packets
    welcomeReceived = 1,
    playerMovement
}

public class Packet : IDisposable
{
    private List<byte> buffer; // The packet buffer
    private byte[] readableBuffer; // Readable buffer
    private int readPos; // Read position

    public Packet()
    {
        buffer = new List<byte>(); // Initialises the buffer
        readPos = 0; // Sets reading position to 0
    }

    public Packet(int id)
    {
        buffer = new List<byte>(); // Initialises the buffer
        readPos = 0; // Sets reading position to 0

        Write(id); // Writes the packet ID to the buffer
    }

    public Packet(byte[] data)
    {
        buffer = new List<byte>(); // Initialises the buffer
        readPos = 0; // Sets reading position to 0

        SetBytes(data); // sets bytes to the data that was passed in
    }

    // Sets packets contents and prepares it to be read
    public void SetBytes(byte[] data)
    {
        Write(data); // Write the data to the buffer
        readableBuffer = buffer.ToArray(); // Converts the buffer to a readable array
    }

    public void WriteLength()
    {
        // Inserts the byte length of the packet at the start of the buffer
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
    }

    public void InsertInt(int value)
    {
        // Inserts the int at the start of the buffer
        buffer.InsertRange(0, BitConverter.GetBytes(value));
    }

    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray(); // Converts the buffer to a readable array
        return readableBuffer; // Returns the readable buffer
    }

    public int Length()
    {
        return buffer.Count; // Returns the length of the buffer
    }

    public int UnreadLength()
    {
        return Length() - readPos; // Returns the remaining unread length of the buffer
    }

    public void Reset(bool shouldReset = true)
    {
        if (shouldReset)
        {
            buffer.Clear(); // Clears the buffer
            readableBuffer = null; // Clears the readable buffer
            readPos = 0; // Resets the read position
        }
        else
        {
            readPos -= 4; // "Unreads" the last read int
        }
    }

    // All of the overloaded write methods which convert data types to bytes and write them to the buffer
    public void Write(byte value)
    {
        buffer.Add(value); // Adds the byte to the buffer
    }

    public void Write(byte[] value)
    {
        buffer.AddRange(value); // Adds the byte array to the buffer
    }

    public void Write(short value)
    {
        buffer.AddRange(BitConverter.GetBytes(value)); // Adds the short to the buffer
    }

    public void Write(int value)
    {
        buffer.AddRange(BitConverter.GetBytes(value)); // Adds the int to the buffer
    }

    public void Write(long value)
    {
        buffer.AddRange(BitConverter.GetBytes(value)); // Adds the long to the buffer
    }

    public void Write(float value)
    {
        buffer.AddRange(BitConverter.GetBytes(value)); // Adds the float to the buffer
    }

    public void Write(bool value)
    {
        buffer.AddRange(BitConverter.GetBytes(value)); // Adds the bool to the buffer
    }

    public void Write(string value)
    {
        Write(value.Length); // Adds the length of the string to the buffer
        buffer.AddRange(Encoding.ASCII.GetBytes(value)); // Adds the string to the buffer
    }

    public void Write(Vector2 value)
    {
        Write(value.x); // Adds the X component of the vector to the buffer
        Write(value.y); // Adds the Y component of the vector to the buffer
    }

    public void Write(Vector3 value)
    {
        Write(value.x); // Adds the X component of the vector to the buffer
        Write(value.y); // Adds the Y component of the vector to the buffer
        Write(value.z); // Adds the Z component of the vector to the buffer
    }

    public void Write(Quaternion value)
    {
        Write(value.x); // Adds the X component of the vector to the buffer
        Write(value.y); // Adds the Y component of the vector to the buffer
        Write(value.z); // Adds the Z component of the vector to the buffer
        Write(value.w); // Adds the W component of the vector to the buffer
    }

    // All of the read methods to read the data from the buffer
    public byte ReadByte(bool moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            byte value = readableBuffer[readPos]; // Get the byte at the read position
            if (moveReadPos)
            {
                // If we should move the read position
                readPos += 1; // Move the read position
            }
            return value; // Return the byte
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }

    public byte[] ReadBytes(int length, bool moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            byte[] value = buffer.GetRange(readPos, length).ToArray(); // Get the bytes at the read position with the amount of length
            if (moveReadPos)
            {
                // If we should move the read position
                readPos += length; // Move the read position
            }
            return value; // Return the bytes
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }

    public short ReadShort(bool moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            short value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
            if (moveReadPos)
            {
                // If we should move the read position
                readPos += 2; // Move the read position
            }
            return value; // Return the short
        }
        else
        {
            throw new Exception("Could not read value of type 'short'!");
        }
    }

    public int ReadInt(bool moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            int value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
            if (moveReadPos)
            {
                // If we should move the read position
                readPos += 4; // Move the read position
            }
            return value; // Return the int
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }

    public long ReadLong(bool moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            long value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
            if (moveReadPos)
            {
                // If we should move the read position
                readPos += 8; // Move the read position
            }
            return value; // Return the long
        }
        else
        {
            throw new Exception("Could not read value of type 'long'!");
        }
    }

    public float ReadFloat(bool moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            float value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
            if (moveReadPos)
            {
                // If we should move the read position
                readPos += 4; // Move the read position
            }
            return value; // Return the float
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }

    public bool ReadBool(bool moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            bool value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
            if (moveReadPos)
            {
                // If we should move the read position
                readPos += 1; // Move the read position
            }
            return value; // Return the bool
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }

    public string ReadString(bool moveReadPos = true)
    {
        try
        {
            int length = ReadInt(); // Get the length of the string
            string value = Encoding.ASCII.GetString(readableBuffer, readPos, length); // Convert the bytes to a string
            if (moveReadPos && value.Length > 0)
            {
                // If we should move the read position
                readPos += length; // Move the read position
            }
            return value; // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    public Vector2 ReadVector2(bool moveReadPos = true)
    {
        return new Vector2(ReadFloat(moveReadPos), ReadFloat(moveReadPos)); // Return a new vector2
    }

    public Vector3 ReadVector3(bool moveReadPos = true)
    {
        return new Vector3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos)); // Return a new vector3
    }

    public Quaternion ReadQuaternion(bool moveReadPos = true)
    {
        return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos)); // Return a new quaternion
    }

    private bool disposed = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                buffer.Clear(); // Clear the buffer
            }

            disposed = true; // Indicate that the instance has been disposed
        }
    }

    public void Dispose()
    {
        Dispose(true); // Dispose of the instance
        GC.SuppressFinalize(this); // Suppress finalization
    }
}

