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
/// Base class for symmetric encryption wrappers.
/// </summary>
namespace SerializationWrapper
{
	[Serializable()]
	public abstract class CryptoWrapperBase
	{

	#region  Algorithm Type 

	  /// <summary>
	  /// The supported encryption algorithms
	  /// </summary>
	  public enum AlgorithmType: int
	  {
		TripleDES,
		Rijndael,
		DES,
		RC2
	  }

	  protected static SymmetricAlgorithm Algorithm(AlgorithmType type)
	  {

		switch (type)
		{
		  case AlgorithmType.DES:
			return DESCryptoServiceProvider.Create();

		  case AlgorithmType.RC2:
			return RC2CryptoServiceProvider.Create();

		  case AlgorithmType.Rijndael:
			return RijndaelManaged.Create();

		  default:
			return TripleDESCryptoServiceProvider.Create();
		}

	  }

	#endregion

	#region  Encrypt/Decrypt 

	  protected static byte[] Decrypt(byte[] cipherText, SymmetricAlgorithm encryptionAlgorithm, string base64Key, string base64IV)
	  {

		if (base64Key == null || base64Key.Length == 0)
		{
		  throw new Exception("Empty Key");
		}
		if (base64IV == null || base64IV.Length == 0)
		{
		  throw new Exception("Empty Initialization Vector");
		}
		encryptionAlgorithm.Key = Convert.FromBase64String(base64Key);
		encryptionAlgorithm.IV = Convert.FromBase64String(base64IV);

		byte[] cipherValue = cipherText;
		byte[] plainValue = new byte[cipherValue.Length + 1];

		MemoryStream memStream = new MemoryStream(cipherValue);
		CryptoStream cryptoStream = new CryptoStream(memStream, encryptionAlgorithm.CreateDecryptor(), CryptoStreamMode.Read);
		try
		{
		  // Decrypt the data
		  cryptoStream.Read(plainValue, 0, plainValue.Length);

		}
		finally
		{
		  // Clear the arrays
		  if (cipherValue != null)
		  {
			Array.Clear(cipherValue, 0, cipherValue.Length);
		  }
		  memStream.Close();
		  //Flush the stream buffer
		  cryptoStream.Close();
		  encryptionAlgorithm.Clear();
		  ((IDisposable)encryptionAlgorithm).Dispose();
		}

		return plainValue;

	  }

	  protected static byte[] Encrypt(byte[] plainText, SymmetricAlgorithm encryptionAlgorithm, string base64Key, string base64IV)
	  {

		if (base64Key == null || base64Key.Length == 0)
		{
		  throw new Exception("Empty Key");
		}
		if (base64IV == null || base64IV.Length == 0)
		{
		  throw new Exception("Empty Initialization Vector");
		}
		encryptionAlgorithm.Key = Convert.FromBase64String(base64Key);
		encryptionAlgorithm.IV = Convert.FromBase64String(base64IV);

		byte[] cipherValue = null;
		byte[] plainValue = plainText;

		MemoryStream memStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memStream, encryptionAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
		try
		{
		  // Write the encrypted information
		  cryptoStream.Write(plainValue, 0, plainValue.Length);
		  cryptoStream.Flush();
		  cryptoStream.FlushFinalBlock();

		  // Get the encrypted stream
		  cipherValue = memStream.ToArray();

		}
		finally
		{
		  // Clear the arrays
		  if (plainValue != null)
		  {
			Array.Clear(plainValue, 0, plainValue.Length);
		  }
		  memStream.Close();
		  cryptoStream.Close();
		  encryptionAlgorithm.Clear();
		  ((IDisposable)encryptionAlgorithm).Dispose();
		}

		return cipherValue;

	  }

	#endregion

	#region  Create Keys 

	  /// <summary>
	  /// Creates a random IV value appropriate for
	  /// the encryption algorithm
	  /// </summary>
	  /// <param name="encryptionAlgorithm">Instance of SymmetricAlgorithm used to create the IV</param>
	  /// <returns>Base64 encoded byte array containing the IV value</returns>
	  protected static string CreateBase64IV(SymmetricAlgorithm encryptionAlgorithm)
	  {

		byte[] iv = null;
		try
		{
		  encryptionAlgorithm.GenerateIV();
		  iv = encryptionAlgorithm.IV;

		}
		finally
		{
		  encryptionAlgorithm.Clear();
		  ((IDisposable)encryptionAlgorithm).Dispose();
		}
		return Convert.ToBase64String(iv);

	  }

	  /// <summary>
	  /// Creates a random key value appropriate for
	  /// the default encryption algorithm
	  /// </summary>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key()
	  {

		return CreateBase64Key(Algorithm(AlgorithmType.TripleDES));

	  }

	  /// <summary>
	  /// Creates a random key value appropriate for
	  /// the default encryption algorithm
	  /// </summary>
	  /// <param name="calg">Type of algorithm to use for encryption</param>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key(AlgorithmType calg)
	  {

		return CreateBase64Key(Algorithm(calg));

	  }

	  /// <summary>
	  /// Creates a random key value by using
	  /// the supplied password as a seed value
	  /// </summary>
	  /// <remarks>
	  /// Keys generated from passwords are far weaker than truly random keys. Also,
	  /// the random generation is seed-based which makes the random number scheme
	  /// weaker than true cryptographic random number generators. Due to this, it is
	  /// recommended that CreateBase64Key be used without a password when possible,
	  /// as that overload generates a cryptographically strong key value.
	  /// </remarks>
	  /// <param name="password">String password value used to generate the key</param>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key(string password)
	  {

		return CreateBase64Key(Algorithm(AlgorithmType.TripleDES));

	  }

	  /// <summary>
	  /// Creates a random key value by using
	  /// the supplied password as a seed value
	  /// </summary>
	  /// <remarks>
	  /// Keys generated from passwords are far weaker than truly random keys. Also,
	  /// the random generation is seed-based which makes the random number scheme
	  /// weaker than true cryptographic random number generators. Due to this, it is
	  /// recommended that CreateBase64Key be used without a password when possible,
	  /// as that overload generates a cryptographically strong key value.
	  /// </remarks>
	  /// <param name="calg">Type of algorithm to use for encryption</param>
	  /// <param name="password">String password value used to generate the key</param>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key(AlgorithmType calg, string password)
	  {

		return CreateBase64Key(Algorithm(calg), password);

	  }

	  /// <summary>
	  /// Creates a random key value appropriate for
	  /// the encryption algorithm
	  /// </summary>
	  /// <param name="encryptionAlgorithm">Instance of SymmetricAlgorithm used to create the key</param>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key(SymmetricAlgorithm encryptionAlgorithm)
	  {

		byte[] key = null;
		try
		{
		  encryptionAlgorithm.GenerateKey();
		  key = encryptionAlgorithm.Key;

		}
		finally
		{
		  encryptionAlgorithm.Clear();
		  ((IDisposable)encryptionAlgorithm).Dispose();
		}
		return Convert.ToBase64String(key);

	  }

	  /// <summary>
	  /// Creates a random key value by using
	  /// the supplied password as a seed value
	  /// </summary>
	  /// <remarks>
	  /// Keys generated from passwords are far weaker than truly random keys. Also,
	  /// the random generation is seed-based which makes the random number scheme
	  /// weaker than true cryptographic random number generators. Due to this, it is
	  /// recommended that CreateBase64Key be used without a password when possible,
	  /// as that overload generates a cryptographically strong key value.
	  /// </remarks>
	  /// <param name="encryptionAlgorithm">Instance of SymmetricAlgorithm used to create the key</param>
	  /// <param name="password">String password value used to generate the key</param>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key(SymmetricAlgorithm encryptionAlgorithm, string password)
	  {

		Random rnd = new Random(password.GetHashCode());
		int size = (int)encryptionAlgorithm.KeySize / 8;
		byte[] data = new byte[size];
		rnd.NextBytes(data);

		return Convert.ToBase64String(data);

	  }

	#endregion

	}


} //end of root namespace