/// <summary>
/// Decoder of Sinclair Spectrum clone (Russian pentagon) Basic.
/// 
/// Sinclair basic documentation, but it's not exactly like that.
/// http://fileformats.archiveteam.org/wiki/Sinclair_BASIC_tokenized_file
/// </summary>

using Kaitai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicDetokenizer
{
    /// <summary>
    /// Basic detokenizer
    /// </summary>
    public class Detokenizer
    {
        public delegate void MessageDelegate(string message);

        /// <summary>
        /// Error message delegate
        /// </summary>
        public MessageDelegate? ErrorMessage { get; set; }

        /// <summary>
        /// Detokenize basic to ASCII code.
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="text">Source code as text</param>
        /// <returns>true if successful, false if not</returns>
        public bool Detokenize(byte[] data, out string text)
        {
            KaitaiStream stream = new KaitaiStream(data);
            StringBuilder code = new StringBuilder();

            try
            {
                DecodeFile(stream, code);
            }
            catch (Exception ex)
            {
                ErrorMessage?.Invoke(ex.ToString());

                // Append same error to code
                code.AppendLine();
                code.AppendLine(ex.ToString());
                text = code.ToString();                
                return false;
            }

            text = code.ToString();
            return true;
        }

        /// <summary>
        /// Decode file
        /// </summary>
        /// <param name="stream">Data stream</param>
        /// <param name="code">Code string builder</param>
        private void DecodeFile(KaitaiStream stream, StringBuilder code)
        {
            while (!stream.IsEof)
            {
                // Decode line
                int lineNr = stream.ReadU2be(); // Big endian
                code.Append($"{lineNr,5} ");

                // Decode length
                int length = stream.ReadU2le(); // Little endian
                if (length < 1)
                {
                    throw new DecoderException(stream, $"Line length can't be zero.");
                }

                // Decode line
                long expectedEnd = stream.Pos + (length - 1);
                DecodeLine(stream, code, length - 1);
                if (stream.Pos != expectedEnd)
                {
                    throw new DecoderException(stream, $"Line consumed more bytes ({expectedEnd - stream.Pos}) than the length ({length}) was.");
                }

                // Check line ending
                if (stream.ReadChar() != '\r')
                {
                    throw new DecoderException(stream, $"Invalid line ending.");
                }

                // Add line ending
                code.AppendLine();
            }
        }

        /// <summary>
        /// Decode line
        /// </summary>
        /// <param name="stream">Data stream</param>
        /// <param name="code">Code string builder</param>
        /// <param name="length">Line length</param>
        /// <returns>true if successful, false if not (error message invoked)</returns>
        private static void DecodeLine(KaitaiStream stream, StringBuilder code, int length)
        {
            long endPos = stream.Pos + length;
            bool needSpace = false;

            // Read all line bytes
            while ((stream.Pos < endPos) && (!stream.IsEof))
            {
                byte b = stream.ReadByte();

                // A token ?
                if (zxSpectrumTokens.ContainsKey(b))
                {
                    if (needSpace)
                    {
                        code.Append(' ');
                    }

                    code.Append(zxSpectrumTokens[b]);
                    code.Append(' ');

                    needSpace = false;
                }
                // Visible ASCII characters ?
                else if ((b >= 0x20) && (b <= 0x7F))
                {
                    code.Append((char)b);
                    needSpace = true;
                }
                // 5-byte number coming up ?
                else if (b == 0x0E)
                {
                    DecodeNumber(stream, code, endPos);
                    needSpace = true;
                }
                // Something else:
                else
                {
                    // Display it in raw format
                    code.Append($"0x{b:X2}");
                    needSpace = true;
                }
            }
        }

        /// <summary>
        /// Decode line
        /// </summary>
        /// <param name="stream">Data stream</param>
        /// <param name="code">Code string builder</param>
        /// <returns>true if successful, false if not (error message invoked)</returns>
        private static void DecodeNumber(KaitaiStream stream, StringBuilder code, long endPos)
        {
            // Valid ?
            if (stream.Pos + 5 > endPos)
            {
                throw new DecoderException(stream, $"Not enough bytes ({endPos - stream.Pos}) to parse number from current line.");
            }

            byte b = stream.ReadByte();

            // Integer ?
            if (b == 0)
            {
                // Get sign
                int sign = stream.ReadByte() switch
                {
                    0 => 1,
                    0xFF => -1,
                    _ => throw new DecoderException(stream, $"Invalid sign byte"),
                };

                // Get number
                int number = sign * stream.ReadU2le();

                // Check last byte
                if (stream.ReadByte() != 0)
                {
                    throw new DecoderException(stream, $"Last integer byte isn't zero.");
                }

                // Value doesn't matter...
            }
            else
            {
                // Floating point
                int exp = b - 0x80;
                int mant = stream.ReadS4be();

                // Value doesn't matter...
            }
        }

        /// <summary>
        /// ZX Spectrum 48/128 BASIC Tokens
        /// http://fileformats.archiveteam.org/wiki/Sinclair_BASIC_tokenized_file
        /// </summary>
        private static readonly Dictionary<byte, string> zxSpectrumTokens = new Dictionary<byte, string>()
        {
            [0xA3] = "SPECTRUM1",
            [0xA4] = "PLAY1",
            [0xA5] = "RND",
            [0xA6] = "INKEY$",
            [0xA7] = "PI",
            [0xA8] = "FN",
            [0xA9] = "POINT",
            [0xAA] = "SCREEN$",
            [0xAB] = "ATTR",
            [0xAC] = "AT",
            [0xAD] = "TAB",
            [0xAE] = "VAL$",
            [0xAF] = "CODE",
            [0xB0] = "VAL",
            [0xB1] = "LEN",
            [0xB2] = "SIN",
            [0xB3] = "COS",
            [0xB4] = "TAN",
            [0xB5] = "ASN",
            [0xB6] = "ACS",
            [0xB7] = "ATN",
            [0xB8] = "LN",
            [0xB9] = "EXP",
            [0xBA] = "INT",
            [0xBB] = "SQR",
            [0xBC] = "SGN",
            [0xBD] = "ABS",
            [0xBE] = "PEEK",
            [0xBF] = "IN",
            [0xC0] = "USR",
            [0xC1] = "STR$",
            [0xC2] = "CHR$",
            [0xC3] = "NOT",
            [0xC4] = "BIN",
            [0xC5] = "OR",
            [0xC6] = "AND",
            [0xC7] = "<=",
            [0xC8] = ">=",
            [0xC9] = "<>",
            [0xCA] = "LINE",
            [0xCB] = "THEN",
            [0xCC] = "TO",
            [0xCD] = "STEP",
            [0xCE] = "DEF FN",
            [0xCF] = "CAT",
            [0xD0] = "FORMAT",
            [0xD1] = "MOVE",
            [0xD2] = "ERASE",
            [0xD3] = "OPEN #",
            [0xD4] = "CLOSE #",
            [0xD5] = "MERGE",
            [0xD6] = "VERIFY",
            [0xD7] = "BEEP",
            [0xD8] = "CIRCLE",
            [0xD9] = "INK",
            [0xDA] = "PAPER",
            [0xDB] = "FLASH",
            [0xDC] = "BRIGHT",
            [0xDD] = "INVERSE",
            [0xDE] = "OVER",
            [0xDF] = "OUT",
            [0xE0] = "LPRINT",
            [0xE1] = "LLIST",
            [0xE2] = "STOP",
            [0xE3] = "READ",
            [0xE4] = "DATA",
            [0xE5] = "RESTORE",
            [0xE6] = "NEW",
            [0xE7] = "BORDER",
            [0xE8] = "CONTINUE",
            [0xE9] = "DIM",
            [0xEA] = "REM",
            [0xEB] = "FOR",
            [0xEC] = "GO TO",
            [0xED] = "GO SUB",
            [0xEE] = "INPUT",
            [0xEF] = "LOAD",
            [0xF0] = "LIST",
            [0xF1] = "LET",
            [0xF2] = "PAUSE",
            [0xF3] = "NEXT",
            [0xF4] = "POKE",
            [0xF5] = "PRINT",
            [0xF6] = "PLOT",
            [0xF7] = "RUN",
            [0xF8] = "SAVE",
            [0xF9] = "RANDOMIZE",
            [0xFA] = "IF",
            [0xFB] = "CLS",
            [0xFC] = "DRAW",
            [0xFD] = "CLEAR",
            [0xFE] = "RETURN",
            [0xFF] = "COPY"
        };

        /// <summary>
        /// Decoder exception
        /// </summary>
        public class DecoderException : Exception
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="stream">Kaitai stream</param>
            /// <param name="message">Message</param>
            public DecoderException(KaitaiStream stream, string message)
                : base(message)
            {
                Pos = stream.Pos;

                long numBefore = Math.Min(Pos, 16);
                stream.Seek(Pos - numBefore);
                BytesBefore = stream.ReadBytes(numBefore);
                BytesAfter = stream.ReadBytesFull().Take(16).ToArray();
            }
            
            public long Pos { get; init; }
            public byte[] BytesBefore { get; init; }
            public byte[] BytesAfter { get; init; }

            /// <summary>
            /// To string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Message + Environment.NewLine +
                    $"  Position {Pos}" + Environment.NewLine +
                    $"  Bytes before: {string.Join("-", BytesBefore.Select(b => b.ToString("X2")))}" + Environment.NewLine +
                    $"  Bytes after:  {string.Join("-", BytesAfter.Select(b => b.ToString("X2")))}" + Environment.NewLine;
            }
        }
    }
}
