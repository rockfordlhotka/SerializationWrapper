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
/// Base class for symmetric signed wrappers.
/// </summary>
namespace SerializationWrapper
{
	[Serializable()]
	public abstract class SignedWrapperBase
	{

	#region  AlgorithmType 

	  /// <summary>
	  /// The supported hash algorithms
	  /// </summary>
	  public enum AlgorithmType: int
	  {
		SHA1,
		MD5,
		HMACSHA1,
		SHA256,
		SHA384,
		SHA512,
		RIPEMD160
	  }

	  protected static HashAlgorithm Algorithm(AlgorithmType type, string base64Key)
	  {
		switch (type)
		{
		  case AlgorithmType.SHA1:
			return SHA1CryptoServiceProvider.Create();
		  case AlgorithmType.MD5:
			return MD5CryptoServiceProvider.Create();
		  case AlgorithmType.SHA256:
			return SHA256Managed.Create();
		  case AlgorithmType.SHA384:
			return SHA384Managed.Create();
		  case AlgorithmType.SHA512:
			return SHA512Managed.Create();
		  case AlgorithmType.RIPEMD160:
			return System.Security.Cryptography.RIPEMD160Managed.Create();
		  default:
			return new HMACSHA1(Convert.FromBase64String(base64Key));
		}
	  }

	#endregion

	#region  CreateKey 

	  /// <summary>
	  /// Creates a random key value appropriate for
	  /// the encryption algorithm
	  /// </summary>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key()
	  {

		SymmetricAlgorithm encryptionAlgorithm = TripleDESCryptoServiceProvider.Create();
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
	  /// <param name="password">String password value used to generate the key</param>
	  /// <returns>Base64 encoded byte array containing the key value</returns>
	  public static string CreateBase64Key(string password)
	  {

		Random rnd = new Random(password.GetHashCode());
		SymmetricAlgorithm encryptionAlgorithm = TripleDESCryptoServiceProvider.Create();
		int size = (int)encryptionAlgorithm.KeySize / 8;
		byte[] data = new byte[size];
		rnd.NextBytes(data);

		return Convert.ToBase64String(data);

	  }

	#endregion

	#region  Array Helpers 

	  protected static bool CompareArrays(byte[] a1, byte[] a2)
	  {

		if (a1.Length != a2.Length)
		{
		  return false;
		}
		int index = 0;
		foreach (byte b in a1)
		{
		  if (b != a2[index])
		  {
			return false;
		  }
		  index += 1;
		}
		return true;

	  }

	  private static byte[] ConcatArrays(byte[] array1, byte[] array2)
	  {

		byte[] result = new byte[array1.Length + array2.Length];
		Array.Copy(array1, 0, result, 0, array1.Length);
		Array.Copy(array2, 0, result, array1.Length, array2.Length);
		return result;

	  }

	#endregion

	#region  ComputeHash 

	  /// <summary>
	  /// Creates a hash value for the supplied data
	  /// </summary>
	  /// <remarks>
	  /// The HMACSHA1 algorithm uses the hash key value as part of its
	  /// algorithm. For all other algorithms the hash key value is used
	  /// as a salt value prior to execution of the hash algorithm.
	  /// </remarks>
	  /// <param name="plainText">The data from which to calculate the hash</param>
	  /// <param name="base64Key">Base64 encoded byte array containing the hash key</param>
	  /// <returns>Byte array containing the hash value</returns>
	  protected static byte[] ComputeHash(byte[] plainText, HashAlgorithm hash, string base64Key)
	  {

		byte[] result = null;
		if (hash is HMACSHA1)
		{
		  result = hash.ComputeHash(plainText, 0, plainText.Length);

		}
		else
		{
		  // use the key as a salt
		  byte[] key = Convert.FromBase64String(base64Key);
		  byte[] data = ConcatArrays(key, plainText);
		  result = hash.ComputeHash(data, 0, plainText.Length);
		}

		return result;

	  }

	#endregion

	}


} //end of root namespace