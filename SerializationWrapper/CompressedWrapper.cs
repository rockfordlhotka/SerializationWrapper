//ORIGINAL LINE: Imports System.Configuration.ConfigurationSettings
//INSTANT C# NOTE: The following line has been modified since C# non-aliased 'using' statements only operate on namespaces:
//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
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
  public class CompressedWrapper : CompressedWrapperBase
  {

    private byte[] _objectData;

    /// <summary>
    /// Returns the wrapped object
    /// </summary>
    /// <returns>The wrapped object</returns>
    public object GetObject()
    {
      return GetObject(CompressionType.Deflate);
    }

    /// <summary>
    /// Returns the wrapped object
    /// </summary>
    /// <returns>The wrapped object</returns>
    /// <param name="compressionType">Compression algorithm to use</param>
    public object GetObject(CompressionType compressionType)
    {

      BinaryFormatter formatter = new BinaryFormatter();
      using (MemoryStream serialized = new MemoryStream(Decompress(_objectData, compressionType)))
      {
        return formatter.Deserialize(serialized);
      }

    }

    /// <summary>
    /// Gets or sets the compressed bytes contained
    /// in the wrapper object.
    /// </summary>
    public byte[] CompressedData
    {
      get
      {
        return _objectData;
      }
      set
      {
        _objectData = value;
      }
    }

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
    public CompressedWrapper(object wrappedObject)
      : this(wrappedObject, CompressionType.Deflate)
    {

    }

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
    public CompressedWrapper(object wrappedObject, CompressionType compressionType)
    {

      BinaryFormatter formatter = new BinaryFormatter();
      using (MemoryStream serialized = new MemoryStream())
      {
        // serialize the object
        formatter.Serialize(serialized, wrappedObject);
        serialized.Position = 0;
        // compress the serialized data
        _objectData = Compress(serialized.ToArray(), compressionType);
      }

    }

  }

} //end of root namespace