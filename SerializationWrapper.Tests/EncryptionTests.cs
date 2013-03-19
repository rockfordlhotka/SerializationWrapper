using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerializationWrapper;

namespace SerializationWrapper.Test
{
  [TestClass]
  public class EncryptionTests
  {

    private string rsaPublicKey;
    private string rsaPrivateKey;

    [TestInitialize]
    public void Setup()
    {

      System.Security.Cryptography.RSACryptoServiceProvider rsaKeys = new System.Security.Cryptography.RSACryptoServiceProvider();
      rsaPublicKey = rsaKeys.ToXmlString(false);
      rsaPrivateKey = rsaKeys.ToXmlString(true);

    }

    [TestMethod]
    public void Combined()
    {

      string cryptoKey = CryptoWrapper.CreateBase64Key();
      string hashKey = SignedWrapper.CreateBase64Key();

      string data = RandomData(200);

      SignedWrapper<string> s = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.RIPEMD160, hashKey);
      CryptoWrapper<SignedWrapper<string>> c = new CryptoWrapper<SignedWrapper<string>>(s, cryptoKey);
      CompressedWrapper<CryptoWrapper<SignedWrapper<string>>> cm = new CompressedWrapper<CryptoWrapper<SignedWrapper<string>>>(c);

      c = cm.GetObject();
      s = c.GetObject(cryptoKey);

      if (s.Verify(SignedWrapper.AlgorithmType.RIPEMD160, hashKey))
      {
        Assert.AreEqual(data, System.Convert.ToString(s.GetObject()), "Data not recovered");

      }
      else
      {
        Assert.Fail("Couldn't verify signature");
      }

    }

    #region  Default Crypto

    [TestMethod]
    public void DefaultEncrypt()
    {

      string cryptoKey = CryptoWrapper.CreateBase64Key();

      string data = RandomData(200);

      CryptoWrapper<string> wrapper = new CryptoWrapper<string>(data, cryptoKey);
      string result = System.Convert.ToString(wrapper.GetObject(cryptoKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    [TestMethod]
    public void DefaultEncryptPassword()
    {

      string cryptoKey = CryptoWrapper.CreateBase64Key("ATe$Tpassw@rd");

      string data = RandomData(200);

      CryptoWrapper<string> wrapper = new CryptoWrapper<string>(data, cryptoKey);
      string result = System.Convert.ToString(wrapper.GetObject(cryptoKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    #endregion

    #region  DES

    [TestMethod]
    public void DESEncrypt()
    {

      string cryptoKey = CryptoWrapper.CreateBase64Key(CryptoWrapper.AlgorithmType.DES);

      string data = RandomData(200);

      CryptoWrapper<string> wrapper = new CryptoWrapper<string>(data, CryptoWrapper.AlgorithmType.DES, cryptoKey);
      string result = System.Convert.ToString(wrapper.GetObject(CryptoWrapper.AlgorithmType.DES, cryptoKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    #endregion

    #region  TripleDES

    [TestMethod]
    public void TripleDESEncrypt()
    {

      string cryptoKey = CryptoWrapper.CreateBase64Key(CryptoWrapper.AlgorithmType.TripleDES);

      string data = RandomData(200);

      CryptoWrapper<string> wrapper = new CryptoWrapper<string>(data, CryptoWrapper.AlgorithmType.TripleDES, cryptoKey);
      string result = System.Convert.ToString(wrapper.GetObject(CryptoWrapper.AlgorithmType.TripleDES, cryptoKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    #endregion

    #region  RC2

    [TestMethod]
    public void RC2Encrypt()
    {

      string cryptoKey = CryptoWrapper.CreateBase64Key(CryptoWrapper.AlgorithmType.RC2);

      string data = RandomData(200);

      CryptoWrapper<string> wrapper = new CryptoWrapper<string>(data, CryptoWrapper.AlgorithmType.RC2, cryptoKey);
      string result = System.Convert.ToString(wrapper.GetObject(CryptoWrapper.AlgorithmType.RC2, cryptoKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    #endregion

    #region  Rijndael

    [TestMethod]
    public void RijndaelEncrypt()
    {

      string cryptoKey = CryptoWrapper.CreateBase64Key(CryptoWrapper.AlgorithmType.Rijndael);

      string data = RandomData(200);

      CryptoWrapper<string> wrapper = new CryptoWrapper<string>(data, CryptoWrapper.AlgorithmType.Rijndael, cryptoKey);
      string result = System.Convert.ToString(wrapper.GetObject(CryptoWrapper.AlgorithmType.Rijndael, cryptoKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    #endregion

    #region  RSA

    [TestMethod]
    public void RSAEncrypt()
    {

      string data = RandomData(20);

      RSACryptoWrapper<string> rsa = new RSACryptoWrapper<string>(data, rsaPublicKey);
      string result = System.Convert.ToString(rsa.GetObject(rsaPrivateKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    [TestMethod]
    public void RSAEncryptLarge()
    {

      string data = RandomData(2000);

      RSACryptoWrapper<string> rsa = new RSACryptoWrapper<string>(data, rsaPublicKey);
      string result = System.Convert.ToString(rsa.GetObject(rsaPrivateKey));
      Assert.AreEqual(data, result, "Data not recovered");

    }

    #endregion

    private string RandomData(int length)
    {

      string result = "";
      while (result.Length < length)
      {
        result += Guid.NewGuid().ToString();
      }
      return result.Substring(0, length);

    }

  }

}