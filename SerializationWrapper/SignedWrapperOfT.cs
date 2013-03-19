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
/// serialized byte stream is signed
/// </summary>
/// <remarks>
/// Use SignedWrapper to easily sign an object
/// so when it is serialized using the BinaryFormatter
/// or SoapFormatter the object's data is signed
/// and can be later verified.
/// </remarks>
namespace SerializationWrapper
{
	[Serializable()]
	public class SignedWrapper<T> : SignedWrapperBase
	{

	  private T mObject;
	  private byte[] mSignature;

	  /// <summary>
	  /// Returns the wrapped object
	  /// </summary>
	  /// <returns>The wrapped object</returns>
	  public T GetObject()
	  {

		return mObject;

	  }

	#region  Verify Signature 

	  /// <summary>
	  /// Verifies the signature of the wrapped object
	  /// </summary>
	  /// <remarks>
	  /// The HMACSHA1 algorithm is used by this method. To use
	  /// other algorithms see the overloads of this method.
	  /// </remarks>
	  /// <param name="base64Key">Base64 encoded byte array containing the hash key</param>
	  /// <returns>True of the signature is valid</returns>
	  public bool Verify(string base64Key)
	  {

		return Verify(AlgorithmType.HMACSHA1, base64Key);

	  }

	  /// <summary>
	  /// Verifies the signature of the wrapped object
	  /// </summary>
	  /// <param name="halg">The type of hash algorithm to use</param>
	  /// <param name="base64Key">Base64 encoded byte array containing the hash key</param>
	  /// <returns>True of the signature is valid</returns>
	  public bool Verify(AlgorithmType halg, string base64Key)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, mObject);
		byte[] signature = ComputeHash(buffer.ToArray(), Algorithm(halg, base64Key), base64Key);

		// check signature here
		if (! (CompareArrays(signature, mSignature)))
		{
		  return false;
		}

		return true;

	  }

	#endregion

	#region  Sign data 

	  /// <summary>
	  /// Creates an instance of the class, initializing it
	  /// with the wrapped object, which is immediately signed
	  /// </summary>
	  /// <remarks>
	  /// The wrappedObject must be marked with the Serializable attribute.
	  /// </remarks>
	  /// <param name="wrappedObject">The object to be signed</param>
	  /// <param name="halg">The type of hash algorithm to use</param>
	  /// <param name="base64Key">Base64 encoded byte array containing the hash key</param>
	  public SignedWrapper(T wrappedObject, AlgorithmType halg, string base64Key)
	  {

		mObject = wrappedObject;
		SignObject(halg, base64Key);

	  }

	  /// <summary>
	  /// Creates an instance of the class, initializing it
	  /// with the wrapped object, which is immediately signed
	  /// </summary>
	  /// <param name="wrappedObject">The object to be signed</param>
	  /// <param name="base64Key">Base64 encoded byte array containing the hash key</param>
	  public SignedWrapper(T wrappedObject, string base64Key)
	  {

		mObject = wrappedObject;
		SignObject(AlgorithmType.HMACSHA1, base64Key);

	  }

	  private void SignObject(AlgorithmType halg, string base64Key)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, mObject);
		mSignature = ComputeHash(buffer.ToArray(), Algorithm(halg, base64Key), base64Key);

	  }

	#endregion

	}

} //end of root namespace