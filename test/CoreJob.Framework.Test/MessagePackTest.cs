using System;
using System.Text;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using MessagePack;
using NUnit.Framework;

namespace CoreJob.Framework.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public class PClass<T>
        {
            [Key(0)]
            public string A { get; set; }

            [Key(1)]
            public T B { get; set; }
        }

        public class SubClass
        {
            [Key(0)]
            public string A { get; set; }

            [Key(1)]
            public string B { get; set; }
        }

        [Test]
        public void Test1()
        {
            var request = new PClass<SubClass>() {
                A = "aaa",
                B = new SubClass() { 
                    A = "cccc",
                    B = "dddd"
                }
            }.SerializeMsgPack();

            Console.WriteLine(MessagePackSerializer.ConvertToJson(request));

            var c = request.DeserializeMsgPack<PClass<SubClass>>();
        }

        [Test]
        public void BeatTest()
        {
            //var pack = string.Empty.Success().SerializeMsgPack();
            //var packStr = pack.ToHexString();
            //var bytes = new byte[] { 239, 191, 189, 239, 191, 189, 200, 160, 239, 191, 189 };
            //var bytes1 = packStr.ToHexBytes();
            //Assert.AreEqual(pack, bytes1);
            //Assert.AreNotEqual(bytes, bytes1);
            //var result = bytes1.DeserializeMsgPack<CoreBaseResponse<string>>().CreateContext();



            var str1 = "’‚ «≤‚ ‘";

            var a = UTF8Encoding.UTF8.GetBytes(str1);
           
            var str2 = UTF8Encoding.UTF8.GetString(a);

            var b = Encoding.UTF8.GetBytes(str1);

            var str3 = Encoding.UTF8.GetString(b);
        }
    }
}