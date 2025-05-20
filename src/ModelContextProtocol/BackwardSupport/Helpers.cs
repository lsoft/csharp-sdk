using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Net.ServerSentEvents.Fakes
{
    internal static class Helpers
    {
        public static void WriteUtf8Number(this IBufferWriter<byte> writer, long value)
        {
#if NET
            const int MaxDecimalDigits = 20;
            Span<byte> buffer = writer.GetSpan(MaxDecimalDigits);
            Debug.Assert(MaxDecimalDigits <= buffer.Length);

            bool success = value.TryFormat(buffer, out int bytesWritten, provider: CultureInfo.InvariantCulture);
            Debug.Assert(success);
            writer.Advance(bytesWritten);
#else
            writer.WriteUtf8String(value.ToString(CultureInfo.InvariantCulture));
#endif
        }

        public static void WriteUtf8String(this IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
        {
            if (value.IsEmpty)
            {
                return;
            }

            Span<byte> buffer = writer.GetSpan(value.Length);
            Debug.Assert(value.Length <= buffer.Length);
            value.CopyTo(buffer);
            writer.Advance(value.Length);
        }

        public static void WriteUtf8String(this IBufferWriter<byte> writer, string value)
        {
            if (value.Length == 0)
            {
                return;
            }

            var array = Encoding.UTF8.GetBytes(value);
            writer.Write(array);

            //int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
            //Span<byte> buffer = writer.GetSpan(maxByteCount);
            //Debug.Assert(maxByteCount <= buffer.Length);
            //var bytesWritten = Encoding.UTF8.GetBytes(value, buffer);
            //writer.Advance(bytesWritten);
        }

        public static bool ContainsLineBreaks(this ReadOnlySpan<char> text) =>
            text.IndexOfAny('\r', '\n') >= 0;
    }


}
