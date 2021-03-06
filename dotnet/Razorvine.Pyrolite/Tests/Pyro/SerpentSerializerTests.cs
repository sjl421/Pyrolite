using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Razorvine.Pyro;
using Razorvine.Serpent;
// ReSharper disable CheckNamespace

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Pyrolite.Tests.Pyro
{
	public class SerpentSerializerTestsNoSets
	{
		[Fact]
		public void TestSerpentVersion()
		{
			Version serpentVersion = new Version(LibraryVersion.Version);
			Assert.True(serpentVersion >= new Version(1, 16));
		}
		
		[Fact]
		public void TestSerializeData()
		{
			Config.SERPENT_INDENT=false;
			Config.SERPENT_SET_LITERALS=false;

			ICollection<object> list = new LinkedList<object>();
			list.Add("hello");
			list.Add(42);
			
			var ser = PyroSerializer.GetFor(Config.SerializerType.serpent);
			byte[] data = ser.serializeData(list);
			string str = Encoding.UTF8.GetString(data);
			Assert.Equal("# serpent utf-8 python2.6\n['hello',42]", str);
			
			List<object> list_obj = (List<object>)ser.deserializeData(data);
			Assert.Equal(list, list_obj);
			
			ISet<string> s = new HashSet<string>();
			s.Add("element1");
			s.Add("element2");
			data = ser.serializeData(s);
			str = Encoding.UTF8.GetString(data);
			Assert.Equal("# serpent utf-8 python2.6\n('element1','element2')", str);
			
			object[] array_obj = (object[]) ser.deserializeData(data);
			Assert.Equal(s, array_obj);
		}

		[Fact]
		public void TestSerializeCall()
		{
			Config.SERPENT_INDENT=false;
			Config.SERPENT_SET_LITERALS=false;

			var ser = PyroSerializer.GetFor(Config.SerializerType.serpent);
			IDictionary<string, object> kwargs = new Dictionary<string, object>();
			kwargs["arg"] = 42;
			object[] vargs = new object[] {"hello"};
			
			byte[] data = ser.serializeCall("objectid", "method", vargs, kwargs);
			string s = Encoding.UTF8.GetString(data);
			Assert.Equal("# serpent utf-8 python2.6\n('objectid','method',('hello',),{'arg':42})", s);
			
			object[] call = (object[])ser.deserializeData(data);
			object[] expected = new object[] {
				"objectid",
				"method",
				new object[] {"hello"},
				new Dictionary<string, object>() {
					{"arg", 42}
				}
			};
			Assert.Equal(expected, call);
		}
	}

	public class SerpentSerializerTestsSets
	{
		[Fact]
		public void TestSerializeData()
		{
			Config.SERPENT_INDENT=false;
			Config.SERPENT_SET_LITERALS=true;

			ISet<string> s = new HashSet<string>();
			s.Add("element1");
			s.Add("element2");
			var ser = PyroSerializer.GetFor(Config.SerializerType.serpent);
			byte[] data = ser.serializeData(s);
			string str = Encoding.UTF8.GetString(data);
			Assert.Equal("# serpent utf-8 python3.2\n{'element1','element2'}", str);
			
			HashSet<object> s2 = (HashSet<object>) ser.deserializeData(data);
			Assert.Equal(s, s2);
		}
		
		[Fact]
		public void TestSerpentBytes()
		{
			Config.SERPENT_INDENT=false;
			Config.SERPENT_SET_LITERALS=true;

			byte[] bytes = Encoding.ASCII.GetBytes("hello");
			SerpentSerializer ser = new SerpentSerializer();
			byte[] data = ser.serializeData(bytes);
			
			string str = Encoding.ASCII.GetString(data);
			Assert.Contains("base64", str);
			
			Parser p = new Parser();
			Object data2 = p.Parse(data).GetData();
			byte[] bytes2 = SerpentSerializer.ToBytes(data2);
			Assert.Equal(Encoding.ASCII.GetBytes("hello"), bytes2);
		}		
	}
}
