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
/// serialized byte stream is signed with DSA
/// </summary>
/// <remarks>
/// Use DSASignedWrapper to easily sign an object
/// so when it is serialized using the BinaryFormatter
/// or SoapFormatter the object's data is signed
/// and can be later verified.
/// </remarks>
namespace SerializationWrapper
{
	[Serializable()]
	public class DSASignedWrapper<T>
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

	#region  Verify signature 

	  /// <summary>
	  /// Verifies the signature of the wrapped object
	  /// </summary>
	  /// <param name="xmlPublicKey">A valid DSA public key in XML format</param>
	  public bool Verify(string xmlPublicKey)
	  {

		DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
		try
		{
		  dsa.FromXmlString(xmlPublicKey);
		  return Verify(dsa);

		}
		finally
		{
		  dsa.Clear();
		  ((IDisposable)dsa).Dispose();
		}

	  }

	  /// <summary>
	  /// Verifies the signature of the wrapped object
	  /// </summary>
	  /// <param name="dsa">An instance of a preconfigured DSACryptoServiceProvider</param>
	  public bool Verify(DSACryptoServiceProvider dsa)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, mObject);

		return dsa.VerifyData(buffer.ToArray(), mSignature);

	  }

	#endregion

	#region  Sign data 

	  /// <summary>
	  /// Creates an instance of the class, initializing it
	  /// with the wrapped object, which is immediately signed
	  /// </summary>
	  /// <param name="wrappedObject">The object to be signed</param>
	  /// <param name="xmlPrivateKey">A valid DSA public/private key pair in XML format</param>
	  public DSASignedWrapper(T wrappedObject, string xmlPrivateKey)
	  {

		DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
		try
		{
		  dsa.FromXmlString(xmlPrivateKey);
		  SignObject(wrappedObject, dsa);

		}
		finally
		{
		  dsa.Clear();
		  ((IDisposable)dsa).Dispose();
		}

	  }

	  /// <summary>
	  /// Creates an instance of the class, initializing it
	  /// with the wrapped object, which is immediately signed
	  /// </summary>
	  /// <param name="wrappedObject">The object to be signed</param>
	  /// <param name="dsa">An instance of a preconfigured DSACryptoServiceProvider</param>
	  public DSASignedWrapper(T wrappedObject, DSACryptoServiceProvider dsa)
	  {

		SignObject(wrappedObject, dsa);

	  }

	  private void SignObject(T wrappedObject, DSACryptoServiceProvider dsa)
	  {

		mObject = wrappedObject;
		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, wrappedObject);

		mSignature = dsa.SignData(buffer.ToArray());

	  }

	#endregion

	}

} //end of root namespace