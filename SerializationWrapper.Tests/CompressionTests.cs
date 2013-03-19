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
  public class CompressionTests
  {

    [TestMethod]
    public void Deflate()
    {

      string data = new String('A', 200); //RandomData(200)

      CompressedWrapper<string> wrapper = new CompressedWrapper<string>(data);
      string result = wrapper.GetObject().ToString();

      Assert.AreEqual(data, result, "Data not recovered");

    }

    [TestMethod]
    public void GZip()
    {

      string data = new String('A', 200);

      CompressedWrapper<string> wrapper = new CompressedWrapper<string>(data, CompressedWrapper.CompressionType.GZip);
      string result = wrapper.GetObject(CompressedWrapper.CompressionType.GZip).ToString();

      Assert.AreEqual(data, result, "Data not recovered");

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

}