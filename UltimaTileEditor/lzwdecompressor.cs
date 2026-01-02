using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    public struct dict_entry
    {
        public byte root;
        public int codeword;
    };

    internal class lzwDecompressor
    {
        const int S_SIZE = 10000;
        const int D_SIZE = 10000;
        const int max_codeword_length = 12;

        dict_entry[] dict;
        byte[] mystack;

        int bits_read = 0;
        int dict_contains = 0;
        int stack_contains = 0;
        long position = 0;

        public lzwDecompressor()
        {
            dict = new dict_entry[D_SIZE];
            mystack = new byte[S_SIZE];
        }

        // ----------------------------------------------
        // Read the next code word from the source buffer
        // ----------------------------------------------
        bool get_next_codeword(out int codeword, byte[] source, int codeword_size)
        {
            byte b0 = 0;
            byte b1 = 0;
            byte b2 = 0;
            int xx, xy, xz;
            xx = bits_read / 8;

            b0 = source[bits_read / 8];
            if (source.Length > bits_read / 8 + 1)
            {
                b1 = source[bits_read / 8 + 1];
            }
            if (source.Length > bits_read / 8 + 2)
            {
                b2 = source[bits_read / 8 + 2];
            }
            xx = b2 << 16;
            xy = b1 << 8;
            xz = b0;
            codeword = xx + xy + xz;
            codeword = codeword >> (bits_read % 8);
            switch (codeword_size)
            {
                case 0x9:
                    codeword = codeword & 0x1ff;
                    break;
                case 0xa:
                    codeword = codeword & 0x3ff;
                    break;
                case 0xb:
                    codeword = codeword & 0x7ff;
                    break;
                case 0xc:
                    codeword = codeword & 0xfff;
                    break;
                default:
                    MessageBox.Show("Error: weird codeword size!");
                    return false;
            }
            bits_read += codeword_size;

            return true;
        }

        void dict_init()
        {
            dict_contains = 0x102;
        }

        void stack_init()
        {
            stack_contains = 0;
        }

        void output_root(byte root, ref byte[] destination)
        {
            destination[position] = root;
            position++;
        }

        byte dict_get_root(int codeword)
        {
            return (dict[codeword].root);
        }

        int dict_get_codeword(int codeword)
        {
            return (dict[codeword].codeword);
        }

        bool stack_is_empty()
        {
            return (stack_contains == 0);
        }

        bool stack_is_full()
        {
            return (stack_contains == mystack.Length);
        }

        void stack_push(byte element)
        {
            if (!stack_is_full())
            {
                mystack[stack_contains] = element;
                stack_contains++;
            }
        }

        byte stack_pop()
        {
            byte element;

            if (!stack_is_empty())
            {
                element = mystack[stack_contains - 1];
                stack_contains--;
            }
            else
            {
                element = 0;
            }
            return element;
        }

        byte stack_gettop()
        {
            if (!stack_is_empty())
            {
                return (mystack[stack_contains - 1]);
            }
            return 0;
        }

        bool get_string(int codeword)
        {
            byte root;
            int current_codeword;

            current_codeword = codeword;
            stack_init();
            while (current_codeword > 0xff)
            {
                root = dict_get_root(current_codeword);
                current_codeword = dict_get_codeword(current_codeword);
                stack_push(root);
            }

            // push the root at the leaf
            try
            {
                byte current_codeword_byte = Convert.ToByte(current_codeword);
                stack_push(current_codeword_byte);
            }
            catch (OverflowException)
            {
                return false;
            }
            return true;
        }

        void dict_add(byte root, int codeword)
        {
            dict[dict_contains].root = root;
            dict[dict_contains].codeword = codeword;
            dict_contains++;
        }

        // -----------------------------------------------------------------------------
        // LZW-decompress from buffer to buffer.
        // -----------------------------------------------------------------------------
        bool lzw_decompressbuffer(byte[] source, ref byte[] destination)
        {
            bool end_marker_reached = false;
            int codeword_size = 9;
            int next_free_codeword = 0x102;
            int dictionary_size = 0x200;

            long bytes_written = 0;

            int cW;
            int pW = 0;
            byte C;
            byte poppedval;

            bits_read = 0;

            while (!end_marker_reached)
            {
                bool bValid = get_next_codeword(out cW, source, codeword_size);
                if (!bValid)
                {
                    return false;
                }
                switch (cW)
                {
                    // re-init the dictionary
                    case 0x100:
                        codeword_size = 9;
                        next_free_codeword = 0x102;
                        dictionary_size = 0x200;
                        dict_init();
                        bValid = get_next_codeword(out cW, source, codeword_size);
                        if (!bValid)
                        {
                            return false;
                        }
                        position = bytes_written;
                        try
                        {
                            byte cWByte = Convert.ToByte(cW);
                            output_root(cWByte, ref destination);
                        }
                        catch (OverflowException)
                        {
                            return false;
                        }
                        bytes_written = position;
                        break;
                    // end of compressed file has been reached
                    case 0x101:
                        end_marker_reached = true;
                        break;
                    // (cW <> 0x100) && (cW <> 0x101)
                    default:
                        if (cW < next_free_codeword)
                        {
                            // codeword is already in the dictionary
                            // create the string associated with cW (on the MyStack)
                            if (!get_string(cW))
                            {
                                return false;
                            }
                            C = stack_gettop();
                            // output the string represented by cW
                            while (!stack_is_empty())
                            {
                                poppedval = stack_pop();
                                position = bytes_written;
                                output_root(poppedval, ref destination);
                                bytes_written = position;
                            }

                            // add pW+C to the dictionary
                            dict_add(C, pW);

                            next_free_codeword++;
                            if (next_free_codeword >= dictionary_size)
                            {
                                if (codeword_size < max_codeword_length)
                                {
                                    codeword_size += 1;
                                    dictionary_size *= 2;
                                }
                            }
                        }
                        else
                        {
                            // codeword is not yet defined
                            // create the string associated with pW (on the MyStack)
                            get_string(pW);
                            C = stack_gettop();
                            // output the string represented by pW
                            while (!stack_is_empty())
                            {
                                position = bytes_written;
                                output_root(stack_pop(), ref destination);
                                bytes_written = position;
                            }
                            // output the char C
                            position = bytes_written;
                            output_root(C, ref destination);
                            bytes_written = position;
                            // the new dictionary entry must correspond to cW
                            // if it doesn't, something is wrong with the lzw-compressed data.
                            if (cW != next_free_codeword)
                            {
                                MessageBox.Show("cW != next_free_codeword!");
                                return false; // ERROR!  Come back later and close
                            }
                            // add pW+C to the dictionary
                            dict_add(C, pW);

                            next_free_codeword++;
                            if (next_free_codeword >= dictionary_size)
                            {
                                if (codeword_size < max_codeword_length)
                                {
                                    codeword_size += 1;
                                    dictionary_size *= 2;
                                }
                            }
                        }
                        break;
                }
                // shift roles - the current cW becomes the new pW
                pW = cW;
            }
            return true;
        }

        // this function only checks a few *necessary* conditions
        // returns "false" if the file doesn't satisfy these conditions
        // return "true" otherwise
        public bool is_valid_lzw_file(byte[] file_bytes)
        {
            // file must contain 4-byte size header and space for the 9-bit value 0x100
            if (file_bytes.Length < 6)
            {
                return false;
            }
            // the last byte of the size header must be 0 (U5's files aren't *that* big)
            if (file_bytes[3] != 0)
            {
                return false;
            }
            // the 9 bits after the size header must be 0x100
            if ((file_bytes[4] != 0) || ((file_bytes[5] & 1) != 1))
            {
                return false;
            }

            return true;
        }

        public int get_uncompressed_size(byte[] file_bytes)
        {
            int uncompressed_file_length = BitConverter.ToInt32(file_bytes, 0);
            return uncompressed_file_length;
        }

        public bool extract(byte[] file_bytes, out byte[]? outFile)
        {
            outFile = null;
            if (is_valid_lzw_file(file_bytes))
            {
                // determine the buffer sizes
                int source_buffer_size = file_bytes.Length - 4;
                int destination_buffer_size = get_uncompressed_size(file_bytes);

                // create the buffers
                byte[] source_buffer = new byte[source_buffer_size];
                byte[] destination_buffer = new byte[destination_buffer_size];

                Array.Copy(file_bytes, 4, source_buffer, 0, file_bytes.Length - 4);

                byte tempval = 255;
                Array.Fill(destination_buffer, tempval);

                // decompress the input file
                if (!lzw_decompressbuffer(source_buffer, ref destination_buffer))
                {
                    return false;
                }

                outFile = destination_buffer;
            }
            return true;
        }

        public bool compress(byte[] file_bytes, string outFile)
        {
            using (BinaryWriter binWriter =
                    new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                binWriter.Write(file_bytes.Length);

                build_dictionary(file_bytes, 0, binWriter);

            }

            return true;
        }

        byte code_word_remainder = 0;
        int code_word_shift = 0;

        void write_code_word(int code_word, int code_word_size, BinaryWriter binWriter)
        {
            int temp_int = code_word << code_word_shift;
            temp_int += code_word_remainder;
            code_word_shift += code_word_size;
            while (code_word_shift >= 8)
            {
                byte temp_byte = (byte)(temp_int & 0xFF);
                binWriter.Write(temp_byte);
                temp_int >>= 8;
                code_word_shift -= 8;
            }
            code_word_remainder = (byte)(temp_int & 0xFF);
        }

        int build_dictionary(byte[] file_bytes, int start_pos, BinaryWriter binWriter)
        {
            code_word_remainder = 0;
            code_word_shift = 0;
            int next_free_codeword = 0x102;
            int codeword_size = 9;
            List<List<byte>> cur_dict = new List<List<byte>>();
            int temp_pos = start_pos;
            bool invalid = false;

            write_code_word(0x100, codeword_size, binWriter);

            while (temp_pos < file_bytes.Length)
            {
                int seq_index = 0;
                bool sequence_found = false;
                List<byte> temp_list = new List<byte>();
                temp_list.Add(file_bytes[temp_pos]);
                temp_pos++;
                if (temp_pos >= file_bytes.Length)
                {
                    // Write the code word
                    write_code_word(temp_list[0], codeword_size, binWriter);
                    break; // We've reached the end, so just write this
                }
                temp_list.Add(file_bytes[temp_pos]);
                while (cur_dict.Any(p => p.SequenceEqual(temp_list)))
                {
                    seq_index = 0;
                    foreach (var curList in cur_dict)
                    {
                        if (curList.SequenceEqual(temp_list))
                        {
                            break;
                        }
                        seq_index++;
                    }

                    sequence_found = true;

                    temp_pos++;
                    if (temp_pos >= file_bytes.Length)
                    {
                        // Write the sequence
                        write_code_word(next_free_codeword + seq_index, codeword_size, binWriter);
                        invalid = true;
                        break; // We've reached the end, so just write this
                    }
                    temp_list.Add(file_bytes[temp_pos]);
                }
                cur_dict.Add(temp_list);
                if (cur_dict.Count == 256 || cur_dict.Count == 768 || cur_dict.Count == 1792)
                {
                    codeword_size++;
                }
                else if (cur_dict.Count >= 3840)
                {
                    temp_pos -= (temp_list.Count - 1);
                    cur_dict.Clear();
                    write_code_word(0x100, codeword_size, binWriter);
                    codeword_size = 9;
                    continue;
                }
                if (invalid)
                {
                    break;
                }
                if (sequence_found)
                {
                    // Write the sequence
                    write_code_word(next_free_codeword + seq_index, codeword_size, binWriter);

                }
                else
                {
                    // Write the code word
                    write_code_word(temp_list[0], codeword_size, binWriter);
                }
            }
            write_code_word(0x101, codeword_size, binWriter);

            if (code_word_shift > 0)
            {
                binWriter.Write(code_word_remainder);
            }

            MessageBox.Show("File written!");
            return -1;
        }
    }
}
