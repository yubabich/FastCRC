using Microsoft.VisualStudio.TestTools.UnitTesting;
using Soft160.Data.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class CRCTest
    {
        [TestMethod]
        public void DataTest()
        {
            string dataFile = "CRC32Data.txt";
            var testData = LoadTestData(dataFile);

            Assert.AreEqual(1025, testData.Count);
            for (int i = 0; i < testData.Count; ++i)
            {
                uint crc = CRC.Crc32(testData[i].Item1);
                Assert.AreEqual(testData[i].Item2, crc, $"Line {i}");
            }
        }

        [TestMethod]
        public void Test42()
        {
            string text = "Answer to the Ultimate Question of Life, the Universe, and Everything";
            uint textCRC32 = 0xf341b721;

            uint crc = CRC.Crc32(Encoding.ASCII.GetBytes(text));
            Assert.AreEqual(textCRC32, crc);
        }

        [TestMethod]
        public void MemoryDataTest()
        {
            string dataFile = "CRC32Data.txt";
            var testData = LoadTestData(dataFile);

            Assert.AreEqual(1025, testData.Count);
            for (int i = 0; i < testData.Count; ++i)
            {
                uint crc = CRC.Crc32(new Memory<byte>(testData[i].Item1));
                Assert.AreEqual(testData[i].Item2, crc, $"Line {i}");
            }
        }

        [TestMethod]
        public void SpanDataTest()
        {
            string dataFile = "CRC32Data.txt";
            var testData = LoadTestData(dataFile);

            Assert.AreEqual(1025, testData.Count);
            for (int i = 0; i < testData.Count; ++i)
            {
                uint crc = CRC.Crc32(new ReadOnlySpan<byte>(testData[i].Item1));
                Assert.AreEqual(testData[i].Item2, crc, $"Line {i}");
            }
        }

        private static List<Tuple<byte[], uint>> LoadTestData(string dataFile)
        {
            var result = new List<Tuple<byte[], uint>>();
            using (var reader = new StreamReader(new FileStream(dataFile, FileMode.Open, FileAccess.Read)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue; //skip it
                    }

                    var parts = line.Split(',');
                    if (parts.Length != 2)
                    {
                        throw new Exception($"File {dataFile} is corrupted");
                    }
                    var data = HexStringToBytes(parts[0]);
                    var crc = uint.Parse(parts[1]);
                    result.Add(Tuple.Create(data, crc));
                }
            }
            return result;
        }

        private static byte[] HexStringToBytes(string s)
        {
            if (s.Length % 2 == 1)
            {
                throw new ArgumentException();
            }
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
