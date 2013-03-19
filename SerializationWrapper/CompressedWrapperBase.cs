using System;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Base class for compression wrappers.
/// </summary>
namespace SerializationWrapper
{
  [Serializable()]
  public abstract class CompressedWrapperBase
  {
    public enum CompressionType : int
    {
      Deflate,
      GZip
    }

    protected static Stream Compression(Stream stream, CompressionType type, CompressionMode mode)
    {
      switch (type)
      {
        case CompressionType.GZip:
          return new GZipStream(stream, mode);
        default:
          return new DeflateStream(stream, mode);
      }
    }

    protected static byte[] Compress(byte[] obj, CompressionType compressionType)
    {
      using (MemoryStream buffer = new MemoryStream())
      {
        using (Stream comp = Compression(buffer, compressionType, CompressionMode.Compress))
        {
          comp.Write(obj, 0, System.Convert.ToInt32(obj.Length));
        }
        return buffer.ToArray();
      }
    }

    protected static byte[] Decompress(byte[] obj, CompressionType compressionType)
    {
      var result = new List<byte>();
      using (MemoryStream buffer = new MemoryStream(obj))
      {
        // decompress the byte array
        using (Stream comp = Compression(buffer, compressionType, CompressionMode.Decompress))
        {
          var b = comp.ReadByte();
          while (b > -1)
          {
            result.Add((byte)b);
            b = comp.ReadByte();
          }
        }
      }
      return result.ToArray();
    }
  }
}