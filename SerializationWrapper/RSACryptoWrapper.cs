//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using System.Security.Cryptography;
//ORIGINAL LINE: Imports System.Configuration.ConfigurationSettings
//INSTANT C# NOTE: The following line has been modified since C# non-aliased 'using' statements only operate on namespaces:
using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Wraps another object such that the object's 
/// serialized byte stream is encrypted using RSA.
/// </summary>
/// <remarks>
/// Use RSACryptoWrapper to easily encrypt an object
/// so when it is serialized using the BinaryFormatter
/// or SoapFormatter the object's data is encrypted.
/// </remarks>
namespace SerializationWrapper
{
	[Serializable()]
	public class RSACryptoWrapper<T>
	{

	  private byte[] mObject;

	#region  Decrypt 

	  /// <summary>
	  /// Returns a deserialized and decrypted instance of
	  /// the wrapped object
	  /// </summary>
	  /// <param name="xmlPrivateKey">A valid RSA public/private key in XML format</param>
	  /// <returns>The wrapped object</returns>
	  public T GetObject(string xmlPrivateKey)
	  {

		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		try
		{
		  rsa.FromXmlString(xmlPrivateKey);

		  return GetObject(rsa);

		}
		finally
		{
		  rsa.Clear();
		  ((IDisposable)rsa).Dispose();
		}

	  }

	  /// <summary>
	  /// Returns a deserialized and decrypted instance of
	  /// the wrapped object
	  /// </summary>
	  /// <param name="rsa">An instance of a preconfigured RSACryptoServiceProvider</param>
	  /// <returns>The wrapped object</returns>
	  public T GetObject(RSACryptoServiceProvider rsa)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream(mObject);
		MemoryStream output = new MemoryStream(System.Convert.ToInt32(buffer.Length));

		int keySizeInBytes = rsa.KeySize / 8;
		int blockSize = keySizeInBytes - 11;
		int iterations = System.Convert.ToInt32(buffer.Length / keySizeInBytes);

		buffer.Position = 0;
		for (int counter = 1; counter <= iterations; counter++)
		{
		  byte[] rgb = new byte[keySizeInBytes];
		  buffer.Read(rgb, 0, keySizeInBytes);
		  byte[] result = rsa.Decrypt(rgb, false);
		  output.Write(result, 0, result.Length);
		}

		output.Position = 0;
		return (T)(formatter.Deserialize(output));

	  }

	#endregion

	#region  Encrypt 

	  /// <summary>
	  /// Creates an instance of the class, initializing
	  /// it with the wrapped object, which is immediately
	  /// serialized and encrypted.
	  /// </summary>
	  /// <remarks>
	  /// The wrappedObject must be marked with the Serializable attribute.
	  /// </remarks>
	  /// <param name="wrappedObject">The object to be encrypted</param>
	  /// <param name="xmlPublicKey">A valid RSA public key in XML format</param>
	  public RSACryptoWrapper(T wrappedObject, string xmlPublicKey)
	  {

		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		try
		{
		  rsa.FromXmlString(xmlPublicKey);

		  Encrypt(wrappedObject, rsa);

		}
		finally
		{
		  rsa.Clear();
		  ((IDisposable)rsa).Dispose();
		}

	  }

	  /// <summary>
	  /// Creates an instance of the class, initializing
	  /// it with the wrapped object, which is immediately
	  /// serialized and encrypted.
	  /// </summary>
	  /// <remarks>
	  /// The wrappedObject must be marked with the Serializable attribute.
	  /// </remarks>
	  /// <param name="wrappedObject">The object to be encrypted</param>
	  /// <param name="rsa">An instance of a preconfigured RSACryptoServiceProvider</param>
	  public RSACryptoWrapper(T wrappedObject, RSACryptoServiceProvider rsa)
	  {

		Encrypt(wrappedObject, rsa);

	  }

	  private void Encrypt(T wrappedObject, RSACryptoServiceProvider rsa)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, wrappedObject);

		int keySizeInBytes = rsa.KeySize / 8;
		int blockSize = keySizeInBytes - 11;
		int iterations = System.Convert.ToInt32(buffer.Length / blockSize);
		int oddBytes = System.Convert.ToInt32(buffer.Length % blockSize);
		if (oddBytes != 0)
		{
		  iterations += 1;
		}

		mObject = new byte[iterations * keySizeInBytes];

		buffer.Position = 0;
		int writeIndex = 0;
		for (int counter = 1; counter <= iterations; counter++)
		{
		  int bytesToEncode = 0;
		  if (counter == 1 & buffer.Length < blockSize)
		  {
			bytesToEncode = System.Convert.ToInt32(buffer.Length);

		  }
		  else if (counter == iterations)
		  {
			bytesToEncode = oddBytes;

		  }
		  else
		  {
			bytesToEncode = blockSize;
		  }

		  byte[] rgb = new byte[bytesToEncode];
		  buffer.Read(rgb, 0, bytesToEncode);
		  byte[] result = rsa.Encrypt(rgb, false);
		  Array.Copy(result, 0, mObject, writeIndex, result.Length);
		  writeIndex += keySizeInBytes;
		}

	  }

	#endregion

	}

} //end of root namespace