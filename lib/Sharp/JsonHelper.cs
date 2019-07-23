using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace lib
{
	public static class JsonHelper
	{
		public static T ParseJson<T>(string record, bool useSimpleDictionaryFormat = false)
			=> ParseJson<T>(Encoding.UTF8.GetBytes(record), useSimpleDictionaryFormat);

		public static T ParseJson<T>(byte[] record, bool useSimpleDictionaryFormat = false)
			=> ParseJson<T>(record, 0, record.Length, useSimpleDictionaryFormat);

		public static T ParseJson<T>(ArraySegment<byte> segment, bool useSimpleDictionaryFormat = false)
			=> ParseJson<T>(segment.Array, segment.Offset, segment.Count, useSimpleDictionaryFormat);

		public static T ParseJson<T>(byte[] record, int offset, int length, bool useSimpleDictionaryFormat = false)
			=> (T)ParseJson(typeof(T), record, offset, length, useSimpleDictionaryFormat);

		public static object ParseJson(Type type, string record, bool useSimpleDictionaryFormat = false)
			=> ParseJson(type, Encoding.UTF8.GetBytes(record), useSimpleDictionaryFormat);

		public static object ParseJson(Type type, byte[] record, bool useSimpleDictionaryFormat = false)
			=> ParseJson(type, record, 0, record.Length, useSimpleDictionaryFormat);

		public static object ParseJson(Type type, byte[] record, int offset, int length, bool useSimpleDictionaryFormat = false)
		{
			var reader = JsonReaderWriterFactory.CreateJsonReader(record, offset, length, XmlDictionaryReaderQuotas.Max);
			return new DataContractJsonSerializer(type, useSimpleDictionaryFormat ? SimpleDictionarySettings : DefaultSettings).ReadObject(reader);
		}

		public static T ParseJson<T>(Stream stream, bool useSimpleDictionaryFormat = false)
			=> (T)ParseJson(typeof(T), stream, useSimpleDictionaryFormat);

		public static object ParseJson(Type type, Stream stream, bool useSimpleDictionaryFormat = false)
		{
			var reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
			return new DataContractJsonSerializer(type, useSimpleDictionaryFormat ? SimpleDictionarySettings : DefaultSettings).ReadObject(reader);
		}

		public static string ToJsonString<T>(this T obj, bool runtime = true, bool useSimpleDictionaryFormat = false, bool neverEmitTypeInfo = false, bool forceEmitTypeInfo = false)
		{
			var bytes = obj.ToJsonSegment(runtime, useSimpleDictionaryFormat, neverEmitTypeInfo, forceEmitTypeInfo);
			return Encoding.UTF8.GetString(bytes.Array, bytes.Offset, bytes.Count);
		}

		/// <summary>Serializing for inline scripts with replacing of dangerous '&lt;' -> '\u003c', '&gt;' -> '\u003e'</summary>
		public static string ToJsonInlineString<T>(this T obj, bool runtime = true, bool useSimpleDictionaryFormat = false, bool forceEmitTypeInfo = false)
		{
			using(var ms = new MemoryStream())
			using(var stream = new AngleBracketsJsonEscapingStream(ms))
			{
				obj.ToJson(stream, runtime, useSimpleDictionaryFormat, forceEmitTypeInfo:forceEmitTypeInfo);
				return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
			}
		}

		public static byte[] ToJson<T>(this T obj, bool runtime = true, bool useSimpleDictionaryFormat = false, bool neverEmitTypeInfo = false, bool forceEmitTypeInfo = false)
		{
			var bytes = obj.ToJsonSegment(runtime, useSimpleDictionaryFormat, neverEmitTypeInfo, forceEmitTypeInfo);
			if(bytes.Offset == 0 && bytes.Count == bytes.Array.Length)
				return bytes.Array;
			var buffer = new byte[bytes.Count];
			Buffer.BlockCopy(bytes.Array, bytes.Offset, buffer, 0, bytes.Count);
			return buffer;
		}

		public static ArraySegment<byte> ToJsonSegment<T>(this T obj, bool runtime = true, bool useSimpleDictionaryFormat = false, bool neverEmitTypeInfo = false, bool forceEmitTypeInfo = false)
		{
			using(var stream = new MemoryStream())
			{
				obj.ToJson(stream, runtime, useSimpleDictionaryFormat, neverEmitTypeInfo, forceEmitTypeInfo:forceEmitTypeInfo);
				return new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
			}
		}

		public static void ToJson<T>(this T obj, Stream stream, bool runtime = true, bool useSimpleDictionaryFormat = false, bool neverEmitTypeInfo = false, bool forceEmitTypeInfo = false)
		{
			var settings = GetSettings(useSimpleDictionaryFormat, neverEmitTypeInfo, forceEmitTypeInfo:forceEmitTypeInfo);
			new DataContractJsonSerializer(runtime ? obj.TryGetRuntimeType() : typeof(T), settings).WriteObject(stream, obj);
		}

		private static DataContractJsonSerializerSettings GetSettings(bool useSimpleDictionaryFormat, bool neverEmitTypeInfo, bool forceEmitTypeInfo)
		{
			return useSimpleDictionaryFormat
				? (neverEmitTypeInfo ? SimpleDictionaryWithoutTypeEmitSettings : (forceEmitTypeInfo ? SimpleDictionaryWithTypeEmitSettings : SimpleDictionarySettings))
				: (neverEmitTypeInfo ? WithoutTypeEmitSettings : (forceEmitTypeInfo ? WithTypeEmitSettings : DefaultSettings));
		}

		public static Type TryGetRuntimeType<T>(this T obj)
		{
			return Equals(obj, null) ? typeof(T) : obj.GetType();
		}

		class AngleBracketsJsonEscapingStream : Stream
		{
			public AngleBracketsJsonEscapingStream(Stream stream) => this.stream = stream;

			public override void Flush() => stream.Flush();
			public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
			public override void SetLength(long value) => throw new NotSupportedException();
			public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

			public override void Write(byte[] buffer, int offset, int count)
			{
				int len;
				var start = offset;
				var end = offset + count;
				for(int idx; (idx = IndexOfAngle(buffer, start, len = end - start)) >= 0;)
				{
					if((len = idx - start) > 0)
						stream.Write(buffer, start, len);
					var escaped = buffer[idx] == LessThan ? LessThanEscaped : GreaterThanEscaped;
					stream.Write(escaped, 0, escaped.Length);
					start = idx + 1;
				}
				if(len > 0)
					stream.Write(buffer, start, len);
			}

			private static int IndexOfAngle(byte[] source, int offset, int count)
			{
				var end = offset + count;
				for(var idx = offset; idx < end; idx++)
				{
					var b = source[idx];
					if(b == LessThan || b == GreaterThan)
						return idx;
				}
				return -1;
			}

			public override bool CanRead => false;
			public override bool CanSeek => false;
			public override bool CanWrite => stream.CanWrite;
			public override long Length => stream.Length;
			public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

			private const byte LessThan = (byte)'<';
			private const byte GreaterThan = (byte)'>';

			private static readonly byte[] LessThanEscaped = Encoding.ASCII.GetBytes("\\u003c");
			private static readonly byte[] GreaterThanEscaped = Encoding.ASCII.GetBytes("\\u003e");

			private readonly Stream stream;
		}

		private static readonly DataContractJsonSerializerSettings DefaultSettings = new DataContractJsonSerializerSettings {SerializeReadOnlyTypes = true, DateTimeFormat = new DateTimeFormat("s", CultureInfo.InvariantCulture), };
		private static readonly DataContractJsonSerializerSettings WithoutTypeEmitSettings = new DataContractJsonSerializerSettings {EmitTypeInformation = EmitTypeInformation.Never, SerializeReadOnlyTypes = true, DateTimeFormat = new DateTimeFormat("s", CultureInfo.InvariantCulture)};
		private static readonly DataContractJsonSerializerSettings WithTypeEmitSettings = new DataContractJsonSerializerSettings {EmitTypeInformation = EmitTypeInformation.Always, SerializeReadOnlyTypes = true, DateTimeFormat = new DateTimeFormat("s", CultureInfo.InvariantCulture)};
		private static readonly DataContractJsonSerializerSettings SimpleDictionarySettings = new DataContractJsonSerializerSettings {UseSimpleDictionaryFormat = true, SerializeReadOnlyTypes = true, DateTimeFormat = new DateTimeFormat("s", CultureInfo.InvariantCulture)};
		private static readonly DataContractJsonSerializerSettings SimpleDictionaryWithoutTypeEmitSettings = new DataContractJsonSerializerSettings {EmitTypeInformation = EmitTypeInformation.Never, UseSimpleDictionaryFormat = true, SerializeReadOnlyTypes = true, DateTimeFormat = new DateTimeFormat("s", CultureInfo.InvariantCulture)};
		private static readonly DataContractJsonSerializerSettings SimpleDictionaryWithTypeEmitSettings = new DataContractJsonSerializerSettings {EmitTypeInformation = EmitTypeInformation.Always, UseSimpleDictionaryFormat = true, SerializeReadOnlyTypes = true, DateTimeFormat = new DateTimeFormat("s", CultureInfo.InvariantCulture)};
	}
}
