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
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// Wraps another object such that the object's 
/// serialized byte stream is encrypted.
/// </summary>
/// <remarks>
/// Use CryptoWrapper to easily encrypt an object
/// so when it is serialized using the BinaryFormatter
/// or SoapFormatter the object's data is encrypted.
/// </remarks>
namespace SerializationWrapper
{
	[Serializable()]
	public class CryptoWrapper : CryptoWrapperBase
	{

	  private byte[] mObject;
	  private string mIV;

	  /// <summary>
	  /// Returns a deserialized and decrypted instance of
	  /// the wrapped object
	  /// </summary>
	  /// <param name="base64Key">Base64 encoded byte array containing the encryption key</param>
	  /// <returns>The wrapped object</returns>
	  public object GetObject(string base64Key)
	  {

		return GetObject(AlgorithmType.TripleDES, base64Key);

	  }

	  /// <summary>
	  /// Returns a deserialized and decrypted instance of
	  /// the wrapped object
	  /// </summary>
	  /// <param name="calg">Type of algorithm to use for decryption</param>
	  /// <param name="base64Key">Base64 encoded byte array containing the encryption key</param>
	  /// <returns>The wrapped object</returns>
	  public object GetObject(AlgorithmType calg, string base64Key)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream(mObject);
		buffer = new MemoryStream(Decrypt(buffer.ToArray(), Algorithm(calg), base64Key, mIV));

		return formatter.Deserialize(buffer);

	  }

	  /// <summary>
	  /// Creates an instance of the class, initializing
	  /// it with the wrapped object, which is immediately
	  /// serialized and encrypted.
	  /// </summary>
	  /// <remarks>
	  /// <para>
	  /// The wrappedObject must be marked with the Serializable attribute.
	  /// </para><para>
	  /// The IV value is randomly generated.
	  /// </para>
	  /// </remarks>
	  /// <param name="wrappedObject">The object to be encrypted</param>
	  /// <param name="base64Key">Base64 encoded byte array containing the encryption key</param>
	  public CryptoWrapper(object wrappedObject, string base64Key) : this(wrappedObject, AlgorithmType.TripleDES, base64Key)

	  {

	  }

	  /// <summary>
	  /// Creates an instance of the class, initializing
	  /// it with the wrapped object, which is immediately
	  /// serialized and encrypted.
	  /// </summary>
	  /// <remarks>
	  /// <para>
	  /// The wrappedObject must be marked with the Serializable attribute.
	  /// </para><para>
	  /// The IV value is randomly generated.
	  /// </para>
	  /// </remarks>
	  /// <param name="wrappedObject">The object to be encrypted</param>
	  /// <param name="calg">Type of algorithm to use for encryption</param>
	  /// <param name="base64Key">Base64 encoded byte array containing the encryption key</param>
	  public CryptoWrapper(object wrappedObject, AlgorithmType calg, string base64Key)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, wrappedObject);
		SymmetricAlgorithm crypto = Algorithm(calg);

		// encrypt here
		mIV = CreateBase64IV(crypto);
		buffer = new MemoryStream(Encrypt(buffer.ToArray(), crypto, base64Key, mIV));
		mObject = buffer.ToArray();

	  }

	}

} //end of root namespace