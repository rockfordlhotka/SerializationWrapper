//INSTANT C# NOTE: Formerly VB.NET project-level imports:
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
  public class SigningTests
  {

    private string rsaPublicKey;
    private string rsaPrivateKey;
    private string dsaPublicKey;
    private string dsaPrivateKey;

    [TestInitialize]
    public void Setup()
    {

    System.Security.Cryptography.RSACryptoServiceProvider rsaKeys = new System.Security.Cryptography.RSACryptoServiceProvider();
    rsaPublicKey = rsaKeys.ToXmlString(false);
    rsaPrivateKey = rsaKeys.ToXmlString(true);

    System.Security.Cryptography.DSACryptoServiceProvider dsaKeys = new System.Security.Cryptography.DSACryptoServiceProvider();
    dsaPublicKey = dsaKeys.ToXmlString(false);
    dsaPrivateKey = dsaKeys.ToXmlString(true);

    }

    [TestMethod]
    public void DefaultSign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, hashKey);
    if (wrapper.Verify(hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void DefaultSignPassword()
    {

    string hashKey = SignedWrapper.CreateBase64Key("ATe$Tpassw@rd");

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, hashKey);
    if (wrapper.Verify(hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void HMACSHA1Sign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.HMACSHA1, hashKey);
    if (wrapper.Verify(SignedWrapper.AlgorithmType.HMACSHA1, hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void MD5Sign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.MD5, hashKey);
    if (wrapper.Verify(SignedWrapper.AlgorithmType.MD5, hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void SHA1Sign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.SHA1, hashKey);
    if (wrapper.Verify(SignedWrapper.AlgorithmType.SHA1, hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void SHA256Sign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.SHA256, hashKey);
    if (wrapper.Verify(SignedWrapper.AlgorithmType.SHA256, hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void SHA384Sign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.SHA384, hashKey);
    if (wrapper.Verify(SignedWrapper.AlgorithmType.SHA384, hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void SHA512Sign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.SHA512, hashKey);
    if (wrapper.Verify(SignedWrapper.AlgorithmType.SHA512, hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void RIPEMD160Sign()
    {

    string hashKey = SignedWrapper.CreateBase64Key();

    string data = RandomData(200);

    SignedWrapper<string> wrapper = new SignedWrapper<string>(data, SignedWrapper.AlgorithmType.RIPEMD160, hashKey);
    if (wrapper.Verify(SignedWrapper.AlgorithmType.RIPEMD160, hashKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(wrapper.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void RSASign()
    {

    string data = RandomData(20);

    RSASignedWrapper<string> rsaSign = new RSASignedWrapper<string>(data, rsaPrivateKey);
    if (rsaSign.Verify(rsaPublicKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(rsaSign.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

    [TestMethod]
    public void DSASign()
    {

    string data = RandomData(20);

    DSASignedWrapper<string> dsaSign = new DSASignedWrapper<string>(data, dsaPrivateKey);
    if (dsaSign.Verify(dsaPublicKey))
    {
      Assert.AreEqual(data, System.Convert.ToString(dsaSign.GetObject()), "Data not recovered");

    }
    else
    {
      Assert.Fail("Couldn't verify signature");
    }

    }

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

} //end of root namespace