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
/// serialized byte stream is signed with RSA
/// </summary>
/// <remarks>
/// Use RSASignedWrapper to easily sign an object
/// so when it is serialized using the BinaryFormatter
/// or SoapFormatter the object's data is signed
/// and can be later verified.
/// </remarks>
namespace SerializationWrapper
{
	[Serializable()]
	public class RSASignedWrapper<T>
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
	  /// <param name="xmlPublicKey">A valid RSA public key in XML format</param>
	  public bool Verify(string xmlPublicKey)
	  {

		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		try
		{
		  rsa.FromXmlString(xmlPublicKey);
		  return Verify(rsa, "SHA1");

		}
		finally
		{
		  rsa.Clear();
		  ((IDisposable)rsa).Dispose();
		}

	  }

	  /// <summary>
	  /// Verifies the signature of the wrapped object
	  /// </summary>
	  /// <param name="rsa">An instance of a preconfigured RSACryptoServiceProvider</param>
	  /// <param name="halg">A hash algorithm (String, algorithm or type) as required by RSACryptoServiceProvider.VerifyData</param>
	  public bool Verify(RSACryptoServiceProvider rsa, object halg)
	  {

		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, mObject);

		return rsa.VerifyData(buffer.ToArray(), halg, mSignature);

	  }

	#endregion

	#region  Sign data 

	  /// <summary>
	  /// Creates an instance of the class, initializing it
	  /// with the wrapped object, which is immediately signed
	  /// </summary>
	  /// <param name="wrappedObject">The object to be signed</param>
	  /// <param name="xmlPrivateKey">A valid RSA public/private key pair in XML format</param>
	  public RSASignedWrapper(T wrappedObject, string xmlPrivateKey)
	  {

		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		try
		{
		  rsa.FromXmlString(xmlPrivateKey);
		  SignObject(wrappedObject, rsa, "SHA1");

		}
		finally
		{
		  rsa.Clear();
		  ((IDisposable)rsa).Dispose();
		}

	  }

	  /// <summary>
	  /// Creates an instance of the class, initializing it
	  /// with the wrapped object, which is immediately signed
	  /// </summary>
	  /// <param name="wrappedObject">The object to be signed</param>
	  /// <param name="xmlPrivateKey">A valid RSA public/private key pair in XML format</param>
	  /// <param name="halg">A hash algorithm (String, algorithm or type) as required by RSACryptoServiceProvider.VerifyData</param>
	  public RSASignedWrapper(T wrappedObject, string xmlPrivateKey, object halg)
	  {

		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		try
		{
		  rsa.FromXmlString(xmlPrivateKey);
		  SignObject(wrappedObject, rsa, halg);

		}
		finally
		{
		  rsa.Clear();
		  ((IDisposable)rsa).Dispose();
		}

	  }

	  /// <summary>
	  /// Creates an instance of the class, initializing it
	  /// with the wrapped object, which is immediately signed
	  /// </summary>
	  /// <param name="wrappedObject">The object to be signed</param>
	  /// <param name="rsa">An instance of a preconfigured RSACryptoServiceProvider</param>
	  /// <param name="halg">A hash algorithm (String, algorithm or type) as required by RSACryptoServiceProvider.VerifyData</param>
	  public RSASignedWrapper(T wrappedObject, RSACryptoServiceProvider rsa, object halg)
	  {

		SignObject(wrappedObject, rsa, halg);

	  }

	  private void SignObject(T wrappedObject, RSACryptoServiceProvider rsa, object halg)
	  {

		mObject = wrappedObject;
		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream buffer = new MemoryStream();
		formatter.Serialize(buffer, wrappedObject);

		mSignature = rsa.SignData(buffer.ToArray(), halg);

	  }

	#endregion

	}

} //end of root namespace