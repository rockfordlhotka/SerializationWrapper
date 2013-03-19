using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Wraps another object such that the object's 
/// serialized byte stream is compressed.
/// </summary>
/// <remarks>
/// Use CompressionWrapper to easily compress an object
/// so when it is serialized using the BinaryFormatter
/// or SoapFormatter the object's data is compressed.
/// </remarks>
namespace SerializationWrapper
{
  [Serializable()]
  public class CompressedWrapper<T> : CompressedWrapperBase
  {
    /// <summary>
    /// Returns the wrapped object
    /// </summary>
    /// <returns>The wrapped object</returns>
    public T GetObject()
    {
      return GetObject(CompressionType.Deflate);
    }

    /// <summary>
    /// Returns the wrapped object
    /// </summary>
    /// <returns>The wrapped object</returns>
    /// <param name="compressionType">Compression algorithm to use</param>
    public T GetObject(CompressionType compressionType)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      using (MemoryStream serialized = new MemoryStream(Decompress(CompressedData, compressionType)))
      {
        return (T)(formatter.Deserialize(serialized));
      }
    }

    /// <summary>
    /// Gets or sets the compressed bytes contained
    /// in the wrapper object.
    /// </summary>
    public byte[] CompressedData { get; set; }

    /// <summary>
    /// Creates an instance of the class, initializing
    /// it with the compressed object, which is immediately
    /// serialized and compressed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The wrappedObject must be marked with the Serializable attribute.
    /// </para>
    /// </remarks>
    /// <param name="wrappedObject">The object to be compressed</param>
    public CompressedWrapper(T wrappedObject)
      : this(wrappedObject, CompressionType.Deflate)
    { }

    /// <summary>
    /// Creates an instance of the class, initializing
    /// it with the compressed object, which is immediately
    /// serialized and compressed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The wrappedObject must be marked with the Serializable attribute.
    /// </para>
    /// </remarks>
    /// <param name="wrappedObject">The object to be compressed</param>
    /// <param name="compressionType">Compression algorithm to use</param>
    public CompressedWrapper(T wrappedObject, CompressionType compressionType)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      using (MemoryStream serialized = new MemoryStream())
      {
        // serialize the object
        formatter.Serialize(serialized, wrappedObject);
        serialized.Position = 0;
        // compress the serialized data
        CompressedData = Compress(serialized.ToArray(), compressionType);
      }
    }
  }
}